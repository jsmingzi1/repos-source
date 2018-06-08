using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_TestSession
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.File.AppendAllText(@"d:\test.log", DateTime.Now.ToString() + ": " + System.Diagnostics.Process.GetCurrentProcess().SessionId + Environment.NewLine);
            SessionHelper sh = new SessionHelper();
            sh.printsession();
            Console.ReadLine();
        }
    }
}
