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

namespace BubbleCloudorg.VisualNunit
{
    public static class NunitManager
    {

        /// <summary>
        /// Synchronously lists test case full names from an assembly with separate NunitRunner process.
        /// </summary>
        /// <param name="assemblyPath">Path to assembly to list test cases from.</param>
        /// <returns></returns>
        public static IList<string> ListTestCases(string assemblyPath)
        {
            try
            {
                // Parse the directory and file information from path.
                string directory = Path.GetDirectoryName(assemblyPath).ToString();
                string fileName = Path.GetFileName(assemblyPath);

                // Starting a new NunitRunner process.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\VisualNunitRunner.exe";
                startInfo.WorkingDirectory = directory;
                startInfo.Arguments = "list " + fileName;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;                
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

                // Read the test case names from project output while process is running.
                IList<string> testCases = new List<string>();
                string line = null;
                while (!process.HasExited)
                {
                    if (!process.StandardOutput.EndOfStream)
                    {
                        line = process.StandardOutput.ReadLine();
                        testCases.Add(line);
                    }
                    System.Threading.Thread.Sleep(10);
                }

                // Read rest of the test cases from the standard output.
                while(!process.StandardOutput.EndOfStream)
                {
                    line = process.StandardOutput.ReadLine();
                    testCases.Add(line);
                }

                return testCases;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
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

            try
            {
                testInformation.Stop = false;
                // Parse the directory and file information from path.
                string directory = Path.GetDirectoryName(testInformation.AssemblyPath).ToString();
                string fileName = Path.GetFileName(testInformation.AssemblyPath);

                // Starting a new NunitRunner process.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\VisualNunitRunner.exe";
                startInfo.WorkingDirectory = directory;
                startInfo.Arguments = "run " + fileName;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

                // Binding the NunitRunner process to debugger.
                DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

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

                // Requesting the NunitRunner process to start executing the test.
                process.StandardInput.WriteLine(testInformation.TestName);

                // Monitoring for debug stop and reading standard output as long as 
                // the NunitRunner process is alive.
                string line = null;
                string resultXmlString = "";
                bool resultXmlStarted = false;
                while (!process.HasExited)
                {
                    if (testInformation.Debug)
                    {
                        bool isProcessStillDebugged = false;
                        foreach (EnvDTE.Process localProcess in dte.Debugger.DebuggedProcesses)
                        {
                            if (localProcess.ProcessID == process.Id)
                            {
                                isProcessStillDebugged = true;
                            }
                        }
                        if (!isProcessStillDebugged)
                        {
                            process.Kill();
                            testInformation.Success = "Aborted";
                            testInformation.FailureMessage = "User aborted.";
                            testInformation.Time = "";
                            return;
                        }
                    }

                    if (testInformation.Stop)
                    {
                        process.Kill();
                        testInformation.Success = "Aborted";
                        testInformation.FailureMessage = "User aborted.";
                        testInformation.Time = "";
                        return;
                    }

                    if (!process.StandardOutput.EndOfStream)
                    {
                        line = process.StandardOutput.ReadLine();
                        if (line == "beginning-of-test-result-xml")
                        {
                            resultXmlStarted = true;
                        }
                        else
                        {
                            if (!resultXmlStarted)
                            {
                                Trace.WriteLine(line);
                            }
                            else
                            {
                                resultXmlString += line;
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(1);
                }

                // Reading rest of the standard output from NunitRunner process.
                while (!process.StandardOutput.EndOfStream)
                {
                    line = process.StandardOutput.ReadLine();
                    if (line == "beginning-of-test-result-xml")
                    {
                        resultXmlStarted = true;
                    }
                    else
                    {
                        if (!resultXmlStarted)
                        {
                            Trace.WriteLine(line);
                        }
                        else
                        {
                            resultXmlString += line;
                        }
                    }
                }

                // Parsing a result XML received from NunitRunner standard output.
                XmlDocument result = new XmlDocument();
                result.LoadXml(resultXmlString);

                // Filling in the TestInformation from result.
                XmlNode caseNode=result.GetElementsByTagName("test-case").Item(0);                
                foreach(XmlAttribute attribute in caseNode.Attributes)
                {
                    if (attribute.Name == "time")
                    {
                        testInformation.Time = attribute.Value;
                    }
                    if (attribute.Name == "success")
                    {
                        testInformation.Success = attribute.Value;
                        if ("True".Equals(testInformation.Success))
                        {
                            testInformation.FailureMessage = "Success";
                        }
                        else
                        {
                            testInformation.FailureMessage = "Failure: ";
                        }
                    }
                }

                testInformation.FailureStackTrace = "";
                foreach (XmlNode failureNode in caseNode.ChildNodes)
                {
                    if (failureNode.LocalName == "failure")
                    {
                        foreach (XmlNode informationNode in failureNode)
                        {
                            if (informationNode.LocalName == "message")
                            {
                                testInformation.FailureMessage += informationNode.InnerText;
                                Trace.WriteLine(testInformation.TestName + " " + testInformation.FailureMessage);
                            }
                            if (informationNode.LocalName == "stack-trace")
                            {
                                testInformation.FailureStackTrace = informationNode.InnerText;
                                Trace.WriteLine(testInformation.FailureStackTrace);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

    }
}
