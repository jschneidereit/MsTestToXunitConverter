using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using MsTestToXunitConverter.xUnit.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MsTestToXunitConverter.xUnit
{
    public class AssertInvocationTests
    {
        private SyntaxAnnotation annotation { get; } = Formatter.Annotation;

        private async Task ExecuteAsyncInvocationTest(string name, Func<ExpressionStatementSyntax, ExpressionStatementSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetExpressionStatementSyntax();

            var newRoot = pod.ActualRoot.ReplaceNode(target, sut(target));
            
            var actual = pod.ActualDocument.WithSyntaxRoot(newRoot);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actualString = (await actual.GetSyntaxRootAsync())?.GetExpressionStatementSyntax().ToEnvNewlineString();
            var expectString = (await expected.GetSyntaxRootAsync())?.GetExpressionStatementSyntax().ToEnvNewlineString();

            Assert.Equal(expected: expectString, actual: actualString);
        }

        //TODO: if only Rikki would let me Curry in C# :P
        private async Task ExecuteAsyncInvocationTest(string name, Func<ExpressionStatementSyntax, SemanticModel, ExpressionStatementSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetExpressionStatementSyntax();
            var newRoot = pod.ActualRoot.ReplaceNode(target, sut(target, pod.ActualModel));

            var actual = pod.ActualDocument.WithSyntaxRoot(newRoot);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actualString = (await actual.GetSyntaxRootAsync())?.GetExpressionStatementSyntax().ToEnvNewlineString();
            var expectString = (await expected.GetSyntaxRootAsync())?.GetExpressionStatementSyntax().ToEnvNewlineString();

            Assert.Equal(expected: expectString, actual: actualString);
        }

        [Fact]
        public async Task ConvertsAssertFail() => await ExecuteAsyncInvocationTest("TestFail", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAreEqualMessage() => await ExecuteAsyncInvocationTest("TestAreEqualMessage", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAreNotEqualMessage() => await ExecuteAsyncInvocationTest("TestAreNotEqualMessage", AssertRewriter.RewriteMappedExpression);
        
        [Fact]
        public async Task ConvertsAreSameMessage() => await ExecuteAsyncInvocationTest("TestAreSameMessage", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAreNotSameMessage() => await ExecuteAsyncInvocationTest("TestAreNotSameMessage", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsIsFalseMessage() => await ExecuteAsyncInvocationTest("TestIsFalseMessage", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsIsNotInstanceOfTypeMessage() => await ExecuteAsyncInvocationTest("TestIsNotInstanceOfTypeMessage", AssertRewriter.RewriteIsNotInstanceOfType);

        [Fact]
        public async Task ConvertsIsNotNullMessage() => await ExecuteAsyncInvocationTest("TestIsNotNullMessage", AssertRewriter.RewriteMappedExpression);
        
        [Fact]
        public async Task ConvertsAssertAreEqual() => await ExecuteAsyncInvocationTest("TestAreEqual", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAssertAreNotEqual() => await ExecuteAsyncInvocationTest("TestAreNotEqual", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertAssertAreSame() => await ExecuteAsyncInvocationTest("TestAreSame", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAssertIsFalse() => await ExecuteAsyncInvocationTest("TestIsFalse", AssertRewriter.RewriteMappedExpression);
        
        [Fact]
        public async Task ConvertsAssertIsNotNull() => await ExecuteAsyncInvocationTest("TestIsNotNull", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAssertIsNull() => await ExecuteAsyncInvocationTest("TestIsNull", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAssertIsTrue() => await ExecuteAsyncInvocationTest("TestIsTrue", AssertRewriter.RewriteMappedExpression);

        [Fact]
        public async Task ConvertsAssertIsInstanceOfType() => await ExecuteAsyncInvocationTest("TestIsInstanceOfType", AssertRewriter.RewriteIsInstanceOfType);

        [Fact]
        public async Task CovnertAssertIsNotInstanceOfType() => await ExecuteAsyncInvocationTest("TestIsNotInstanceOfType", AssertRewriter.RewriteIsNotInstanceOfType);

        [Fact]
        public async Task ConvertsAssertContains() => await ExecuteAsyncInvocationTest("TestContains", AssertRewriter.RewriteContains);
        
        [Fact]
        public async Task ConvertsAssertInconclusive() => await ExecuteAsyncInvocationTest("TestInconculsive", AssertRewriter.RewriteInconclusive);

        [Fact(Skip = "https://xunit.github.io/docs/comparisons.html specifies this, but I don't see it and since they're the same, don't care.")]
        public void ConvertsAssertDoesNotContain() => Assert.True(false, "Method should be skipped");
    }
}
