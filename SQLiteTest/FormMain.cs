using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Runtime.InteropServices;


// This is a C# project to demonstrage how well SQLite can handle Multithreading
// The documentation for this project and the accompanying C++ projects can be found here:
// (the location may be moved by the webmaster of the wiki)
// https://mywiki.grp.haufemg.com/pages/viewpage.action?pageId=156088657


// Two different approaches should be shown:
// Pooling: Using different connections for different threads and
// Using the same connection with multiple threads simultaneously

namespace SQLiteTest
{

    public partial class frmMain : Form
    {
        SQLiteConnection con = null;

        CThreads threads;


        // These threads are used for simultaneous writing...
        List<Thread> listThreads = new List<Thread>();

        // This thread is used for reading
        // ( Constantly polling the data in 
        //   the database and refreshing them )
        // Can be run simultanously to writing threads
        ViewThread viewThread;

        String strDatabaseFile = "";
        String strVersion = "";
        String strSQLiteVersion = "";
        const String DB_FILE = "Multisqlite.db";   //

        private int appID = -1;

        private delegate void AddListDelegate(List<String> list);

        public void AddListSafe(List<String> list)
        {
            if (lbGeneral.InvokeRequired)
            {
                var d = new AddListDelegate(AddListSafe);
                lbGeneral.Invoke(d, new object[] { list });
            }
            else
            {
                lbGeneral.Items.Clear();
                foreach (string str in list)
                {
                    lbGeneral.Items.Add(str);
                }

            }
        }

        public frmMain()
        {
            InitializeComponent();

            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            String strDatabaseDir = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData));
            if (strDatabaseDir.Length == 0)
                strDatabaseDir = "C:";
            if (strDatabaseDir[strDatabaseDir.Length - 1] != '\\')
                strDatabaseDir += '\\';
            strDatabaseDir += "MultiSQLite\\";
            if (!System.IO.Directory.Exists(strDatabaseDir))
                System.IO.Directory.CreateDirectory(strDatabaseDir);
            strDatabaseFile = strDatabaseDir + DB_FILE;

            System.IO.FileAttributes databaseAttributes = 0;

            if (File.Exists(strDatabaseFile))
                databaseAttributes = System.IO.File.GetAttributes(strDatabaseFile);
            else
                databaseAttributes = System.IO.FileAttributes.Offline;

            var versionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            strVersion = versionInfo.FileVersion;
            lbGeneral.Items.Add(appName + " version: " + strVersion);


            DateTime dt = DateTime.Now;
            string cs = "Data Source=" + strDatabaseFile + ";Version=3;Pooling=True;Max Pool Size=100;";
            SQLiteCommand cmd = null;

            bool bDatabaseExisted = File.Exists(strDatabaseFile);
            con = new SQLiteConnection(cs);
            con.Open();
          
            String strConnected = "";
            if (con.State == ConnectionState.Open)
            {
                if (bDatabaseExisted)
                {
                    int nRevision = getRevision();
                    if (nRevision < 2)
                    {
                        lbGeneral.Items.Add("Error: Old database....");
                        lbGeneral.Items.Add("       Deleting tables");
                        lbGeneral.Items.Add("");
                        execQuery("drop table if exists version");
                        execQuery("drop table if exists testtable");
                        execQuery("drop table if exists apps");
                    }
                }

                setSQLiteVersion();
                if (databaseAttributes == System.IO.FileAttributes.Offline)
                {
                    databaseAttributes = System.IO.File.GetAttributes(strDatabaseFile);
                    lbGeneral.Items.Add("Database created: " + strDatabaseFile);
                }
                else
                {
                    lbGeneral.Items.Add("Database opened: " + strDatabaseFile);
                }
                lbGeneral.Items.Add("Database accessible for " + ((databaseAttributes == System.IO.FileAttributes.ReadOnly) ? " Read only" : "Read + Write"));



                String strDeleteFrom = "";
                String strInsert = "";


                execQuery("Create Table if NOT Exists version (id INTEGER PRIMARY KEY AUTOINCREMENT, revision INTEGER) ");
                execQuery("Create Table if NOT Exists testtable (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP) ");
                execQuery("Create Table if NOT Exists apps (id INTEGER PRIMARY KEY AUTOINCREMENT, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  tsLastPoll TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name TEXT) ");
                execQuery("insert into apps (name) values ('" + appName + "')");
                setAppID();


                // Create a table that can hold text-data along with the thread-id of the thread that created the data
                strInsert = String.Format("insert into testtable (appID,threadid,text) values ({0},0,'{1}')", appID, dt.ToString());
                execQuery(strInsert);
                execQuery("Delete from version");
                execQuery("insert into version (id,revision) values (0,2)");


                lbGeneral.Items.Add("SQLite Version: " + strSQLiteVersion);
            }
            else
            {
                lbGeneral.Items.Add("Error: Could not opened database: " + strDatabaseFile);
            }

            threads = new CThreads(appID, strDatabaseFile);
            tiUpdateApps.Enabled = true;
            tiPollApps.Enabled = true;
        }

        protected void execQuery(String strQuery)
        {
            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand(strQuery, con);
            cmd.ExecuteNonQuery();
        }


        public void setSQLiteVersion()
        {
            SQLiteCommand cmd = null;
            string stm = "SELECT SQLITE_VERSION()";
            cmd = new SQLiteCommand(stm, con);
            strSQLiteVersion = cmd.ExecuteScalar().ToString();
        }


        public int getRevision()
        {
            string stm = "SELECT SQLITE_VERSION()";
            int nRevision = -1;

            try
            {
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand(stm, con);
                string strVersion = cmd.ExecuteScalar().ToString();


                cmd = new SQLiteCommand("SELECT * FROM version order by id DESC LIMIT 1", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String strID = reader["revision"].ToString();
                    nRevision = Convert.ToInt32(strID);
                    this.Text = "Haufe Multi-SQLite for C# <ID: " + appID + ">";
                    //this.Text = strID;
                }
            }
            catch(Exception e)
            {

            }
            return nRevision;
        }

        public void setAppID()
        {
            string stm = "SELECT SQLITE_VERSION()";

            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand(stm, con);
            string strVersion = cmd.ExecuteScalar().ToString();


            cmd = new SQLiteCommand("SELECT* FROM apps order by id DESC LIMIT 1", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String strID = reader["id"].ToString();
                appID = Convert.ToInt32(strID);
                this.Text = "Haufe Multi-SQLite for C# <ID: " + appID + ">";
                //this.Text = strID;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            frmMain frm1 = this;
            viewThread = new ViewThread(frm1, appID, strDatabaseFile);
        }

        private void btnShowContent_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            lbGeneral.Items.Clear();

            SQLiteCommand cmd = null;


            cmd = new SQLiteCommand("Select * from testtable", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String strAppID = reader["appID"].ToString();
                String strThreadID = reader["threadID"].ToString();
                String strText = (string)reader["text"];

                lbGeneral.Items.Add("AppID: " + strAppID + ", ThreadID: " + strThreadID + "   -    " + strText);
            }
        }

        private void btnStopThreads_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            threads.running = false;
            btnStartThreads.Enabled = !threads.running;
            btnStopThreads.Enabled = threads.running;
        }

        private void btnShowStatus_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            lbGeneral.Items.Clear();

            String strRunning = "NO";
            if (threads.running)
                strRunning = "YES";

            lbGeneral.Items.Add("Running: " + strRunning);

            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand("Select count(text) as cnt from testtable", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string str = "Count=" + reader["cnt"];

                lbGeneral.Items.Add(str);
            }
        }

        private void btnViewThread_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            Thread vt = new Thread(viewThread.view);
            viewThread.running = !viewThread.running;
            vt.Start();
        }

        private void tiUpdateApps_Tick(object sender, EventArgs e)
        {
            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand("update apps set tsLastPoll = CURRENT_TIMESTAMP where id=" + appID, con);
            cmd.ExecuteNonQuery();
        }

        private void tiPollApps_Tick(object sender, EventArgs e)
        {
            if (tcTabs.SelectedTab == tabPage2)
            {
                lbActiveApps.Items.Clear();
                
                string stm = "SELECT * from apps";

                SQLiteCommand cmd = null;


                cmd = new SQLiteCommand("Select id,name from apps where strftime('%s', 'now') - strftime('%s', tsLastPoll) < 30", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strID = (string)reader["id"].ToString();
                    string strName = (string)reader["name"].ToString();

                    lbActiveApps.Items.Add(strName + " " + strID);
                }
            }
            else if (tcTabs.SelectedTab == tsApps)
            {
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("Select distinct apps.id as AppID,apps.name as AppName from apps where strftime('%s', 'now') -strftime('%s', tsLastPoll)  < 30", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<TreeNode> activeNodes = new List<TreeNode>();
                while (reader.Read())
                {
                    string strID = (string)reader["AppID"].ToString();
                    string strName = (string)reader["AppName"].ToString();
                    TreeNode insertNode = new TreeNode(strName + " <ID: " + strID + ">");
                    NodeDefinition nd = new NodeDefinition();
                    nd.strAppID = strID;
                    nd.nodeType = NodeDefinition.NodeType.ntApp;
                    insertNode.Tag = nd;
                    string strInsertText = strName + " <ID: " + strID + ">";



                    bool bFound = false;
                    foreach (TreeNode node in treeApps.Nodes)
                    {
                        if (node.Text.Equals(strInsertText))
                            bFound = true;
                    }
                    if (!bFound)
                    {
                        treeApps.Nodes.Add(insertNode);
                    }


                    activeNodes.Add(insertNode);
                }
                for (int x = treeApps.Nodes.Count - 1; x >= 0; x--)
                {
                    bool bFound = false;

                    if (treeApps.Nodes[x].Nodes.Count == 0)
                        treeApps.Nodes[x].Nodes.Add("");

                    for (int y = 0; y < activeNodes.Count; y++)
                    {
                        if (activeNodes[y].Text == treeApps.Nodes[x].Text)
                            bFound = true;
                    }
                    if (!bFound)
                        treeApps.Nodes.RemoveAt(x);
                }
            }
        }

        private void treeApps_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

        }        

        private void treeApps_BeforeExpand_1(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            node.Nodes.Clear();

            NodeDefinition nd = (NodeDefinition)node.Tag;
            
            
            if (nd.nodeType == NodeDefinition.NodeType.ntApp)
            {
                NodeDefinition ndThreadCountHeadline = new NodeDefinition();
                ndThreadCountHeadline.nodeType = NodeDefinition.NodeType.ntThreadCountHeadline;
                ndThreadCountHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeThreadCountHeadline = new TreeNode("Count:");
                treeNodeThreadCountHeadline.Nodes.Add("");
                treeNodeThreadCountHeadline.Tag = ndThreadCountHeadline;
                node.Nodes.Add(treeNodeThreadCountHeadline);

                NodeDefinition ndThreadHeadline = new NodeDefinition();
                ndThreadHeadline.nodeType = NodeDefinition.NodeType.ntThreadHeadline;
                ndThreadHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeThreadHeadline = new TreeNode("Threads:");
                treeNodeThreadHeadline.Nodes.Add("");
                treeNodeThreadHeadline.Tag = ndThreadHeadline;
                node.Nodes.Add(treeNodeThreadHeadline);
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadCountHeadline)
            {
                NodeDefinition ndThreadCount = new NodeDefinition();
                ndThreadCount.nodeType = NodeDefinition.NodeType.ntThreadCount;
                ndThreadCount.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(distinct threadid) as CNT from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeThreadCount = new TreeNode(strCNT);
                    treeNodeThreadCount.Tag = ndThreadCount;
                    node.Nodes.Add(treeNodeThreadCount);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadHeadline)
            {

                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select distinct threadid from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NodeDefinition ndThread = new NodeDefinition();
                    ndThread.nodeType = NodeDefinition.NodeType.ntThread;
                    ndThread.strAppID = nd.strAppID;
                    string strThreadID = (string)reader["ThreadID"].ToString();                    
                    TreeNode treeNodeThread = new TreeNode("Thread: " + strThreadID);                    
                    ndThread.strThreadID = strThreadID;
                    treeNodeThread.Tag = ndThread;
                    treeNodeThread.Nodes.Add("");
                    node.Nodes.Add(treeNodeThread);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThread)
            {
                NodeDefinition ndEntry = new NodeDefinition();
                ndEntry.nodeType = NodeDefinition.NodeType.ntEntry;
                ndEntry.strAppID = nd.strAppID;
                ndEntry.strThreadID = nd.strThreadID;
                SQLiteCommand cmd = null;                
                cmd = new SQLiteCommand("select id,text,appid,threadid,tsCreated from testtable where appID=" + nd.strAppID + " and threadID=" +nd.strThreadID, con);                
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strID = (string)reader["ID"].ToString();
                    string strText = (string)reader["Text"].ToString();
                    string strThreadID = (string)reader["ThreadID"].ToString();
                    string strAppID = (string)reader["AppID"].ToString();
                    string strTSCreated = (string)reader["tsCreated"].ToString();
                    string str = "ID: " + strID + ", AppID: " + strAppID + ", ThreadID: " + strThreadID + ", Created: " + strTSCreated + " " + strText;
                    TreeNode treeNodeEntry = new TreeNode(str);
                    treeNodeEntry.Tag = ndEntry;
                    treeNodeEntry.Nodes.Add("");
                    node.Nodes.Add(treeNodeEntry);
                }
            }

            if (node.Nodes.Count == 0)
            node.Nodes.Add("");                
        }

        private void buttonStartThreads_Click(object sender, EventArgs e)
        {
            listThreads.Clear();

            tcTabs.SelectedIndex = 0;

            for (int i = 0;i<numThreads.Value;i++)
            {
                listThreads.Add(new Thread(threads.insert_thread_function));
            }
            threads.running = true;
            
            foreach(Thread thread in listThreads) {
                thread.Start();
            }

            btnStartThreads.Enabled = !threads.running;
            btnStopThreads.Enabled = threads.running;

            /*
            // Two different threads are created for simultaneous writing
            // each thread creates it's own instance of a database object (pooling)
            thr1 = new Thread(threads.m1);
            thr2 = new Thread(threads.m2);

            // The threads are set to state running, so that the infinite loop can be interrupted
            threads.running = true;

            // Both threads are started
            thr1.Start();
            thr2.Start();
            */
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            threads.running = false;
            if (con == null)
                return;
            execQuery("update apps set tsLastPoll = NULL where id = " + appID);
        }
    }

    // This class contains a thread that is constantly polling the database
    // And refreshing GUI-elements
    // The threads uses it's own database-object (pooling) and can be
    // executed in parallell to writing-threads
    public class ViewThread
    {
        SQLiteConnection con = null;
        public Boolean running = false;
        frmMain frm;
        private int appID;
        public ViewThread(frmMain frm, int appID, String strDatabaseFile)
        {
            string cs = "Data Source=" + strDatabaseFile + ";Version=3;Pooling=True;Max Pool Size=100;";
            con = new SQLiteConnection(cs);
            con.Open();
            this.frm = frm;
            this.appID = appID;
        }

        public void view()
        {
            while (running)
            {
                List<string> list = new List<String>();
                for (int i = 0; i < 3; i++)
                {
                    SQLiteCommand cmd = null;
                    String strCMD = "SELECT text FROM (SELECT * FROM testtable where threadID=" + i + " and appID=" + appID + " ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ";
                    //String strCMD = "SELECT text FROM (SELECT * FROM testtable where threadID=" + i + " ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ";
                    cmd = new SQLiteCommand(strCMD, con);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string str = "Count=" + reader["text"];
                        list.Add(str);
                    }

                }
                frm.AddListSafe(list);
            }
        }
    }

    public class CThreads
    {
        // Two different Connection-Objets
        SQLiteConnection con1 = null;
        SQLiteConnection con2 = null;

        static int maxThreadID;

        private int appID;

        // Control-Variable if thread is running
        public Boolean running = false;

        // Constructor cretes a separate connection for each thread
        public CThreads(int appID, String strDatabaseFile)
        {
            this.appID = appID;
            maxThreadID = 0;

            // First connection is opened, note Max Pool Size=100
            string cs = "Data Source=" + strDatabaseFile + ";Version=3;Pooling=True;Max Pool Size=100;";
            // First object is instantiated
            con1 = new SQLiteConnection(cs);
            con1.Open();

            // Second connection is opened, note Max Pool Size=100
            con2 = new SQLiteConnection(cs);
            con2.Open();
            SQLiteCommand cmd = null;
        }


        public void insert_thread_function()
        {
            int threadID = ++maxThreadID;
            
            SQLiteCommand cmd = null;

            // The first thread is writing to the database in an infinite loop
            // using it's own instance of the DB-connection
            // writing can be stopped when setting running = false
            while (running)
            {
                DateTime dt = DateTime.Now;
                String str = "";
                int nNow = (int)(DateTime.Now.Ticks % Int32.MaxValue);
                Random rand = new Random(nNow);
                do
                {
                    str = str + (char)rand.Next(65, 65 + 32);
                } while (rand.Next(1, 255) != 100);
                
                String strInsert = String.Format("insert into testtable (threadid,text,appid) values ({0},'{1}', {2})", threadID, str,  appID);
                cmd = new SQLiteCommand(strInsert, con1);
                cmd.ExecuteNonQuery();
            }
        }
    }

    class NodeDefinition
    {
        public enum NodeType { ntApp, ntThread, ntThreadHeadline, ntThreadCountHeadline, ntThreadCount, ntEntry };
        public NodeType nodeType;
        public string strAppID = "";
        public string strThreadID = "";
    }
}
