using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using System.Diagnostics;
using NUnit.Util;
using System.IO;

namespace VisualNunitRunner
{
    /// <summary>
    /// Skeletal implementation of event listener for console test execution.
    /// </summary>
    public class ConsoleTestRunListener : EventListener
    {
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
