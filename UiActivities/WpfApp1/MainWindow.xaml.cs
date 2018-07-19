using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HookProc _globalLlMouseHookCallback;
        IntPtr _hGlobalLlMouseHook = IntPtr.Zero;
        IntPtr _hGlobalLlKeyboardHook = IntPtr.Zero;
        DispatcherTimer timer = null;
        FlaUI.Core.Shapes.Point pt = null;
        UIA3Automation automation;
        public MainWindow()
        {
            InitializeComponent();
            textBox1.IsReadOnly = false;
            textBox1.AcceptsReturn = true;
            automation = new UIA3Automation();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();
        }

       

        private void Timer_Tick(object sender, EventArgs e)
        {
            FlaUI.Core.Shapes.Point pt1 = FlaUI.Core.Input.Mouse.Position;
            AutomationElement e1;
            try
            {
                e1 = automation.FromPoint(pt1);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            //AutomationElement e1 = automation.FromPoint(pt1);
            if (e1.Properties.ProcessId == Process.GetCurrentProcess().Id)
                return;

            e1.DrawHighlight(false, System.Windows.Media.Colors.Blue, TimeSpan.FromSeconds(1.4));

            
            //MessageBox.Show("hi");
        }


        //start
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_hGlobalLlMouseHook == IntPtr.Zero)
            {
                //textBox1.Text = "Mouse Hook is started";
                SetUpHook();
                this.Hide();
            }
        }

        //stop
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_hGlobalLlMouseHook != IntPtr.Zero)
            {
                ClearHook();
                _hGlobalLlMouseHook = IntPtr.Zero;
            }
                
        }

        private void SetUpHook()
        {
            //Logger.Debug("Setting up global mouse hook");
            timer.Start();
            // Create an instance of HookProc.
            _globalLlMouseHookCallback = LowLevelMouseProc;

            _hGlobalLlMouseHook = NativeMethods.SetWindowsHookEx(
                HookType.WH_MOUSE_LL,
                _globalLlMouseHookCallback,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                0);

            if (_hGlobalLlMouseHook == IntPtr.Zero)
            {
                //Logger.Fatal("Unable to set global mouse hook");
                throw new Win32Exception("Unable to set MouseHook");
            }

            //var mar = NativeMethods.LoadLibraryEx("useru32.dll", IntPtr.Zero, 0);
            ///////////e
            //Process curProcess = Process.GetCurrentProcess();
            //ProcessModule curModule = curProcess.MainModule;
            //_hGlobalLlKeyboardHook = NativeMethods.SetWindowsHookEx(
            //    HookType.WH_KEYBOARD_LL,
            //    HookCallback,
            //    //Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
            //    //mar,
            //    NativeMethods.GetModuleHandle(curModule.ModuleName),
            //    0);

            //if (_hGlobalLlKeyboardHook == IntPtr.Zero)
            //{
            //    //Logger.Fatal("Unable to set global mouse hook");
            //    throw new Win32Exception("Unable to set KeyHook");
            //}
        }

        public int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)KeyboardMessage.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                //MessageBox.Show("key down "+ vkCode.ToString());
            }

            return 0;// NativeMethods.CallNextHookEx(_hGlobalLlKeyboardHook, nCode, wParam, lParam);
        }

        private void ClearHook()
        {
            //Logger.Debug("Deleting global mouse hook");
            timer.Stop();
            if (_hGlobalLlMouseHook != IntPtr.Zero)
            {
                // Unhook the low-level mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(_hGlobalLlMouseHook))
                    throw new Win32Exception("Unable to clear MouseHoo;");

                _hGlobalLlMouseHook = IntPtr.Zero;
            }

            if (_hGlobalLlKeyboardHook != IntPtr.Zero)
            {
                // Unhook the low-level mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(_hGlobalLlKeyboardHook))
                    throw new Win32Exception("Unable to clear KeyboardHoo;");

                _hGlobalLlKeyboardHook = IntPtr.Zero;
            }
        }

        void PrintElement()
        {
            //var automation = new UIA3Automation();
            FlaUI.Core.Shapes.Point pt1 = FlaUI.Core.Input.Mouse.Position;
            AutomationElement e1 = automation.FromPoint(pt1);
            if (e1.Properties.ProcessId == Process.GetCurrentProcess().Id)
                return;

            e1.DrawHighlight(true, System.Windows.Media.Colors.Blue, TimeSpan.FromSeconds(1));
            
            //textBox1.Text = Process.GetProcessById(e.Properties.ProcessId).ProcessName + "(" + e.Properties.ProcessId + ")" + Environment.NewLine;
            //PrintElementHierarchy(e);
            textBox1.Text += FormatXPath(GetXPath(e1)) + Environment.NewLine;

            //if (true)
            //{
            //    FlaUI.Core.Shapes.Point pt1 = FlaUI.Core.Input.Mouse.Position;
            //    AutomationElement e = automation.FromPoint(pt1);
            //    e.DrawHighlight(true, System.Windows.Media.Colors.Blue, TimeSpan.FromSeconds(1));
            //}
            //else
            //{
            //    //AutomationElement e = automation.FromPoint(pt);
            //    //e.DrawHighlight(true, System.Windows.Media.Colors.Blue, TimeSpan.FromSeconds(1));
            //    //if (e.Properties.ProcessId == Process.GetCurrentProcess().Id)
            //    //    return;
            //    //textBox1.Text = Process.GetProcessById(e.Properties.ProcessId).ProcessName + "(" + e.Properties.ProcessId + ")" + Environment.NewLine;
            //    //PrintElementHierarchy(e);
            //}


        }

        void PrintElementHierarchy(AutomationElement e)
        {
            if (e.Parent != null)
            {
                PrintElementHierarchy(e.Parent);
                textBox1.Text += "Id: ";
                if (e.Properties.AutomationId.IsSupported)
                    textBox1.Text += e.AutomationId;

                textBox1.Text += "    Type: " + e.ControlType;
                textBox1.Text += "    Cls: ";
                if (e.Properties.ClassName.IsSupported)
                    textBox1.Text += e.ClassName.ToString();
                textBox1.Text += "    Name: " + e.Name + Environment.NewLine;
            }
        }

        public int LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Get the mouse WM from the wParam parameter
                var wmMouse = (MouseMessage)wParam;
                if (wmMouse == MouseMessage.WM_LBUTTONDOWN /*&& LeftButtonState == ButtonState.Released*/)
                {
                    //Logger.Debug("Left Mouse down");
                }
                if (wmMouse == MouseMessage.WM_LBUTTONUP /*&& LeftButtonState == ButtonState.Down*/)
                {
                    //if (System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control))
                    {
                        //Invoke(new Action(() =>
                        //{
                        //pt = FlaUI.Core.Input.Mouse.Position;
                        //}));
                        PrintElement();

                        //Thread thread1 = new Thread(new ThreadStart(A));
                        //thread1.Start();
                        //thread1.Join();
                        //MessageBox.Show("start to clean hook");
                        ClearHook();
                        this.Show();
                        //Application.Current.Dispatcher.Invoke(new Action(() =>
                        //{
                        //    PrintElement();
                        //}));
                        //
                        if (System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control))
                            return 0;
                    }

                    //Logger.Debug("Left Mouse up");
                }

                if (wmMouse == MouseMessage.WM_RBUTTONDOWN /*&& RightButtonState == ButtonState.Released*/)
                {
                    //Logger.Debug("Right Mouse down");
                }
                if (wmMouse == MouseMessage.WM_RBUTTONUP /*&& RightButtonState == ButtonState.Down*/)
                {
                    //Logger.Debug("Right Mouse up");
                }
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(_hGlobalLlMouseHook, nCode, wParam, lParam);
        }

        public void A()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PrintElement();
            }));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ClearHook();
        }

        private string FormatXPath(string path)
        {
            string final = "";
            string previous = "";
            foreach(var i in path.Split('/'))
            {
                if (i.Contains("["))
                {
                    previous = "";
                }
                else
                {
                    if (previous == i)
                        continue;

                    previous = i;
                }
                final += "/" + i;
            }
            return final;
        }
        private string GetXPath(AutomationElement e)
        {
            if (e.Parent is null)
            {
                return "/";
            }
            else
            {
                return GetXPath(e.Parent) + "/" + e.ControlType + (String.IsNullOrEmpty(e.Name) ? "" : ("[@Name='" + e.Name + "']"));
            }
        }
        //test
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(FormatXPath("/Windows/Panel/Panel/Button[@Name='9']"));
            // /Pane/Pane[@Name='Running applications']/ToolBar[@Name='Running applications']/Button[@Name='Calculator']
            // /Pane/Pane[@Name='Running applications']/ToolBar[@Name='Running applications']/Button[@Name='Calculator']
            // /Pane/Pane/Pane[@Name='Running applications']/ToolBar[@Name='Running applications']/Button[@Name='Calculator']
            // /Pane/Pane/Pane[@Name='Running applications']/ToolBar[@Name='Running applications']/Button[@Name='Calculator']
            Thread.Sleep(3000);
            var et = automation.FromPoint(FlaUI.Core.Input.Mouse.Position);
            var tmp = FlaUI.Core.Debug.GetXPathToElement(et);
            var e1 = automation.GetDesktop();
            var e3 = e1.FindFirstByXPath("/Pane/Pane/Pane[@Name='Running applications']/ToolBar[@Name='Running applications']/Button[@Name='Calculator']");
            e3.Click();
            string[] str =
            {
                "/Window[@Name='Calculator']//Button[@Name='9']",
                "/Window[@Name='Calculator']/Pane/Button[@Name='Add']",
                "/Window[@Name='Calculator']/Pane[@Name='']/Button[@Name='6']",
                "/Window[@Name='Calculator']/Pane[@Name='']/Button[@Name='Equals']"
            };

            foreach (var i in System.Text.RegularExpressions.Regex.Split(textBox1.Text, Environment.NewLine))
            {
                if (i.Trim().Length == 0)
                    continue;
                var e2 = e1.FindFirstByXPath(i);
                
                e2.Click();
                Thread.Sleep(1000);
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MessageBox.Show("left button pressed");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Form1 f = new Form1();
            f.ShowDialog();
        }
    }
}
