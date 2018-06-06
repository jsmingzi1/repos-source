using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScreenRecorderLib;

namespace ScreenRecorder
{
    class Program
    {
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                Console.WriteLine("Console window closing, death imminent");
            }
            return false;
        }

        static Recorder recorder = null;
        static void Main(string[] args)
        {
            if (args.Count() != 1 || Directory.Exists(args[0]) == false)
            {
                Console.WriteLine("invalid parameter");
                PrintHelp();
                return;
            }

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            try
            {
                while (true)
                {
                    int i;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception");
                File.AppendAllText(@"d:\1234.txt", "456");
            }
            finally
            {
                Console.WriteLine("finally");
                File.AppendAllText(@"d:\123.txt", "456");
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine(Process.GetCurrentProcess().ProcessName + " <Path>");
        }
    }
}
