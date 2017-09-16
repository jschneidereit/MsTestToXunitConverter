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
        public void TestInconculsive()
        {
            Assert.Inconclusive("This used to be inconclusive. We set as failure so the user can run tests immediately.");
        }
    }
}