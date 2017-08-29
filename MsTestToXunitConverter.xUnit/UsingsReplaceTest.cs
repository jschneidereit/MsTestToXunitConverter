using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class UsingsReplaceTest
    {
        [Fact]
        public void SimpleUsingsReplaceTest()
        {
            var compilations = ResourceHelper.GetTestCompilations(null);

            var actual = compilations.Item1.Usings.Select(u => u.ReplaceUsing(Transformer.MSTEST_USING, Transformer.XUNIT_USING).ToString()).ToList();
            var expected = compilations.Item2.Usings.Select(u => u.ToString()).ToList();

            Assert.Equal(expected, actual);
        }
    }
}
