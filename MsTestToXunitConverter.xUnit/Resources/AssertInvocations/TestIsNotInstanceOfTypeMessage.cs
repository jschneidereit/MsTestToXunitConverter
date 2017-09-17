using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MsTestToXunitConverter.xUnit
{
    class TestClass
    {
        public void TestIsNotInstanceOfTypeMessage()
        {
            Assert.IsNotInstanceOfType(1, typeof(Int32), "message");
        }
    }
}