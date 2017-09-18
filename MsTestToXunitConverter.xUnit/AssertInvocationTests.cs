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

        private async Task ExecuteAsyncInvocationTest(string name, Func<InvocationExpressionSyntax, InvocationExpressionSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetInvocation();

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target));
            
            var actual = pod.ActualDocument.WithSyntaxRoot(new_root);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actual_string = (await actual.GetSyntaxRootAsync())?.GetInvocation().ToEnvNewlineString();
            var expect_string = (await expected.GetSyntaxRootAsync())?.GetInvocation().ToEnvNewlineString();

            Assert.Equal(expected: expect_string, actual: actual_string);
        }

        //TODO: if only Rikki would let me Curry in C# :P
        private async Task ExecuteAsyncInvocationTest(string name, Func<InvocationExpressionSyntax, SemanticModel, InvocationExpressionSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetInvocation();

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target, pod.ActualModel));

            target = pod.ActualRoot.GetInvocation();
            new_root = pod.ActualRoot.ReplaceNode(target, sut(target, pod.ActualModel));

            var actual = pod.ActualDocument.WithSyntaxRoot(new_root);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actual_string = (await actual.GetSyntaxRootAsync())?.GetInvocation().ToString(); //.ToEnvNewlineString();
            var expect_string = (await expected.GetSyntaxRootAsync())?.GetInvocation().ToString(); //.ToEnvNewlineString();

            Assert.Equal(expected: expect_string, actual: actual_string);
        }

        [Fact]
        public async Task ConvertsAssertFail() => await ExecuteAsyncInvocationTest("TestFail", AssertRewriter.RewriteFail);

        [Fact]
        public async Task ConvertsAreEqualMessage() => await ExecuteAsyncInvocationTest("TestAreEqualMessage", AssertRewriter.RewriteAreEqual);

        [Fact]
        public async Task ConvertsAreNotEqualMessage() => await ExecuteAsyncInvocationTest("TestAreNotEqualMessage", AssertRewriter.RewriteAreNotEqual);
        
        [Fact]
        public async Task ConvertsAreSameMessage() => await ExecuteAsyncInvocationTest("TestAreSameMessage", AssertRewriter.RewriteAreSame);

        [Fact]
        public async Task ConvertsAreNotSameMessage() => await ExecuteAsyncInvocationTest("TestAreNotSameMessage", AssertRewriter.RewriteAreNotSame);

        [Fact]
        public async Task ConvertsIsFalseMessage() => await ExecuteAsyncInvocationTest("TestIsFalseMessage", AssertRewriter.RewriteIsFalse);

        [Fact]
        public async Task ConvertsIsNotInstanceOfTypeMessage() => await ExecuteAsyncInvocationTest("TestIsNotInstanceOfTypeMessage", AssertRewriter.RewriteIsNotInstanceOfType);

        [Fact]
        public async Task ConvertsIsNotNullMessage() => await ExecuteAsyncInvocationTest("TestIsNotNullMessage", AssertRewriter.RewriteIsNotNull);
        
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

        [Fact]
        public async Task ConvertsAssertInconclusive() => await ExecuteAsyncInvocationTest("TestInconculsive", AssertRewriter.RewriteInconclusive);

        [Fact(Skip = "https://xunit.github.io/docs/comparisons.html specifies this, but I don't see it and since they're the same, don't care.")]
        public void ConvertsAssertDoesNotContain() => Assert.True(false, "Method should be skipped");
    }
}
