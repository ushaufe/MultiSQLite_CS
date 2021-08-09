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
        const String DB_FILE = "Multisqlite.db";
        const int DB_REVISION = 3;

        private int appID = -1;

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
                    if (nRevision < DB_REVISION )
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
                promptOut("Database accessible for " + ((databaseAttributes == System.IO.FileAttributes.ReadOnly) ? " Read only" : "Read + Write"));



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
                    strDoc = strDoc  + searchArr[searchArr.Length-1];
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
        }

        void promptOut(String str, Color color)
        {
            int selStart = rePrompt.TextLength;
            rePrompt.AppendText(str + "\r\n" );
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
                str= str.Substring(3, str.Length - 3).Trim();
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

        private void btnViewThread_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            Thread vt = new Thread(viewThread.view);
            viewThread.running = !viewThread.running;
            btnViewThread.Enabled = viewThread.running;
            vt.Start();
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

                
                NodeDefinition ndAppHeadline = new NodeDefinition();
                ndAppHeadline.nodeType = NodeDefinition.NodeType.ntAppHeadline;
                TreeNode nodeAppHeadline = new TreeNode("Active Apps");
                nodeAppHeadline.Nodes.Add("");
                nodeAppHeadline.Tag = ndAppHeadline;                
                foreach (TreeNode node1 in treeApps.Nodes)
                {
                    if (node1.Text.Equals(nodeAppHeadline.Text))                        
                    {
                        bFound = true;
                        nodeAppHeadline = node1;
                        break;
                    }
                }
                if (!bFound)
                {
                    treeApps.Nodes.Add(nodeAppHeadline);                   
                }
                activeNodes.Add(nodeAppHeadline);
                nodeAppHeadline.Expand();
                
                
                NodeDefinition ndCountTotalEntriesHeadline = new NodeDefinition();
                ndCountTotalEntriesHeadline.nodeType = NodeDefinition.NodeType.ntCountTotalEntriesHeadline;
                TreeNode nodeCountTotalEntriesHeadline = new TreeNode("Total Entries Count:");
                nodeCountTotalEntriesHeadline.Nodes.Add("");
                nodeCountTotalEntriesHeadline.Tag = ndCountTotalEntriesHeadline;
                foreach (TreeNode node2 in nodeCountTotalEntriesHeadline.Nodes)
                {
                    if (node2.Text.Equals(nodeCountTotalEntriesHeadline.Text))
                    {
                        bFound = true;
                        nodeCountTotalEntriesHeadline = node2;
                        break;
                    }
                }
                if (!bFound)
                {
                    treeApps.Nodes.Add(nodeCountTotalEntriesHeadline);
                }                
                activeNodes.Add(nodeCountTotalEntriesHeadline);

                NodeDefinition ndCountTotalThreadsActiveHeadline = new NodeDefinition();
                ndCountTotalThreadsActiveHeadline.nodeType = NodeDefinition.NodeType.ntCountTotalThreadsActiveHeadline;
                TreeNode nodeCountTotalThreadsActiveHeadline = new TreeNode("Total Active Threads Count:");
                nodeCountTotalThreadsActiveHeadline.Nodes.Add("");
                nodeCountTotalThreadsActiveHeadline.Tag = ndCountTotalThreadsActiveHeadline;
                foreach (TreeNode node3 in nodeCountTotalThreadsActiveHeadline.Nodes)
                {
                    if (node3.Text.Equals(nodeCountTotalThreadsActiveHeadline.Text))
                    {
                        bFound = true;
                        nodeCountTotalThreadsActiveHeadline = node3;
                        break;
                    }
                }
                if (!bFound)
                {
                    treeApps.Nodes.Add(nodeCountTotalThreadsActiveHeadline);
                }
                activeNodes.Add(nodeCountTotalThreadsActiveHeadline);

                //ntTotalThroughputHeadline
                NodeDefinition ndTotalThroughputHeadlineHeadline = new NodeDefinition();
                ndTotalThroughputHeadlineHeadline.nodeType = NodeDefinition.NodeType.ntTotalThroughputHeadline;
                TreeNode nodeTotalThroughputHeadlineHeadline = new TreeNode("Total Throughput:");
                nodeTotalThroughputHeadlineHeadline.Nodes.Add("");
                nodeTotalThroughputHeadlineHeadline.Tag = ndTotalThroughputHeadlineHeadline;
                foreach (TreeNode node3 in nodeTotalThroughputHeadlineHeadline.Nodes)
                {
                    if (node3.Text.Equals(nodeTotalThroughputHeadlineHeadline.Text))
                    {
                        bFound = true;
                        nodeTotalThroughputHeadlineHeadline = node3;
                        break;
                    }
                }
                if (!bFound)
                {
                    treeApps.Nodes.Add(nodeTotalThroughputHeadlineHeadline);
                }
                activeNodes.Add(nodeTotalThroughputHeadlineHeadline);


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
        }

        private void treeApps_BeforeExpand_1(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;                      
            NodeDefinition nd = (NodeDefinition)node.Tag;


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
                    TreeNode insertNode = new TreeNode(strName + " <ID: " + strID + ">");
                    NodeDefinition ndInsertNode = new NodeDefinition();
                    ndInsertNode.strAppID = strID;
                    ndInsertNode.nodeType = NodeDefinition.NodeType.ntApp;
                    insertNode.Tag = ndInsertNode;
                    string strInsertText = strName + " <ID: " + strID + ">";

                    bFound = false;
                    foreach (TreeNode no in node.Nodes)
                    {
                        if (no.Text.Equals(strInsertText))
                            bFound = true;
                    }
                    if (!bFound)
                    {
                        node.Nodes.Add(insertNode);
                    }


                    activeNodes.Add(insertNode);
                }
                
                for (int x = node.Nodes.Count - 1; x >= 0; x--)
                {
                    bFound = false;

                    if (node.Nodes[x].Nodes.Count == 0)
                        node.Nodes[x].Nodes.Add("");

                    for (int y = 0; y < activeNodes.Count; y++)
                    {
                        if (activeNodes[y].Text.Equals(node.Nodes[x].Text))
                            bFound = true;
                    }
                    if (!bFound)
                        node.Nodes.RemoveAt(x);
                }
                
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntApp)
            {
                node.Nodes.Clear();
                NodeDefinition ndCountThreadHeadline = new NodeDefinition();
                ndCountThreadHeadline.nodeType = NodeDefinition.NodeType.ntCountThreadHeadline;
                ndCountThreadHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeCountThreadHeadline = new TreeNode("Threads Count:");
                treeNodeCountThreadHeadline.Nodes.Add("");
                treeNodeCountThreadHeadline.Tag = ndCountThreadHeadline;
                node.Nodes.Add(treeNodeCountThreadHeadline);

                NodeDefinition ndCountAppEntriesHeadline = new NodeDefinition();
                ndCountAppEntriesHeadline.nodeType = NodeDefinition.NodeType.ntCountAppEntriesHeadline;
                ndCountAppEntriesHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeCountAppEntriesHeadline = new TreeNode("Entries Count:");
                treeNodeCountAppEntriesHeadline.Nodes.Add("");
                treeNodeCountAppEntriesHeadline.Tag = ndCountAppEntriesHeadline;
                node.Nodes.Add(treeNodeCountAppEntriesHeadline);

                NodeDefinition ndThreadHeadline = new NodeDefinition();
                ndThreadHeadline.nodeType = NodeDefinition.NodeType.ntThreadHeadline;
                ndThreadHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeThreadHeadline = new TreeNode("Threads:");
                treeNodeThreadHeadline.Nodes.Add("");
                treeNodeThreadHeadline.Tag = ndThreadHeadline;
                node.Nodes.Add(treeNodeThreadHeadline);

                NodeDefinition ndAppThroughputHeadline = new NodeDefinition();
                ndAppThroughputHeadline.nodeType = NodeDefinition.NodeType.ntAppThroughputHeadline;
                ndAppThroughputHeadline.strAppID = nd.strAppID;
                TreeNode treeNodeThroughputHeadline = new TreeNode("Throughput:");
                treeNodeThroughputHeadline.Nodes.Add("");
                treeNodeThroughputHeadline.Tag = ndAppThroughputHeadline;
                node.Nodes.Add(treeNodeThroughputHeadline);
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountThreadHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndTotalThreadCount = new NodeDefinition();
                ndTotalThreadCount.nodeType = NodeDefinition.NodeType.ntTotalThreadCount;
                ndTotalThreadCount.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(distinct threadid) as CNT from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeTotalThreadCount = new TreeNode(strCNT);
                    treeNodeTotalThreadCount.Tag = ndTotalThreadCount;
                    node.Nodes.Add(treeNodeTotalThreadCount);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndThread = new NodeDefinition();
                ndThread.nodeType = NodeDefinition.NodeType.ntThread;
                TreeNode treeNodeThread = new TreeNode("GUI-Thread");
                treeNodeThread.ForeColor = Color.Black;
                treeNodeThread.Nodes.Add("");
                ndThread.strThreadID = "0";
                ndThread.strAppID = nd.strAppID;
                ndThread.parentColor = Color.Black;
                treeNodeThread.Tag = ndThread;
                node.Nodes.Add(treeNodeThread);
               
                SQLiteCommand cmd = null;                
                cmd = new SQLiteCommand("select distinct threadid, isActive from threads where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ndThread = new NodeDefinition();
                    ndThread.nodeType = NodeDefinition.NodeType.ntThread;
                    ndThread.strAppID = nd.strAppID;
                    string strThreadID = (string)reader["ThreadID"].ToString();
                    string strIsActive = (string)reader["isActive"].ToString();
                    treeNodeThread = new TreeNode("");                                        
                    if (strIsActive.Equals("0"))
                    {                        
                        treeNodeThread.ForeColor = Color.Red;
                        ndThread.parentColor = Color.Red;
                    }
                    else
                    {                        
                        treeNodeThread.ForeColor = Color.Green;
                        ndThread.parentColor = Color.Green;
                    }                    
                    treeNodeThread.Text = "Thread #" + strThreadID;
                    ndThread.strThreadID = strThreadID;
                    treeNodeThread.Tag = ndThread;
                    treeNodeThread.Nodes.Add("");
                    node.Nodes.Add(treeNodeThread);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThread)
            {
                node.Nodes.Clear();
                NodeDefinition ndEntryHeadline = new NodeDefinition();
                ndEntryHeadline.nodeType = NodeDefinition.NodeType.ntEntryHeadline;
                ndEntryHeadline.strAppID = nd.strAppID;
                ndEntryHeadline.strThreadID = nd.strThreadID;
                ndEntryHeadline.parentColor = nd.parentColor;
                TreeNode treeNodeEntryHeadline = new TreeNode("Entries:");
                treeNodeEntryHeadline.ForeColor = nd.parentColor;
                treeNodeEntryHeadline.Tag = ndEntryHeadline;
                treeNodeEntryHeadline.Nodes.Add("");
                node.Nodes.Add(treeNodeEntryHeadline);

                NodeDefinition ndCountThreadEntry = new NodeDefinition();
                ndCountThreadEntry.nodeType = NodeDefinition.NodeType.ntCountThreadEntriesHeadline;
                ndCountThreadEntry.strAppID = nd.strAppID;
                ndCountThreadEntry.strThreadID = nd.strThreadID;
                ndCountThreadEntry.parentColor = nd.parentColor;
                TreeNode treeNodeCountThreadEntry = new TreeNode("Count Thread Entries:");
                treeNodeCountThreadEntry.ForeColor = nd.parentColor;
                treeNodeCountThreadEntry.Tag = ndCountThreadEntry;
                treeNodeCountThreadEntry.Nodes.Add("");
                node.Nodes.Add(treeNodeCountThreadEntry);

                NodeDefinition ndThreadThroughputHeadline = new NodeDefinition();
                ndThreadThroughputHeadline.nodeType = NodeDefinition.NodeType.ntThreadThroughputHeadline;
                ndThreadThroughputHeadline.strAppID = nd.strAppID;
                ndThreadThroughputHeadline.strThreadID = nd.strThreadID;
                ndThreadThroughputHeadline.parentColor = nd.parentColor;
                TreeNode treeThreadThroughputHeadline = new TreeNode("Throughput:");
                treeThreadThroughputHeadline.ForeColor = nd.parentColor;
                treeThreadThroughputHeadline.Tag = ndThreadThroughputHeadline;
                treeThreadThroughputHeadline.Nodes.Add("");
                node.Nodes.Add(treeThreadThroughputHeadline);                                    
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntEntryHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndEntry = new NodeDefinition();
                ndEntry.nodeType = NodeDefinition.NodeType.ntEntry;
                ndEntry.strAppID = nd.strAppID;
                ndEntry.strThreadID = nd.strThreadID;
                ndEntry.parentColor = nd.parentColor;
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
                    TreeNode treeNodeEntry = new TreeNode(str);
                    treeNodeEntry.ForeColor = nd.parentColor;
                    treeNodeEntry.Tag = ndEntry;
                    treeNodeEntry.Nodes.Add("");
                    node.Nodes.Add(treeNodeEntry);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountAppEntriesHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndCountAppEntries = new NodeDefinition();
                ndCountAppEntries.nodeType = NodeDefinition.NodeType.ntCountAppEntries;
                ndCountAppEntries.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable where appID=" + nd.strAppID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeAppEntries = new TreeNode(strCNT);
                    treeNodeAppEntries.ForeColor = nd.parentColor;
                    treeNodeAppEntries.Tag = ndCountAppEntries;
                    treeNodeAppEntries.Nodes.Add("");
                    node.Nodes.Add(treeNodeAppEntries);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountThreadEntriesHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndCountThreadEntries = new NodeDefinition();
                ndCountThreadEntries.nodeType = NodeDefinition.NodeType.ntCountThreadEntries;
                ndCountThreadEntries.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable where appID=" + nd.strAppID + " and threadID=" + nd.strThreadID, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeThreadEntries = new TreeNode(strCNT);
                    treeNodeThreadEntries.ForeColor = nd.parentColor;
                    treeNodeThreadEntries.Tag = ndCountThreadEntries;
                    treeNodeThreadEntries.Nodes.Add("");
                    node.Nodes.Add(treeNodeThreadEntries);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntCountTotalEntriesHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndCountTotalEntries = new NodeDefinition();
                ndCountTotalEntries.nodeType = NodeDefinition.NodeType.ntCountTotalEntries;
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand("select count(id) as CNT from testtable", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeCountTotalEntries = new TreeNode(strCNT);
                    treeNodeCountTotalEntries.Tag = ndCountTotalEntries;
                    treeNodeCountTotalEntries.Nodes.Add("");
                    node.Nodes.Add(treeNodeCountTotalEntries);
                }
            }

            
            else if (nd.nodeType == NodeDefinition.NodeType.ntCountTotalThreadsActiveHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndCountTotalThreadsActive = new NodeDefinition();
                ndCountTotalThreadsActive.nodeType = NodeDefinition.NodeType.ntCountTotalThreadsActive;
                ndCountTotalThreadsActive.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                //cmd = new SQLiteCommand("select count(distinct appID||','||threadid) as CNT from testtable where threadid in (select th.threadid from threads th,apps ap where th.isActive=1 and ap.id=th.appID and  strftime('%s', 'now') -strftime('%s', ap.tsLastPoll)  < 30) ", con);
                cmd = new SQLiteCommand("select count(distinct th.appID|| ',' || th.threadid) as CNT from testtable tt,apps ap, threads th where tt.appID = th.appID and tt.threadID = th.threadID and th.isActive = 1 and ap.id = th.appID and ap.isActive=1", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeNodeCountTotalThreadsActive = new TreeNode(strCNT);
                    treeNodeCountTotalThreadsActive.Tag = ndCountTotalThreadsActive;
                    node.Nodes.Add(treeNodeCountTotalThreadsActive);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntThreadThroughputHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndThreadThroughput = new NodeDefinition();
                ndThreadThroughput.nodeType = NodeDefinition.NodeType.ntThreadThroughput;
                ndThreadThroughput.strAppID = nd.strAppID;
                ndThreadThroughput.strThreadID = nd.strThreadID;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID={0} and tt.threadID={1} and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1", nd.strAppID, nd.strThreadID);
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeThreadThroughput = new TreeNode(strCNT + " Inserts / sec.");
                    treeThreadThroughput.ForeColor = nd.parentColor;
                    treeThreadThroughput.Tag = ndThreadThroughput;
                    node.Nodes.Add(treeThreadThroughput);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntAppThroughputHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndThreadThroughput = new NodeDefinition();
                ndThreadThroughput.nodeType = NodeDefinition.NodeType.ntAppThroughput;
                ndThreadThroughput.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID={0} and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1", nd.strAppID);
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {                    
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeAppThroughput = new TreeNode(strCNT + " Inserts / sec.");                    
                    treeAppThroughput.Tag = ndThreadThroughput;
                    node.Nodes.Add(treeAppThroughput);
                }
            }

            else if (nd.nodeType == NodeDefinition.NodeType.ntTotalThroughputHeadline)
            {
                node.Nodes.Clear();
                NodeDefinition ndTotalThroughput = new NodeDefinition();
                ndTotalThroughput.nodeType = NodeDefinition.NodeType.ntTotalThroughput;
                ndTotalThroughput.strAppID = nd.strAppID;
                SQLiteCommand cmd = null;
                String strThroughput = String.Format("select count(tt.id)/60 as CNT from testtable tt, apps ap, threads th where tt.appID in (select id from apps where isActive=1) and tt.threadID=th.threadID and tt.appID=ap.id and th.AppID=ap.ID and ap.isActive=true and th.isActive=true and (ROUND((JULIANDAY('now') -JULIANDAY(tt.tsCreated)) *86400) / 60 )<1");
                cmd = new SQLiteCommand(strThroughput, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string strCNT = (string)reader["CNT"].ToString();
                    TreeNode treeTotalThroughput = new TreeNode(strCNT + " Inserts / sec.");
                    treeTotalThroughput.Tag = ndTotalThroughput;
                    node.Nodes.Add(treeTotalThroughput);
                }
            }
            

            if (node.Nodes.Count == 0)
                node.Nodes.Add("");
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
                foreach(string strField in column)
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
            rePrompt.SelectionColor= Color.White;           
            rePrompt.SelectedText = str;

        }

        private void rePrompt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rePrompt.SelectionStart = rePrompt.Text.Length;
                int cursorPosition = rePrompt.SelectionStart;
                int cmdIndex = rePrompt.GetLineFromCharIndex(cursorPosition)-2;
                if (cmdIndex < 0)
                    return;
                String strCMD = unprompt(rePrompt.Lines[cmdIndex]);                
                if (strCMD.Trim().Length == 0)
                    return;
                SQLiteCommand cmd = null;
                if (con==null)
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
                        foreach(String strCol in table[0])
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
                
                String strInsert = String.Format("insert into testtable (threadid,text,appid) values ({0},'{1}', {2})", threadID, str,  appID);
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
        public enum NodeType {      ntAppHeadline,
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
                                    ntTotalThroughput
        };

        public NodeType nodeType;
        public string strAppID = "";
        public string strThreadID = "";
        public Color parentColor = Color.Black;
    }
}
