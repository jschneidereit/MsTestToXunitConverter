using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class TestInitializeToConstructorTests
    {
        [Fact(DisplayName = "TestInitialize - converts to call to method from ctor")]
        public void ConvertTestInitialize()
        {
            var tclass = ResourceHelper.GetTestClass("TestInitializeA");

            var actual = tclass.Item1.StripTestInitializerAttribute().ToFullString();
            var expected = tclass.Item2.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "TestInitialize - appends call to existing ctor")]
        public void AppendTestInitialize()
        {
            var tclass = ResourceHelper.GetTestClass("TestInitializeB");

            var actual = tclass.Item1.StripTestInitializerAttribute().ToFullString();
            var expected = tclass.Item2.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            Assert.Equal(expected, actual);
        }
    }
}
