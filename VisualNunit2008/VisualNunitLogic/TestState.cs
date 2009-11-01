using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualNunitLogic
{
    /// <summary>
    /// Allowed test states.
    /// </summary>
    public enum TestState
    {
        /// <summary>
        /// Test has not been run.
        /// </summary>
        None,
        /// <summary>
        /// Test has been run succesfully.
        /// </summary>
        Success,
        /// <summary>
        /// Test run failed.
        /// </summary>
        Failure,
        /// <summary>
        /// Test was aborted before completion.
        /// </summary>
        Aborted
    }
}
