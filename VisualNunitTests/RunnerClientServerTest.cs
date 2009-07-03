using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using VisualNunitLogic;
using System.Threading;

namespace VisualNunitTests
{
    [TestFixture]
    public class RunnerClientServerTest
    {
        [Test]
        public void TestListingTests()
        {
            RunnerServer runnerServer = new RunnerServer("VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient(Process.GetCurrentProcess());

            Assert.AreEqual(8, runnerClient.TestCases.Count);

            runnerClient.Dispose();
        }

        [Test]
        public void TestRunningTest()
        {
            RunnerServer runnerServer = new RunnerServer("VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient(Process.GetCurrentProcess());

            Assert.AreEqual(8, runnerClient.TestCases.Count);
            
            runnerClient.Dispose();

        }
    }
}
