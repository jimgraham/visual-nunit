using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using VisualNunitLogic;

namespace VisualNunitLogic
{
    /// <summary>
    /// Runner information value object.
    /// </summary>
    public class RunnerInformation
    {
        /// <summary>
        /// Runner client for test execution control.
        /// </summary>
        public RunnerClient Client;
        /// <summary>
        /// Runner process.
        /// </summary>
        public Process Process;
    }
}
