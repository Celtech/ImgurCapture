using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Imgur_Capture
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(!checkBox3.Checked)
            {
                textBox1.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory", textBox1.Text);
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Save Local", checkBox3.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Only Image", checkBox7.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Format", comboBox1.SelectedIndex.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open", checkBox5.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link", checkBox4.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound", checkBox6.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Start Up", checkBox1.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Context Menu", checkBox2.CheckState.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound", comboBox3.Text);
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Selector", comboBox2.SelectedItem.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Index", comboBox2.SelectedIndex.ToString());
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Thickness", numericUpDown1.Value.ToString());
            ToolBox.StartUp(checkBox1);
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            string apiResult = wc.DownloadString("http://localhost/ImgurHub/api.php?action=account&email=" + ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Email"));
            int i = 0;
            foreach (var itemlist in apiResult.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                listView1.Items[i].SubItems.Add(itemlist);
                i++;
            }
                

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                this.comboBox2.Items.Add(c.Name);
            }

            this.textBox1.Text = ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory");
            if(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Save Local") == "Checked")
                checkBox3.CheckState = CheckState.Checked;
            else
                checkBox3.CheckState = CheckState.Unchecked;

            if(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Context Menu") == "Checked")
                checkBox2.CheckState = CheckState.Checked;
            else
                checkBox2.CheckState = CheckState.Unchecked;

            if(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open") == "Checked")
                checkBox5.CheckState = CheckState.Checked;
            else
                checkBox5.CheckState = CheckState.Unchecked;

            if(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link") == "Checked")
                checkBox4.CheckState = CheckState.Checked;
            else
                checkBox4.CheckState = CheckState.Unchecked;

            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound") == "Checked")
                checkBox6.CheckState = CheckState.Checked;
            else
                checkBox6.CheckState = CheckState.Unchecked;

            if(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Start Up") == "Checked")
                checkBox1.CheckState = CheckState.Checked;
            else
                checkBox1.CheckState = CheckState.Unchecked;

            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Only Image") == "Checked")
                checkBox7.CheckState = CheckState.Checked;
            else
                checkBox7.CheckState = CheckState.Unchecked;

            comboBox1.SelectedIndex = Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Format"));

            comboBox2.SelectedIndex = Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Design", "Index"));
           
            comboBox3.Text = ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound");
            
            numericUpDown1.Value = Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Design", "Thickness"));

            panel1.Height = Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Design", "Thickness"));
            panel1.BackColor = Color.FromName(comboBox2.SelectedItem.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowDialog();
            textBox1.Text = fb.SelectedPath;
        }

        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0)
            {
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = Color.FromName(n);
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string color = this.comboBox2.SelectedItem.ToString();
            this.panel1.BackColor = Color.FromName(color);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            panel1.Height = Convert.ToInt32(numericUpDown1.Value.ToString());
        }

        public static void ThreadProc()
        {
            Application.Run(new Main());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Account", "Email", "");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Account", "Password", "");
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
            t.Start();
            Capture capture = (Capture)Application.OpenForms["Capture"];
            capture.Close();
            this.Close();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Text = "Press a key"; 
        }

        private void button6_KeyPress(object sender, KeyPressEventArgs e)
        {
            button6.Text = e.KeyChar.ToString();
            label1.Focus();
        }

        private void button7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                button7.Text = "Alt";
            }

            if (e.Control)
            {
                button7.Text = "Control";
            }

            if (e.Shift)
            {
                button7.Text = "Shift";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Text = "Press a key"; 
        }
    }
}
