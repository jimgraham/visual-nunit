using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;
using System.Xml;

namespace VisualNunitLogic
{
    public class RunnerClient : IDisposable
    {
        private string pipeName;
        private NamedPipeClientStream pipe;
        
        private byte[] readBuffer = new byte[65344];

        public IList<string> TestCases = new List<string>();

        public RunnerClient(Process serverProcess)
        {
            this.pipeName = "VisualNunitRunner-" + Process.GetCurrentProcess().Id;
            this.pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.WriteThrough);
            this.pipe.Connect(50);
            this.pipe.ReadMode = PipeTransmissionMode.Message;
            
            string testCaseNames="";
            while (true)
            {
                int readByteCount = this.pipe.Read(readBuffer, 0, readBuffer.Length);
                testCaseNames += Encoding.UTF8.GetString(readBuffer, 0, readByteCount);
                if (this.pipe.IsMessageComplete)
                {
                    break;
                }
            }

            string[] testCaseNameArray = testCaseNames.Split('\n');
            foreach (string testCaseName in testCaseNameArray)
            {
                TestCases.Add(testCaseName);
            }
        }

        public void Dispose()
        {
            pipe.Dispose();
            pipe = null;
        }

        public void RunTest(TestInformation testInformation)
        {
            byte[] testNameBytes = Encoding.UTF8.GetBytes(testInformation.TestName);
            pipe.Write(testNameBytes, 0, testNameBytes.Length);

            string resultXml = "";
            while (true)
            {
                int readByteCount = pipe.Read(readBuffer, 0, readBuffer.Length);
                resultXml += Encoding.UTF8.GetString(readBuffer, 0, readByteCount);
                if (pipe.IsMessageComplete)
                {
                    break;
                }
            }

            // Parsing a result XML received from NunitRunner standard output.
            XmlDocument result = new XmlDocument();
            result.LoadXml(resultXml);

            // Filling in the TestInformation from result.
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
