using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MsTestToXunitConverter.xUnit
{
    class DisposeA : IDisposable
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