using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;

namespace VisualNunitRunner
{
    public class RunnerClient
    {
        private string pipeName;
        private NamedPipeClientStream pipe;
        
        private byte[] readBuffer = new byte[65344];
        private int readByteCount = 0;

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
                readByteCount = this.pipe.Read(readBuffer, 0, readBuffer.Length);
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



    }
}
