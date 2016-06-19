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
using System.Media;
using System.Net.Mail;
using Microsoft.Win32;
using System.Diagnostics;

namespace Imgur_Capture
{
    class ToolBox
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString,
            uint nSize, string lpFileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpString, string lpFileName);

        public static string fetchformat()
        {
            string[] format = {".JPEG",".PNG",".BMP",".GIF"};
            int i = Convert.ToInt32(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Format"));
            return format[i];
        }

        public static void WriteINISetting(string iniFilePath, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniFilePath);
        }

        public static string ReadINISetting(string iniFilePath, string section, string key)
        {
            var retVal = new StringBuilder(255);

            GetPrivateProfileString(section, key, "", retVal, 255, iniFilePath);

            return retVal.ToString();
        }

        public static void SoundFX(string path)
        {
            SoundPlayer simpleSound = new SoundPlayer(path);
            simpleSound.Play();
        }

        public static void StartUp(CheckBox chk)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (chk.Checked)
                rk.SetValue(Application.ProductName, Application.ExecutablePath.ToString());
            else
                rk.DeleteValue(Application.ProductName, false);            
        }

        public static void ErrorReport(Exception s)
        {
            DialogResult input = MessageBox.Show(null,"Whoops! Something went wrong. " + s.Message + "\n\nWould you like to error report this?","Error Report",MessageBoxButtons.YesNo);
            if(input == DialogResult.Yes)
            {
                const string WEBSERVICE_URL = "https://api.github.com/repos/celtech/snapr/issues";
                string jsonData = "{ \"title\" : \"" + s.Message + "\", \"body\":\" Source = " + s.Source + " Data = " + s.Data + "\"  }";
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(WEBSERVICE_URL);
                    
                    if (webRequest != null)
                    {
                        webRequest.Method = "POST";
                        webRequest.Timeout = 20000;
                        webRequest.ContentType = "application/json";
                        webRequest.KeepAlive = false;
                        webRequest.ServicePoint.Expect100Continue = false;
                        webRequest.UserAgent = "Snapr";

                        webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes("snaprdebug:snaprdebug123")));

                        using (System.IO.Stream q = webRequest.GetRequestStream())
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(q))
                                sw.Write(jsonData);
                        }

                        using (System.IO.Stream q = webRequest.GetResponse().GetResponseStream())
                        {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(q))
                            {
                                var jsonResponse = sr.ReadToEnd();
                                MessageBox.Show(String.Format("Response: {0}", jsonResponse));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


            }
        }

        /*public static string UploadImage(string image)
        {
            WebClient w = new WebClient();
            try
            {
                byte[] response = w.UploadFile("http://imgurhub.com/upload.php", ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory") + @"\" + image);
                string s = w.Encoding.GetString(response);
                //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("link\":\"(.*?)\"");
                //Match match = reg.Match(result);
                //if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Only Image") == "Checked")
                //    return match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/");//.Replace("i.","").Replace(".jpg","");
               // else
                //    return match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/").Replace("i.", "").Replace(".png", "");
                return s;
            }
            catch (Exception s)
            {
                MessageBox.Show("Something went wrong. " + s.Message);
                return "Failed!";
            }
        }*/

        
        public static string UploadImage(string image)
        {
            string ClientId = "a36053e5d006adc";
            WebClient w = new WebClient();
            w.Headers.Add("Authorization", "Client-ID " + ClientId);
            System.Collections.Specialized.NameValueCollection Keys = new System.Collections.Specialized.NameValueCollection();
            try
            {
                Keys.Add("image", Convert.ToBase64String(File.ReadAllBytes(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory") + @"\" + image)));
                Keys.Add("description", "Uploaded using Imgur Hub, imgurhub.com get it free today");
                Keys.Add("title", "Uploaded using Imgur Hub, imgurhub.com get it free today");
                byte[] responseArray = w.UploadValues("https://api.imgur.com/3/image", Keys);
                dynamic result = Encoding.ASCII.GetString(responseArray);
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("link\":\"(.*?)\"");
                Match match = reg.Match(result);
                if (ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Only Image") == "Checked")
                {
                    string final = match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/");//.Replace("i.","").Replace(".jpg","");
                    w.DownloadString("http://localhost/ImgurHub/api.php?action=add&email=" + ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Email") + "&url=" + final);
                    return final;
                }
                else
                {
                    string final = match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/").Replace("i.", "").Replace(".png", "");
                    w.DownloadString("http://localhost/ImgurHub/api.php?action=add&email=" + ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Account", "Email") + "&url=" + final);
                    return final;
                }
            }
            catch (Exception s)
            {
                //MessageBox.Show("Something went wrong. " + s.Message);
                ToolBox.ErrorReport(s);
                return "Failed!";
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Point CursorPosition
        {
            get;
            protected set;
        }

        public static void CaptureActive(string name)
        {
            Rectangle bounds;
            var foregroundWindowsHandle = GetForegroundWindow();
            var rrect = new Rect();
            GetWindowRect(foregroundWindowsHandle, ref rrect);
            bounds = new Rectangle(rrect.Left, rrect.Top, rrect.Right - rrect.Left, rrect.Bottom - rrect.Top);
            CursorPosition = new Point(Cursor.Position.X - rrect.Left, Cursor.Position.Y - rrect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
                }

                if (!Directory.Exists(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory")))
                    Directory.CreateDirectory(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory"));

                bitmap.Save(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory") + @"\" + name + fetchformat(), ImageFormat.Png);
            }
        }

        public static void CaptureArea(Rectangle rect, string name)
        {
            using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);
                }

                if (!Directory.Exists(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory")))
                    Directory.CreateDirectory(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory"));

                bitmap.Save(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory") + @"\" + name + fetchformat(), ImageFormat.Png);
            }
        }

        public static void FirstRun()
        {
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory", Application.StartupPath + @"\Images");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Save Local", "Checked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Format", "0");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Auto Open", "Unchecked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Copy Link", "Checked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Only Image", "Checked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Sound", "Checked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Start Up", "Checked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Context Menu", "Unchecked");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Selector", "Cyan");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Index", "21");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Design", "Thickness", "1");
            ToolBox.WriteINISetting(Application.StartupPath + @"\config.ini", "Application Settings", "Sound", "Pop.wav");
            CheckBox chk = new CheckBox();
            chk.CheckState = CheckState.Checked;
            ToolBox.StartUp(chk);
        }

        public static void CaptureDesktop(string name)
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(screenLeft, screenTop), Point.Empty, new Size(screenWidth, screenHeight));
                }

                if (!Directory.Exists(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory")))
                    Directory.CreateDirectory(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory"));

                bitmap.Save(ToolBox.ReadINISetting(Application.StartupPath + @"\config.ini", "Capture Settings", "Directory") + @"\" + name + fetchformat(), ImageFormat.Png);
            }
        }
    }
}
