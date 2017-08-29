using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTestToXunitConverter.xUnit.Resources
{
    class TestInitializeA
    {
        [TestInitialize]
        public void TestInitialize()
        {

        }
    }

    class TestInitializeB
    {
        TestInitializeB()
        {
            var x = 1 + 2;
        }

        [TestInitialize]
        public void TestInitialize()
        {

        }
    }

    class TestDisposeA
    {
        [TestCleanup]
        public void Cleanup()
        {

        }
    }

    class TestDisposeB : IDisposable
    {
        [TestCleanup]
        public void Cleanup()
        {

        }

        public void Dispose()
        {
            var x = 1 + 2;
        }
    }

    class TestDisposeC : ICloneable
    {
        [TestCleanup]
        public void Cleanup()
        {

        }
    }
}
