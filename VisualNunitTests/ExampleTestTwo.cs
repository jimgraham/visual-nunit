using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VisualNunitTests
{
    [TestFixture]
    public class ExampleTestTwo
    {
        [Test]
        public void TestTwoSuccess()
        {
        }

        [Test]
        public void TestTwoException()
        {
            throw new Exception("Test Exception");
        }

        [Test]
        public void TestTwoAssert()
        {
            Assert.Fail("Test Assert");
        }
    }
}
