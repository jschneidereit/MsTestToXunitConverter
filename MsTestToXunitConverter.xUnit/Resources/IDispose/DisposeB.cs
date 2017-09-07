using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MsTestToXunitConverter.xUnit
{
    class DisposeB : IDisposable
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
}