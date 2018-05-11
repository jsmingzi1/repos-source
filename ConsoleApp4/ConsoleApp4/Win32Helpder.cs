using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public class Win32Helper
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();


        [System.Flags]
        enum LoadLibraryFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
            LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
            LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
            LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,
            LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
            LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
            LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,
            LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        internal delegate void SendSAS(bool bUser);
        public static void Win7AboveSendSAS(bool bUser)
        {
            IntPtr hModule = LoadLibraryEx("sas.dll", IntPtr.Zero, 0);
            if (hModule == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr fptr = GetProcAddress(hModule, "SendSAS");
            SendSAS drs = (SendSAS)Marshal.GetDelegateForFunctionPointer(fptr, typeof(SendSAS));
            drs(bUser); // call via a function pointer
        }

        [DllImport("user32", EntryPoint = "OpenDesktopA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 OpenDesktop(string lpszDesktop, Int32 dwFlags, bool fInherit, Int32 dwDesiredAccess);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 CloseDesktop(Int32 hDesktop);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 SwitchDesktop(Int32 hDesktop);

        public static bool IsWorkstationLocked()
        {
            const int DESKTOP_SWITCHDESKTOP = 256;
            int hwnd = -1;
            int rtn = -1;

            hwnd = OpenDesktop("Default", 0, false, DESKTOP_SWITCHDESKTOP);

            if (hwnd != 0)
            {
                rtn = SwitchDesktop(hwnd);
                if (rtn == 0)
                {
                    // Locked
                    CloseDesktop(hwnd);
                    return true;
                }
                else
                {
                    // Not locked
                    CloseDesktop(hwnd);
                }
            }
            else
            {
                // Error: "Could not access the desktop..."
            }

            return false;
        }
    }

}
