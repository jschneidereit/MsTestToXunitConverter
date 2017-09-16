using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MsTestToXunitConverter.xUnit
{
    class TestClass
    {
        public void TestInconculsive()
        {
            Assert.True(false, "This used to be inconclusive. We set as failure so the user can run tests immediately.");
        }
    }
}