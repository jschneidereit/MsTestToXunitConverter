using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Xunit;
using MsTestToXunitConverter;

namespace MsTestToXunitConverter.xUnit
{
    public class ToFactTests
    {
        private MethodDeclarationSyntax GetTestMethod(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source, options: new CSharpParseOptions(kind: SourceCodeKind.Script));
            var root = tree.GetRoot() as CompilationUnitSyntax;

            return root.Members.OfType<MethodDeclarationSyntax>().First();
        }

        [Fact(DisplayName = "Converts TestMethod to fact")]
        public void ConvertTestMethod()
        {
            var method = GetTestMethod(@"[TestMethod]public void foo() {}");
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(@"[Fact]public void foo() {}").ToString();
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Converts ignore to Skip")]
        public void ConvertTestMethodIgnore()
        {

        }

        [Fact(DisplayName = "Converts description to DisplayName")]
        public void ConvertTestMethodDescription()
        {

        }

        [Fact(DisplayName = "Converts all three attributes to one")]
        public void ConvertTestMethodIgnoreAndDescription()
        {

        }

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public void ConvertPassesOverNormalMethods()
        {

        }
    }
}
