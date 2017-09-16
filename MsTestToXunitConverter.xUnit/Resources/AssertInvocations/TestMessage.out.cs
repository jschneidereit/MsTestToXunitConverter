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
        public void TestMessage()
        {
            Assert.Equal(1, 2); //message
            
            Assert.NotEqual(1, 2); //message
            
            Assert.NotSame(1, 1); //message
            
            Assert.Same(1, 1); //message
            
            Assert.IsNotType<Int32>(1); //message
            
            Assert.NotNull(1); //message
            
            Assert.Null(1); //message
            
            Assert.False(true, "message");
            
            Assert.True(true, "message");
        }
    }
}