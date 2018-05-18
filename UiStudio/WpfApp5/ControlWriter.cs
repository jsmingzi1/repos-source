using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfApp5
{

    public class ControlWriter : TextWriter
    {
        private System.Windows.Controls.TextBox textbox;
        public ControlWriter(System.Windows.Controls.TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {

            if (textbox.Dispatcher.CheckAccess())
            {
                textbox.Text += value;
            }
            else
            {
                textbox.Dispatcher.Invoke(delegate { textbox.Text += value; });
            }
        }

        public override void Write(string value)
        {
            if (textbox.Dispatcher.CheckAccess())
            {
                textbox.Text += value;
            }
            else
            {
                textbox.Dispatcher.Invoke(delegate{textbox.Text += value;});
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
