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
        public void TestAreNotSameMessage()
        {
            Assert.NotSame(1, 1); //message
        }
    }
}