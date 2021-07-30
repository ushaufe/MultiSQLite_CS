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


// This is a C# project to demonstrage how well SQLite can handle Multithreading
// The documentation for this project and the accompanying C++ projects can be found here:
// (the location may be moved by the webmaster of the wiki)
// https://mywiki.grp.haufemg.com/pages/viewpage.action?pageId=156088657


// Two different approaches should be shown:
// Pooling: Using different connections for different threads and
// Using the same connection with multiple threads simultaneously

namespace SQLiteTest
{

    public partial class Form1 : Form
    {
        SQLiteConnection con = null;
       
        CThreads threads;
        

        // These threads are used for simultaneous writing...
        Thread thr1, thr2;

        // This thread is used for reading
        // ( Constantly polling the data in 
        //   the database and refreshing them )
        // Can be run simultanously to writing threads
        ViewThread viewThread;

        String strDatabaseFile = "";
        String strVersion = "";
        String strSQLiteVersion = "";
        const String DB_FILE = "demo.db";   //

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

        public Form1()
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
           
                        
            con = new SQLiteConnection(cs);
            con.Open();


            String strConnected = "";
            if (con.State == ConnectionState.Open)
            {
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
                lbGeneral.Items.Add( "Database accessible for " + ( (databaseAttributes == System.IO.FileAttributes.ReadOnly) ? " Read only" : "Read + Write") );


                
                String strDeleteFrom = "";
                String strInsert = "";


                cmd = new SQLiteCommand("Create Table if NOT Exists version (id INTEGER PRIMARY KEY AUTOINCREMENT, revision INTEGER) ", con);
                cmd.ExecuteNonQuery();
                strDeleteFrom = "";
                strInsert = "";

                cmd = new SQLiteCommand("Create Table if NOT Exists testtable (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadID INTEGER, appID INTEGER) ", con);
                cmd.ExecuteNonQuery();
                strDeleteFrom = "";
                strInsert = "";


                cmd = new SQLiteCommand("Create Table if NOT Exists apps (id INTEGER PRIMARY KEY AUTOINCREMENT, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  tsLastPoll TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name TEXT) ", con);
                cmd.ExecuteNonQuery();
                strDeleteFrom = "";
                strInsert = "";


                strInsert = String.Format("insert into apps (name) values ('" + appName + "')");
                cmd = new SQLiteCommand(strInsert, con);
                cmd.ExecuteNonQuery();

                setAppID();

                // Create a table that can hold text-data along with the thread-id of the thread that created the data
                strInsert = String.Format("insert into testtable (appID,threadid,text) values ({0},0,'{1}')", appID, dt.ToString());
                cmd = new SQLiteCommand(strInsert, con);
                cmd.ExecuteNonQuery();

                strDeleteFrom = String.Format("Delete from version");
                cmd = new SQLiteCommand(strDeleteFrom, con);
                cmd.ExecuteNonQuery();

                strInsert = String.Format("insert into version (id,revision) values (0,1)");
                cmd = new SQLiteCommand(strInsert, con);
                cmd.ExecuteNonQuery();

                lbGeneral.Items.Add("SQLite Version: " + strSQLiteVersion);
            }
            else
            {
                lbGeneral.Items.Add("Error: Could not opened database: " + strDatabaseFile);
            }

            threads = new CThreads(appID,strDatabaseFile);
            tiUpdateApps.Enabled = true;
            tiPollApps.Enabled = true;
}


        public void setSQLiteVersion()
        {
            SQLiteCommand cmd = null;
            string stm = "SELECT SQLITE_VERSION()";
            cmd = new SQLiteCommand(stm, con);
            strSQLiteVersion = cmd.ExecuteScalar().ToString();           
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
           Form1 frm1 = this;
            viewThread = new ViewThread(frm1,appID,strDatabaseFile);
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

                lbGeneral.Items.Add( "AppID: " + strAppID +  ", ThreadID: " + strThreadID + "   -    "  + strText);
            }
        }

        private void btnStopThreads_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            threads.running = false;
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
            cmd = new SQLiteCommand("update apps set tsLastPoll = CURRENT_TIMESTAMP where id="+appID, con);
            cmd.ExecuteNonQuery();            
        }

        private void tiPollApps_Tick(object sender, EventArgs e)
        {
            lbActiveApps.Items.Clear();

            // SELECT strftime('%s','now') -strftime('%s', Timestamp) from test where strftime('%s', 'now') - strftime('%s', Timestamp) > 930
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

        private void buttonStartThreads_Click(object sender, EventArgs e)
        {
            tcTabs.SelectedIndex = 0;
            // Two different threads are created for simultaneous writing
            // each thread creates it's own instance of a database object (pooling)
            thr1 = new Thread(threads.m1);
            thr2 = new Thread(threads.m2);
            
            // The threads are set to state running, so that the infinite loop can be interrupted
            threads.running = true;
            
            // Both threads are started
            thr1.Start();            
            thr2.Start();
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
        Form1 frm;
        private int appID;
        public ViewThread(Form1 frm, int appID, String strDatabaseFile)
        {
            string cs = "Data Source="+strDatabaseFile+";Version=3;Pooling=True;Max Pool Size=100;";
            con = new SQLiteConnection(cs);
            con.Open();
            this.frm = frm;
            this.appID = appID;
        }

        public void view()
        {
            while(running)
            {
                List<string> list = new List<String>();
                for (int i=0;i<3;i++)
                {
                    SQLiteCommand cmd = null;
                    String strCMD = "SELECT text FROM (SELECT * FROM testtable where threadID=" + i + " and appID=" + appID + " ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ";
                    //String strCMD = "SELECT text FROM (SELECT * FROM testtable where threadID=" + i + " ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ";
                    cmd = new SQLiteCommand(strCMD , con);
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

        private int appID;

        // Control-Variable if thread is running
        public Boolean running = false;

        // Constructor cretes a separate connection for each thread
        public CThreads(int appID, String strDatabaseFile)
        {
            this.appID = appID;
            
            // First connection is opened, note Max Pool Size=100
            string cs = "Data Source="+strDatabaseFile+";Version=3;Pooling=True;Max Pool Size=100;";
            // First object is instantiated
            con1 = new SQLiteConnection(cs);
            con1.Open();

            // Second connection is opened, note Max Pool Size=100
            con2 = new SQLiteConnection(cs);
            con2.Open();
            SQLiteCommand cmd = null;
        }

        public void m1()
        {
            SQLiteCommand cmd = null;
            
            // The first thread is writing to the database in an infinite loop
            // using it's own instance of the DB-connection
            // writing can be stopped when setting running = false
            while (running)
            {
                DateTime dt = DateTime.Now;
                String strInsert = String.Format("insert into testtable (threadid,text,appid) values (1,'T1: {0}', {1})", dt.ToString(),appID);
                cmd = new SQLiteCommand(strInsert, con1);
                cmd.ExecuteNonQuery();
            }
        }


        public void m2()
        {
            SQLiteCommand cmd = null;

            // The second thread is writing to the database in an infinite loop
            // using it's own instance of the DB-connection
            // writing can be stopped when setting running = false
            while (running)
            {
                DateTime dt = DateTime.Now;
                String strInsert = String.Format("insert into testtable (threadid,text,appid) values (2,'T2: {0}', {1})", dt.ToString(),appID);
                cmd = new SQLiteCommand(strInsert, con2);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
