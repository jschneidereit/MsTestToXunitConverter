using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class ExpectedExceptionToLambdaTests
    {
        [Fact(DisplayName = "ExpectedException - Gets converted to simple lambda")]
        public void ConvertTestMethod()
        {
            var method = ResourceHelper.GetTestMethod("DoSomethingBad");

            var actual = method.Item1.StripExpectedExceptionAttribute().ToFullString();
            var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

            Assert.Equal(expected, actual);
        }
    }
}
