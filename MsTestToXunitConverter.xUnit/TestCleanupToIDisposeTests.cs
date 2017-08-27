using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class TestCleanupToIDisposeTests
    {
        [Fact(DisplayName = "TestCleanup - converts to call to method from Dispose")]
        public void ConvertTestCleanup()
        {
            var tclass = ResourceHelper.GetTestClass("TestDisposeA");

            var actual = tclass.Item1.StripTestCleanupAttribute().ToFullString();
            var expected = tclass.Item2.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "TestCleanup - prepends call to existing Dispose")]
        public void AppendtestCleanup()
        {
            var tclass = ResourceHelper.GetTestClass("TestDisposeB");

            var actual = tclass.Item1.StripTestCleanupAttribute().ToFullString();
            var expected = tclass.Item2.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "TestCleanup - properly preserves existing basetypes")]
        public void PreserveBaseTypes()
        {
            var tclass = ResourceHelper.GetTestClass("TestDisposeC");

            var actual = tclass.Item1.StripTestCleanupAttribute().ToFullString();
            var expected = tclass.Item2.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            Assert.Equal(expected, actual);
        }
    }
}
