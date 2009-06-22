using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using EnvDTE;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BubbleCloudorg.VisualNunit
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class NuniView : UserControl, IVsSolutionEvents, IVsUpdateSolutionEvents
    {
        public NuniView()
        {
            InitializeComponent();

            uint cookie;
            int result = 0;
            IVsSolution2 solutionService = (IVsSolution2)Package.GetGlobalService(typeof(SVsSolution));
            result=solutionService.AdviseSolutionEvents(this, out cookie);


            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Added solution service event listener: {0}", result));
            IVsSolutionBuildManager buildService = (IVsSolutionBuildManager)Package.GetGlobalService(typeof(SVsSolutionBuildManager));
            result=buildService.AdviseUpdateSolutionEvents(this, out cookie);
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Added build service event listener: {0}", result));

            RefreshTreeView();

        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogChar(char charCode)
        {
              // If we're the top-level form or control, we need to do the mnemonic handling
              if (charCode != ' ' && ProcessMnemonic(charCode))
              {
                    return true;
              }
              return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        /// Enable the IME status handling for this control.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                return true;
            }
        }

        public void RefreshTreeView()
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            treeView1.Nodes.Clear();

            foreach (Project project in dte.Solution.Projects)
            {
                Configuration configuration = project.ConfigurationManager.ActiveConfiguration;

                if (configuration.IsBuildable)
                {

                    String localPath = "";
                    String outputPath = "";
                    String outputFileName = "";

                    foreach (Property property in project.Properties)
                    {
                        try
                        {
                            if ("LocalPath".Equals(property.Name))
                            {
                                localPath = (string)property.Value;
                            }
                            if ("OutputFileName".Equals(property.Name))
                            {
                                outputFileName = (string)property.Value;
                            }
                            //Trace.WriteLine("Project Property: " + property.Name + "=" + property.Value);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                    }

                    foreach (Property property in configuration.Properties)
                    {
                        try
                        {
                            if ("OutputPath".Equals(property.Name))
                            {
                                outputPath = (string)property.Value;
                            }
                            //Trace.WriteLine("Configuration Property: " + property.Name + "=" + property.Value);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                    }

                    /*Trace.WriteLine("Project.Kind=" + project.Kind);
                    Trace.WriteLine("Project.LocalPath=" + localPath);
                    Trace.WriteLine("Project.OutputPath=" + outputPath);
                    Trace.WriteLine("Project.OutputFileName=" + outputFileName);*/

                    String assemblyPath = localPath + outputPath + outputFileName;
                    TreeNode assemblyNode = new TreeNode(project.Name);

                    if (File.Exists(assemblyPath))
                    {
                        IList<string> testCases = NunitManager.ListTestCases(assemblyPath);

                        foreach (string testCase in testCases)
                        {
                            TreeNode testNode = new TreeNode(testCase);

                            TestInformation testInformation = new TestInformation();
                            testInformation.AssemblyPath = assemblyPath;
                            testInformation.TestName = testCase;

                            testNode.Tag = testInformation;
                            assemblyNode.Nodes.Add(testNode);
                        }

                    }
                    assemblyNode.Expand();
                    treeView1.Nodes.Add(assemblyNode);

                }

            }
        }

        private TestInformation testToRun = null;
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            treeView1.Enabled = false;

            if (treeView1.SelectedNode != null)
            {
                TreeNode node = treeView1.SelectedNode;

                if (node.Tag != null)
                {
                    testToRun = (TestInformation)node.Tag;
                    backgroundWorker1.RunWorkerAsync();
                }

            }

        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show(this,
                            string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "NUnit View");
        }

        private void treeView1_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void treeView1_EnabledChanged(object sender, EventArgs e)
        {
        }

        private void treeView1_Enter(object sender, EventArgs e)
        {
        }

        #region BackgrounWorker Events

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);

            NunitManager.RunTestCase(testToRun);

            backgroundWorker1.ReportProgress(100);

        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (testToRun.Time.Length > 0)
            {
                treeView1.SelectedNode.Text = testToRun.TestName + " " + testToRun.Success + " (" + testToRun.Time + "s)";
            }
            else
            {
                treeView1.SelectedNode.Text = testToRun.TestName + " " + testToRun.Success;
            }
            treeView1.Enabled = true;
        }

        #endregion

        #region IVsSolutionEvents Members

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            RefreshTreeView();
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Project loaded: {0}", pRealHierarchy));
            RefreshTreeView();
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            RefreshTreeView();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Project unload: {0}", pRealHierarchy.ToString()));
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsUpdateSolutionEvents Members

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Cancel()
        {
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            RefreshTreeView();
            return VSConstants.S_OK;
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        #endregion

    }
}
