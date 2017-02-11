using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleLookup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                 
            }
            catch(Exception ex)
            {

            }
        }

        private void txtp_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtp_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void LoadData()
        {
            try
            {
                listView1.Items.Clear();
                var parser = new TextFieldParser("Data.txt");
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TextFieldType = FieldType.Delimited;
                var fields = parser.ReadFields();
                while (fields != null)
                {
                    var lvw = listView1.Items.Add(fields[0]);
                    lvw.SubItems.Add(fields[1]);
                    lvw.SubItems.Add(EncDec.Decrypt(fields[2], txtp.Text));
                    fields = parser.ReadFields();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadData();
            }
        }
    }
}
