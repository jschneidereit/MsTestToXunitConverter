using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTestToXunitConverter.xUnit.Resources
{
    class TestInvocations
    {
        public void TestAreEqual()
        {
            Assert.AreEqual(1, 1);
        }

        public void TestAreNotEqual()
        {
            Assert.AreNotEqual(1, 2);
        }

        public void TestAreSame()
        {
            var o = "something";
            Assert.AreSame(o, o);
        }

        public void TestContains()
        {
            StringAssert.Contains("ayyy", "ay");
        }

        public void TestDoesNotContain()
        {
            //No idea what xunit is talking about
        }

        public void TestInconculsive()
        {
            Assert.Inconclusive("This should get commented out");
        }

        public void TestIsFalse()
        {
            Assert.IsFalse(false);
        }

        public void TestIsInstanceOfType()
        {
            List<int> x = new List<int>();
            Assert.IsInstanceOfType(x, typeof(List<int>));
        }

        public void TestIsNotInstanceOfType()
        {
            Assert.IsNotInstanceOfType(null, typeof(List<int>));
        }

        public void TestIsNotNull()
        {
            int? x = 1;
            Assert.IsNotNull(x);
        }

        public void TestIsNull()
        {
            int? x = null;
            Assert.IsNull(x);
        }

        public void TestIsTrue()
        {
            Assert.IsTrue(true);
        }
    }
}
