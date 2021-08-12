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
using System.Xml.Linq;
using static SQLiteTest.NodeDefinition;
using System.Runtime.InteropServices.ComTypes;




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
        static String strVersion = "";
        String strSQLiteVersion = "";
        String strDBAccessMode = "";
        const String DB_FILE = "Multisqlite.db";
        const int DB_REVISION = 3;

        private int appID = -1;
        string appName = "";

        static public String getVersion() { return strVersion; }

        private delegate void AddListDelegate(List<String> list);

        public void AddListSafe(List<String> list)
        {
            if (rePrompt.InvokeRequired)
            {
                var d = new AddListDelegate(AddListSafe);
                rePrompt.Invoke(d, new object[] { list });
            }
            else
            {
                rePrompt.Clear();
                foreach (string str in list)
                {
                    promptOut(str);
                }

            }
        }

        public frmMain()
        {
            InitializeComponent();


            appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

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
            promptOut(appName + " version: " + strVersion);
            lblVersion.Text = "Version: " + strVersion;


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
                    if (nRevision < DB_REVISION)
                    {
                        promptOut("Error: Old database....");
                        promptOut("       Deleting tables");
                        promptOut("");
                        execQuery("drop table if exists version");
                        execQuery("drop table if exists testtable");
                        execQuery("drop table if exists apps");
                    }
                }

                setSQLiteVersion();
                if (databaseAttributes == System.IO.FileAttributes.Offline)
                {
                    databaseAttributes = System.IO.File.GetAttributes(strDatabaseFile);
                    promptOut("Database created: " + strDatabaseFile);
                }
                else
                {
                    promptOut("Database opened: " + strDatabaseFile);
                }
                strDBAccessMode = "Database accessible for " + ((databaseAttributes == System.IO.FileAttributes.ReadOnly) ? " Read only" : "Read + Write");
                promptOut(strDBAccessMode);



                String strDeleteFrom = "";
                String strInsert = "";


                execQuery("Create Table if NOT Exists version (id INTEGER PRIMARY KEY AUTOINCREMENT, revision INTEGER) ");
                execQuery("Create Table if NOT Exists testtable (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP) ");
                execQuery("Create Table if NOT Exists apps (id INTEGER PRIMARY KEY AUTOINCREMENT, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  tsLastPoll TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name TEXT, isActive INTEGER DEFAULT FALSE) ");
                execQuery("Create Table if NOT Exists threads ( id INTEGER PRIMARY KEY AUTOINCREMENT, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP, isActive INTEGER DEFAULT FALSE ) ");
                cmd = new SQLiteCommand("update apps set tsLastPoll = CURRENT_TIMESTAMP where id=" + appID, con);
                execQuery("update apps set isActive=0 where tsLastPoll is null ");
                execQuery("insert into apps (name, isActive) values ('" + appName + "', true)");
                setAppID();


                // Create a table that can hold text-data along with the thread-id of the thread that created the data
                strInsert = String.Format("insert into testtable (appID,threadid,text) values ({0},0,'{1}')", appID, dt.ToString());
                execQuery(strInsert);
                execQuery("Delete from version");
                execQuery("insert into version (id,revision) values (0," + DB_REVISION + ")");


                promptOut("SQLite Version: " + strSQLiteVersion);
            }
            else
            {
                promptOut("Error: Could not opened database: " + strDatabaseFile);
            }

            threads = new CThreads(appID, strDatabaseFile);
            tiUpdateApps.Enabled = true;
            tiPollApps.Enabled = true;
            promptOut("");
            promptOut("Type in an SQL command that will be applied directly on the database.", Color.Lime);
            promptOut("", Color.Lime);
            promptOut("Type \"SELECT name FROM sqlite_master\" to show the structure of the database.", Color.Lime);
            promptOut("", Color.Lime);
            promptOut("Note: The commands will be executed on the GUI-thread.", Color.Lime);
            prompt();
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
            catch (Exception e)
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
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            frmMain frm1 = this;
            viewThread = new ViewThread(frm1, appID, strDatabaseFile);
        }

        bool getParentDirectoryContains(ref String strPath, String strSearch)
        {
            string str = strPath;
            if (str.Length == 0)
                return false;
            if (!str[str.Length - 1].Equals("\\"))
                str = strPath + "\\";

            string[] searchArr = strSearch.Split('/');

            if (searchArr.Length == 0)
                return false;

            while (Directory.Exists(str = System.IO.Path.Combine(str, @"..\")))
            {
                DirectoryInfo di = System.IO.Directory.GetParent(str);
                if (di == null)
                    return false;
                str = di.FullName;
                if (str[str.Length - 1] != '\\')
                    str = str + "\\";

                string strDoc = str;
                for (int i = 0; i < searchArr.Length - 1; i++)
                    strDoc = strDoc + searchArr[i] + "\\";

                if (Directory.Exists(str))
                {
                    strDoc = strDoc + searchArr[searchArr.Length - 1];
                    if (File.Exists(strDoc))
                    {
                        strPath = strDoc;
                        return true;
                    }
                }
            }
            return false;
        }

        private void btnShowContent_Click(object sender, EventArgs e)
        {

        }

        private void btnStopThreads_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedTab = tsApps;
            threads.running = false;
            btnStartThreads.Enabled = !threads.running;
            btnStopThreads.Enabled = threads.running;
            mnuStartThread.Enabled = btnStartThreads.Enabled;
            mnuStopThread.Enabled = btnStopThreads.Enabled;
            lblNumThreads.Enabled = !threads.running;
            numThreads.Enabled = !threads.running;
        }

        void promptOut(String str, Color color)
        {
            int selStart = rePrompt.TextLength;
            rePrompt.AppendText(str + "\r\n");
            int selEnd = rePrompt.TextLength;
            rePrompt.SelectionStart = selStart;
            rePrompt.SelectionLength = selEnd - selStart;
            rePrompt.SelectionColor = color;
            rePrompt.ScrollToCaret();
        }

        void promptOut(String str)
        {
            promptOut(str, Color.White);
        }

        void prompt()
        {
            rePrompt.AppendText("\r\n" + "$:> ");
            rePrompt.ScrollToCaret();
        }

        String unprompt(String str)
        {
            str.Trim();
            if (str.Length < 3)
                return str;
            if (str.ToUpper().Substring(0, 3).Equals("$:>"))
                str = str.Substring(3, str.Length - 3).Trim();
            return str;
        }

        private void btnShowStatus_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            rePrompt.Clear();

            String strRunning = "NO";
            if (threads.running)
                strRunning = "YES";

            rePrompt.Clear();
            promptOut("Running: " + strRunning);

            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand("Select count(text) as cnt from testtable", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string str = "Count=" + reader["cnt"];

                promptOut(str);
            }
        }

        void updateNodeRecursive(TreeNode node)
        {
            updateNode(node, NodeDefinition.NodeAction.LiveView);
            foreach (TreeNode subNode in node.Nodes)
            {
                updateNodeRecursive(subNode);
            }
        }

        private void btnViewThread_Click(object sender, EventArgs e)
        {
            tiLiveUpdate.Enabled = !tiLiveUpdate.Enabled;

            if (tiLiveUpdate.Enabled)
                btnLiveUpdate.BackColor = Color.LightBlue;
            else
            {
                btnLiveUpdate.BackColor = SystemColors.ButtonFace;
                btnLiveUpdate.UseVisualStyleBackColor = true;
            }

            /*
            tcTabs.SelectedIndex = 0;
            Thread vt = new Thread(viewThread.view);
            viewThread.running = !viewThread.running;
            return;
            btnLiveUpdate.Enabled = viewThread.running;
            vt.Start();
            */
        }

        private void tiUpdateApps_Tick(object sender, EventArgs e)
        {
            lblVersion.ForeColor = Color.Red;
            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand("update apps set tsLastPoll = CURRENT_TIMESTAMP where id=" + appID, con);
            cmd.ExecuteNonQuery();
            cmd = new SQLiteCommand("update apps set isActive=0 where strftime('%s', 'now') -strftime('%s', tsLastPoll)  > 30 ", con);
            cmd.ExecuteNonQuery();
            tiConnectionQuery.Enabled = true;
        }

        private void tiPollApps_Tick(object sender, EventArgs e)
        {
            if (tcTabs.SelectedTab == tsApps)
            {
                List<TreeNode> activeNodes = new List<TreeNode>();
                bool bFound = false;


                TreeNode nodeAppHeadline = NodeDefinition.Add(NodeDefinition.NodeType.ntAppHeadline, "Active Apps", true, treeApps.Nodes, ref activeNodes, Color.Black, "", "");
                nodeAppHeadline.Expand();

                NodeDefinition.Add(NodeDefinition.NodeType.ntTotalStatusHeadline, "Total Statistics:", true, treeApps.Nodes, ref activeNodes, Color.SteelBlue, "", "");
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusHeadline, "Status:", true, treeApps.Nodes, ref activeNodes, Color.Coral, "", "");

                treeApps_BeforeExpand_1(sender, new TreeViewCancelEventArgs(nodeAppHeadline, false, new TreeViewAction()));
            }

        }

        private void treeApps_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

        }



        private void buttonStartThreads_Click(object sender, EventArgs e)
        {
            listThreads.Clear();

            tcTabs.SelectedTab = tsApps;

            for (int i = 0; i < numThreads.Value; i++)
            {
                listThreads.Add(new Thread(threads.insert_thread_function));
            }
            threads.running = true;

            foreach (Thread thread in listThreads)
            {
                thread.Start();
            }

            btnStartThreads.Enabled = !threads.running;
            lblNumThreads.Enabled = !threads.running;
            numThreads.Enabled = !threads.running;
            mnuStartThread.Enabled = btnStartThreads.Enabled;
            btnStopThreads.Enabled = threads.running;
            mnuStopThread.Enabled = btnStopThreads.Enabled;
        }

        void updateNode(TreeNode node, NodeDefinition.NodeAction nodeAction)
        {
            NodeDefinition nd = (NodeDefinition)node.Tag;
            if (nd == null)
                return;
            if ((nodeAction == NodeDefinition.NodeAction.LiveView) && (!NodeDefinition.isLiveable(nd.nodeType)))
                return;
            if (nodeAction == NodeDefinition.NodeAction.LiveView)
            {
                TreeNode currentNode = node;
                while (currentNode.Parent!=null)
                {
                    currentNode = currentNode.Parent;
                    if (currentNode.IsExpanded == false)
                        return;
                }
            }

            if (nd.nodeType == NodeDefinition.NodeType.ntAppHeadline)
            {
                bool bFound = false;
                List<TreeNode> activeNodes = new List<TreeNode>();
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("Select distinct apps.id as AppID,apps.name as AppName from apps where isActive=1 ", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strID = (string)reader["AppID"].ToString();
                    string strName = (string)reader["AppName"].ToString();

                    NodeDefinition.Add(NodeDefinition.NodeType.ntApp, " <ID: " + strID + ">", true, node.Nodes, ref activeNodes, Color.Black, strID);
                }

                NodeDefinition.removeInactives(node.Nodes, ref activeNodes, NodeDefinition.NodeType.ntApp);
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntApp)
            {
                node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                NodeDefinition.Add(NodeDefinition.NodeType.ntCountThreadHeadline, "Threads Count:", true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID);
                NodeDefinition.Add(NodeDefinition.NodeType.ntCountAppEntriesHeadline, "Entries Count:", true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID);
                NodeDefinition.Add(NodeDefinition.NodeType.ntThreadHeadline, "Threads:", true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID);
                NodeDefinition.Add(NodeDefinition.NodeType.ntAppThroughputHeadline, "Throughput:", true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID);
            }

            else if (nd.nodeType == NodeType.ntTotalStatusHeadline)
            {
                node.Nodes.Clear();
                List<TreeNode> activeNodes = null;

                NodeDefinition.Add(NodeDefinition.NodeType.ntCountTotalEntriesHeadline, "Total Entries Count:", true, node.Nodes, ref activeNodes, nd.parentColor, "", "");
                NodeDefinition.Add(NodeDefinition.NodeType.ntCountTotalThreadsActiveHeadline, "Total Active Threads Count:", true, node.Nodes, ref activeNodes, nd.parentColor, "", "");
                NodeDefinition.Add(NodeDefinition.NodeType.ntTotalThroughputHeadline, "Total Throughput:", true, node.Nodes, ref activeNodes, nd.parentColor);
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountThreadHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(distinct threadid) as CNT from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntTotalThreadCount, strCNT, true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadHeadline)
            {
                node.Nodes.Clear();
                List<TreeNode> activeNodes = null;

                NodeDefinition ndThread = new NodeDefinition();

                TreeNode treeNodeThread = NodeDefinition.Add(NodeDefinition.NodeType.ntThread, "GUI-Thread", true, node.Nodes, ref activeNodes, Color.Black, nd.strAppID, "0");

                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select distinct threadid, isActive from threads where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strThreadID = (string)reader["ThreadID"].ToString();
                    string strIsActive = (string)reader["isActive"].ToString();
                    Color color = Color.Black;
                    if (strIsActive.Equals("0"))
                    {
                        color = Color.Red;
                    }
                    else
                    {
                        color = Color.Green;
                    }

                    NodeDefinition.Add(NodeDefinition.NodeType.ntThread, "Thread #" + strThreadID, true, node.Nodes, ref activeNodes, color, nd.strAppID, strThreadID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThread)
            {
                node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                NodeDefinition.Add(NodeDefinition.NodeType.ntEntryHeadline, "Entries:", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                NodeDefinition.Add(NodeDefinition.NodeType.ntCountThreadEntriesHeadline, "Count Thread Entries:", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                NodeDefinition.Add(NodeDefinition.NodeType.ntThreadThroughputHeadline, "Throughput:", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntEntryHeadline)
            {
                node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select id,text,appid,threadid,tsCreated from testtable where appID=" + nd.strAppID + " and threadID=" + nd.strThreadID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strID = (string)reader["ID"].ToString();
                    string strText = (string)reader["Text"].ToString();
                    string strThreadID = (string)reader["ThreadID"].ToString();
                    string strAppID = (string)reader["AppID"].ToString();
                    string strTSCreated = (string)reader["tsCreated"].ToString();
                    string str = "ID: " + strID + ", AppID: " + strAppID + ", ThreadID: " + strThreadID + ", Created: " + strTSCreated + " " + strText;
                    NodeDefinition.Add(NodeDefinition.NodeType.ntEntry, str, true, node.Nodes, ref activeNodes, nd.parentColor, strAppID, strThreadID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountAppEntriesHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntCountAppEntries, strCNT, true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountThreadEntriesHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable where appID=" + nd.strAppID + " and threadID=" + nd.strThreadID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntCountThreadEntries, strCNT, true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountTotalEntriesHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntCountTotalEntries, strCNT, true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                }
            }


            else if (nd.nodeType == NodeDefinition.NodeType.ntCountTotalThreadsActiveHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(distinct th.appID|| ',' || th.threadid) as CNT from testtable tt,apps ap, threads th where tt.appID = th.appID and tt.threadID = th.threadID and th.isActive = 1 and ap.id = th.appID and ap.isActive=1", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntCountTotalThreadsActive, strCNT, true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);

                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadThroughputHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID={0} and tt.threadID={1} and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1", nd.strAppID, nd.strThreadID);
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntThreadThroughput, strCNT + " Inserts / sec.", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntAppThroughputHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID={0} and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1", nd.strAppID);
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntThreadThroughput, strCNT + " Inserts / sec.", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                    //currentNode.Text = strCNT + " Inserts / sec.";
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntTotalThroughputHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID in (select id from apps where isActive=1) and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1");
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    NodeDefinition.Add(NodeDefinition.NodeType.ntTotalThroughput, strCNT + " Inserts / sec.", true, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID);
                }
            }

            else if (nd.nodeType == NodeType.ntStatusHeadline)
            {
                //node.Nodes.Clear();
                List<TreeNode> activeNodes = null;
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(strDatabaseFile);
                var versionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                String strDebugLevel;
                #if DEBUG
                    strDebugLevel = "Debug";
                #else
                    strDebugLevel = "Relase";
                #endif

                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, "Application: " + appName + " version: " + strVersion + " (" + strDebugLevel + ")", false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeDefinition.NodeType.ntStatusItem.ToString() + "_1") ;
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, "Application Instance: " + appID, false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeDefinition.NodeType.ntStatusItem.ToString() + "_2");                
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, "Database File: " + strDatabaseFile, false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeDefinition.NodeType.ntStatusItem.ToString() + "_3");
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, "SQLite-Version: " + strSQLiteVersion, false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeDefinition.NodeType.ntStatusItem.ToString() + "_4");
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, strDBAccessMode, false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeDefinition.NodeType.ntStatusItem.ToString() + "_5");                
                NodeDefinition.Add(NodeDefinition.NodeType.ntStatusItem, "Database-Size: " + GetBytesReadable(fileInfo.Length), false, node.Nodes, ref activeNodes, nd.parentColor, nd.strAppID, nd.strThreadID, NodeType.ntStatusFileSize.ToString());                
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntTotalStatusHeadline)
            {

            }


            if (node.Nodes.Count == 0)
                node.Nodes.Add("");
        }

        private void treeApps_BeforeExpand_1(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            updateNode(node, NodeDefinition.NodeAction.Update);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            threads.running = false;
            if (con == null)
                return;
            execQuery("update apps set isActive=0 where id = " + appID);
        }

        private void tcTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            tiPollApps_Tick(null, null);
        }

        private void tiConnectionQuery_Tick(object sender, EventArgs e)
        {
            lblVersion.ForeColor = Color.Green;
            tiConnectionQuery.Enabled = false;
        }

        private void btnShowManual_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            string strPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string strViewer = strPath;
            string strManual = strPath;

            if (!getParentDirectoryContains(ref strViewer, "Tools/Viewer.exe"))
            {
                MessageBox.Show("Error: The viwer for the user's manual was not found.");
                return;
            }

            if (!getParentDirectoryContains(ref strManual, "Doc/Haufe_MultiSQLite_CS_Manual.pdf"))
            {
                MessageBox.Show("Error: The user's manual was not found.");
                return;
            }
            proc.StartInfo.FileName = strViewer;
            proc.StartInfo.Arguments = strManual;
            proc.Start();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        void printTable(List<List<string>> table)
        {
            int selStart = rePrompt.TextLength;
            String str = "";
            foreach (List<string> column in table)
            {
                string strCol = "";
                foreach (string strField in column)
                {
                    if (strCol.Length > 0)
                        strCol += ";";
                    strCol = strCol + strField;
                }
                str += strCol + "\n";
            }
            int selEnd = rePrompt.TextLength;
            rePrompt.SelectionStart = selStart;
            rePrompt.SelectionLength = selEnd - selStart;
            rePrompt.SelectionColor = Color.White;
            rePrompt.SelectedText = str;

        }

        private void rePrompt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rePrompt.SelectionStart = rePrompt.Text.Length;
                int cursorPosition = rePrompt.SelectionStart;
                int cmdIndex = rePrompt.GetLineFromCharIndex(cursorPosition) - 2;
                if (cmdIndex < 0)
                    return;
                String strCMD = unprompt(rePrompt.Lines[cmdIndex]);
                if (strCMD.Trim().Length == 0)
                    return;
                SQLiteCommand cmd = null;
                if (con == null)
                {
                    promptOut("");
                    promptOut("Error: No Database Connection", Color.OrangeRed);
                    prompt();
                    return;
                }
                cmd = new SQLiteCommand(strCMD, con);
                try
                {
                    List<List<string>> table = new List<List<string>>();
                    List<string> columns = new List<string>();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string strColumn = reader.GetName(i);
                        columns.Add(strColumn);
                    }
                    table.Add(columns.GetRange(0, columns.Count));
                    while ((reader != null) && (reader.Read()))
                    {
                        columns.Clear();
                        foreach (String strCol in table[0])
                        {
                            string str = (string)reader[strCol].ToString();
                            columns.Add(str);
                        }
                        table.Add(columns.GetRange(0, columns.Count));
                    }
                    printTable(table);
                    prompt();
                }
                catch (Exception ex)
                {

                    promptOut("");
                    promptOut("Error: The SQLite-database throws the following error:\n" + ex.Message + "\n", Color.OrangeRed);
                    prompt();
                    return;
                }
            }
        }

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        private void tiLiveUpdate_Tick(object sender, EventArgs e)
        {
            tiLiveUpdate.Enabled = false;
            btnLiveUpdate.UseVisualStyleBackColor = true;            
            btnLiveUpdate.BackColor = Color.DarkBlue;
            btnLiveUpdate.Invalidate();
            btnLiveUpdate.Update();
            btnLiveUpdate.Refresh();          
            System.Windows.Forms.Application.DoEvents();
            tiLiveUpdateFlicker.Enabled = true;
            foreach (TreeNode node in treeApps.Nodes)
            {
                updateNodeRecursive(node);
            }
            tiLiveUpdate.Enabled = true; ;
        }

        private void btnLiveUpdate_Click(object sender, EventArgs e)
        {

            tiLiveUpdate.Enabled = !tiLiveUpdate.Enabled;

            if (tiLiveUpdate.Enabled)
            {                
                btnLiveUpdate.BackColor = Color.LightBlue;
                mnuLiveUpdate.Checked = true;
            }
            else
            {
                btnLiveUpdate.BackColor = SystemColors.ButtonFace;
                btnLiveUpdate.UseVisualStyleBackColor = true;
                mnuLiveUpdate.Checked = false;
                tiLiveUpdateFlicker.Enabled = false;
            }

            /*
            tcTabs.SelectedIndex = 0;
            Thread vt = new Thread(viewThread.view);
            viewThread.running = !viewThread.running;
            return;
            btnLiveUpdate.Enabled = viewThread.running;
            vt.Start();
            */
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuExit_DropDownOpened(object sender, EventArgs e)
        {
            
        }

        private void menuExit_CheckStateChanged(object sender, EventArgs e)
        {
      
        }

        private void mnuStartThread_Click(object sender, EventArgs e)
        {
            buttonStartThreads_Click(sender, e);
        }

        private void mnuStopThread_Click(object sender, EventArgs e)
        {
            btnStopThreads_Click(sender, e);
        }

        private void mnuLiveUpdate_Click(object sender, EventArgs e)
        {
            btnLiveUpdate_Click(sender, e);
        }

        private void tiLiveUpdateFlicker_Tick(object sender, EventArgs e)
        {
            btnLiveUpdate.BackColor = Color.LightBlue;
            tiLiveUpdateFlicker.Enabled = false;
        }

        private void mnuShowManual_Click(object sender, EventArgs e)
        {
            btnShowManual_Click(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout frmAbout = new FormAbout();
            frmAbout.StartPosition = FormStartPosition.CenterScreen;
            frmAbout.ShowDialog();
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

                String strStartThread = String.Format("insert into threads (threadid,appID,isActive) values ({0},'{1}',1)", threadID, appID);
                cmd = new SQLiteCommand(strStartThread, con1);
                cmd.ExecuteNonQuery();

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

                    String strInsert = String.Format("insert into testtable (threadid,text,appid) values ({0},'{1}', {2})", threadID, str, appID);
                    cmd = new SQLiteCommand(strInsert, con1);
                    cmd.ExecuteNonQuery();
                }

                String strStopThread = String.Format("update threads set isActive=0 where threadID={0} and appID={1} ", threadID, appID);
                cmd = new SQLiteCommand(strStopThread, con1);
                cmd.ExecuteNonQuery();
            }
        }

        class NodeDefinition
        {
            public enum NodeType
            {
                ntAppHeadline,
                ntApp,
                ntCountTotalEntriesHeadline,
                ntCountTotalEntries,
                ntCountTotalThreadsActiveHeadline,
                ntCountTotalThreadsActive,
                ntThread,
                ntThreadHeadline,
                ntCountThreadHeadline,
                ntTotalThreadCount,
                ntEntryHeadline,
                ntEntry,
                ntCountAppEntriesHeadline,
                ntCountAppEntries,
                ntCountThreadEntriesHeadline,
                ntCountThreadEntries,
                ntThreadThroughputHeadline,
                ntThreadThroughput,
                ntAppThroughputHeadline,
                ntAppThroughput,
                ntTotalThroughputHeadline,
                ntTotalThroughput,
                ntTotalStatusHeadline,
                ntTotalStatus,
                ntStatusHeadline,
                ntStatusItem,
                
                ntStatusFileSize

            };

            public enum NodeAction { LiveView, Update };

            public NodeType nodeType;
            public string strAppID = "";
            public string strThreadID = "";
            public string strID = "";
            public Color parentColor = Color.Black;

            static public bool isLiveable(NodeType type)
            {
                switch (type)
                {
                    case NodeType.ntThreadThroughputHeadline:
                    case NodeType.ntAppThroughputHeadline:
                    case NodeType.ntTotalThroughputHeadline:
                    case NodeType.ntStatusHeadline:                    
                    case NodeType.ntCountTotalThreadsActiveHeadline:
                    case NodeType.ntCountTotalEntriesHeadline:
                    case NodeType.ntCountAppEntriesHeadline:
                    case NodeType.ntCountThreadHeadline:
                    case NodeType.ntCountThreadEntriesHeadline:

                        //case NodeType.ntThreadThroughput:
                    return true;
                }
                return false;
            }

            static public void removeInactives(TreeNodeCollection nodes, ref List<TreeNode> activeNodes, NodeType type)
            {
                bool bFound = false;

                switch (type)
                {
                    case NodeDefinition.NodeType.ntApp:
                        {
                            for (int x = nodes.Count - 1; x >= 0; x--)
                            {
                                bFound = false;

                                if (nodes[x].Nodes.Count == 0)
                                    nodes[x].Nodes.Add("");

                                for (int y = 0; y < activeNodes.Count; y++)
                                {
                                    if (activeNodes[y].Text.Equals(nodes[x].Text))
                                        bFound = true;
                                }
                                if (!bFound)
                                    nodes.RemoveAt(x);
                            }
                            break;
                        }
                }
            }

            static public TreeNode Add(NodeType nodeType, String name, bool expandable, TreeNodeCollection nodes, ref List<TreeNode> activeNodes, Color color, String strAppID = "", String strThreadID = "", String strID = "")
            {
                TreeNode node = new TreeNode(name);
                if (expandable)
                {
                    node.Nodes.Add("");
                }
                NodeDefinition nd = new NodeDefinition();
                nd.strAppID = strAppID;
                nd.strThreadID = strThreadID;
                nd.strID = strID;
                nd.nodeType = nodeType;
                nd.parentColor = color;
                node.Tag = nd;
                node.ForeColor = color;
                String strNodeText = node.Text;
                Color nodeColor = node.ForeColor;
                for (int i = nodes.Count - 1; i >= 0; i--)
                    if (nodes[i].Text.Length == 0)
                        nodes.RemoveAt(i);
                Add(ref node, nodes, ref activeNodes);
                node.Text = strNodeText;
                node.ForeColor = nodeColor;
                return node;
            }
            static public void Add(ref TreeNode newNode, TreeNodeCollection nodes, ref List<TreeNode> activeNodes)
            {
                bool bFound = false;

                foreach (TreeNode node in nodes)
                {
                    NodeDefinition nd = (NodeDefinition)node.Tag;
                    switch (((NodeDefinition)newNode.Tag).nodeType)
                    {
                        case NodeType.ntAppHeadline:
                        case NodeType.ntCountTotalEntriesHeadline:
                        case NodeType.ntCountTotalThreadsActiveHeadline:
                        case NodeType.ntCountTotalThreadsActive:
                        case NodeType.ntTotalThroughputHeadline:
                        case NodeType.ntTotalThreadCount:
                        case NodeType.ntThreadHeadline:

                        case NodeType.ntEntryHeadline:
                        case NodeType.ntCountThreadEntriesHeadline:
                        case NodeType.ntThreadThroughputHeadline:

                        case NodeType.ntCountThreadHeadline:
                        case NodeType.ntCountAppEntriesHeadline:

                        case NodeType.ntCountAppEntries:
                        case NodeType.ntCountThreadEntries:
                        case NodeType.ntCountTotalEntries:

                        //case NodeType.ntCountTotalThreadsActiveHeadline:                                       
                        case NodeType.ntAppThroughputHeadline:
                        case NodeType.ntTotalThroughput:

                        case NodeType.ntTotalStatusHeadline:
                        case NodeType.ntStatusHeadline:

                            if (((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType)
                            {
                                bFound = true;
                                newNode = node;
                            }
                            break;
                        case NodeDefinition.NodeType.ntApp:
                        case NodeType.ntAppThroughput:

                            if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                               (((NodeDefinition)(newNode.Tag)).strAppID == nd.strAppID))
                            {
                                bFound = true;
                                newNode = node;
                            }
                            break;
                        case NodeType.ntThread:
                        case NodeType.ntThreadThroughput:
                            if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                               (((NodeDefinition)(newNode.Tag)).strAppID == nd.strAppID) &&
                                (((NodeDefinition)(newNode.Tag)).strThreadID == nd.strThreadID))
                            {
                                bFound = true;
                                newNode = node;
                            }
                            break;
                        //( 
                        case NodeType.ntEntry:                        
                            if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                                (((NodeDefinition)(newNode.Tag)).strID == nd.strID))
                            {
                                bFound = false;                            
                            }
                            break;                    
                    case NodeType.ntStatusItem:
                    case NodeType.ntStatusFileSize:
                        if ((((NodeDefinition)(newNode.Tag)).nodeType == nd.nodeType) &&
                            (((NodeDefinition)(newNode.Tag)).strID == nd.strID))
                        {
                            bFound = true;
                            newNode = node;
                        }
                        break;
                }
                    if (bFound == true)
                        break;
                }
                if (!bFound)
                {
                    nodes.Add(newNode);
                }
                if (activeNodes != null)
                    activeNodes.Add(newNode);

            }
        }
    }

