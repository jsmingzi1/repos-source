using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceModel; //for WCF
using System.ServiceProcess;   //for Windows Service
using System.Configuration;
using System.ServiceModel.Description;
using System.Configuration.Install; //for Windows Service Installer
using System.Runtime.InteropServices; //for pinvoke
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace ConsoleApp4
{
    //public class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Create the binding to be used by the service.
    //        //"http://localhost:8888"
    //        //"net.tcp://localhost:9999"
    //        using (ServiceHost host = new ServiceHost(typeof(CalculatorService), 
    //            new Uri[] { new Uri("http://localhost:8888"), new Uri("net.tcp://localhost:9999") }))
    //        {
    //            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
    //            //smb.HttpGetEnabled = true;  --if enable this, the default behavior will be http, that even below http endpoint not add suffix, still can access it by default base address;
    //            //                           that mean, if enable, baseaddr + suffix can access, baseaddr still can access;
    //            //                           but if disable by default, only baseaddr + suffix can access, this is safer for us
    //            //smb.HttpGetUrl = new Uri(baseAddress);
    //            host.Description.Behaviors.Add(smb);

    //            BasicHttpBinding binding1 = new BasicHttpBinding();
    //            host.AddServiceEndpoint(typeof(ICalculator), binding1, "");
    //            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "ppp");
                

    //            NetTcpBinding binding2 = new NetTcpBinding();
    //            host.AddServiceEndpoint(typeof(ICalculator), binding2, "");
    //            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "ppp");
               

    //            host.Open();
    //            Console.ReadLine();
    //            host.Close();
    //        }
    //    }
    //}
    public class ClientWinService : ServiceBase
    {
        ServiceHost host = null;
        public static SessionChangeReason m_SessionState = SessionChangeReason.SessionUnlock;
        public static Thread m_ScanThread = null;

        static void Main(string[] args)
        {
            ServiceBase.Run(new ClientWinService());
        }

        public ClientWinService()
        {
            ServiceName = "ClientWinService";
            this.CanHandleSessionChangeEvent = true;
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            //if (e.Reason == SessionSwitchReason.SessionLock)
            //{
            //    m_SessionState = SessionSwitchReason.SessionLock;
            //    //I left my desk
            //}
            //else if (e.Reason == SessionSwitchReason.SessionUnlock)
            //{
            //    m_SessionState = SessionSwitchReason.SessionUnlock;
            //    //I returned to my desk
            //}
            m_SessionState = changeDescription.Reason;
            //Utils.WriteLog(changeDescription.Reason.ToString());
            //base.OnSessionChange(changeDescription);
        }

        protected override void OnStart(String[] args)
        {
            if (m_ScanThread == null)
            {
                m_ScanThread = new Thread(new ThreadStart(ThreadProc));
                m_ScanThread.Start();
                //Utils.WriteLog("Local scan thread is started now.");
            }
            StartWCFService();
        }
        void StartWCFService()
        {
            if (host != null)
            {
                host.Close();
            }
            // Create the binding to be used by the service.
            //"http://localhost:8888"
            //"net.tcp://localhost:9999"
            host = new ServiceHost(typeof(WinStationService),
                new Uri[] { new Uri("http://localhost:8888"), new Uri("net.tcp://localhost:9999") });
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                //smb.HttpGetEnabled = true;  --if enable this, the default behavior will be http, that even below http endpoint not add suffix, still can access it by default base address;
                //                           that mean, if enable, baseaddr + suffix can access, baseaddr still can access;
                //                           but if disable by default, only baseaddr + suffix can access, this is safer for us
                //smb.HttpGetUrl = new Uri(baseAddress);
                host.Description.Behaviors.Add(smb);

                BasicHttpBinding binding1 = new BasicHttpBinding();
                ServiceEndpoint point1 = host.AddServiceEndpoint(typeof(IWinStation), binding1, "");
                point1.Name = "http";
                ServiceEndpoint point2 = host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "ppp");
                point2.Name = "httpmeta";

                NetTcpBinding binding2 = new NetTcpBinding();
                ServiceEndpoint point3 = host.AddServiceEndpoint(typeof(IWinStation), binding2, "");
                point3.Name = "nettcp";
                ServiceEndpoint point4 = host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "ppp");
                point4.Name = "nettcpmeta";

                host.Open();
                //Console.ReadLine();
                //host.Close();
            }

        protected override void OnStop()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }


        public static void ThreadProc()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = System.IO.Path.GetDirectoryName(path);

            while (true)
            {
                
                if (File.Exists(directory + "\\" + "unlock.txt"))
                {
                    Utils.WriteLog("found unlock.txt, start to unlock action");
                    Utils.UnLockWinStation("", "", "", 0);
                    File.Delete(directory + "\\" + "unlock.txt");
                    Utils.WriteLog("found unlock.txt, end to unlock action");
                    Thread.Sleep(2000);
                }
                else if (File.Exists(directory + "\\" + "lock.txt"))
                {
                    Utils.WriteLog("found lock.txt, start to lock action");
                    Win32Helper.LockWorkStation();
                    File.Delete(directory + "\\" + "lock.txt");
                    Thread.Sleep(2000);
                }
                else
                {
                    //Utils.WriteLog("Local scan thread, nothing found will sleep 5s.");
                    //Thread.Sleep(5000);
                }
            }
        }
    }


    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "ClientWinService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }




}
