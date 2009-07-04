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
using System.Drawing;
using VisualNunitLogic;

namespace BubbleCloudorg.VisualNunit
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class NuniView : UserControl, IVsSolutionEvents, IVsUpdateSolutionEvents
    {
        private Bitmap successIcon = null;
        private Bitmap failureIcon = null;
        private Bitmap abortedIcon = null;
        private Bitmap emptyIcon = null;
        private Bitmap runIcon = null;
        private Bitmap debugIcon = null;
        private Bitmap stopIcon = null;

        public NuniView()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            emptyIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Empty.png"));
            successIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Success.png"));
            failureIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Failure.png"));
            abortedIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Aborted.png"));
            runIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Run.png"));
            debugIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Debug.png"));
            stopIcon = new Bitmap(assembly.GetManifestResourceStream("BubbleCloudorg.VisualNunit.Icons.Stop.png"));

            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;

            runTestsButton.Image = runIcon;
            statusButton.Image = emptyIcon;

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

        #region NUnit View Refresh

        private Queue<ProjectInformation> projectsToLoad = new Queue<ProjectInformation>();
        private ProjectInformation currentlyLoadingProject = null;
        private IList<string> loadedTestCases = null;

        public void RefreshView()
        {
            if (!dataGridView1.Visible)
            {
                return;
            }

            Trace.TraceInformation("Refreshing NUnit View.");

            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Success", typeof(string)));
            table.Columns.Add(new DataColumn("Namespace", typeof(string)));
            table.Columns.Add(new DataColumn("Case", typeof(string)));
            table.Columns.Add(new DataColumn("Test", typeof(string)));
            table.Columns.Add(new DataColumn("Time", typeof(string)));
            table.Columns.Add(new DataColumn("Message", typeof(string)));
            table.Columns.Add(new DataColumn("TestInformation", typeof(TestInformation)));

            foreach (Project project in dte.Solution.Projects)
            {
                try
                {

                    if (project.ConfigurationManager == null)
                    {
                        continue;
                    }
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
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex);
                            }
                        }

                        if (!projectComboBox.Items.Contains(project.Name))
                        {
                            projectComboBox.Items.Add(project.Name);
                        }

                        if (projectComboBox.SelectedIndex != -1 && !project.Name.Equals(projectComboBox.SelectedItem))
                        {
                            continue;
                        }

                        String assemblyPath = localPath + outputPath + outputFileName;

                        if (!assemblyPath.EndsWith(".dll"))
                        {
                            continue;
                        }

                        if (File.Exists(assemblyPath))
                        {
                            ProjectInformation projectInformation=new ProjectInformation();
                            projectInformation.Name=project.Name;
                            projectInformation.AssemblyPath=assemblyPath;
                            projectsToLoad.Enqueue(projectInformation);                           
                        }

                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error refreshing NUnit view: " + ex.ToString());
                }
            }

            dataGridView1.DataSource = table;

            if (projectsToLoad.Count > 0&&!testListWorker.IsBusy)
            {
                currentlyLoadingProject = projectsToLoad.Dequeue();
                testListWorker.RunWorkerAsync();
            }
        }

        private void testListWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                loadedTestCases = NunitManager.StartRunner(currentlyLoadingProject.AssemblyPath);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error loading test cases for project " + currentlyLoadingProject.Name+": "+ex.ToString());
            }
        }

        private void testListWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string assemblyPath = currentlyLoadingProject.AssemblyPath;
            DataTable dataTable = (DataTable)dataGridView1.DataSource;

            if (loadedTestCases != null)
            {
                foreach (string testCase in loadedTestCases)
                {
                    TestInformation testInformation = new TestInformation();
                    testInformation.AssemblyPath = assemblyPath;
                    testInformation.TestName = testCase;

                    string[] nameParts = testCase.Split('.');

                    string testName;
                    string caseName;
                    string testNamespace;

                    if (nameParts.Length > 2)
                    {
                        testName = nameParts[nameParts.Length - 1];
                        caseName = nameParts[nameParts.Length - 2];
                        testNamespace = testCase.Substring(0, testCase.Length - (caseName.Length + testName.Length + 2));
                    }
                    else
                    {
                        testName = nameParts[nameParts.Length - 1];
                        caseName = nameParts[nameParts.Length - 2];
                        testNamespace = "";
                    }

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


                    DataRow row = dataTable.Rows.Add(new Object[] {
                                        "", 
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

            dataGridView1.ClearSelection();

            if (projectsToLoad.Count > 0&&!testListWorker.IsBusy)
            {
                currentlyLoadingProject = projectsToLoad.Dequeue();
                testListWorker.RunWorkerAsync();
            }
        }

        #endregion

        #region NUnit View Events

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

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_VisibleChanged(object sender, EventArgs e)
        {
            Trace.WriteLine("Visible changed.");
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataRow dataRow = ((DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem).Row;
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells["Success"];
            string success = (string)dataRow["Success"];
            if ("True".Equals(success))
            {
                if (cell.Value != successIcon)
                {
                    cell.Value = successIcon;
                }
            }
            else if ("False".Equals(success))
            {
                if (cell.Value != failureIcon)
                {
                    cell.Value = failureIcon;
                }
            }
            else if ("Aborted".Equals(success))
            {
                if (cell.Value != abortedIcon)
                {
                    cell.Value = abortedIcon;
                }
            }
            else
            {
                if (cell.Value != emptyIcon)
                {
                    cell.Value = emptyIcon;
                }
            }

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells["Debug"].Value = debugIcon;
            dataGridView1.Rows[e.RowIndex].Cells["Success"].Value = emptyIcon;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        #endregion

        #region Test Runs

        private Queue<TestInformation> testsToRun = new Queue<TestInformation>();
        private TestInformation currentTest = null;
        private int testsToRunStartCount = 1;

        private void runTests_Click(object sender, EventArgs e)
        {
            if (currentTest == null)
            {
                statusButton.Image = emptyIcon;
                statusLabel.Text = "";

                if (dataGridView1.SelectedRows.Count == 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                        TestInformation testInformation = (TestInformation)dataRow["TestInformation"];
                        testInformation.Debug = false;

                        testInformation.Success = "";
                        testInformation.FailureMessage = "";
                        testInformation.FailureStackTrace = "";
                        testInformation.Time = "";
                        dataRow["Success"] = testInformation.Success;
                        dataRow["Time"] = testInformation.Time;
                        dataRow["Message"] = testInformation.FailureMessage;

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

                        testInformation.Success = "";
                        testInformation.FailureMessage = "";
                        testInformation.FailureStackTrace = "";
                        testInformation.Time = "";
                        dataRow["Success"] = testInformation.Success;
                        dataRow["Time"] = testInformation.Time;
                        dataRow["Message"] = testInformation.FailureMessage;

                        testsToRun.Enqueue(testInformation);
                    }
                }
                if (testsToRun.Count > 0)
                {
                    testsToRunStartCount = testsToRun.Count;
                    currentTest = testsToRun.Dequeue();
                    runTestsButton.Text = "Stop";
                    runTestsButton.Image = stopIcon;
                    NunitManager.PreRunTestCase(currentTest);
                    testRunWorker.RunWorkerAsync();
                }
            }
            else
            {
                testsToRun.Clear();
                testsToRunStartCount = 1;
                NunitManager.AbortTestCase(currentTest);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Debug")
            {
                if (currentTest == null)
                {
                    statusButton.Image = emptyIcon;
                    statusLabel.Text = "";

                    DataRow dataRow = ((DataRowView)dataGridView1.CurrentRow.DataBoundItem).Row;
                    currentTest = (TestInformation)dataRow["TestInformation"];

                    currentTest.Success = "";
                    currentTest.FailureMessage = "";
                    currentTest.FailureStackTrace = "";
                    currentTest.Time = "";
                    dataRow["Success"] = currentTest.Success;
                    dataRow["Time"] = currentTest.Time;
                    dataRow["Message"] = currentTest.FailureMessage;

                    currentTest.Debug = true;
                    testsToRunStartCount = 1;
                    runTestsButton.Text = "Stop";
                    runTestsButton.Image = stopIcon;
                    NunitManager.PreRunTestCase(currentTest);
                    testRunWorker.RunWorkerAsync();
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Stacktrace")
            {
                TestDetailsForm testDetailsForm = new TestDetailsForm();
                DataRow row = ((DataRowView)dataGridView1.CurrentRow.DataBoundItem).Row;
                TestInformation testInformation = (TestInformation)row["TestInformation"];
                testDetailsForm.SetTestInformation(testInformation);
                testDetailsForm.ShowDialog();
            }
        }

        private void testRunWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            testRunWorker.ReportProgress(100 * (testsToRunStartCount - testsToRun.Count) / (testsToRunStartCount + 1));

            NunitManager.RunTestCase(currentTest);

            testRunWorker.ReportProgress(100 * (testsToRunStartCount - testsToRun.Count + 1) / (testsToRunStartCount + 1));

        }

        private void testRunWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void testRunWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            NunitManager.PostRunTestCase(currentTest);

            {
                DataRow dataRow = currentTest.DataRow;
                dataRow["Success"] = currentTest.Success;
                dataRow["Time"] = currentTest.Time;
                dataRow["Message"] = currentTest.FailureMessage;
            }

            if (testsToRun.Count > 0)
            {
                currentTest = testsToRun.Dequeue();
                NunitManager.PreRunTestCase(currentTest);
                testRunWorker.RunWorkerAsync();
            }
            else
            {
                currentTest = null;
                runTestsButton.Text = "Run";
                runTestsButton.Image = runIcon;

                int successes=0;
                int failures=0;
                int aborts=0;
                int unknowns=0;
                int total=dataGridView1.Rows.Count;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                    if("True".Equals(dataRow["Success"]))
                    {
                        successes++;
                    }
                    else if("False".Equals(dataRow["Success"]))
                    {
                        failures++;
                    }
                    else if("Aborted".Equals(dataRow["Success"]))
                    {
                        aborts++;
                    }
                    else
                    {
                        unknowns++;
                    }
                }

                statusButton.Image=emptyIcon;
                if(successes==total)
                {
                    statusButton.Image=successIcon;
                }
                if(failures!=0)
                {
                    statusButton.Image=failureIcon;
                }

                statusLabel.Text="Total tests run: "+(successes+failures)+" Failures: "+failures+" "+(successes+failures!=0?"("+(100*failures/(successes+failures))+"%)":"");

            }
        }

        #endregion

        #region Solution Events

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
            //RefreshView();
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
            RefreshView();
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

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            NunitManager.StopRunners();
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

    }
}
