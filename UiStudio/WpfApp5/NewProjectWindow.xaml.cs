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

namespace UiStudio
{
    /// <summary>
    /// NewProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        private string m_ret_filename;
        public NewProjectWindow(string foldername)
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
                m_filenamelabel.Content = "Empty project, please input new project name";
            }
            else if (Directory.Exists(m_foldername.Text + "\\" +m_filename.Text))
            {
                m_filenamelabel.Content = "Existing path, please input new project name";
            }
            else
            {
                m_ret_filename = m_filename.Text;
                //this.Close();
                DialogResult = true;
            }
        }

        //cancel
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //this.Close();
            DialogResult = false;
        }

        private void Button_Click_SelectPath(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_foldername.Text = dlg.SelectedPath;
            }
        }
    }
}
