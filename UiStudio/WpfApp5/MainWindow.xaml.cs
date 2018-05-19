using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Toolbox;
using System.Activities.Presentation.Validation;
using System.Activities.Statements;
using System.Activities.Tracking;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace WpfApp5
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string m_projectfolder = "";
        Dictionary<LayoutDocument, WorkflowDesigner> m_DesignerList;
        Dictionary<int, SourceLocation> textLineToSourceLocationMap;
        int i = 0;
        public MainWindow()
        {
            InitializeComponent();

            m_DesignerList = new Dictionary<LayoutDocument, WorkflowDesigner>();
            m_documentpane.Children.Clear();
            Console.SetOut(new ControlWriter(m_console_textbox));
            textLineToSourceLocationMap = new Dictionary<int, SourceLocation>();

            AddToolBox();
            LoadProject("d:\\ws\\");
        }

        //Load Default Project
        bool LoadProject(string projectfolder)
        {
            if (m_projectfolder == projectfolder)
                return true;
            if (projectfolder.EndsWith("\\") || projectfolder.EndsWith("/"))
                m_projectfolder = projectfolder.Substring(0, projectfolder.Length - 1);
            else
                m_projectfolder = projectfolder;

            m_textbox_projectfolder.Text = m_projectfolder;
            string [] files = Directory.GetFiles(m_projectfolder);

            m_treeview_projectfolder.Items.Clear();
            TreeViewItem itemTop = new TreeViewItem();
            itemTop.Header = new DirectoryInfo(m_projectfolder).Name;
            
            m_treeview_projectfolder.Items.Add(itemTop);

            foreach(string file in files)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = System.IO.Path.GetFileName(file);
                item.ToolTip = file;
                itemTop.Items.Add(item);
            }
            m_treeview_projectfolder.MouseDoubleClick += new MouseButtonEventHandler(treView_MouseDoubleClick);

            m_documentpane.Children.Clear();
            m_DesignerList.Clear();
            m_outlinepane.Content = null;
            m_propertiespane.Content = null;
            
            return true;
        }


        private void treView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = GetSelectedItem((FrameworkElement)e.OriginalSource, m_treeview_projectfolder);

            if (tvi.HasItems == false && tvi.Header.ToString().ToLower().EndsWith(".xaml"))
            { 
                //MessageBox.Show(tvi.ToolTip.ToString(), "double click prompt", 0);
                LoadXaml(tvi.ToolTip.ToString());
            }
        }
        private TreeViewItem GetSelectedItem(UIElement sender, UIElement objTreeViewControl)
        {
            Point point = sender.TranslatePoint(new Point(0, 0), objTreeViewControl);
            var isHitTestAvailable = objTreeViewControl.InputHitTest(point) as DependencyObject;
            while (isHitTestAvailable != null && !(isHitTestAvailable is TreeViewItem))
            {
                isHitTestAvailable = VisualTreeHelper.GetParent(isHitTestAvailable);
            }
            return isHitTestAvailable as TreeViewItem;
        }


        //Load a xaml file
        bool LoadXaml(string filename)
        {

            LayoutDocument doc = null;
            bool bExist = false;
            foreach(LayoutDocument d in m_documentpane.Children)
            {
                if (d.Title.ToString() == System.IO.Path.GetFileName(filename) && d.ToolTip.ToString() == filename)
                {
                    doc = d;
                    bExist = true;
                    break;
                }
            }
            if (bExist==false)
            {
                doc = new LayoutDocument();
                doc.Title = System.IO.Path.GetFileName(filename);
                doc.ToolTip = filename;
                m_documentpane.Children.Add(doc);
                AddNewDesigner(doc, null);
                
            }

            m_documentpane.SelectedContentIndex = m_documentpane.IndexOfChild(doc);
            doc.IsSelected = true;
            LoadPropertyAndOutline(doc);
            return true;
        }

        bool AddNewDesigner(LayoutDocument doc, ActivityBuilder activity)
        {
            DesignerMetadata dm = new DesignerMetadata();
            dm.Register();

            WorkflowDesigner designer = new WorkflowDesigner();
            designer.Context.Services.GetService<DesignerConfigurationService>().AnnotationEnabled = true;
            //Targeting the .NET 4.5 Framework for the configuration service for the WF Designer control
            designer.Context.Services.GetService<DesignerConfigurationService>().TargetFrameworkName = new
            System.Runtime.Versioning.FrameworkName(".NET Framework", new Version(4, 5));

            doc.Content = designer.View;
            if (activity is null)
                designer.Load(doc.ToolTip.ToString());
            else
            {
                designer.Load(activity);
                designer.Save(doc.ToolTip.ToString());
            }
            doc.IsSelectedChanged += Doc_IsSelectedChanged;
            doc.Closed += Doc_Closed;
            designer.Context.Services.Publish<IValidationErrorService>(new DebugValidationErrorService());
            designer.TextChanged += Designer_TextChanged;
            m_DesignerList.Add(doc, designer);
            //Adding annotations
            
            return true;
        }

        private void Designer_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("designer text changed");
            //throw new NotImplementedException();
        }

        private void Doc_Closed(object sender, EventArgs e)
        {
            LayoutDocument doc = (LayoutDocument)sender;
            AppendLog(doc.Title + " has beeen closed");
            foreach (LayoutDocument i in m_DesignerList.Keys)
            {
                if (i.ToolTip.ToString() == doc.ToolTip.ToString())
                {
                    m_DesignerList.Remove(i);
                    break;
                }
            }
        }

        private void Doc_IsSelectedChanged(object sender, EventArgs e)
        {
            LayoutDocument doc = (LayoutDocument)sender;
            if (doc.IsSelected)
            {
                LoadPropertyAndOutline(doc);
                AppendLog(doc.Title + " has beeen selected");
            }
            else
            {
                UnloadPropertyAndOutline(doc);
                AppendLog(doc.Title + " has beeen de selected");
            }
        }
        private void UnloadPropertyAndOutline(LayoutDocument doc)
        {
            m_propertiespane.Content = null;
            m_outlinepane.Content = null;
        }

        private void LoadPropertyAndOutline(LayoutDocument doc)
        {
            foreach(LayoutDocument i in m_DesignerList.Keys)
            {
                if (i.ToolTip.ToString() == doc.ToolTip.ToString())
                {
                    m_propertiespane.Content = m_DesignerList[i].PropertyInspectorView;
                    m_outlinepane.Content = m_DesignerList[i].OutlineView;
                    break;
                }
            }
        }

        private void AppendLog(string content)
        {
            File.AppendAllText(@"d:\ws\1.txt", content + Environment.NewLine);
        }

        //Add Tool Box Functions
        private ToolboxControl GetToolboxControl()
        {
            //System.Activities.Statements.
            // Create the ToolBoxControl.  
            ToolboxControl ctrl = new ToolboxControl();

            // Create a category.  
            ToolboxCategory category = new ToolboxCategory("Activities");


            string[] list = { "Assign", "CancellationScope", "CompensableActivity", "Compensate", "CompensationExtension", "Confirm", "CreateBookmarkScope", "Delay", "DeleteBookmarkScope", "DoWhile", "DurableTimerExtension", "Flowchart", "FlowDecision", "FlowStep", "InvokeAction", "If", "InvokeDelegate", "InvokeMethod", "NoPersistScope", "Parallel", "Persist", "Pick", "PickBranch", "Rethrow", "Sequence", "State", "StateMachine", "TerminateWorkflow", "Throw", "TransactionScope", "Transition", "While", "WorkflowTerminatedException", "WriteLine" };
            foreach (string item in list)
            {
                ToolboxItemWrapper tool = new ToolboxItemWrapper("System.Activities.Statements." + item,
                typeof(Sequence).Assembly.FullName, null, item);
                category.Add(tool);
            }


            // Add the category to the ToolBox control.  
            ctrl.Categories.Add(category);
            return ctrl;
        }
        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            Grid.SetColumn(tc, 0);
            Grid.SetRowSpan(tc, 2);
            m_grid_toolbox.Children.Add(tc);
        }

        private void RibbonButton_Run(object sender, RoutedEventArgs e)
        {
            if (m_documentpane.Children.Count > 0)
            {
                foreach(LayoutDocument d in m_DesignerList.Keys)
                {
                    if (m_documentpane.SelectedContent.ToolTip.ToString()==d.ToolTip.ToString())
                    {
                        RunDesigner(m_DesignerList[d]);
                        break;
                    }
                }
                
            }
        }

        private void RunDesigner(WorkflowDesigner designer)
        {
            m_console_textbox.Text = "Workflow Start\r\n";
            try
            {
                //m_console_textbox.Text += designer.Text + "\r\n";
                //designer.text
                designer.Flush();
                //m_console_textbox.Text += designer.Text + "\r\n";
                // create stream with textbox contents
                StringBuilder data = new StringBuilder();
                Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(designer.Text));

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
                        m_console_textbox.Text += string.Format("{0} : {1}\r\n", key, results[key]);
                    }
                }
                else
                {
                    //textBox.Text += "<no data returned>\r\n";
                }
            }
            catch (Exception ex)
            {
                m_console_textbox.Text += string.Format("Error executing the Xaml. Exception type: {0}\r\nDetails:\r\n--------\r\n{1}\r\n", ex.GetType().FullName, ex.ToString());
            }
            m_console_textbox.Text += "Workflow End\r\n";
        }

        //save
        private void RibbonButton_Save(object sender, RoutedEventArgs e)
        {
            if (m_documentpane.Children.Count > 0)
            {
                foreach (LayoutDocument d in m_DesignerList.Keys)
                {
                    if (m_documentpane.SelectedContent.ToolTip.ToString() == d.ToolTip.ToString())
                    {
                        m_DesignerList[d].Flush();
                        m_DesignerList[d].Save(d.ToolTip.ToString());
                        break;
                    }
                }
            }

        }

        //open
        private void RibbonButton_Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "xaml";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fullname = dlg.FileName;
                string filename = System.IO.Path.GetFileName(fullname);
                string foldername = System.IO.Path.GetDirectoryName(fullname);

                if (foldername.ToLower() == m_projectfolder.ToLower())
                {
                    MessageBox.Show("project folder is same, no need switch");
                    LoadXaml(fullname);
                }
                else
                {
                    MessageBox.Show("project folder is diff, need switch" + foldername + "," + m_projectfolder);
                    LoadProject(foldername);
                    LoadXaml(fullname);
                }
                //NewFlow(dlg.FileName);
                //this.wd.Load(dlg.FileName);
                //this.wd.Flush();
            }
        }

        private void RibbonButton_NewSequence(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_projectfolder))
            {
                MessageBox.Show("you should at least have a project first, then can create new file");
            }
            else
            {
                NewFileWindow win = new NewFileWindow(m_projectfolder);
                win.ShowDialog();
                string filename = win.GetFileName("");
                if (!string.IsNullOrEmpty(filename))
                {
                    NewDesignerFile(filename, 0);
                }
            }
        }

        private void RibbonButton_NewFlowchart(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_projectfolder))
            {
                MessageBox.Show("you should at least have a project first, then can create new file");
            }
            else
            {
                NewFileWindow win = new NewFileWindow(m_projectfolder);
                win.ShowDialog();
                string filename = win.GetFileName("");
                if (!string.IsNullOrEmpty(filename))
                {
                    NewDesignerFile(filename, 1);
                }
            }
        }
        private void RibbonButton_NewStateMachine(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_projectfolder))
            {
                MessageBox.Show("you should at least have a project first, then can create new file");
            }
            else
            {
                NewFileWindow win = new NewFileWindow(m_projectfolder);
                win.ShowDialog();
                string filename = win.GetFileName("");
                if (!string.IsNullOrEmpty(filename))
                {
                    NewDesignerFile(filename, 2);
                }
            }
        }

        //type 0 sequence
        //type 1 flowchart
        //type 2 statemachine
        private bool NewDesignerFile(string shortfilename, int type=0)
        {
            LayoutDocument doc = new LayoutDocument();
            doc.Title = shortfilename;
            doc.ToolTip = m_projectfolder + "\\" + shortfilename;
            m_documentpane.Children.Add(doc);
            ActivityBuilder builder;
            switch (type)
            {
                case 0:
                    builder = BuildBaseActivitySequence(doc.Title);
                    break;
                case 1:
                    builder = BuildBaseActivityFlowchart(doc.Title);
                    break;
                case 2:
                    builder = BuildBaseActivityStateMachine(doc.Title);
                    break;
                default:
                    builder = BuildBaseActivitySequence(doc.Title);
                    break;
            }
            AddNewDesigner(doc, builder);
            m_documentpane.SelectedContentIndex = m_documentpane.IndexOfChild(doc);
            doc.IsSelected = true;
            LoadPropertyAndOutline(doc);
            return true;
        }

        private ActivityBuilder BuildBaseActivitySequence(string name)
        {
            try
            {
                ActivityBuilder builder = new ActivityBuilder
                {
                    Name = name,
                    Implementation = new Sequence()
                    {
                    }
                };
                return builder;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private ActivityBuilder BuildBaseActivityFlowchart(string name)
        {
            try
            {
                ActivityBuilder builder = new ActivityBuilder
                {
                    Name = name,
                    Implementation = new Flowchart()
                    {
                    }
                };
                return builder;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private ActivityBuilder BuildBaseActivityStateMachine(string name)
        {
            try
            {
                ActivityBuilder builder = new ActivityBuilder
                {
                    Name = name,
                    Implementation = new StateMachine()
                    {
                    }
                };
                return builder;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RibbonButton_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
         * 
         * BELOW CODES ARE FOR DEBUG
         * CODES MIGRATE FROM SAMPLE PROJECT 
         * D:\WF_WCF_Samples\WF_WCF_Samples\WF\Application\VisualWorkflowTracking\CS
         * 
         */
        
        private void RibbonButton_Debug(object sender, RoutedEventArgs e)
        {
            if (m_documentpane.ChildrenCount > 0)
            {
                string fullname = m_documentpane.SelectedContent.ToolTip.ToString();
                WorkflowDesigner designer = m_DesignerList[(LayoutDocument)m_documentpane.SelectedContent];
                DebugWorkflow(designer, fullname);
            }
        }

        //get root node from *.xaml
        Activity GetRootRuntimeWorkflowElement(string fullname)
        {
            Activity root = ActivityXamlServices.Load(fullname);
            WorkflowInspectionServices.CacheMetadata(root);

            IEnumerator<Activity> enumerator1 = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
            //Get the first child of the x:class
            enumerator1.MoveNext();
            root = enumerator1.Current;
            return root;
        }

        Activity GetRuntimeExecutionRoot(string fullname)
        {
            Activity root = ActivityXamlServices.Load(fullname);
            WorkflowInspectionServices.CacheMetadata(root);

            return root;
        }

        // Get root WorkflowElement.  Currently only handle when the object is ActivitySchemaType or WorkflowElement.
        // May return null if it does not know how to get the root activity.
        Activity GetRootWorkflowElement(object rootModelObject)
        {
            System.Diagnostics.Debug.Assert(rootModelObject != null, "Cannot pass null as rootModelObject");

            Activity rootWorkflowElement;
            IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else // Loose xaml case.
            {
                rootWorkflowElement = rootModelObject as Activity;
            }
            return rootWorkflowElement;
        }

       // # region Helper Methods
        object GetRootInstance(WorkflowDesigner designer)
        {
            ModelService modelService = designer.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue();
            }
            else
            {
                return null;
            }
        }

        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService(WorkflowDesigner designer, string fullname)
        {
            object rootInstance = GetRootInstance(designer);
            Dictionary<object, SourceLocation> sourceLocationMapping = new Dictionary<object, SourceLocation>();
            Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();

            if (rootInstance != null)
            {
                Activity documentRootElement = GetRootWorkflowElement(rootInstance);
                SourceLocationProvider.CollectMapping(/*GetRootRuntimeWorkflowElement(fullname)*/documentRootElement, documentRootElement, sourceLocationMapping,
                    designer.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);
                Activity ac = GetRootRuntimeWorkflowElement(fullname);
                string filename = designer.Context.Items.GetValue<WorkflowFileItem>().LoadedFile;
                SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                    designer.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

            }

            // Notify the DebuggerService of the new sourceLocationMapping.
            // When rootInstance == null, it'll just reset the mapping.
            //DebuggerService debuggerService = debuggerService as DebuggerService;
            if (designer.DebugManagerView != null)
            {
                ((DebuggerService)designer.DebugManagerView).UpdateSourceLocations(designerSourceLocationMapping);
            }

            return sourceLocationMapping;
        }

        void ShowDebug(SourceLocation srcLoc, WorkflowDesigner designer)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {
                    designer.DebugManagerView.CurrentLocation = srcLoc;

                }));

        }

        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;
        }

        //Provide Debug Adornment on the Activity being executed
        void textBox1_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (m_documentpane.ChildrenCount == 0)
                return;
            WorkflowDesigner designer = m_DesignerList[(LayoutDocument)m_documentpane.SelectedContent];
            string fullname = m_documentpane.SelectedContent.ToolTip.ToString();
            string text = this.m_console_textbox.Text;

            int index = 0;
            int lineClicked = 0;
            while (index < text.Length)
            {
                if (text[index] == '\n')
                    lineClicked++;
                if (this.m_console_textbox.SelectionStart <= index)
                    break;

                index++;
            }


            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                try
                {
                    //Tell Debug Service that the Line Clicked is _______
                    designer.DebugManagerView.CurrentLocation = textLineToSourceLocationMap[lineClicked];
                }
                catch (Exception)
                {
                    //If the user clicks other than on the tracking records themselves.
                    designer.DebugManagerView.CurrentLocation = new SourceLocation(fullname, 1, 1, 1, 10);
                }
            }));

        }

        //Run the Workflow with the tracking participant
        public void DebugWorkflow(WorkflowDesigner designer, string fullname)
        {
            WorkflowInvoker instance = new WorkflowInvoker(GetRuntimeExecutionRoot(fullname));

            //Mapping between the Object and Line No.
            Dictionary<object, SourceLocation> wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService(designer, fullname);

            //Mapping between the Object and the Instance Id
            Dictionary<string, Activity> activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);

            //#region Set up Custom Tracking
            const String all = "*";
            VisualTrackingParticipant simTracker = new VisualTrackingParticipant()
            {
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries =
                        {
                            new CustomTrackingQuery()
                            {
                                Name = all,
                                ActivityName = all
                            },
                            new WorkflowInstanceQuery()
                            {
                                // Limit workflow instance tracking records for started and completed workflow states
                                States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                            },
                            new ActivityStateQuery()
                            {
                                // Subscribe for track records from all activities for all states
                                ActivityName = all,
                                States = { all },

                                // Extract workflow variables and arguments as a part of the activity tracking record
                                // VariableName = "*" allows for extraction of all variables in the scope
                                // of the activity
                                Variables =
                                {
                                    { all }
                                }
                            }
                        }
                }
            };

            //#endregion

            simTracker.ActivityIdToWorkflowElementMap = activityIdToWfElementMap;

            //As the tracking events are received
            simTracker.TrackingRecordReceived += (trackingParticpant, trackingEventArgs) =>
            {
                if (trackingEventArgs.Activity != null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        String.Format("<+=+=+=+> Activity Tracking Record Received for ActivityId: {0}, record: {1} ",
                        trackingEventArgs.Activity.Id,
                        trackingEventArgs.Record
                        )
                    );

                    ShowDebug(wfElementToSourceLocationMap[trackingEventArgs.Activity], designer);

                    this.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() =>
                    {
                        //Textbox Updates
                        m_console_textbox.AppendText(trackingEventArgs.Activity.DisplayName + " " + ((ActivityStateRecord)trackingEventArgs.Record).State + "\n");
                        m_console_textbox.AppendText("******************\n");
                        textLineToSourceLocationMap.Add(i, wfElementToSourceLocationMap[trackingEventArgs.Activity]);
                        i = i + 2;

                        //Add a sleep so that the debug adornments are visible to the user
                        System.Threading.Thread.Sleep(1000);
                    }));

                }
            };

            instance.Extensions.Add(simTracker);
            ThreadPool.QueueUserWorkItem(new WaitCallback((context) =>
            {
                //Invoking the Workflow Instance with Input Arguments
                //instance.Invoke(new Dictionary<string, object> { { "decisionVar", true } }, new TimeSpan(1, 0, 0));
                instance.Invoke(new Dictionary<string, object> {  }, new TimeSpan(1, 0, 0));

                //This is to remove the final debug adornment
                this.Dispatcher.Invoke(DispatcherPriority.Render
                    , (Action)(() =>
                    {
                        designer.DebugManagerView.CurrentLocation = new SourceLocation(fullname, 1, 1, 1, 10);
                    }));

            }));

        }
    }

}
