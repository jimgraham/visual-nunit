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
    public static class NunitInterface
    {
        public static IList<string> ListTestCases(string assemblyPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(assemblyPath).ToString();
                string fileName = Path.GetFileName(assemblyPath);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\VisualNunitRunner.exe";
                startInfo.WorkingDirectory = directory;
                startInfo.Arguments = "list " + fileName;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

                IList<string> testCases = new List<string>();
                string line = null;

                while (!process.HasExited)
                {
                    line = process.StandardOutput.ReadLine();
                    if (line != null)
                    {
                        testCases.Add(line);
                    }
                    System.Threading.Thread.Sleep(1);
                }

                while ((line = process.StandardOutput.ReadLine()) != null)
                {
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

        public static void RunTestCase(TestInformation testInformation)
        {

            try
            {
                string directory = Path.GetDirectoryName(testInformation.AssemblyPath).ToString();
                string fileName = Path.GetFileName(testInformation.AssemblyPath);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString() + "\\VisualNunitRunner.exe";
                startInfo.WorkingDirectory = directory;
                startInfo.Arguments = "run " + fileName;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

                DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

                foreach (EnvDTE.Process localProcess in dte.Debugger.LocalProcesses)
                {
                    if (localProcess.ProcessID == process.Id)
                    {
                        int processId = process.Id;
                        string localProcessName = localProcess.Name;
                        localProcess.Attach();
                    }
                }

                process.StandardInput.WriteLine(testInformation.TestName);
                string line = null;

                //char[] buffer = new char[1024];
                while (!process.HasExited)
                {
                    bool isDebugged = false;
                    foreach (EnvDTE.Process localProcess in dte.Debugger.DebuggedProcesses)
                    {
                        if (localProcess.ProcessID == process.Id)
                        {
                            isDebugged = true;
                        }
                    }
                    if (!isDebugged)
                    {
                        process.Kill();
                        testInformation.Success = "Aborted";
                        testInformation.Time = "";
                        return;
                    }


                    /*int readCount=process.StandardOutput.Read(buffer, 0, buffer.Length);
                    line = new string(buffer, 0, readCount);
                    if (line.Length != 0)
                    {
                        if (line == "end-of-test-output")
                        {
                            break;
                        }
                        Console.WriteLine(line);
                    }*/

                    if (!process.StandardOutput.EndOfStream && line != "end-of-test-output")
                    {
                        line = process.StandardOutput.ReadLine();
                        if (line != null)
                        {
                            if (line != "end-of-test-output")
                            {
                                Console.WriteLine(line);
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(1);
                }

                if (line != "end-of-test-output")
                {
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        if (line == "end-of-test-output")
                        {
                            break;
                        }
                        Console.WriteLine(line);
                    }
                }

                string output = process.StandardOutput.ReadToEnd();
                XmlDocument result = new XmlDocument();
                result.LoadXml(output);
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
                    }
                }

                foreach (XmlNode failureNode in caseNode.ChildNodes)
                {
                    if (failureNode.LocalName == "failure")
                    {
                        foreach (XmlNode informationNode in failureNode)
                        {
                            if (informationNode.LocalName == "message")
                            {
                                testInformation.FailureMessage = informationNode.InnerText;
                                Trace.TraceInformation(testInformation.TestName + " failure message: " + testInformation.FailureMessage);
                            }
                            if (informationNode.LocalName == "stack-trace")
                            {
                                testInformation.FailureStackTrace = informationNode.InnerText;
                                Trace.TraceInformation(testInformation.FailureStackTrace);
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
