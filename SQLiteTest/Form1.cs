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

        private delegate void AddListDelegate(List<String> list);

        public void AddListSafe(List<String> list)
        {
            if (lb.InvokeRequired)
            {
                var d = new AddListDelegate(AddListSafe);
                lb.Invoke(d, new object[] { list });
            }
            else
            {
                lb.Items.Clear();
                foreach (string str in list)
                {                    
                    lb.Items.Add(str);
                }
                
            }
        }

        public Form1()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            string cs = "Data Source=demo.db;Version=3;Pooling=True;Max Pool Size=100;";
            SQLiteCommand cmd = null;
            threads = new CThreads();
            


            con = new SQLiteConnection(cs);
            con.Open();
                       


            if (con.State == ConnectionState.Open)
            {                
                cmd = new SQLiteCommand("Create Table if NOT Exists testtable (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadid INTEGER) ", con);
                cmd.ExecuteNonQuery();
                String strDeleteFrom = "";
                String strInsert = "";

                strDeleteFrom = String.Format("Delete from testtable");
                cmd = new SQLiteCommand(strDeleteFrom, con);
                cmd.ExecuteNonQuery();

                // Create a table that can hold text-data along with the thread-id of the thread that created the data
                strInsert = String.Format("insert into testtable (threadid,text) values (0,'{0}')", dt.ToString());
                cmd = new SQLiteCommand(strInsert, con);
                cmd.ExecuteNonQuery();
            }            
        }

 
    

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1 frm1 = this;
            viewThread = new ViewThread(frm1);
        }

        private void btnShowContent_Click(object sender, EventArgs e)
        {
            lb.Items.Clear();

            string stm = "SELECT SQLITE_VERSION()";

            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand(stm, con);
            string strVersion = cmd.ExecuteScalar().ToString();
            lb.Items.Add(strVersion);

            cmd = new SQLiteCommand("Select text from testtable", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string str = (string)reader["text"];

                lb.Items.Add(str);
            }
        }

        private void btnStopThreads_Click(object sender, EventArgs e)
        {
            threads.running = false;
        }

        private void btnShowStatus_Click(object sender, EventArgs e)
        {

            lb.Items.Clear();

            String strRunning = "NO";
            if (threads.running)          
                strRunning = "YES";

            lb.Items.Add("Running: " + strRunning);
            
            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand("Select count(text) as cnt from testtable", con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string str = "Count=" + reader["cnt"];

                lb.Items.Add(str);
            }
        }

        private void btnViewThread_Click(object sender, EventArgs e)
        {
            Thread vt = new Thread(viewThread.view);
            viewThread.running = !viewThread.running;
            vt.Start();
        }

        private void buttonStartThreads_Click(object sender, EventArgs e)
        {            
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
        public ViewThread(Form1 frm)
        {
            string cs = "Data Source=demo.db;Version=3;Pooling=True;Max Pool Size=100;";
            con = new SQLiteConnection(cs);
            con.Open();
            this.frm = frm;
        }

        public void view()
        {
            while(running)
            {
                List<string> list = new List<String>();
                for (int i=0;i<3;i++)
                {
                    SQLiteCommand cmd = null;
                    cmd = new SQLiteCommand("SELECT text FROM (SELECT * FROM testtable where threadid= "+i+" ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ", con);
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

        // Control-Variable if thread is running
        public Boolean running = false;

        // Constructor cretes a separate connection for each thread
        public CThreads()
        {
            // First connection is opened, note Max Pool Size=100
            string cs = "Data Source=demo.db;Version=3;Pooling=True;Max Pool Size=100;";
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
                String strInsert = String.Format("insert into testtable (threadid,text) values (1,'T1: {0}')", dt.ToString());
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
                String strInsert = String.Format("insert into testtable (threadid,text) values (2,'T2: {0}')", dt.ToString());
                cmd = new SQLiteCommand(strInsert, con2);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
