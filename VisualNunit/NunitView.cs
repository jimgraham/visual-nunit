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
using System.Data;

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

            RefreshView();

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

        public void RefreshView()
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Success", typeof(bool)));
            table.Columns.Add(new DataColumn("Namespace", typeof(string)));
            table.Columns.Add(new DataColumn("Case", typeof(string)));
            table.Columns.Add(new DataColumn("Test", typeof(string)));
            table.Columns.Add(new DataColumn("Time", typeof(string)));
            table.Columns.Add(new DataColumn("Message", typeof(string)));
            table.Columns.Add(new DataColumn("TestInformation", typeof(TestInformation)));

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


                    if (!projectComboBox.Items.Contains(project.Name))
                    {
                        projectComboBox.Items.Add(project.Name);
                    }

                    if (projectComboBox.SelectedIndex!=-1&&!project.Name.Equals(projectComboBox.SelectedItem))
                    {
                        continue;
                    }

                    String assemblyPath = localPath + outputPath + outputFileName;

                    if (File.Exists(assemblyPath))
                    {
                        IList<string> testCases = NunitManager.ListTestCases(assemblyPath);

                        foreach (string testCase in testCases)
                        {
                            TestInformation testInformation = new TestInformation();
                            testInformation.AssemblyPath = assemblyPath;
                            testInformation.TestName = testCase;

                            string[] nameParts = testCase.Split('.');

                            string testName = nameParts[nameParts.Length - 1];
                            string caseName = nameParts[nameParts.Length - 2];
                            string testNamespace = testCase.Substring(0, testCase.Length-(caseName.Length + testName.Length + 2));

                            if (namespaceComboBox.SelectedIndex != -1 && !testNamespace.Equals(namespaceComboBox.SelectedItem))
                            {
                                continue;
                            }

                            if (caseComboBox.SelectedIndex != -1 && !caseName.Equals(caseComboBox.SelectedItem))
                            {
                                continue;
                            }

                            if (!namespaceComboBox.Items.Contains(testNamespace))
                            {
                                namespaceComboBox.Items.Add(testNamespace);
                            }

                            if (!caseComboBox.Items.Contains(caseName))
                            {
                                caseComboBox.Items.Add(caseName);
                            }


                            DataRow row=table.Rows.Add(new Object[] {
                                "True".Equals(testInformation.Success), 
                                testNamespace,
                                caseName,
                                testName,
                                testInformation.Time,
                                testInformation.FailureMessage,
                                testInformation
                            });

                            testInformation.DataRow = row;
                        }

                    }

                }

            }

            dataGridView1.DataSource = table;
            dataGridView1.ClearSelection();
        }

        private Queue<TestInformation> testsToRun = new Queue<TestInformation>();
        private TestInformation currentTest = null;
        private int testsToRunStartCount = 1;

        private void runTests_Click(object sender, EventArgs e)
        {
            if (currentTest == null)
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                        TestInformation testInformation = (TestInformation)dataRow["TestInformation"];
                        testInformation.Debug = false;
                        testsToRun.Enqueue(testInformation);
                    }
                }
                else
                {
                    List<DataRow> dataRows=new List<DataRow>();
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        dataRows.Add(((DataRowView)row.DataBoundItem).Row);
                    }
                    dataRows.Reverse();
                    foreach(DataRow dataRow in dataRows)
                    {
                        TestInformation testInformation = (TestInformation)dataRow["TestInformation"];
                        testInformation.Debug = false;
                        testsToRun.Enqueue(testInformation);
                    }
                }
                if (testsToRun.Count > 0)
                {
                    testsToRunStartCount = testsToRun.Count;
                    currentTest = testsToRun.Dequeue();
                    runTests.Text = "Stop";
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            else
            {
                testsToRun.Clear();
                testsToRunStartCount = 1;
                currentTest.Stop = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Debug")
            {
                if (currentTest == null)
                {
                    DataRow row = ((DataRowView)dataGridView1.CurrentRow.DataBoundItem).Row;
                    currentTest = (TestInformation)row["TestInformation"];
                    currentTest.Debug = true;
                    testsToRunStartCount = 1;
                    runTests.Text = "Stop";
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
            backgroundWorker1.ReportProgress(100*(testsToRunStartCount-testsToRun.Count)/(testsToRunStartCount+1));

            NunitManager.RunTestCase(currentTest);

            backgroundWorker1.ReportProgress(100 * (testsToRunStartCount - testsToRun.Count + 1) /(testsToRunStartCount+1));

        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            DataRow dataRow = currentTest.DataRow;
            dataRow["Success"] = "True".Equals(currentTest.Success);
            dataRow["Time"] = currentTest.Time;
            dataRow["Message"] = currentTest.FailureMessage;

            if (testsToRun.Count > 0)
            {
                currentTest = testsToRun.Dequeue();
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                currentTest = null;
                runTests.Text = "Run";
            }
        }

        #endregion

        #region IVsSolutionEvents Members

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            RefreshView();
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Project loaded: {0}", pRealHierarchy));
            RefreshView();
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            RefreshView();
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
            RefreshView();
            return VSConstants.S_OK;
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        #endregion

        private void projectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            namespaceComboBox.Items.Clear();
            namespaceComboBox.SelectedItem = null;
            namespaceComboBox.Text = null;
            caseComboBox.Items.Clear();
            caseComboBox.SelectedItem = null;
            caseComboBox.Text = null;
            RefreshView();
        }

        private void namespaceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            caseComboBox.Items.Clear();
            caseComboBox.SelectedItem = null;
            caseComboBox.Text = null;
            RefreshView();
        }

        private void caseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshView();
        }



    }
}
