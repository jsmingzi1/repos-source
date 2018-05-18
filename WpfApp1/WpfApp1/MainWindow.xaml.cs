using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.ComponentModel;
using System.Activities.Presentation.Validation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Activities.XamlIntegration;
using System.Windows.Forms;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private WorkflowDesigner wd;
        // private Sequence
        private System.Windows.Controls.TextBox textBox;
        public MainWindow()
        {
            InitializeComponent();

            this.AddToolBox();
            NewFlow("");
            //// Register the metadata  
            //RegisterMetadata();

            //// Add the WFF Designer  
            //AddDesigner("");


            //this.AddPropertyInspector();
            //this.AddOutline();
            //this.AddConsole();

            //wd.Context.Services.Publish<IValidationErrorService>(new DebugValidationErrorService());

        }

        private void NewFlow(string filename)
        {
            RegisterMetadata();
            AddDesigner(filename);
            this.AddPropertyInspector();
            this.AddOutline();
            this.AddConsole();
            wd.Context.Services.Publish<IValidationErrorService>(new DebugValidationErrorService());
        }

        private void AddOutline()
        {
            Grid.SetRow(this.wd.OutlineView, 1);
            Grid.SetColumn(this.wd.OutlineView, 2);

            grid1.Children.Add(wd.OutlineView);

        }

        private void AddConsole()
        {
            Grid.SetRow(textBox, 2);
            Grid.SetColumnSpan(textBox, 3);
            grid1.Children.Add(textBox);
            Console.SetOut(new ControlWriter(textBox));
        }
        private void AddDesigner(string filename)
        {
            //Create an instance of WorkflowDesigner class.  
            this.wd = new WorkflowDesigner();

            textBox = new System.Windows.Controls.TextBox();
            //ad.
            //Place the designer canvas in the middle column of the grid.  
            Grid.SetColumn(this.wd.View, 1);
            Grid.SetRowSpan(this.wd.View, 2);

            //Adding annotations
            wd.Context.Services.GetService<DesignerConfigurationService>().AnnotationEnabled = true;
            //Targeting the .NET 4.5 Framework for the configuration service for the WF Designer control
            wd.Context.Services.GetService<DesignerConfigurationService>().TargetFrameworkName = new
            System.Runtime.Versioning.FrameworkName(".NET Framework", new Version(4, 5));

            //Load a new Sequence as default.  
            if (string.IsNullOrEmpty(filename))

                this.wd.Load(BuildBaseActivity());
            else
                this.wd.Load(filename);


            //Add the designer canvas to the grid.  
            grid1.Children.Add(this.wd.View);
        }

        private void RegisterMetadata()
        {
            DesignerMetadata dm = new DesignerMetadata();
            dm.Register();
        }

        private ToolboxControl GetToolboxControl()
        {
            //System.Activities.Statements.
            // Create the ToolBoxControl.  
            ToolboxControl ctrl = new ToolboxControl();

            // Create a category.  
            ToolboxCategory category = new ToolboxCategory("Activities");
            //AddToCollection<T>

            // Create Toolbox items.  
            //ToolboxItemWrapper tool1 =
            //    new ToolboxItemWrapper("System.Activities.Statements.Assign",
            //    typeof(Assign).Assembly.FullName, null, "Assign");

            //ToolboxItemWrapper tool2 = new ToolboxItemWrapper("System.Activities.Statements.Sequence",
            //    typeof(Sequence).Assembly.FullName, null, "Sequence");

            //ToolboxItemWrapper tool3 = new ToolboxItemWrapper("System.Activities.Statements.WriteLine",
            //    typeof(Sequence).Assembly.FullName, null, "WriteLine");

            //ToolboxItemWrapper tool4 = new ToolboxItemWrapper("System.Activities.Statements.FlowStep",
            //    typeof(Sequence).Assembly.FullName, null, "FlowStep");


            string[] list = { "Assign", "CancellationScope", "CompensableActivity", "Compensate", "CompensationExtension", "Confirm", "CreateBookmarkScope", "Delay", "DeleteBookmarkScope", "DoWhile", "DurableTimerExtension", "Flowchart", "FlowDecision", "FlowStep", "InvokeAction", "If", "InvokeDelegate", "InvokeMethod", "NoPersistScope", "Parallel", "Persist", "Pick", "PickBranch", "Rethrow", "Sequence", "State", "StateMachine", "TerminateWorkflow", "Throw", "TransactionScope", "Transition", "While", "WorkflowTerminatedException", "WriteLine" };
            foreach (string item in list)
            {
                ToolboxItemWrapper tool = new ToolboxItemWrapper("System.Activities.Statements." + item,
                typeof(Sequence).Assembly.FullName, null, item);
                category.Add(tool);
            }

            // Add the Toolbox items to the category.  
            //category.Add(tool1);
            //category.Add(tool2);
            //category.Add(tool3);
            //category.Add(tool4);

            // Add the category to the ToolBox control.  
            ctrl.Categories.Add(category);
            return ctrl;
        }

        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            Grid.SetColumn(tc, 0);
            Grid.SetRowSpan(tc, 2);
            grid1.Children.Add(tc);
        }

        private void AddPropertyInspector()
        {
            Grid.SetColumn(wd.PropertyInspectorView, 2);
            grid1.Children.Add(wd.PropertyInspectorView);
        }

        //save
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "1.xaml"; //default file name
            dlg.DefaultExt = ".xaml"; //default file extension
            dlg.Filter = "XAML File (.xaml)|*.xaml"; //filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                this.wd.Save(dlg.FileName);
            }

        }

        //run
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            textBox.Text = "Workflow Start\r\n";
            try
            {
                wd.Flush();
                // create stream with textbox contents
                StringBuilder data = new StringBuilder();
                Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(wd.Text));

                // invoke workflow
                Activity wf = ActivityXamlServices.Load(stream);
                IDictionary<string, object> results = WorkflowInvoker.Invoke(wf);

                //textBox.Text = wd.Text;
                // show results
                //textBox.Text = "Workflow Executed";
                //textBox.Text += "\r\nReturned data:";
                if (results.Count > 0)
                {
                    foreach (string key in results.Keys)
                    {
                        textBox.Text += string.Format("{0} : {1}\r\n", key, results[key]);
                    }
                }
                else
                {
                    //textBox.Text += "<no data returned>\r\n";
                }
            }
            catch (Exception ex)
            {
                textBox.Text += string.Format("Error executing the Xaml. Exception type: {0}\r\nDetails:\r\n--------\r\n{1}\r\n", ex.GetType().FullName, ex.ToString());
            }
            textBox.Text += "Workflow End\r\n";
        }

        //open
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "xaml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {

                NewFlow(dlg.FileName);
                //this.wd.Load(dlg.FileName);
                this.wd.Flush();
            }
        }

        private ActivityBuilder BuildBaseActivity()
        {
            try
            {
                ActivityBuilder builder = new ActivityBuilder
                {
                    Name = "CustomWorkflow",
                    Implementation = new Sequence()
                    {
//                        Activities =
//{
//new Assign<Int32>()
//}
                    }
                };
                return builder;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    class DebugValidationErrorService : IValidationErrorService
    {
        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            errors.ToList().ForEach(vei => Debug.WriteLine(string.Format("Error: {0} ", vei.Message)));
        }
    }


    public class ControlWriter : TextWriter
    {
        private System.Windows.Controls.TextBox textbox;
        public ControlWriter(System.Windows.Controls.TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }

    public class CustomActivity:CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<double> FirstNumber { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

        }
    }
}
