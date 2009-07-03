using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using NUnit.Core;
using System.IO;

namespace VisualNunitRunner
{

    /// <summary>
    /// Server class allowing for running multiple tests in the same process using named pipe for 
    /// communication with the visual studio process.
    /// </summary>
    public class RunnerServer
    {
        private string assemblyName;
        private string pipeName;

        private NamedPipeServerStream pipe;

        private Thread thread;
        private bool isStopRequested;

        public RunnerServer(string assemblyName)
        {
            this.assemblyName = assemblyName;
            this.pipeName="VisualNunitRunner-"+Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// Start server.
        /// </summary>
        public void Start()
        {
            // Thread lock to avoid collision with concurrent start invocation or run method exit phase.
            lock (this)
            {
                if (thread != null)
                {
                    throw new Exception("Already started.");
                }

                isStopRequested = false;
                pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.WriteThrough, 65344, 65344);
                thread = new Thread(Run);
                thread.Start();
            }
        }

        /// <summary>
        /// Request server stop.
        /// </summary>
        public void Stop()
        {
            isStopRequested = true;
        }

        private void Run()
        {
            // List the unit tests available to the pipe as single message.
            ListTests();

            while (!isStopRequested)
            {
                // Running up to 50 tests before sleeping.
                for (int i = 0; i < 50; i++)
                {

                    // Read pipe for tests to run.

                    if (isStopRequested)
                    {
                        break;
                    }
                }

                Thread.Sleep(10);
            }

            // Thread lock to avoid collision with start method invocation.
            lock (this)
            {
                pipe.Disconnect();
                pipe.Dispose();
                pipe = null;
                thread = null;
            }
        }

        /// <summary>
        /// List the unit tests available to the pipe as single message.
        /// </summary>
        private void ListTests()
        {
            TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
            Queue<ITest> testQueue = new Queue<ITest>();
            testQueue.Enqueue(testSuite);
            String testNames = "";
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
                    testNames += test.TestName.FullName + "\n";
                }
            }
            byte[] testNameBytes = Encoding.UTF8.GetBytes(testNames);
            pipe.Write(testNameBytes, 0, testNameBytes.Length);
        }

    }
}
