using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Xunit;

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
            var method = GetTestMethod(@"[TestMethod]public void Foo() {}");
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(@"[Fact]public void Foo() {}").ToString();
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Converts ignore to Skip")]
        public void ConvertTestMethodIgnore()
        {
            var method = GetTestMethod(@"[Ignore(""reason"")]public void Foo() {}");
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(@"[Fact(Skip = ""reason"")]public void Foo() {}").ToString();
            Assert.Equal(expected, actual);

        }

        [Fact(DisplayName = "Converts description to DisplayName")]
        public void ConvertTestMethodDescription()
        {
            var method = GetTestMethod(@"[Description(""description"")]public void Foo() {}");
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(@"[Fact(DisplayName = ""description"")]public void Foo() {}").ToString();
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Converts all three attributes to one")]
        public void ConvertTestMethodIgnoreAndDescription()
        {
            var method = GetTestMethod(@"[TestMethod][Skip(""reason"")][Description(""description"")]public void Foo() {}");
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(@"[Fact(DisplayName = ""description"", Skip = ""reason"")]public void Foo() {}").ToString();
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public void ConvertPassesOverNormalMethods()
        {
            const string testmethod = @"[RandomThing]public void Foo() {}";
            var method = GetTestMethod(testmethod);
            var actual = method.StripSurjectiveFactAttributes().ToString();

            var expected = GetTestMethod(testmethod).ToString();
            Assert.Equal(expected, actual);
        }
    }
}
