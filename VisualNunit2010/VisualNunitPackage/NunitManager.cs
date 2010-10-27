using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using EnvDTE;
using System.Xml;
using Microsoft.VisualStudio.Shell;
using VisualNunitLogic;

namespace BubbleCloudorg.VisualNunit
{

    /// <summary>
    /// Singleton managing access to nunit runner processes. 
    /// Offers methods for starting runners, stopping runners, 
    /// running tests and aborting tests.
    /// </summary>
    public static class NunitManager
    {
        private static IDictionary<string, RunnerInformation> testRunners = new Dictionary<string, RunnerInformation>();

        /// <summary>
        /// Starts or restarts runner process and lists test cases it hosts.
        /// </summary>
        /// <param name="assemblyPath">Path to assembly to list test cases from.</param>
        /// <returns></returns>
        public static IList<string> StartRunner(string assemblyPath)
        {

            // If runner already exists then shutdown the existing runner.
            if (testRunners.ContainsKey(assemblyPath))
            {
                RunnerInformation runnerInformation = testRunners[assemblyPath];
                try
                {
                    runnerInformation.Client.Disconnect();
                }
                catch (Exception)
                {
                }
                try
                {
                    runnerInformation.Process.WaitForExit(200);
                }
                catch (Exception)
                {
                }
                try
                {
                    if (!runnerInformation.Process.HasExited)
                    {
                        runnerInformation.Process.Kill();
                    }
                }
                catch (Exception)
                {
                }
                testRunners.Remove(assemblyPath);
            }

            try
            {
                // Parse the directory and file information from path.
                string directory = Path.GetDirectoryName(assemblyPath).ToString();
                string fileName = Path.GetFileName(assemblyPath);

                // Starting a new NunitRunner process.

                // determine what item 
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\VisualNunitRunner.exe";
                startInfo.WorkingDirectory = directory;
                startInfo.Arguments = "serve " + fileName;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardInput = false;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                RunnerInformation runnerInformation = new RunnerInformation();
                runnerInformation.Process = System.Diagnostics.Process.Start(startInfo);

                // determine which mode we are in
                runnerInformation.Client = new RunnerClient(runnerInformation.Process);

                if (runnerInformation.Client.TestCases.Count > 0)
                {
                    testRunners.Add(assemblyPath, runnerInformation);
                }
                else
                {
                    runnerInformation.Client.Disconnect();
                }

                return runnerInformation.Client.TestCases;
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to list tests for " + assemblyPath + ": " + e.ToString());
                return new List<string>();
            }

        }

        /// <summary>
        /// Stops runner processes build events can update dlls.
        /// </summary>
        public static void StopRunners()
        {
            foreach (RunnerInformation runnerInformation in testRunners.Values)
            {
                try
                {
                    runnerInformation.Client.Disconnect();
                }
                catch (Exception)
                {
                }
                try
                {
                    runnerInformation.Process.Kill();
                }
                catch (Exception)
                {
                }
            }
            testRunners.Clear();
        }

        /// <summary>
        /// Prepares development environment for test case running.
        /// This has to be executed from UI thread.
        /// </summary>
        /// <param name="testInformation"></param>
        public static void PreRunTestCase(TestInformation testInformation)
        {
            testInformation.Stop = false;

            try
            {

                if (testInformation.Debug)
                {
                    if (!testRunners.ContainsKey(testInformation.AssemblyPath))
                    {
                        return;
                    }

                    RunnerInformation runnerInformation = testRunners[testInformation.AssemblyPath];

                    // Attaching the NunitRunner process to debugger.
                    DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

                    foreach (EnvDTE.Process localProcess in dte.Debugger.LocalProcesses)
                    {
                        if (localProcess.ProcessID == runnerInformation.Process.Id)
                        {
                            int processId = runnerInformation.Process.Id;
                            string localProcessName = localProcess.Name;
                            localProcess.Attach();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error attaching process to debugger: " + ex.ToString());
            }

        }

        /// <summary>
        /// Runs a single test case in separate NunitRunner process synchronously.
        /// </summary>
        /// <param name="testInformation">Information identifying the test case and containing place holders for result information.</param>
        /// <param name="debug">Set to true to enable debug mode.</param>
        public static void RunTestCase(TestInformation testInformation,bool explicitRun)
        {
            if (!testRunners.ContainsKey(testInformation.AssemblyPath))
            {
                return;
            }

            RunnerInformation runnerInformation = testRunners[testInformation.AssemblyPath];

            try
            {
                runnerInformation.Client.RunTest(testInformation, explicitRun);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error running test:" + e.ToString());
            }

        }

        /// <summary>
        /// Recovers development environment from test case running.
        /// This has to be executed from UI thread.
        /// </summary>
        /// <param name="testInformation"></param>
        public static void PostRunTestCase(TestInformation testInformation)
        {
            try
            {
                if (testInformation.Debug)
                {
                    if (!testRunners.ContainsKey(testInformation.AssemblyPath))
                    {
                        return;
                    }

                    RunnerInformation runnerInformation = testRunners[testInformation.AssemblyPath];

                    // Detaching the NunitRunner process from debugger.
                    DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

                    foreach (EnvDTE.Process localProcess in dte.Debugger.DebuggedProcesses)
                    {
                        if (localProcess.ProcessID == runnerInformation.Process.Id)
                        {
                            localProcess.Detach(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error detaching process from debugger: " + ex.ToString());
            }

        }

        /// <summary>
        /// Aborts execution of test case by restarting the test runner process.
        /// </summary>
        /// <param name="testInformation"></param>
        public static void AbortTestCase(TestInformation testInformation)
        {
            if (!testRunners.ContainsKey(testInformation.AssemblyPath))
            {
                return;
            }
            testInformation.Stop = true;
            StartRunner(testInformation.AssemblyPath);
        }

    }

}
