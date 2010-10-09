using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VisualNunitTests
{
    [TestFixture]
    public class ExampleTheory
    {
        [Theory]
        public void TestTheory([Values(1, 2, 3)] int x)
        {
            Assume.That(x > 2,"x > 2");
            Assume.That(x > 1,"x > 1");
        }
    }
}
