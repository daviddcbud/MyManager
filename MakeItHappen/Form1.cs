using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeItHappen
{
    //erecording2017 project at S&S has the info you need in case you lose app.config
    public partial class Form1 : Form
    {
        bool encrypt = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            encrypt = true;
            EncryptDecryptFolder(txtFrom.Text);
             
             
        }

        private void EncryptDecryptFolder(string folder)
        {
            var basePath = folder.Replace(txtFrom.Text, "").TrimStart(@"\"[0]);
            var newDir = "";
            if (basePath == "")
            {
                newDir = txtTo.Text;
            }
            else
            {
                  newDir = System.IO.Path.Combine(txtTo.Text, basePath);
            }
            if (!System.IO.Directory.Exists(basePath))
            {
                System.IO.Directory.CreateDirectory(newDir);
            }
            var folders = System.IO.Directory.GetDirectories(folder);
             
            foreach (var f in folders)
            {
                EncryptDecryptFolder(f);
            }

            foreach(var file in System.IO.Directory.GetFiles(folder))
            {
                var info = new System.IO.FileInfo(file);
                var name = info.Name;
                var newpath = System.IO.Path.Combine(newDir, name);
                var bytes = System.IO.File.ReadAllBytes(file);
                if (encrypt)
                {
                    var encrypted = Encryption.Encrypt(bytes);
                    System.IO.File.WriteAllBytes(newpath, encrypted);
                }
                else
                {
                    var decrypted = Encryption.Decrypt(bytes);
                    System.IO.File.WriteAllBytes(newpath, decrypted);
                }
            }
             

        }

        private void button1_Click(object sender, EventArgs e)
        {
            encrypt = false;
            EncryptDecryptFolder(txtFrom.Text);
        }
    }
}
