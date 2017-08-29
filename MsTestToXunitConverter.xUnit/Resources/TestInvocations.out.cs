using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MsTestToXunitConverter.xUnit.Resources
{
    class TestInvocations
    {
        public void TestAreEqual()
        {
            Assert.Equal(1, 1);
        }

        public void TestAreNotEqual()
        {
            Assert.NotEqual(1, 2);
        }

        public void TestAreSame()
        {
            var o = "something";
            Assert.Same(o, o);
        }

        public void TestContains()
        {
            Assert.Contains("ayyy", "ay");
        }

        public void TestDoesNotContain()
        {
            //No idea what xunit is talking about
        }

        public void TestInconculsive()
        {
            //Assert.Inconclusive("This should get commented out");
        }

        public void TestIsFalse()
        {
            Assert.False(false);
        }

        public void TestIsInstanceOfType()
        {
            List<int> x = new List<int>();
            Assert.IsType<List<int>>(x);
        }

        public void TestIsNotInstanceOfType()
        {
            Assert.IsNotType<List<int>>(null);
        }

        public void TestIsNotNull()
        {
            int? x = 1;
            Assert.NotNull(x);
        }

        public void TestIsNull()
        {
            int? x = null;
            Assert.Null(x);
        }

        public void TestIsTrue()
        {
            Assert.True(true);
        }
    }
}
