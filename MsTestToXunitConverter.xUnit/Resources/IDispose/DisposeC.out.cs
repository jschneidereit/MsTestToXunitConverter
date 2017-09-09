using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MsTestToXunitConverter.xUnit
{
    class DisposeC : ICloneable, IDisposable
    {
        public void Cleanup()
        {

        }

        void Dispose()
        {
            Cleanup();
        }
    }