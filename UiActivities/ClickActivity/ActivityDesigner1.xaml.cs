using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClickActivity
{
    // ActivityDesigner1.xaml 的交互逻辑
    public partial class ClickActivityDemoDesigner
    {
        HookProc _globalLlMouseHookCallback;
        IntPtr _hGlobalLlMouseHook;

        //HookProc _globalLlKeyboardHookCallback;
        IntPtr _hGlobalLlKeyboardHook;

        DispatcherTimer timer = null;
        UIA3Automation automation;
        AutomationElement HostElement = null;
        AutomationElement LastElement = null;
        FlaUI.Core.Shapes.Rectangle LastRect = null;

        String LastXPath = "";
        public ClickActivityDemoDesigner()
        {
            InitializeComponent();
            _hGlobalLlMouseHook = IntPtr.Zero;

            _hGlobalLlKeyboardHook = IntPtr.Zero;

            automation = new UIA3Automation();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);//.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();
            foreach (var i in automation.GetDesktop().FindAllChildren())
            {
                if (i.Properties.ProcessId == Process.GetCurrentProcess().Id)
                {
                    HostElement = i;
                    //i.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Minimized);
                    break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MinimizeCurrent();
            var path = this.textbox2.Expression.Content.ComputedValue.ToString();
            Task.Run(() =>
            {
                Click(path);
                NormalizeCurrent();
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                FlaUI.Core.Shapes.Point pt1 = FlaUI.Core.Input.Mouse.Position;
                AutomationElement e1 = automation.FromPoint(pt1);
                if (e1.Properties.ProcessId == Process.GetCurrentProcess().Id)
                    return;

                e1.DrawHighlight(false, System.Windows.Media.Colors.Blue, TimeSpan.FromMilliseconds(500));
                if (e1 != LastElement)
                {
                    LastElement = e1;
                    LastXPath = GetXPath(e1);
                    LastRect = e1.BoundingRectangle;
                }
                
            }
            catch (Exception)
            { }
        }


        //start
        private void Start()
        {
            SetUpHook();
            if (timer.IsEnabled == false)
                timer.Start();
        }

        //stop
        private void Stop()
        {

            ClearHook();
            if (timer.IsEnabled)
                timer.Stop();
        }

        private void SetUpHook()
        {
            var mar = NativeMethods.LoadLibraryEx("user32.dll", IntPtr.Zero, 0);
            if (_hGlobalLlMouseHook == IntPtr.Zero)
            {
                // Create an instance of HookProc.
                _globalLlMouseHookCallback = LowLevelMouseProc;
                
                _hGlobalLlMouseHook = NativeMethods.SetWindowsHookEx(
                    HookType.WH_MOUSE_LL,
                    _globalLlMouseHookCallback,
                    mar,
                    0);

                if (_hGlobalLlMouseHook == IntPtr.Zero)
                {
                    throw new Win32Exception("Unable to set MouseHook with last error code: " + NativeMethods.GetLastError().ToString());
                }
            }

            //if (_hGlobalLlKeyboardHook == IntPtr.Zero)
            //{
            //    // Create an instance of HookProc.
            //    //var mar = NativeMethods.LoadLibraryEx("user32.dll", IntPtr.Zero, 0);
            //    _hGlobalLlKeyboardHook = NativeMethods.SetWindowsHookEx(
            //        HookType.WH_KEYBOARD_LL,
            //        LowLevelKeyboardProc,
            //        //Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
            //        mar,
            //        0);

            //    if (_hGlobalLlKeyboardHook == IntPtr.Zero)
            //    {
            //        MessageBox.Show("set keyboard hook failed");
            //        throw new Win32Exception("Unable to set KeyboardHook with last error code: " + NativeMethods.GetLastError().ToString());
            //    }
            //}


        }

        private void ClearHook()
        {
            //Logger.Debug("Deleting global mouse hook");

            if (_hGlobalLlMouseHook != IntPtr.Zero)
            {
                // Unhook the low-level mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(_hGlobalLlMouseHook))
                    throw new Win32Exception("Unable to clear MouseHook;");

                _hGlobalLlMouseHook = IntPtr.Zero;
            }

            if (_hGlobalLlKeyboardHook != IntPtr.Zero)
            {
                // Unhook the low-level keyboard hook
                if (!NativeMethods.UnhookWindowsHookEx(_hGlobalLlKeyboardHook))
                    throw new Win32Exception("Unable to clear KeyboardHook;");

                _hGlobalLlKeyboardHook = IntPtr.Zero;
            }
        }

        bool PrintElement()
        {
            //var automation = new UIA3Automation();
            //FlaUI.Core.Shapes.Point pt1 = FlaUI.Core.Input.Mouse.Position;
            AutomationElement e1;
            string result;
            if (LastElement is null)
                return false;
            //if (LastElement is null)
            //{
            //    try
            //    {
                    
            //        e1 = automation.FromPoint(pt1);
            //    }
            //    catch (FileNotFoundException exception)
            //    {
            //        return false;
            //    }

            //    if (e1.Properties.ProcessId == Process.GetCurrentProcess().Id)
            //        return false;

            //    e1.DrawHighlight(false, System.Windows.Media.Colors.Blue, TimeSpan.FromSeconds(1));
            //    result = GetXPath(e1);// FormatXPath(GetXPath(e1));
            //}
            //else
            {
                e1 = LastElement;
                result = LastXPath;
            }

            ModelItem.Properties["Text"].SetValue(new InArgument<String>(result));
            //Bitmap bt = e1.Capture();
            //bt.Save(@"d:\3.bmp");
            //e1.
            String filename = @"d:\ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            ScreenShot(filename, (int)LastRect.Top, (int)LastRect.Left, (int)LastRect.Width, (int)LastRect.Height);

            BitmapImage image = new BitmapImage(new Uri(filename, UriKind.Absolute));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.Freeze();
            img.Source = image;
            
            return true;
        }

        void ScreenShot(string filename, int top, int left, int width, int height)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            using (Bitmap bmp = new Bitmap(width + 200, height + 80))
            {
                
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Opacity = .0;
                    g.CopyFromScreen(left - 100, top - 40, 0, 0, bmp.Size);
                    bmp.Save(filename);
                    Opacity = 1;
                }
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
                        //Dispatcher.Invoke(new Action(() =>
                        //{
 
                        //timer.Dispatcher.Invoke(new Action(() =>
                        //{ 
                            if (PrintElement())
                            {
                                Stop();
                                NormalizeCurrent();
                                //Application.Current.Dispatcher.Invoke(new Action(() =>
                                //{
                                //    Thread.Sleep(3000);
                                //    NormalizeCurrent();
                                //}));
                            }
                        //));
                        
                        
                        //
                        //if (System.Windows.Forms.Control.ModifierKeys.HasFlag(System.Windows.Forms.Keys.Control))
                        //    return 1;
                        //else
                        //    return 0;
                    }

                    //Logger.Debug("Left Mouse up");
                }

                if (wmMouse == MouseMessage.WM_RBUTTONDOWN /*&& RightButtonState == ButtonState.Released*/)
                {
                    //Logger.Debug("Right Mouse down");
                }
                if (wmMouse == MouseMessage.WM_RBUTTONUP /*&& RightButtonState == ButtonState.Down*/)
                {
                    Stop();
                    NormalizeCurrent();
                }
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(_hGlobalLlMouseHook, nCode, wParam, lParam);
        }

        public int LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)KeyboardMessage.WM_KEYDOWN)
            {
                //MessageBox.Show("key down Captured");
                int vkCode = Marshal.ReadInt32(lParam);
                if ((System.Windows.Forms.Keys)vkCode == System.Windows.Forms.Keys.Escape)
                {
                    //MessageBox.Show("ESC Captured");
                    Stop();
                }
                
            }

            // Pass the hook information to the next hook procedure in chain
            return NativeMethods.CallNextHookEx(_hGlobalLlKeyboardHook, nCode, wParam, lParam);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ClearHook();
        }

        private string FormatXPath(string path)
        {
            string final = "";
            string previous = "";
            foreach (var i in path.Split('/'))
            {
                if (i.Contains("["))
                {
                    previous = "";
                }
                else
                {
                    if (previous == i && i.Length == 0)
                        continue;

                    previous = i;
                }
                final += "/" + i;
            }
            return final;
        }
        private string GetXPath(AutomationElement e)
        {
            var parent = e.Automation.TreeWalkerFactory.GetControlViewWalker().GetParent(e);
            if (parent is null)
            {
                return "";
            }
            else
            {
                return GetXPath(parent) + "/" + e.ControlType + (String.IsNullOrEmpty(e.Name) ? "" : ("[@Name='" + e.Name + "']"));
            }
        }

        //get top window
        AutomationElement GetTopWindow(AutomationElement e)
        {
            if (e is null || e.Parent is null)
                return null;

            AutomationElement e1 = e.Parent;
            if (e1.Parent is null)
            {
                return e;
            }
            return GetTopWindow(e.Parent);


        }

        private void Click(string xpath)
        {
            if (automation is null)
                return;
            try
            {
                var e1 = automation.GetDesktop();
                var e2 = e1.FindFirstByXPath(xpath);
                var e3 = GetTopWindow(e2);
                if (e3 is null)
                {
                    MessageBox.Show("top window is null");
                    return;
                }
                if (e3.Patterns.Window.Pattern.WindowVisualState == WindowVisualState.Minimized)
                {
                    e3.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
                }

                e2.SetForeground();
                e2.Click();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //test
        private void test()
        {
            //MessageBox.Show("button click: " + this.textbox2.Expression.Content.ComputedValue);
            //MessageBox.Show(FormatXPath("/Windows/Panel/Panel/Button[@Name='9']"));
            var e1 = automation.GetDesktop();
            MessageBox.Show("kk");
            MessageBox.Show("button click: " + this.textbox2.Expression.Content.ComputedValue);
            MessageBox.Show(ModelItem.Properties["Text"].ToString());
            if (textbox2.Expression.Content.ComputedValue.ToString().Trim().Length == 0)
                return;

            MessageBox.Show(textbox2.Expression.Content.ComputedValue.ToString());
            try
            {
                var e2 = e1.FindFirstByXPath(textbox2.Expression.Content.ComputedValue.ToString().Trim());
                var e3 = GetTopWindow(e2);
                if (e3 is null)
                {
                    MessageBox.Show("top window is null");
                    return;
                }
                if (e3.Patterns.Window.Pattern.WindowVisualState == WindowVisualState.Minimized)
                {
                    e3.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
                }
                MinimizeCurrent();
                e2.SetForeground();
                e2.Click();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void ActivityDesigner_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
            //MessageBox.Show("Unload");
        }

        void MinimizeCurrent()
        {
            //if (HostElement is null)
            //{
            //    MessageBox.Show("hosted app is null");
            //}
            //else
            {
                //HostElement.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Minimized);
                //Application.Current.MainWindow.Hide();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Application.Current.MainWindow.Hide();
                }));
            }
        }

        void NormalizeCurrent()
        {
            //if (HostElement is null)
            //{
            //    MessageBox.Show("hosted app is null");
            //    //Monitor.
            //}
            //else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Application.Current.MainWindow.Show();
                }));
                //HostElement.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
                //Application.Current.MainWindow.Dispatcher.Invoke(()=> {  });
                //System.Windows.Forms.Control.Invoke(new System.Windows.Forms.MethodInvoker(delegate () { Application.Current.MainWindow.Show(); }));
                //Application.Current.MainWindow.Show();
                //HostElement = null;
            }
            //var d = automation.GetDesktop();
            //foreach (var i in d.FindAllChildren())
            //{
            //    if (i.Properties.ProcessId == Process.GetCurrentProcess().Id)
            //    {
            //        i.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
            //        break;
            //    }
            //}
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MinimizeCurrent();
                Start();
        }
    }
}
