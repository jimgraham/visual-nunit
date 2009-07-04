using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VisualNunitLogic
{
    /// <summary>
    /// Test information value object.
    /// </summary>
    public class TestInformation
    {
        /// <summary>
        /// Name of the test.
        /// </summary>
        public string TestName;
        /// <summary>
        /// Path to assembly dll this test belongs to.
        /// </summary>
        public string AssemblyPath;
        /// <summary>
        /// Last execution time.
        /// </summary>
        public string Time;
        /// <summary>
        /// Test state.
        /// </summary>
        public TestState TestState;
        /// <summary>
        /// Test failure message.
        /// </summary>
        public string FailureMessage;
        /// <summary>
        /// Test failure stack trace.
        /// </summary>
        public string FailureStackTrace;
        /// <summary>
        /// User inferface model data row used by NUnit View.
        /// </summary>
        public DataRow DataRow;
        /// <summary>
        /// Whether test should be run in debug mode.
        /// </summary>
        public bool Debug;
        /// <summary>
        /// Whether test stop has been requested.
        /// </summary>
        public bool Stop;
    }
}
