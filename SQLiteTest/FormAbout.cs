using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteTest
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// Append formatted text to a Rich Text Box control 
        /// </summary>
        /// <param name="rtb">Rich Text Box to which horizontal bar is to be added</param>
        /// <param name="text">Text to be appended to Rich Text Box</param>
        /// <param name="textColour">Colour of text to be appended</param>
        /// <param name="isBold">Flag indicating whether appended text is bold</param>
        /// <param name="alignment">Horizontal alignment of appended text</param>
        private void AppendFormattedText(RichTextBox rtb, string text, Color textColour, Boolean isBold, HorizontalAlignment alignment)
        {
            int start = rtb.TextLength;
            rtb.AppendText(text);
            int end = rtb.TextLength; // now longer by length of appended text

            // Select text that was appended
            rtb.Select(start, end - start);

            #region Apply Formatting
            rtb.SelectionColor = textColour;
            rtb.SelectionAlignment = alignment;
            rtb.SelectionFont = new Font(
                 rtb.SelectionFont.FontFamily,
                 rtb.SelectionFont.Size,
                 (isBold ? FontStyle.Bold : FontStyle.Regular));
            #endregion

            // Unselect text
            rtb.SelectionLength = 0;
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            AppendFormattedText(reText, "\nHaufe MultiSQLite for C#\n\n", Color.FromArgb(0,121,255), true, HorizontalAlignment.Center);
            AppendFormattedText(reText, "(C) 2021 Haufe Group SE \n\n", Color.FromArgb(0, 121, 255), true, HorizontalAlignment.Center);
            AppendFormattedText(reText, "Uwe.Stahlschmidt@haufe-lexware.com\n\n", Color.FromArgb(0, 121, 255), true, HorizontalAlignment.Center);

            
            AppendFormattedText(reText, "MultiSQLite for C# makes it possible to test multiple connections with SQLite. Connections can be created from different threads from within the application or from multiple instances running simultaneously. \n\nDetailed information about performance and database status is displayed. In addition an edition for C++ is available, that allows testing in mixed development environements.", Color.Black, false, HorizontalAlignment.Left);
            lblVersion.Text = "Version: " + frmMain.getVersion();
        }
    }
}
