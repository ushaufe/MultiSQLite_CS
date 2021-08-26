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
using static MultiSQLite.NodeDefinition;
using System.Runtime.InteropServices.ComTypes;




namespace MultiSQLite
{
    public class PromptCommands
    {
        public ConnectionClass connection;
        static RichTextBox rePrompt;
        //FrmMain formMain;

        public PromptCommands(ref RichTextBox rePrompt, ref ConnectionClass connection)
        {
            this.connection = connection;
            PromptCommands.rePrompt = rePrompt;
        }

        public void Disconnect(ref int appID)
        {
            if (connection.get().State != ConnectionState.Open)
            {
                Out("Error: Database is already closed", Color.Red);
                Prompt();
                return;
            }
            connection.Disconnect(ref appID);
            Prompt();            
        }

        public void Connect(ref int appID, ref DBThreads threads)
        {
            if (connection.get().State == ConnectionState.Open)
            {
                Out("Error: Database already opened. Please close first.", Color.Red);
                Prompt();
                return;
            }
            connection.Connect(ref appID, ref threads);
            Prompt();
        }


        public void Out(String str, bool newLine = true)
        {
            Out(str, Color.White, newLine);
        }

        public void Out(String str, Color color, bool newLine = true)
        {
            if (rePrompt.Lines.Length > 0)
                if ((rePrompt.Lines[rePrompt.Lines.Length-1].Length > 0) && (newLine))
                    rePrompt.AppendText("\r\n");
            int selStart = rePrompt.TextLength;
            if (newLine)
                rePrompt.AppendText(str + "\r\n");
            else
                rePrompt.AppendText(str);
            int selEnd = rePrompt.TextLength;
            rePrompt.SelectionStart = selStart;
            rePrompt.SelectionLength = selEnd - selStart;
            rePrompt.SelectionColor = color;
            rePrompt.ScrollToCaret();
        }

        public void Prompt()
        {
            rePrompt.AppendText("\r\n" + "$:> ");
            rePrompt.ScrollToCaret();
        }
    }
}
