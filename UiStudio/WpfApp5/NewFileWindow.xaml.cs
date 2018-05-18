using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp5
{
    /// <summary>
    /// NewFileWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewFileWindow : Window
    {
        private string m_ret_filename;
        public NewFileWindow(string foldername)
        {
            InitializeComponent();
            m_foldername.Text = foldername;
            m_ret_filename = "";
        }

        //ok
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_filename.Text))
            {
                m_filenamelabel.Content = "Wrong filename, please input new filename";
            }
            else if (File.Exists(m_foldername.Text + "\\" +GetFileName(m_filename.Text)))
            {
                m_filenamelabel.Content = "Existing filename, please input new filename";
            }
            else
            {
                m_ret_filename = m_filename.Text;
                this.Close();
                //this.DialogResult = true;
            }
        }

        //cancel
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            //this.DialogResult = false;
        }

        public string GetFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename) && string.IsNullOrEmpty(m_ret_filename))
                return "";

            string file = "";
            if (string.IsNullOrEmpty(filename))
                file = m_ret_filename;
            else
                file = filename;

                if (file.ToLower().EndsWith(".xaml"))
                    return file;
                else
                    return file + ".xaml";
            

        }
    }
}
