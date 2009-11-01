using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VisualNunitTests
{
    [TestFixture]
    public class ExampleTestOne
    {
        [Test]
        public void TestOneSuccess()
        {
        }

        [Test]
        public void TestOneException()
        {
            throw new Exception("Test Exception");
        }

        [Test]
        public void TestOneAssert()
        {
            Assert.Fail("Test Assert");
        }
    }
}
