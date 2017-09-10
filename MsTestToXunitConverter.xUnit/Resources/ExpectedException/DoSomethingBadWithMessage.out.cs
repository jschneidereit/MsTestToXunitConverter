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
        public void DoSomethingBadWithMessage()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                Foo();
            });
            Assert.Equal("Bad thing happened", ex.Message);
        }
    }
}