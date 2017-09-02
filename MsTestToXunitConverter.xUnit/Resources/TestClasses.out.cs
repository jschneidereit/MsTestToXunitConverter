using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit.Resources
{
    class TestInitializeA
    {
        public void TestInitialize()
        {
        }

        TestInitializeA()
        {
            TestInitialize();
        }
    }

    class TestInitializeB
    {
        TestInitializeB()
        {
                var x = 1 + 2;
            TestInitialize();
        }

        public void TestInitialize()
        {

        }
    }

    class TestDisposeA : IDisposable
    {
        public void Cleanup()
        {

        }

        void Dispose()
        {
            Cleanup();
        }
    }

    class TestDisposeB : IDisposable
    {
        public void Cleanup()
        {

        }

        public void Dispose()
        {
            Cleanup();
            var x = 1 + 2;
        }
    }

    class TestDisposeC : ICloneable, IDisposable
    {
        public void Cleanup()
        {

        }

        void Dispose()
        {
            Cleanup();
        }
    }
}
