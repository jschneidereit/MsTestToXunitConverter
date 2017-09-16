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
        public void TestMessage()
        {
            Assert.AreEqual(1, 2, "message");

            Assert.AreNotEqual(1, 2, "message");

            Assert.AreNotSame(1, 1, "message");

            Assert.AreSame(1, 1, "message");

            Assert.IsNotInstanceOfType(1, typeof(Int32), "message");

            Assert.IsNotNull(1, "message");

            Assert.IsNull(1, "message");

            Assert.IsFalse(true, "message");

            Assert.IsTrue(true, "message");
        }
    }
}