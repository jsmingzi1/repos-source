using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
//using FlaUI.UIA2;
using FlaUI.UIA3;
using FlaUI.Core.Shapes;
using FlaUI.Core.Input;
using System.Threading;
using System.Diagnostics;
//using FlaUInspect.Core;

namespace ConsoleApp_FlaUI
{
    class Program
    {

        static void Main(string[] args)
        {
            MouseHook.SetHook(null);
            Thread.Sleep(3000);

            var automation = new UIA3Automation();
            Point pt = Mouse.Position;
            AutomationElement e = automation.FromPoint(pt);
            Console.WriteLine("process: " + e.Properties.ProcessId + ", " + Process.GetProcessById(e.Properties.ProcessId).ProcessName);
            if (e.Properties.AutomationId.IsSupported)
                Console.WriteLine("Automation Id(Control Id):" + e.AutomationId);
            Console.WriteLine("Control Type:" + e.ControlType);
            Console.WriteLine("ClassName:" + e.ClassName);
            Console.WriteLine("Name:" + e.Name);

            Console.WriteLine("Hierarchy Print");
            PrintElementHierarchy(e);
            Console.ReadLine();
        }

        static void PrintElementHierarchy(AutomationElement e)
        {
            if (e.Parent != null)
            {
                PrintElementHierarchy(e.Parent);
            }
            if (e.Properties.AutomationId.IsSupported)
                Console.Write("Automation Id(Control Id):" + e.AutomationId);
            Console.Write(", Control Type:" + e.ControlType);
            Console.Write(", ClassName:" + e.ClassName);
            Console.WriteLine(", Name:" + e.Name);
        }

        static void PrintAll()
        {
            //var app = Application.Launch("notepad.exe");
            using (var automation = new UIA3Automation())
            {
                AutomationElement e = automation.GetDesktop();
                Console.WriteLine(e.ToString());
                if (e.FindAllChildren().Count() > 0)
                {
                    PrintElement(e);
                }
                Console.ReadLine();

                //var window = app.GetMainWindow(automation);
                //Console.WriteLine(window.Title);
            }
        }

        static void PrintElement(AutomationElement top, string prefix="    ")
        {
            foreach(var e in top.FindAllChildren())
            {
                Console.WriteLine(prefix + e.Name);
                if (e.FindAllChildren().Count() > 0)
                {
                    PrintElement(e, prefix + "    ");
                }
            }
        }
    }
}
