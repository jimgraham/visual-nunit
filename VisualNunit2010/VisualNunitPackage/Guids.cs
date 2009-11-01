// Guids.cs
// MUST match guids.h
using System;

namespace BubbleCloudorg.VisualNunit
{
    static class GuidList
    {
        public const string guidVisualNunitPackagePkgString = "aae34e95-05a0-4514-83ac-38db6ff3a34f";
        public const string guidVisualNunitPackageCmdSetString = "69b78fbf-2b52-4df9-b9cb-a499cd2f7bac";
        public const string guidToolWindowPersistanceString = "53895457-12a1-4ce3-886a-923af0e61e0f";

        public static readonly Guid guidVisualNunitPackageCmdSet = new Guid(guidVisualNunitPackageCmdSetString);
    };
}