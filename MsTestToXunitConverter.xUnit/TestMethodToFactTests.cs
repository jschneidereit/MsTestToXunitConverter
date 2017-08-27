using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using Xunit;


namespace MsTestToXunitConverter.xUnit
{
    public class TestMethodToFactTests
    {
        [Fact(DisplayName = "Converts TestMethod to fact")]
        public void ConvertTestMethod()
        {
            var method = ResourceHelper.GetTestMethod("TestMethodA");

            var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
            var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();            
            
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Converts ignore to Skip")]
        public void ConvertTestMethodIgnore()
        {
            var methods = new List<string> { "IgnoreA", "IgnoreB", "IgnoreC", "IgnoreD" };
            methods.ForEach(m =>
            {
                var method = ResourceHelper.GetTestMethod(m);

                var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
                var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

                

                Assert.Equal(expected, actual);
            });
        }

        [Fact(DisplayName = "Converts description to DisplayName")]
        public void ConvertTestMethodDescription()
        {
            var methods = new List<string> { "DescriptionA", "DescriptionB", "DescriptionC" };
            methods.ForEach(m =>
            {
                var method = ResourceHelper.GetTestMethod(m);

                var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
                var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

                Assert.Equal(expected, actual);
            });
        }

        [Fact(DisplayName = "Converts all three attributes to one")]
        public void ConvertTestMethodIgnoreAndDescription()
        {
            var methods = new List<string> { "TestMethodB", "TestMethodC" };
            methods.ForEach(m =>
            {
                var method = ResourceHelper.GetTestMethod(m);

                var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
                var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

                Assert.Equal(expected, actual);
            });
        }

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public void ConvertPassesOverNormalMethods()
        {
            var method = ResourceHelper.GetTestMethod("TestMethodX");

            var actual = method.Item1.StripSurjectiveFactAttributes().NormalizeWhitespace(elasticTrivia:true).ToFullString();
            var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

            Assert.Equal(expected, actual);
        }
    }
}
