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
    public static class NunitManager
    {
        private static IDictionary<string, RunnerInformation> testRunners = new Dictionary<string, RunnerInformation>();

        /// <summary>
        /// Synchronously lists test case full names from an assembly with separate NunitRunner process.
        /// </summary>
        /// <param name="assemblyPath">Path to assembly to list test cases from.</param>
        /// <returns></returns>
        public static IList<string> ListTestCases(string assemblyPath)
        {
            // If runner already exists then shutdown the existing runner.
            if (testRunners.ContainsKey(assemblyPath))
            {
                RunnerInformation runnerInformation = testRunners[assemblyPath];
                try
                {
                    runnerInformation.Client.Dispose();
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
                runnerInformation.Client = new RunnerClient(runnerInformation.Process);

                testRunners.Add(assemblyPath, runnerInformation);

                return runnerInformation.Client.TestCases;
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to list tests for " + assemblyPath + ": " + e.ToString());
                return new List<string>();
            }

        }

        /// <summary>
        /// Runs a single test case in separate NunitRunner process synchronously.
        /// </summary>
        /// <param name="testInformation">Information identifying the test case and containing place holders for result information.</param>
        /// <param name="debug">Set to true to enable debug mode.</param>
        public static void RunTestCase(TestInformation testInformation)
        {
            if (!testRunners.ContainsKey(testInformation.AssemblyPath))
            {
                return;
            }

            RunnerInformation runnerInformation = testRunners[testInformation.AssemblyPath];

            testInformation.Stop = false;

            System.Diagnostics.Process process = runnerInformation.Process;

            // Binding the NunitRunner process to debugger.
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            try
            {
                if (testInformation.Debug)
                {
                    foreach (EnvDTE.Process localProcess in dte.Debugger.LocalProcesses)
                    {
                        if (localProcess.ProcessID == process.Id)
                        {
                            int processId = process.Id;
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


            try
            {
                runnerInformation.Client.RunTest(testInformation);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error running test:" + e.ToString());
            }

            try
            {
                foreach (EnvDTE.Process localProcess in dte.Debugger.DebuggedProcesses)
                {
                    if (localProcess.ProcessID == process.Id)
                    {
                        localProcess.Detach(false);
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError("Error detaching process from debugger: " + ex.ToString());
            }
        }

    }

}
