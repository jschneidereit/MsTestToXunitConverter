using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class MethodAttributeTests
    {
        private SyntaxAnnotation Annotation { get; } = Formatter.Annotation;

        private async Task ExecuteAsyncMethodTest(string name, Func<MethodDeclarationSyntax, SyntaxAnnotation, MethodDeclarationSyntax> sut, SyntaxAnnotation annotation)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetMethod(name);

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target, annotation));
            
            var actual = pod.ActualDocument.WithSyntaxRoot(new_root);
            actual = await Formatter.FormatAsync(actual, annotation: annotation);
            var expected = pod.ExpectedDocument;

            var actual_string = actual.GetSyntaxRootAsync().Result.GetMethod(name).ToString();
            var expect_string = expected.GetSyntaxRootAsync().Result.GetMethod(name).ToString();

            Assert.Equal(expected: expect_string, actual: actual_string);
        }

        [Fact(DisplayName = "ExpectedException - Gets converted to simple lambda")]
        public async Task ConvertSimpleExpectedException() => await ExecuteAsyncMethodTest("DoSomethingBad", Transformer.StripExpectedExceptionAttribute, Annotation);

        [Fact(DisplayName = "ExpectedException - Converted to a lambda with a multiline body")]
        public async Task ConvertComplexExpectedException() => await ExecuteAsyncMethodTest("DoSeveralThingsBad", Transformer.StripExpectedExceptionAttribute, Annotation);

        [Fact(DisplayName = "Converts TestMethod to fact")]
        public async Task ConvertTestMethod() => await ExecuteAsyncMethodTest("TestMethodA", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts ignore to Skip A")]
        public async Task ConvertTestMethodIgnoreA() => await ExecuteAsyncMethodTest("IgnoreA", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts ignore to Skip B")]
        public async Task ConvertTestMethodIgnoreB() => await ExecuteAsyncMethodTest("IgnoreB", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts ignore to Skip C")]
        public async Task ConvertTestMethodIgnoreC() => await ExecuteAsyncMethodTest("IgnoreC", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts ignore to Skip D")]
        public async Task ConvertTestMethodIgnoreD() => await ExecuteAsyncMethodTest("IgnoreD", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts description to DisplayName A")]
        public async Task ConvertTestMethodDescriptionA() => await ExecuteAsyncMethodTest("DescriptionA", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts description to DisplayName B")]
        public async Task ConvertTestMethodDescriptionB() => await ExecuteAsyncMethodTest("DescriptionB", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts description to DisplayName C")]
        public async Task ConvertTestMethodDescriptionC() => await ExecuteAsyncMethodTest("DescriptionC", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts all three attributes to one B")]
        public async Task ConvertTestMethodIgnoreAndDescriptionB() => await ExecuteAsyncMethodTest("TestMethodB", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Converts all three attributes to one C")]
        public async Task ConvertTestMethodIgnoreAndDescriptionC() => await ExecuteAsyncMethodTest("TestMethodC", Transformer.StripSurjectiveFactAttributes, Annotation);

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public async Task ConvertPassesOverNormalMethods() => await ExecuteAsyncMethodTest("TestMethodX", Transformer.StripSurjectiveFactAttributes, Annotation);
    }
}
