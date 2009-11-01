// Guids.cs
// MUST match guids.h
using System;

namespace BubbleCloudorg.VisualNunit
{
    static class GuidList
    {
        public const string guidVisualNunitPkgString = "10d0dacc-0283-49e8-97e3-78a5bb87645f";
        public const string guidVisualNunitCmdSetString = "068b3718-0049-4e50-86c5-15f9ea1200e9";
        public const string guidToolWindowPersistanceString = "de16a11b-c6ec-4ee7-91d9-018efb193249";

        public static readonly Guid guidVisualNunitCmdSet = new Guid(guidVisualNunitCmdSetString);
    };
}