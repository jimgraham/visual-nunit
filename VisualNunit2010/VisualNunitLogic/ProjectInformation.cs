using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualNunitLogic
{
    /// <summary>
    /// Project information value object.
    /// </summary>
    public class ProjectInformation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectInformation()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="platformName">the name of the project platform Name</param>
        public ProjectInformation( string platformName )
        {
            this.Platform = ProjectInformation.ParsePlatformName(platformName);
        }

        /// <summary>
        /// Project name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Project output assembly path.
        /// </summary>
        public string AssemblyPath;

        /// <summary>
        /// The platform type for this Project
        /// </summary>
        public PlatformType Platform;

        /// <summary>
        /// Convert a platform name to an enum
        /// </summary>
        /// <param name="platformName">the name of the platform</param>
        /// <returns>a <see cref="PlatformType"/></returns>
        private static PlatformType ParsePlatformName(string platformName)
        {
            PlatformType type = PlatformType.AnyCPU;
            Enum.TryParse<PlatformType>(platformName.Replace(" ", string.Empty), true, out type);
            return type;
        }
    }
}
