using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (strDatabaseFile.Length > 0)
            {
                string cs = "Data Source=" + strDatabaseFile + ";Version=3;Pooling=True;Max Pool Size=100;";
                con = new SQLiteConnection(cs);
                con.Open();
                this.frm = frm;
                this.appID = appID;
            }
        }

        public void view()
        {
            while (running)
            {
                List<string> list = new List<String>();
                for (int i = 0; i < 3; i++)
                {
                    SQLiteCommand cmd = null;
                    String strCMD = "SELECT text FROM (SELECT * FROM multisqlite_entries where threadID=" + i + " and appID=" + appID + " ORDER BY id DESC LIMIT 2) ORDER BY id ASC; ";
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
}
