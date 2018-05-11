using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct Stu
    {
        public int Age;
        [System.Runtime.InteropServices.MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //write
        private void button1_Click(object sender, EventArgs e)
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".",
                "TestPipe",
                PipeDirection.InOut
            );
            pipeClient.Connect();

            //char[] str = new char[200];
            //char[] src = "hi, this is a test!".ToCharArray();

            //Array.Copy(src, str, src.Length);

            //byte[] dst = Encoding.ASCII.GetBytes(str);
            //byte[] rt = new byte[200];
            //Array.Copy(dst, rt, dst.Length);
            //MessageBox.Show(Encoding.ASCII.GetString(rt));
            Stu stu = new Stu();
            stu.Age = 10;
            stu.Name = "123456";
            byte[] b = this.getBytes(stu);
            pipeClient.Write(b, 0, b.Length);

            pipeClient.Close();
        }
        //read
        private void button2_Click(object sender, EventArgs e)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream(
                "TestPipe",
                PipeDirection.InOut,
                10,
                PipeTransmissionMode.Byte,
                PipeOptions.None
            );
            pipeServer.WaitForConnection();

            Thread.Sleep(10000);
            //StreamReader read = new StreamReader(pipeServer);
            //char[] s = new char[100];
            //while (read.Peek()>=0)
            //{
            //    read.Read(s, 0, s.Length);
            //}

            //while (true)
            {
                byte[] buff = new byte[255];
                int c = pipeServer.Read(buff, 0, 255);

                MessageBox.Show(c.ToString());
                if (c > 0)
                {
                    Stu s = fromBytes(buff);
                    MessageBox.Show(s.Age.ToString());
                    MessageBox.Show(s.Name);
                    //string str = Encoding.ASCII.GetString(buff);
                    //MessageBox.Show(str);
                    listBox1.Items.Add(s.Name);
                }
                else
                    Thread.Sleep(3000);
            }

        }

        //and from MyStructure to byte[]:
        public byte[] getBytes(Stu str)
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
 
        public Stu fromBytes(byte[] arr)
        {
            Stu str = new Stu();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (Stu)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
    }
}
