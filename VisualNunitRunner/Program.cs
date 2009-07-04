using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Util;
using System.Diagnostics;
using System.IO;
using NUnit.Core.Builders;
using NUnit.Core.Filters;
using VisualNunitLogic;
using System.Threading;

namespace VisualNunitRunner
{
    /// <summary>
    /// Test runner process which can run either in interactive command line mode or
    /// named pipe based service mode to be used with visual studio. 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CoreExtensions.Host.InitializeService();

                string assemblyName = "";
                if (args.Length == 2)
                {
                    assemblyName = args[1];
                }
                else
                {
                    Console.WriteLine("usage: VisualNunitRunner.exe [run|list|serve] file");
                    return;
                }

                if (args.Length == 2 && args[0] == "run")
                {
                    // Run tests according to console input.
                    ConsoleTraceListener consoleListener = new ConsoleTraceListener();
                    Trace.Listeners.Add(consoleListener);

                    ConsoleTestRunListener runnerListener = new ConsoleTestRunListener();
                    SimpleNameFilter testFilter = new SimpleNameFilter();
                    testFilter.Add(Console.ReadLine());

                    TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
                    TestResult result = testSuite.Run(runnerListener, testFilter);

                    if (result.StackTrace != null && result.StackTrace.Length > 0)
                    {
                        Trace.TraceError(result.StackTrace);
                    }

                    Console.WriteLine("beginning-of-test-result-xml");
                    StringBuilder builder = new StringBuilder();
                    new XmlResultWriter(new StringWriter(builder)).SaveTestResult(result);
                    Console.WriteLine(builder.ToString());
                }
                else if (args.Length == 2 && args[0] == "list")
                {
                    // List tests to console output.
                    ConsoleTraceListener consoleListener = new ConsoleTraceListener();
                    Trace.Listeners.Add(consoleListener);

                    TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
                    Queue<ITest> testQueue = new Queue<ITest>();
                    testQueue.Enqueue(testSuite);
                    while (testQueue.Count > 0)
                    {
                        ITest test = testQueue.Dequeue();
                        if (test.Tests != null)
                        {
                            foreach (ITest childTest in test.Tests)
                            {
                                testQueue.Enqueue(childTest);
                            }
                        }
                        else
                        {
                            Console.WriteLine(test.TestName.FullName);
                        }
                    }
                }
                else if (args.Length == 2 && args[0] == "serve")
                {
                    // Run in service mode to serve visual studio via named pipes.
                    RunnerServer runnerServer = new RunnerServer(assemblyName);
                    while (runnerServer.IsAlive)
                    {
                        Thread.Sleep(10);
                    }
                    System.Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("VisualNunitRunner failed: "+e);
            }
        }
    }
}
