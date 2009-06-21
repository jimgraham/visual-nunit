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

namespace VisualNunitRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CoreExtensions.Host.InitializeService();

                ConsoleTraceListener consoleListener = new ConsoleTraceListener();
                Trace.Listeners.Add(consoleListener);

                string assemblyName = "";
                if (args.Length == 2)
                {
                    assemblyName = args[1];
                }
                else
                {
                    Console.WriteLine("usage: VisualNunitRunner.exe [run|list] file");
                    return;
                }

                if (args.Length == 2 && args[0] == "run")
                {

                    RunnerListener runnerListener = new RunnerListener();
                    SimpleNameFilter testFilter = new SimpleNameFilter();
                    testFilter.Add(Console.ReadLine());

                    TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
                    TestResult result = testSuite.Run(runnerListener, testFilter);

                }
                else if (args.Length == 2 && args[0] == "list")
                {
                    TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
                    Queue<ITest> testQueue = new Queue<ITest>();
                    testQueue.Enqueue(testSuite);
                    while (testQueue.Count>0)
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
            }
            catch (Exception e)
            {
                Trace.TraceError("VisualNunitRunner failed: "+e);
            }
        }
    }
}
