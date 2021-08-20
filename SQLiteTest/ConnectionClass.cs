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

namespace SQLiteTest
{
    public class ConnectionClass
    {
        Form form;
        private String strDatabaseFile = "";
        private String strSQLiteVersion = "";
        private String strDBAccessMode = "";
        
        public const String DB_FILE = "Multisqlite.db";
        public const int DB_VERSION_MIN = 1000;
        public const int DB_VERSION_MAX = 2000;
        public String DB_VERSION_STR = "2.0.0.0";
        public const int DB_VERSION = 2000;

       

        //private int appID = -1;
        private string appName = "";

        public String getDatabaseFile() { return strDatabaseFile;  }
        public String getSQLiteVersion() { return strSQLiteVersion;  }
        public String getDBAccessMode() { return strDBAccessMode; }

        public PromptCommands prompt;


        public SQLiteConnection con = null;
        public ConnectionClass(Form form, PromptCommands prompt, String appName)
        {
            this.form = form;
            this.appName = appName;
            this.prompt = prompt;
            prompt.connection = this;
        }

        public SQLiteConnection get()
        {
            return con;
        }

        public int getDBVersionNumber()
        {
            String strDBVersion = getDBVersion();

            do // First get rid of spaces like "  \r"
            {
                strDBVersion = strDBVersion.Replace(".", "");
            } while (strDBVersion.Contains("."));

            
            int version = 0;
            if (Int32.TryParse(strDBVersion, out version))
                return version;
            else
                return 0;
        }


        public void setSQLiteVersion()
        {
            SQLiteCommand cmd = null;
            string stm = "SELECT SQLITE_VERSION()";
            cmd = new SQLiteCommand(stm, con);
            strSQLiteVersion = cmd.ExecuteScalar().ToString();
        }

        public void setAppID(Form frm, ref int appID, String strDatabaseFile = "")
        {
            if (get().State != ConnectionState.Open)
            {
                appID = -1;
                frm.Text = "Haufe Multi-SQLite for C# <Connection Closed>";
                return;
            }
            
            string stm = "SELECT SQLITE_VERSION()";

            SQLiteCommand cmd = null;
            cmd = new SQLiteCommand(stm, get());
            string strVersion = cmd.ExecuteScalar().ToString();


            cmd = new SQLiteCommand("SELECT* FROM multisqlite_apps order by id DESC LIMIT 1", get());
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String strID = reader["id"].ToString();
                appID = Convert.ToInt32(strID);
                frm.Text = "Haufe Multi-SQLite for C# <" +strDatabaseFile +" ID: " + appID + ">";
            }
        }

        public void Connect(ref int appID, ref DBThreads threads, String strExternalFile = "")
        {
            String strDatabaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if (strDatabaseDir.Length == 0)
                strDatabaseDir = "C:";
            if (strDatabaseDir[strDatabaseDir.Length - 1] != '\\')
                strDatabaseDir += '\\';
            strDatabaseDir += "Haufe\\MultiSQLite\\";
            if (!System.IO.Directory.Exists(strDatabaseDir))
                System.IO.Directory.CreateDirectory(strDatabaseDir);
            strDatabaseFile = strDatabaseDir + DB_FILE;

            if ((strExternalFile.Length > 0) && (File.Exists(strExternalFile)))
                strDatabaseFile = strExternalFile;

            System.IO.FileAttributes databaseAttributes = 0;
            string cs = "Data Source=" + strDatabaseFile + ";Version=3;Pooling=True;Max Pool Size=100;";

            con = new SQLiteConnection(cs);

            if (File.Exists(strDatabaseFile))
                databaseAttributes = System.IO.File.GetAttributes(strDatabaseFile);
            else
                databaseAttributes = System.IO.FileAttributes.Offline;

            DateTime dt = DateTime.Now;

            SQLiteCommand cmd = null;

            bool bDatabaseExisted = File.Exists(strDatabaseFile);

            con.Open();

            int dbVersion = 0;

            String strConnected = "";
            if (con.State == ConnectionState.Open)
            {
                if (bDatabaseExisted)
                {
                    //String strDBVersion = getDBVersion();
                    //if (!strDBVersion.Equals(DB_VERSION.Trim()))
                     dbVersion = getDBVersionNumber();
                    if ((strExternalFile.Length==0) && ((dbVersion<DB_VERSION_MIN) || (dbVersion>DB_VERSION_MAX)))
                    {    
                        con.Close();
                        //con = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        bool bFileDeleted = false;
                        try
                        {
                            if (dbVersion<DB_VERSION_MIN)
                                prompt.Out("Error: Invalid database....");
                            else
                                prompt.Out("Error: The database File is too new for this application....");
                            prompt.Out("       Trying to delete database file...");
                            File.Delete(strDatabaseFile);
                            bFileDeleted = true;
                        }
                        catch (Exception e)
                        {
                            bFileDeleted = false;
                        }
                        if (File.Exists(strDatabaseFile))
                        {
                            bFileDeleted = false;
                        }
                        if (bFileDeleted)
                        {
                            prompt.Out("       Old database has been removed");
                        }
                        else
                        {
                            prompt.Out("Error: Old database could not have been removed: " + strDatabaseFile);
                            return;
                        }
                        con = new SQLiteConnection(cs);
                        con.Open();

                        if (con.State != ConnectionState.Open)
                        {
                            {
                                prompt.Out("Error: Could not recreate Database database: " + strDatabaseFile);
                                return;
                            }
                        }
                        else
                        {
                            prompt.Out("Database recreated: " + strDatabaseFile);
                        }
                        prompt.Out("");
                    }
                    else if (dbVersion < DB_VERSION)
                    {
                        if (strExternalFile.Length==0)
                        {
                            prompt.Out("Old database, try updating....");
                        }
                        else 
                        {
                           
                        }
                        
                    }
                }

                setSQLiteVersion();
                if (databaseAttributes == System.IO.FileAttributes.Offline)
                {
                    databaseAttributes = System.IO.File.GetAttributes(strDatabaseFile);
                    prompt.Out("Database created: " + strDatabaseFile);
                }
                else
                {
                    prompt.Out("Database opened: " + strDatabaseFile);
                }
                strDBAccessMode = "Database accessible for " + ((databaseAttributes == System.IO.FileAttributes.ReadOnly) ? " Read only" : "Read + Write");
                prompt.Out(strDBAccessMode);



                String strDeleteFrom = "";
                String strInsert = "";


                if (dbVersion < DB_VERSION_MIN)
                {
                    if (strExternalFile.Length == 0)
                    {
                        execQuery("Create Table if NOT Exists version (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT) ");
                        execQuery("Create Table if NOT Exists testtable (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP) ");
                        execQuery("Create Table if NOT Exists apps (id INTEGER PRIMARY KEY AUTOINCREMENT, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  tsLastPoll TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name TEXT, isActive INTEGER DEFAULT FALSE) ");
                        execQuery("Create Table if NOT Exists threads ( id INTEGER PRIMARY KEY AUTOINCREMENT, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP, isActive INTEGER DEFAULT FALSE ) ");
                    }
                    dbVersion = 1000;
                }
                
               
                execQuery(strInsert);
                if (dbVersion < 1100)
                {
                    execQuery("Drop table if exists multisqlite_version");
                    execQuery("Create Table if NOT Exists multisqlite_version (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, version TEXT) ");
                    execQuery("insert into multisqlite_version (id,name,version) values (0,'MULTISQLITE', '" + "1.1.0.0" + "')");
                    dbVersion = 1100;                    
                }

                if (dbVersion < 2000)
                {
                    if (strExternalFile.Length > 0)
                    {
                        execQuery("Create Table if NOT Exists multisqlite_entries (id INTEGER PRIMARY KEY AUTOINCREMENT, text VARCHAR, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP) ");
                        execQuery("Create Table if NOT Exists multisqlite_apps (id INTEGER PRIMARY KEY AUTOINCREMENT, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  tsLastPoll TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name TEXT, isActive INTEGER DEFAULT FALSE) ");
                        execQuery("Create Table if NOT Exists multisqlite_threads ( id INTEGER PRIMARY KEY AUTOINCREMENT, threadID INTEGER, appID INTEGER, tsCreated TIMESTAMP DEFAULT CURRENT_TIMESTAMP, isActive INTEGER DEFAULT FALSE ) ");
                    }
                    else
                    {
                        execQuery("Alter Table testtable RENAME TO multisqlite_entries ");
                        execQuery("Alter Table apps RENAME TO multisqlite_apps  ");
                        execQuery("Alter Table threads RENAME TO multisqlite_threads  ");
                    }
                    execQuery("Update multisqlite_version set version='" + DB_VERSION_STR + "'");    
                    dbVersion = 2000;                    
                }
                    

                // Create a table that can hold text-data along with the thread-id of the thread that created the data
                cmd = new SQLiteCommand("update multisqlite_apps set tsLastPoll = CURRENT_TIMESTAMP where id=" + appID, get());
                execQuery("update multisqlite_apps set isActive=0 where tsLastPoll is null ");
                execQuery("insert into multisqlite_apps (name, isActive) values ('" + appName + "', true)");
                setAppID(form, ref appID, strExternalFile);
                strInsert = String.Format("insert into multisqlite_entries (appID,threadid,text) values ({0},0,'{1}')", appID, dt.ToString());
                execQuery(strInsert);

                prompt.Out("SQLite Version: " + strSQLiteVersion);
            }
            else
            {
                prompt.Out("Error: Could not opened database: " + strDatabaseFile);
            }

            threads = new DBThreads(appID, strDatabaseFile);
        }

        public void Disconnect(ref int appID)
        {
            get().Close();
            prompt.Out("Database connection is closed");           
            setAppID(form, ref appID);
        }

        public String getDBVersion()
        {
            string stm = "SELECT SQLITE_VERSION()";
            //int nRevision = -1;
            String strDBVersion = "0";
            String strName = "";

            try
            {
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand(stm, get());
                string strVersion = cmd.ExecuteScalar().ToString();


                cmd = new SQLiteCommand("SELECT * FROM multisqlite_version order by id DESC LIMIT 1", get());
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strName = reader["name"].ToString();
                    strDBVersion = reader["version"].ToString();
                    //this.Text = "Haufe Multi-SQLite for C# <ID: " + appID + ">";
                    //this.Text = strID;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            if (!strName.ToUpper().Equals("MULTISQLITE"))
                return "0";
            else
                return strDBVersion.Trim(); ;
        }

        public void execQuery(String strQuery)
        {
            try
            {
                SQLiteCommand cmd = null;
                cmd = new SQLiteCommand(strQuery, get());
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

    public class DBThreads
    {
        // Two different Connection-Objets
        SQLiteConnection con1 = null;
        SQLiteConnection con2 = null;

        static int maxThreadID;

        private int appID;

        // Control-Variable if thread is running
        public Boolean running = false;

        // Constructor cretes a separate connection for each thread
        public DBThreads(int appID, String strDatabaseFile)
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
        }


        public void insert_thread_function()
        {
            int threadID = ++maxThreadID;

            SQLiteCommand cmd = null;

            String strStartThread = String.Format("insert into multisqlite_threads (threadid,appID,isActive) values ({0},'{1}',1)", threadID, appID);
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

                String strInsert = String.Format("insert into multisqlite_entries (threadid,text,appid) values ({0},'{1}', {2})", threadID, str, appID);
                cmd = new SQLiteCommand(strInsert, con1);
                cmd.ExecuteNonQuery();
            }

            String strStopThread = String.Format("update multisqlite_threads set isActive=0 where threadID={0} and appID={1} ", threadID, appID);
            cmd = new SQLiteCommand(strStopThread, con1);
            cmd.ExecuteNonQuery();
        }
    }
}
