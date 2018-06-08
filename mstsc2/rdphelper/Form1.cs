using MSTSCLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rdphelper
{
    public partial class Form1 : Form
    {
        string ip;
        string username;
        string domain;
        bool bShow = true;
        bool bConsole = true;
        string password = "";
        public Form1(string ip = "127.0.0.1", string username = "lcm1", string password = "mz@880330", bool bShow = false, bool bConsole = false)
        {
            InitializeComponent();
            this.ip = ip;
            this.username = username;
            if (username.Contains("\\"))
            {
                this.domain = username.Split('\\')[0];
                this.username = username.Split('\\')[1];
            }
            this.password = password;
            this.bShow = bShow;
            this.bConsole = bConsole;
            this.Visible = bShow;
            this.password = password;
            this.Text = ip + " - Remote Desktop Connection";
        }

        private void connectRDP(string hostname, string domain, string username, string password)
        {
            
            //m_rdpview.FullScreen = true;
            m_rdpview.Server = hostname;
            m_rdpview.Domain = domain;
            m_rdpview.UserName = username;

            m_rdpview.AdvancedSettings8.ClearTextPassword = password;
            m_rdpview.Width = 1024;
            m_rdpview.Height = 800;
           
            //m_rdpview.
            //this.Width = 1024 + 80;
            //this.Height = 800 + 80;
            if (Process.GetCurrentProcess().SessionId != 0)
            {
                //reset the resolution of screen
                m_rdpview.Height = Screen.PrimaryScreen.Bounds.Height;
                m_rdpview.Width = Screen.PrimaryScreen.Bounds.Width;
            }
            else if (ConfigurationManager.AppSettings.Count >= 2 
                && ConfigurationManager.AppSettings.AllKeys.Contains("resolution_x") 
                && ConfigurationManager.AppSettings.AllKeys.Contains("resolution_y"))
            {
                m_rdpview.Width = int.Parse(ConfigurationManager.AppSettings["resolution_x"]);
                m_rdpview.Height = int.Parse(ConfigurationManager.AppSettings["resolution_y"]);
            }
            m_rdpview.AdvancedSettings8.AuthenticationLevel = 2;
            m_rdpview.AdvancedSettings8.EnableCredSspSupport = true;
            m_rdpview.AdvancedSettings8.NegotiateSecurityLayer = true;

            m_rdpview.Connect();
        }

        private void m_rdpview_OnConnecting(object sender, EventArgs e)
        {
            Console.WriteLine("m_rdpview_OnConnecting");
        }

        private void m_rdpview_OnAuthenticationWarningDismissed(object sender, EventArgs e)
        {
            Console.WriteLine("m_rdpview_OnAuthenticationWarningDismissed");
        }

        private void m_rdpview_OnAuthenticationWarningDisplayed(object sender, EventArgs e)
        {
            Console.WriteLine("m_rdpview_OnAuthenticationWarningDisplayed");
        }

        private void m_rdpview_OnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
        {
            Console.WriteLine("m_rdpview_OnLogonError");
        }

        private void m_rdpview_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            Console.WriteLine("m_rdpview_OnLogonError");
            Application.Exit();
        }

        private void m_rdpview_OnWarning(object sender, AxMSTSCLib.IMsTscAxEvents_OnWarningEvent e)
        {
            Console.WriteLine("m_rdpview_OnWarning");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //m_rdpview.Left = 7;
            //m_rdpview.Top = 90;
            //m_rdpview.Height = this.Height - 140;
            //m_rdpview.Width = this.Width - 35;
            m_rdpview.Left = 0;
            m_rdpview.Top = 0;
            m_rdpview.Height = this.Height;
            m_rdpview.Width = this.Width;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (bShow == false)
            {
                this.Opacity = 0;
                this.ShowInTaskbar = false;
            }
            else
            {
                m_rdpview.FullScreen = true;
            }

            if (m_rdpview.Connected == 0)
            {
                connectRDP(ip, domain, username, password);
            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("Shown");
        }

        private void m_rdpview_OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("m_rdpview_OnConnected");
        }

        private void m_rdpview_OnLoginComplete(object sender, EventArgs e)
        {
            Console.WriteLine("m_rdpview_OnLoginComplete");
            TakeScreenShot();
            if (bConsole)
            {
                SessionHelper hlp = new SessionHelper();
                int sessionid = -1;
                foreach(var s in hlp.getsessionlist())
                {
                    if (s.sessionname == "console")
                    {
                        sessionid = s.sessionid;
                        break;
                    }
                }

                if (sessionid == -1)
                {
                    Console.WriteLine("Console winstation doesn't exist, won't switch to it");
                    return;
                }

                bool bLock = false;
                foreach(var p in Process.GetProcesses())
                {
                    if (p.ProcessName == "LogonUI" && p.SessionId == sessionid)
                    {
                        bLock = true;
                    }
                }

                if (bLock == false)
                {
                    Console.WriteLine("Console winstation isn't lock, won't switch to it");
                    return;
                }
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = "tscon.exe";
                si.Arguments = sessionid.ToString() + " /dest:console /PASSWORD:" + password;
                si.Verb = "runas";
                Process.Start(si);
            }

        }

        private void m_rdpview_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Console.WriteLine("m_rdpview_OnDisconnected");
            Application.Exit();
        }



        private void TakeScreenShot()
        {
            Bitmap bit = new Bitmap(m_rdpview.Width, m_rdpview.Height);
            Rectangle rect = new Rectangle(10, 10, 100, 100);


            this.DrawToBitmap(bit, rect);

            bit.Save(@"d:\1.bmp");

            Image img = Win32.CaptureWindow(m_rdpview.Handle);
            img.Save(@"d:\2.bmp");
        }
    }
}
