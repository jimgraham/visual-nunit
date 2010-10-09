using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VisualNunitTests
{
    [TestFixture]
    public class ExampleParametrizedTest
    {
        [Test]
        public void TestParametrization([Values(1,2,3)] int x,[Values("Success","Failure","Exception")] string s)
        {
            if ("Failure".Equals(s))
            {
                Assert.Fail("Test Failure Assert");
            }
            if ("Exception".Equals(s))
            {
                throw new Exception("Test Exception");
            }

        }
    }
}
