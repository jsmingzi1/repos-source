using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct MessageData
    {
        public int nCode;
        [System.Runtime.InteropServices.MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string User;
        [System.Runtime.InteropServices.MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Pass;
    }

    class Utils
    {
        public static Config cfg = null;
        public static bool UnLockWinStation(string username, string password, string others, int type)
        {
            bool bRet = true;
            if (type == 0)
            {
                //without username and password, use default encrypted ones
                if (ClientWinService.m_SessionState == SessionChangeReason.RemoteConnect 
                    || ClientWinService.m_SessionState == SessionChangeReason.SessionLogon
                    || ClientWinService.m_SessionState == SessionChangeReason.SessionUnlock
                    || ClientWinService.m_SessionState == SessionChangeReason.ConsoleConnect)
                {
                    Utils.WriteLog("no need do actual action, as screen is not locked with last state "+ClientWinService.m_SessionState.ToString());
                    return true;
                }
                else
                {
                    if (cfg == null)
                    {
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        string directory = System.IO.Path.GetDirectoryName(path);
                        if (File.Exists(directory + "\\" + "config.ini")==false)
                        {
                            WriteLog("The config.ini does not exist, so stop the default unlock action.");
                            return false;
                        }
                        cfg = new Config();
                        cfg.load(directory + "\\config.ini");
                    }
                    string defaultUsername = CryptoEngine.Decrypt(cfg.getValue("username"), Config.cpuid());
                    string defaultPassword = CryptoEngine.Decrypt(cfg.getValue("password"), Config.cpuid());
                    //Utils.WriteLog("try to send username and password " + defaultUsername + defaultPassword);
                    Win32Helper.Win7AboveSendSAS(false);
                    SendUserNameAndPassword(defaultUsername, defaultPassword);
                    //Utils.WriteLog("after send username and password, try to send sas");
                    
                }
            }

            return bRet;
        }

        public static void SendUserNameAndPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return;
            try
            {
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".",
                    "TestPipe",
                    PipeDirection.InOut
                );

                pipeClient.Connect(10000);

                MessageData mess = new MessageData();
                mess.nCode = 1;
                mess.User = username;
                mess.Pass = password;

                byte[] b = getBytes(mess);
                //WriteLog("Write Pipe");
                pipeClient.Write(b, 0, b.Length);
                //WriteLog("End Write Pipe");
                pipeClient.Close();
                //WriteLog("Write Closed");
            } catch (Exception e) {
                WriteLog("Exception in SendUserNameAndPassword " + e.ToString());
            }
        }

        public static bool LoadCfg()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = System.IO.Path.GetDirectoryName(path);

            if (File.Exists(directory + "\\" + "config.ini"))
            {
                cfg = new Config();
                cfg.load(directory + "\\" + "config.ini");
            }
            else
            {
                return false;
            }

            return true;
        }

        //and from MyStructure to byte[]:
        private static byte[] getBytes(MessageData str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        //and from byte[] to MyStructure:

        private static MessageData fromBytes(byte[] arr)
        {
            MessageData str = new MessageData();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (MessageData)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        public static void WriteLog(string info, int type=0)
        {
            using (StreamWriter sw = File.AppendText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\run.log"))
            {
                sw.WriteLine(DateTime.Now.ToString("") + ":" + info);
            }
        }
    }
}
