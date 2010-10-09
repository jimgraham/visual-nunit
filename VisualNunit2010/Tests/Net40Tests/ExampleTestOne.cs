using System;
using System.Collections.Generic;
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

        [Test, Explicit]
        public void TestOneExplicit()
        {
        }
    }
}
