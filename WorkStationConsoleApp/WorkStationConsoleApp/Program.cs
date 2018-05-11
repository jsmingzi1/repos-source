using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinStationConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Win32Helper.Win7AboveSendSAS(true);
            Console.Read();
        }
    }
}
