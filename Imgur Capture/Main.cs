using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Imgur_Capture
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private bool login(string email, string password, bool auto)
        {
            WebClient wc = new WebClient();
            string apiReturn = wc.DownloadString("http://localhost/ImgurHub/api.php?action=login&email=" + email + "&password=" + password);
            switch (apiReturn)
            {
                case "success":
                    if (!auto)
                        saveProfile();
                    return true;

                case "failed":
                    MessageBox.Show("Whoops, the email or password you entered was incorrect","Whoops!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return false;

                default:
                    MessageBox.Show("Whoops, we can't seem to reach the servers. Please try again in a few seconds","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return false;
            }
        }

        private string hashString(string password)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(password);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private void saveProfile()
        {
            
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Account", "Email", textBox1.Text);
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Account", "Password", hashString(textBox2.Text));
        }

        public static void ThreadProc()
        {
            Application.Run(new Capture());
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Password").Length > 0 && ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Email").Length > 0)
            {
                //if (login(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Email"), ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Password"),true))
                //{
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                    this.Close();
               // }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(login(textBox1.Text, hashString(textBox2.Text),false))
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
                t.SetApartmentState(System.Threading.ApartmentState.STA);
                t.Start();
                this.Close();
            }
        }
    }
}
