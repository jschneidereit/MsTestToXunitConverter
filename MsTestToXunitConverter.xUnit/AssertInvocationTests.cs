using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MsTestToXunitConverter.xUnit
{
    public class AssertInvocationTests
    {
        private SyntaxAnnotation annotation { get; } = Formatter.Annotation;

        private async Task ExecuteAsyncInvocationTest(string name, Func<InvocationExpressionSyntax, InvocationExpressionSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetInvocation();

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target));

            var actual = pod.ActualDocument.WithSyntaxRoot(new_root);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actual_string = actual.GetSyntaxRootAsync().Result.GetInvocation().ToString();
            var expect_string = expected.GetSyntaxRootAsync().Result.GetInvocation().ToString();

            Assert.Equal(expected: expect_string, actual: actual_string);
        }

        [Fact]
        public async Task ConvertsAssertAreEqual() => await ExecuteAsyncInvocationTest("TestAreEqual", AssertRewriter.RewriteAreEqual);

        [Fact]
        public async Task ConvertsAssertAreNotEqual() => await ExecuteAsyncInvocationTest("TestAreNotEqual", AssertRewriter.RewriteAreNotEqual);

        [Fact]
        public async Task ConvertAssertAreSame() => await ExecuteAsyncInvocationTest("TestAreSame", AssertRewriter.RewriteAreSame);

        [Fact]
        public async Task ConvertsAssertContains() => await ExecuteAsyncInvocationTest("TestContains", AssertRewriter.RewriteContains);
        
        [Fact]
        public async Task ConvertsAssertIsFalse() => await ExecuteAsyncInvocationTest("TestIsFalse", AssertRewriter.RewriteIsFalse);

        [Fact]
        public async Task ConvertsAssertIsInstanceOfType() => await ExecuteAsyncInvocationTest("TestIsInstanceOfType", AssertRewriter.RewriteIsInstanceOfType);

        [Fact]
        public async Task CovnertAssertIsNotInstanceOfType() => await ExecuteAsyncInvocationTest("TestIsNotInstanceOfType", AssertRewriter.RewriteIsNotInstanceOfType);

        [Fact]
        public async Task ConvertsAssertIsNotNull() => await ExecuteAsyncInvocationTest("TestIsNotNull", AssertRewriter.RewriteIsNotNull);

        [Fact]
        public async Task ConvertsAssertIsNull() => await ExecuteAsyncInvocationTest("TestIsNull", AssertRewriter.RewriteIsNull);

        [Fact]
        public async Task ConvertsAssertIsTrue() => await ExecuteAsyncInvocationTest("TestIsTrue", AssertRewriter.RewriteIsTrue);

        [Fact(Skip = "https://xunit.github.io/docs/comparisons.html specifies this, but I don't see it and since they're the same, don't care.")]
        public void ConvertsAssertDoesNotContain() => Assert.True(false, "Method should be skipped");

        [Fact]
        public void ConvertsAssertInconclusive()
        {
            var actual = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Assert"), IdentifierName("Inconclusive")));
            actual = actual.RewriteInconclusive();

            Assert.Equal("//Not supported by xunitAssert.Inconclusive()", actual.ToFullString()); //This doesn't feel good, but I wanted to try it out anyway.
        }
    }
}
