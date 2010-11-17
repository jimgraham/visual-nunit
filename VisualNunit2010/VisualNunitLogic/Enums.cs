using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualNunitLogic
{
    /// <summary>
    /// Supported platforms. These conform to the compiled versions
    /// of the runner program.
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// The "Any CPU" platform.
        /// </summary>
        AnyCPU,

        /// <summary>
        /// The "x86" platform
        /// </summary>
        X86,
    }
}
