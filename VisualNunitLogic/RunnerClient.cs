using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;
using System.Xml;

namespace VisualNunitLogic
{
    /// <summary>
    /// Runner process client which uses named pipes to communicate 
    /// with the runner process to list and execute tests.
    /// </summary>
    public class RunnerClient
    {
        /// <summary>
        /// Name of the named pipe.
        /// </summary>
        private string pipeName;
        /// <summary>
        /// Named pipe used to communicate with the server process.
        /// </summary>
        private NamedPipeClientStream pipe;        
        /// <summary>
        /// Input buffer used to read from named pipe.
        /// </summary>
        private byte[] readBuffer = new byte[65344];
        /// <summary>
        /// List of test cases hosted by the test runner process.
        /// </summary>
        public IList<string> TestCases = new List<string>();

        /// <summary>
        /// Constructs new runner client with default pipe name prefix.
        /// </summary>
        /// <param name="serverProcess">Server process executing RunnerServer.</param>
        public RunnerClient(Process serverProcess)
        {
            ConstructRunnerClient("VisualNunitRunner", serverProcess);
        }

        /// <summary>
        /// Constructs new runner client with custom pipe name prefix.
        /// </summary>
        /// <param name="pipePrefix">Custom pipe name prefix.</param>
        /// <param name="serverProcess">Server process executing RunnerServer.</param>
        public RunnerClient(String pipePrefix, Process serverProcess)
        {
            ConstructRunnerClient(pipePrefix, serverProcess);
        }

        /// <summary>
        /// Construction logic shared by constructors.
        /// </summary>
        /// <param name="pipePrefix"></param>
        /// <param name="serverProcess"></param>
        private void ConstructRunnerClient(String pipePrefix,Process serverProcess)
        {
            // Configuring the named pipe for communication with RunnerServer.
            this.pipeName = pipePrefix +"-"+ serverProcess.Id;
            this.pipe = new NamedPipeClientStream("localhost", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            this.pipe.Connect(5000);
            this.pipe.ReadMode = PipeTransmissionMode.Message;

            // Blocking read of the test case names hosting in RunnerServer.
            string testCaseNames = "";
            while (true)
            {
                int readByteCount = this.pipe.Read(readBuffer, 0, readBuffer.Length);
                testCaseNames += Encoding.UTF8.GetString(readBuffer, 0, readByteCount);
                if (this.pipe.IsMessageComplete)
                {
                    break;
                }
            }

            // Parsing the test name list.
            string[] testCaseNameArray = testCaseNames.Split('\n');
            for (int i = 1; i < testCaseNameArray.Length; i++)
            {
                TestCases.Add(testCaseNameArray[i]);
            }
        }

        /// <summary>
        /// Disconnects client from RunnerServer.
        /// </summary>
        public void Disconnect()
        {
            // Thread lock to allow for graceful exit from result reading loop.
            lock (this)
            {
                pipe.Close();
            }
        }

        /// <summary>
        /// Signals RunnerServer to execute the test and fills in test result to test information object.
        /// </summary>
        /// <param name="testInformation">The test to be executed.</param>
        public void RunTest(TestInformation testInformation)
        {
            // Signal the test execution.
            byte[] testNameBytes = Encoding.UTF8.GetBytes(testInformation.TestName);
            pipe.Write(testNameBytes, 0, testNameBytes.Length);
            pipe.Flush();

            // Blocking read to the test result.
            string resultXml = "";
            while(true)
            {
                int readByteCount = pipe.Read(readBuffer, 0, readBuffer.Length);
                resultXml += Encoding.UTF8.GetString(readBuffer, 0, readByteCount);

                // Thread lock to allow for graceful exit from result reading loop.
                lock (this)
                {
                    if(!pipe.IsConnected) 
                    {
                        break;
                    }
                    if(pipe.IsMessageComplete)
                    {
                        break;
                    }
                }
            }

            // If user aborted then fill manually result information else parse it from result xml.
            if (testInformation.Stop)
            {
                // Filling in abort information.
                testInformation.Success = "Aborted";
                testInformation.FailureMessage = "Aborted";
                testInformation.Time = "";
            }
            else
            {
                // Parsing a result XML received from NunitRunner standard output.
                XmlDocument result = new XmlDocument();
                result.LoadXml(resultXml);

                // Filling in general information from result.
                XmlNode caseNode = result.GetElementsByTagName("test-case").Item(0);
                foreach (XmlAttribute attribute in caseNode.Attributes)
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

                // Filling in failure information from result.
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
                            }
                            if (informationNode.LocalName == "stack-trace")
                            {
                                testInformation.FailureStackTrace = informationNode.InnerText;
                            }
                        }
                    }
                }


            }
        }

    }
}
