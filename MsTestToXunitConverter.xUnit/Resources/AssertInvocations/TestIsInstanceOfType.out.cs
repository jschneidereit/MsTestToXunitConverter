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
        public void TestIsInstanceOfType()
        {
            List<int> x = new List<int>();
            Assert.IsType<List<int>>(x);
        }
    }
}