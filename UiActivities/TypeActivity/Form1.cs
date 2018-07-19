using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypeActivity
{
    public partial class Form1 : Form
    {
        int Sec = 5;
        int Pos = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void SetPos()
        {
            if (Pos == 0)
            {
                this.Left = Screen.PrimaryScreen.Bounds.Width - 100;
                this.Top = Screen.PrimaryScreen.Bounds.Height - 100;
            }
            else
            {
                this.Left = 100 - this.Width;
                this.Top = 100 - this.Height;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Height = 50;
            this.Width = 70;

            panel1.Left = 4;
            panel1.Top = 4;
            panel1.Height = this.Height - 8;
            panel1.Width = this.Width - 8;
            label1.Left = 4;
            label1.Top = 4;
            label1.Height = panel1.Height - 8;
            label1.Width = panel1.Width - 8;

            SetPos();
            label1.Text = "" + Sec;
            timer1.Start();
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if (Pos == 0)
                Pos = 1;
            else
                Pos = 0;

            SetPos();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Sec--;
            if (Sec > 0)
                label1.Text = "" + Sec;
            else
                this.Close();
        }
    }
}
