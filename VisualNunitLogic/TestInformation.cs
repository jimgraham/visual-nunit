using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VisualNunitLogic
{
    public class TestInformation
    {
        public string TestName;
        public string AssemblyPath;
        public string Time;
        public string Success;
        public string FailureMessage;
        public string FailureStackTrace;
        public DataRow DataRow;
        public bool Debug;
        public bool Stop;
    }
}
