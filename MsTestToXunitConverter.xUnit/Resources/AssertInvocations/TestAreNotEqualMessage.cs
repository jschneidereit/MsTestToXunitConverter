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
        public void TestAreNotEqualMessage()
        {
            Assert.AreNotEqual(1, 2, "message");
        }
    }
}