﻿using System;
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
            RunnerServer runnerServer = new RunnerServer("TestVisualNunitRunner", "VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient("TestVisualNunitRunner", Process.GetCurrentProcess());

            Assert.AreEqual(8, runnerClient.TestCases.Count);

            runnerClient.Disconnect();
        }

        [Test]
        public void TestRunningSuccessTest()
        {
            RunnerServer runnerServer = new RunnerServer("TestVisualNunitRunner", "VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient("TestVisualNunitRunner", Process.GetCurrentProcess());

            TestInformation testInformation = new TestInformation();
            testInformation.TestName = "VisualNunitTests.ExampleTestOne.TestOneSuccess";

            runnerClient.RunTest(testInformation);

            Assert.AreEqual(TestState.Success,testInformation.TestState);
            runnerClient.Disconnect();
        }

        [Test]
        public void TestRunningExceptionTest()
        {
            RunnerServer runnerServer = new RunnerServer("TestVisualNunitRunner", "VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient("TestVisualNunitRunner", Process.GetCurrentProcess());

            TestInformation testInformation = new TestInformation();
            testInformation.TestName = "VisualNunitTests.ExampleTestOne.TestOneException";

            runnerClient.RunTest(testInformation);

            Assert.AreEqual(TestState.Failure, testInformation.TestState);
            Assert.AreEqual("Failure: System.Exception : Test Exception",testInformation.FailureMessage);
            runnerClient.Disconnect();
        }

        [Test]
        public void TestRunningAssertTest()
        {
            RunnerServer runnerServer = new RunnerServer("TestVisualNunitRunner", "VisualNunitTests.dll");
            RunnerClient runnerClient = new RunnerClient("TestVisualNunitRunner", Process.GetCurrentProcess());

            TestInformation testInformation = new TestInformation();
            testInformation.TestName = "VisualNunitTests.ExampleTestOne.TestOneAssert";

            runnerClient.RunTest(testInformation);

            Assert.AreEqual(TestState.Failure, testInformation.TestState);
            Assert.AreEqual("Failure: Test Assert", testInformation.FailureMessage);
            runnerClient.Disconnect();
        }
    }
}
