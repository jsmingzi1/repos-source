using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Configuration;

namespace rdphelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(ConfigurationManager.AppSettings["resolution"]);
            Console.WriteLine(Environment.OSVersion + " is Server " + IsServerVersion().ToString());
            Dictionary<string, string> cfg = new Dictionary<string, string>();

            if (VerifyParameter(args, cfg))
            {
                
                //session 0 , need create rdp
                bool bNeedRDP = true;
                SessionHelper helper = new SessionHelper();
                foreach(var s in helper.getsessionlist())
                {
                    if (s.username == cfg["user"] && s.sessionstate == SessionHelper.WTS_CONNECTSTATE_CLASS.WTSActive)
                    {
                        bool bExist = false;
                        foreach(var p in Process.GetProcesses())
                        {
                            if (p.ProcessName == "LogonUI")
                            {
                                //locked still need recreate rdp
                                bExist = true;
                                break;
                            }
                        }
                        if (bExist)
                            bNeedRDP = true;
                        else
                            bNeedRDP = false;
                        break;
                    }
                }
                if (bNeedRDP == false)
                {
                    Console.WriteLine("Already have active session without lock state, with user " + cfg["user"] + "so no need process this request");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new Form1("127.0.0.1", cfg["user"], cfg["pass"], bool.Parse(cfg["show"]), bool.Parse(cfg["console"])));
            }
            else
            {
                PrintHelp();
                //Console.ReadLine();
            }
            //else
            //{
            //    //normal process need unlock it
            //    while (true)
            //    {
            //        ProcessStartInfo info = new ProcessStartInfo("tscon.exe");
            //        info.Arguments = Process.GetCurrentProcess().SessionId.ToString() + " /dest:console";
            //        info.UseShellExecute = false;
            //        info.Verb = "runas";
            //        bool b = File.Exists(@"d:\unlock.txt");
            //        while (b == false)
            //        {
            //            b = File.Exists(@"d:\unlock.txt");
            //        }
            //        Process.Start(info);
            //        File.Delete(@"d:\unlock.txt");
            //    }
            //}
        }


        public static bool VerifyParameter(string[] args, Dictionary<string, string> cfg)
        {
            string[] para = new string[]{ "user", "pass", "show", "console" };
            foreach(var i in args)
            {
                if (i.ToLower() == "show" || i.ToLower() == "console")
                {
                    cfg.Add(i.ToLower(), "true");
                    continue;
                }
                else if (i.Contains(":"))
                {
                    string key = System.Text.RegularExpressions.Regex.Split(i, ":")[0].ToLower();
                    string value = System.Text.RegularExpressions.Regex.Split(i, ":")[1];
                    if (para.Contains(key))
                        cfg.Add(key, value);
                    else
                        return false;
                }
                else
                {
                    return false;
                }

            }

            if (cfg.Keys.Contains("user") == false && cfg.Keys.Contains("pass") == false)
                return false;
            if (cfg.Keys.Contains("show") == false)
                cfg.Add("show", "false");
            if (cfg.Keys.Contains("console") == false)
                cfg.Add("console", "false");

            if (Process.GetCurrentProcess().SessionId == 0)
                cfg["show"] = "false";
            return true;
        }

        public static void PrintHelp()
        {
            Console.WriteLine("This application should be executed in 0 session.");
            string appname = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            Console.WriteLine(appname + " user:<username> pass:<password> [show] [console]");
        }

        public static bool IsServerVersion()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            {
                foreach (ManagementObject managementObject in searcher.Get())
                {
                    // ProductType 将是以下之一： 
                    // 1: 工作站 
                    // 2: 域控制器 
                    // 3: 服务器 
                    uint productType = (uint)managementObject.GetPropertyValue("ProductType");
                    return productType != 1;
                }
            }
            return false;
        }
    }
}
