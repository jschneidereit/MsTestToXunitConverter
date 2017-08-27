using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace MsTestToXunitConverter.xUnit
{
    public class TestMethodToFactTests
    {
        /// <summary>
        /// (Actual, Expected)
        /// </summary>
        private (MethodDeclarationSyntax, MethodDeclarationSyntax) GetTestMethod(string name)
        {
            MethodDeclarationSyntax GetMethod(string text, string functionname)
            {
                var tree = CSharpSyntaxTree.ParseText(text, options: new CSharpParseOptions(kind: SourceCodeKind.Script));                
                var root = tree.GetRoot() as CompilationUnitSyntax;
                
                var methods = root.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>();
                    
                return methods.Single(m => m.Identifier.ToString().Equals(functionname, System.StringComparison.OrdinalIgnoreCase));
            }

            var source = Properties.Resources.TestMethod.ToString();
            var result = Properties.Resources.TestMethod_out.ToString();

            return (GetMethod(source, name), GetMethod(result, name));
        }

        [Fact(DisplayName = "Converts TestMethod to fact")]
        public void ConvertTestMethod()
        {
            var method = GetTestMethod("TestMethodA");

            var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
            var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();            
            
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Converts ignore to Skip")]
        public void ConvertTestMethodIgnore()
        {
            var methods = new List<string> { "IgnoreA", "IgnoreB", "IgnoreC" };
            methods.ForEach(m =>
            {
                var method = GetTestMethod(m);

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
                var method = GetTestMethod(m);

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
                var method = GetTestMethod(m);

                var actual = method.Item1.StripSurjectiveFactAttributes().ToFullString();
                var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

                Assert.Equal(expected, actual);
            });
        }

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public void ConvertPassesOverNormalMethods()
        {
            var method = GetTestMethod("TestMethodX");

            var actual = method.Item1.StripSurjectiveFactAttributes().NormalizeWhitespace(elasticTrivia:true).ToFullString();
            var expected = method.Item2.NormalizeWhitespace(elasticTrivia: true).ToString();

            Assert.Equal(expected, actual);
        }
    }
}
