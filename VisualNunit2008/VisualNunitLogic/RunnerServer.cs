using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using NUnit.Core;
using System.IO;
using NUnit.Core.Filters;
using NUnit.Util;

namespace VisualNunitLogic
{

    /// <summary>
    /// Server test runner class allowing for running multiple tests 
    /// in the same process using named pipe for communication with the 
    /// RunnerClient.
    /// </summary>
    public class RunnerServer : EventListener
    {
        /// <summary>
        /// Name of the assembly dll containing tests for this runner.
        /// </summary>
        private string assemblyName;
        /// <summary>
        /// Name of the pipe used to communicate with RunnerClient.
        /// </summary>
        private string pipeName;
        /// <summary>
        /// Named pipe used to communicate with RunnerClient.
        /// </summary>
        private NamedPipeServerStream pipe;
        /// <summary>
        /// Server thread which signals with RunnerClient and executes tests.
        /// </summary>
        private Thread thread;
        /// <summary>
        /// Read buffer used when reading from named pipe.
        /// </summary>
        private byte[] readBuffer = new byte[65344];
        /// <summary>
        /// Whether server thread is alive.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return thread.IsAlive;
            }
        }

        /// <summary>
        /// Constructs runner server with default pipe prefix for given assembly dll.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly dll containing tests.</param>
        public RunnerServer(string assemblyName)
        {
            ConstructRunnerServer("VisualNunitRunner", assemblyName);
        }

        /// <summary>
        /// Constructs runner server with custom pipe prefix for given assembly dll.
        /// </summary>
        /// <param name="pipePrefix">Custom pipe name prefix.</param>
        /// <param name="assemblyName">The name of the assembly dll containing tests.</param>
        public RunnerServer(string pipePrefix, string assemblyName)
        {
            ConstructRunnerServer(pipePrefix, assemblyName);
        }

        /// <summary>
        /// Construction logic shared by constructors.
        /// </summary>
        /// <param name="pipePrefix">Custom pipe name prefix.</param>
        /// <param name="assemblyName">The name of the assembly dll containing tests.</param>
        public void ConstructRunnerServer(string pipePrefix, string assemblyName)
        {
            this.assemblyName = assemblyName;
            this.pipeName=pipePrefix+"-"+Process.GetCurrentProcess().Id;

            // Construction of pipe and server thread.
            pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None);
            thread = new Thread(Run);

            // Starting server execution in separate thread.
            thread.Start();
        }

        /// <summary>
        /// Server thread run method. Reads pipe for test names, executes tests and writes result xml to pipe.
        /// </summary>
        private void Run()
        {
            // Wait until connected.
            pipe.WaitForConnection();

            // List the unit tests available to the pipe as single message.
            ListTests();

            try
            {
                while (pipe.IsConnected)
                {
                    // Read pipe for tests to run. Blocks until test name is received or client is disconnected.
                    string testName = ReadTestName();
                    if (testName != null)
                    {
                        string result = RunTest(testName);
                        WriteTestResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception in RunnerServer main loop:"+e);
            }

            // Release pipe resources.
            pipe.Dispose();
        }

        /// <summary>
        /// List the unit tests available to the pipe as single message.
        /// </summary>
        private void ListTests()
        {
            // Build test suite from the given assembly.
            TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
            
            // Recursively browse tests and concatenate full test names to string in separate lines. 
            Queue<ITest> testQueue = new Queue<ITest>();
            testQueue.Enqueue(testSuite);
            String testNames = "Tests:";
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
                    if (testNames.Length > 0)
                    {
                        testNames += "\n";
                    }
                    testNames += test.TestName.FullName;
                }
            }

            // Write test names to the pipe.
            byte[] testNameBytes = Encoding.UTF8.GetBytes(testNames);
            pipe.Write(testNameBytes, 0, testNameBytes.Length);
            pipe.Flush();
        }

        /// <summary>
        /// Runs the requested test and return result XML.
        /// </summary>
        /// <param name="testName"></param>
        /// <returns>Result XML document</returns> 
        private string RunTest(string testName)
        {
            // Execute the give test.
            SimpleNameFilter testFilter = new SimpleNameFilter();
            testFilter.Add(testName);
            TestSuite testSuite = new TestBuilder().Build(assemblyName, true);
            TestResult result = testSuite.Run(this, testFilter);

            // Trace error stack trace.
            if (result.StackTrace != null && result.StackTrace.Length > 0)
            {
                Trace.TraceError(result.StackTrace);
            }

            // Serialize result to XML.
            StringBuilder builder = new StringBuilder();
            new XmlResultWriter(new StringWriter(builder)).SaveTestResult(result);

            return builder.ToString();          
        }

        /// <summary>
        /// Blocking read to pipe for retrieving the test name to run.
        /// </summary>
        /// <returns>Test name</returns>
        private string ReadTestName()
        {
            // Test case name should always fit in buffer.
            int readByteCount = pipe.Read(readBuffer, 0, readBuffer.Length);
            if (readByteCount > 0)
            {
                return Encoding.UTF8.GetString(readBuffer, 0, readByteCount);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Writes result xml document to pipe.
        /// </summary>
        /// <param name="result"></param>
        private void WriteTestResult(String result)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(result.ToString());
            pipe.Write(resultBytes, 0, resultBytes.Length);
            pipe.Flush();
        }

        #region EventListener Members

        public void RunFinished(Exception exception)
        {
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunStarted(string name, int testCount)
        {
        }

        public void SuiteFinished(TestResult result)
        {
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void TestFinished(TestResult result)
        {
        }

        public void TestOutput(TestOutput testOutput)
        {
        }

        public void TestStarted(TestName testName)
        {
        }

        public void UnhandledException(Exception exception)
        {
        }

        #endregion
    }
}
