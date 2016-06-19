using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net; 
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;

namespace Imgur_Capture
{
    public partial class Capture : Form
    {
        Rectangle rect;
        Random rnd = new Random();
        string counter;
        string url;
        Point q;
        bool paint = false;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        public Capture()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Updater(1.112m);
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.Red;
            this.Opacity = 1;
            this.Size = new System.Drawing.Size(screenWidth, screenHeight);
            this.Location = new System.Drawing.Point(screenLeft, screenTop);
            this.TopMost = true;
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            RegisterHotKey(this.Handle, 1, (int)KeyModifier.Control, 0x31/*1*/);
            RegisterHotKey(this.Handle, 2, (int)KeyModifier.Control, 0x32/*2*/);
            RegisterHotKey(this.Handle, 3, (int)KeyModifier.Control, 0x33/*3*/);
            RegisterHotKey(this.Handle, 4, (int)KeyModifier.Control, 0x34/*4*/);
            if(!File.Exists("config.ini"))
                ToolBox.FirstRun();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                ToolBox.ErrorReport(ex);
            }
            finally
            {
                Application.Exit();
            }
        }

        public void Updater(decimal current)
        {
            toolStripMenuItem6.Text = "Imgur Hub v" + current;
            string url = "https://raw.githubusercontent.com/Celtech/Snapr/master/version";
            WebClient wc = new WebClient();
            string data = wc.DownloadString(url);
            decimal test = Convert.ToDecimal(data);
            if (test > current)
            {
                DialogResult input = MessageBox.Show(null, "New version " + data + " is avalible!\nWould you like to download now?", "Updater", MessageBoxButtons.YesNo);
                if (input == DialogResult.Yes)
                {
                    Process.Start("https://github.com/Celtech/Snapr/releases/");
                    Application.Exit();
                }
            }
            notifyIcon1.ShowBalloonTip(10, "Imgur Hub", "You are running v" + current, ToolTipIcon.Info);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312 && (int)m.WParam == 1)
            {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
                this.Visible = false;
                counter = rnd.Next(100000000, 999999999).ToString();
                ToolBox.CaptureDesktop(counter);
                url = ToolBox.UploadImage(counter + ToolBox.fetchformat());

                notifyIcon1.ShowBalloonTip(10, "You have a new upload!", url, ToolTipIcon.Info);
                
                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound") == "Checked")
                    ToolBox.SoundFX(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound"));

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link") == "Checked")
                    Clipboard.SetText(url);

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open") == "Checked")
                    Process.Start(url);
            }

            if (m.Msg == 0x0312 && (int)m.WParam == 2)
            {
                paint = true;
                rect = new Rectangle(0, 0, 0, 0);
                this.Opacity = 1;
                this.Visible = true; 
            }

            if (m.Msg == 0x0312 && (int)m.WParam == 3)
            {
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);
                this.Visible = false;
                counter = rnd.Next(100000000, 999999999).ToString();
                ToolBox.CaptureActive(counter);
                url = ToolBox.UploadImage(counter + ToolBox.fetchformat());

                notifyIcon1.ShowBalloonTip(10, "You have a new upload!", url, ToolTipIcon.Info);

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound") == "Checked")
                    ToolBox.SoundFX(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound"));

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link") == "Checked")
                    Clipboard.SetText(url);

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open") == "Checked")
                    Process.Start(url);
            }

            if (m.Msg == 0x0312 && (int)m.WParam == 4)
            {
                rect = new Rectangle(0, 0, 0, 0);
                paint = false;
                this.Invalidate();
                this.Visible = false;
            }
        }
        
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            q.X = e.X;
            q.Y = e.Y;
            this.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!rect.Size.IsEmpty)
            {
                paint = false;
                this.Invalidate();
                this.Visible = false;
                counter = rnd.Next(100000000, 999999999).ToString();
                ToolBox.CaptureArea(rect, counter);
                url = ToolBox.UploadImage(counter + ToolBox.fetchformat());

                notifyIcon1.ShowBalloonTip(10, "You have a new upload!", url, ToolTipIcon.Info);

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound") == "Checked")
                    ToolBox.SoundFX(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound"));

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link") == "Checked")
                    Clipboard.SetText(url);

                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open") == "Checked")
                    Process.Start(url);
            }
            rect = new Rectangle(0, 0, 0, 0);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                rect = new Rectangle(
                Math.Min(q.X, e.X),
                Math.Min(q.Y, e.Y),
                Math.Abs(e.X - q.X),
                Math.Abs(e.Y - q.Y));

                
            }
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.DoubleBuffered = true;
            if (paint)
            {
                System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                using (Pen pen = new Pen(Color.FromName(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Design", "Selector")), Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Design", "Thickness"))))
                {
                    e.Graphics.FillRectangle(myBrush, rect);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Process.Start(url);  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://imgur.com");
            }catch(Exception ex){
                ToolBox.ErrorReport(ex);
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            rect = new Rectangle(0, 0, 0, 0);
            this.Opacity = .5;
            this.Visible = true; 
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            counter = rnd.Next(100000000, 999999999).ToString();
            ToolBox.CaptureDesktop(counter);
            url = ToolBox.UploadImage(counter + ToolBox.fetchformat());

            notifyIcon1.ShowBalloonTip(10, "You have a new upload!", url, ToolTipIcon.Info);

            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound") == "Checked")
                ToolBox.SoundFX(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound"));

            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link") == "Checked")
                Clipboard.SetText(url);

            if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open") == "Checked")
                Process.Start(url);
        }
    }
}
