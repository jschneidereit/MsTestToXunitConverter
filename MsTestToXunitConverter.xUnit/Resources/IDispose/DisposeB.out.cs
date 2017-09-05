using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MsTestToXunitConverter.xUnit
{
    class DisposeB : IDisposable
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
}