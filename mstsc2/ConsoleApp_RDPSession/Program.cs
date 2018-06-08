using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp_RDPSession
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Process.GetCurrentProcess().SessionId);
            Console.WriteLine(Environment.UserName);
            if (SystemInformation.TerminalServerSession)
            {
                Console.WriteLine("current session is running in rdp");
            }
            else
            {
                Console.WriteLine("current session is running in console");
            }
            SessionHelper helper = new SessionHelper();
            helper.printsession();

            Console.ReadLine();
            Thread thread = new Thread(new ThreadStart(runThread));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            do
            {
                Console.WriteLine("press exit to quit the program");
            } while ("exit" == Console.ReadLine());

            thread.Abort();
        }

        static void runThread()
        {
            //RDPSession rdp = new RDPSession();
            //rdp.connect("127.0.0.1", "", "lcm1", "mz@880330");
            File.AppendAllText(@"d:\1.log", "start run\r\n");
            Application.Run(new RDPSession("127.0.0.1", "", "lcm1", "mz@880330"));
            File.AppendAllText(@"d:\1.log", "end run\r\n");
        }
    }

    class RDPSession:Form
    {
        private AxMSTSCLib.AxMsRdpClient8NotSafeForScripting m_rdpview;

        private void init()
        {
            this.m_rdpview = new AxMSTSCLib.AxMsRdpClient8NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.m_rdpview)).BeginInit();
            this.m_rdpview.Enabled = true;
            this.m_rdpview.CreateControl();
            this.m_rdpview.OnConnecting += new System.EventHandler(this.m_rdpview_OnConnecting);
            this.m_rdpview.OnConnected += new System.EventHandler(this.m_rdpview_OnConnected);
            this.m_rdpview.OnLoginComplete += new System.EventHandler(this.m_rdpview_OnLoginComplete);
            this.m_rdpview.OnFatalError += new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(this.m_rdpview_OnFatalError);
            this.m_rdpview.OnWarning += new AxMSTSCLib.IMsTscAxEvents_OnWarningEventHandler(this.m_rdpview_OnWarning);
            this.m_rdpview.OnAuthenticationWarningDisplayed += new System.EventHandler(this.m_rdpview_OnAuthenticationWarningDisplayed);
            this.m_rdpview.OnAuthenticationWarningDismissed += new System.EventHandler(this.m_rdpview_OnAuthenticationWarningDismissed);
            this.m_rdpview.OnLogonError += new AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEventHandler(this.m_rdpview_OnLogonError);
            ((System.ComponentModel.ISupportInitialize)(this.m_rdpview)).EndInit();
        }

        private void m_rdpview_OnConnecting(object sender, EventArgs e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnConnecting");
        }

        private void m_rdpview_OnAuthenticationWarningDismissed(object sender, EventArgs e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnAuthenticationWarningDismissed");
        }

        private void m_rdpview_OnAuthenticationWarningDisplayed(object sender, EventArgs e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnAuthenticationWarningDisplayed");
        }

        private void m_rdpview_OnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnLogonError");
        }

        private void m_rdpview_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnFatalError");
        }

        private void m_rdpview_OnWarning(object sender, AxMSTSCLib.IMsTscAxEvents_OnWarningEvent e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnWarning");
        }

        private void m_rdpview_OnConnected(object sender, EventArgs e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnConnected");
        }

        private void m_rdpview_OnLoginComplete(object sender, EventArgs e)
        {
            File.AppendAllText(@"d:\1.log", "m_rdpview_OnLoginComplete");
        }

        public void connect(string host, string domain, string username, string password)
        {
            m_rdpview.Server = host;
            m_rdpview.Domain = domain;
            m_rdpview.UserName = username;

            m_rdpview.AdvancedSettings8.ClearTextPassword = password;
            m_rdpview.AdvancedSettings8.AuthenticationLevel = 2;
            m_rdpview.AdvancedSettings8.EnableCredSspSupport = true;
            m_rdpview.AdvancedSettings8.NegotiateSecurityLayer = false;
            m_rdpview.Connect();
        }

        public void disconnect()
        {
            if (m_rdpview.Connected == 1)
            {
                m_rdpview.Disconnect();
            }
        }

        public RDPSession(string host, string domain, string username, string password)
        {
            init();
            connect(host, domain, username, password);
        }
        ~RDPSession()
        {
            disconnect();
        }
    }
}
