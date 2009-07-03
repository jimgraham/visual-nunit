using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace VisualNunitTests
{
    [TestFixture]
    public class ExampleTestThree
    {
        public ExampleTestThree()
        {
            throw new Exception("TestConstructionException");
        }

        [Test]
        public void ExampleTestWithConstructionException()
        {
        }
    }
}
