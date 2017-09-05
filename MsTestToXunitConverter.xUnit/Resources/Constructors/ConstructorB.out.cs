using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MsTestToXunitConverter.xUnit
{
    class ConstructorB
    {
        ConstructorB()
        {
            var x = 1 + 2;
            SomeInitMethod();
        }

        public void SomeInitMethod()
        {

        }
    }
}