using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MsTestToXunitConverter.xUnit
{
    class ConstructorB
    {
        ConstructorB()
        {
            var x = 1 + 2;
        }

        [TestInitialize]
        public void SomeInitMethod()
        {

        }
    }
}