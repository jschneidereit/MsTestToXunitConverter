using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public static class FuncExtensions
    {

    }

    public class MethodAttributeTests
    {
        private SyntaxAnnotation Annotation { get; } = Formatter.Annotation;

        private Func<MethodDeclarationSyntax, MethodDeclarationSyntax> AddAnnotation(
            Func<MethodDeclarationSyntax, SyntaxAnnotation, MethodDeclarationSyntax> sut,
            SyntaxAnnotation annotation) => sut.Apply(annotation);

        private async Task ExecuteAsyncMethodTest(string name, Func<MethodDeclarationSyntax, MethodDeclarationSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetMethod(name);

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target));

            var document = pod.ActualDocument.WithSyntaxRoot(new_root);
            var result = await Formatter.FormatAsync(document, annotation: Annotation);

            Assert.Equal(result.ToString(), pod.ExpectedDocument.ToString());
        }

        [Fact(DisplayName = "ExpectedException - Gets converted to simple lambda")]
        public async Task ConvertSimpleExpectedException() => await ExecuteAsyncMethodTest("DoSomethingBad", AddAnnotation(Transformer.StripExpectedExceptionAttribute, Annotation));

        [Fact(DisplayName = "ExpectedException - Converted to a lambda with a multiline body")]
        public async Task ConvertComplexExpectedException() => await ExecuteAsyncMethodTest("DoSeveralThingsBad", AddAnnotation(Transformer.StripExpectedExceptionAttribute, Annotation));

        [Fact(DisplayName = "Converts TestMethod to fact")]
        public async Task ConvertTestMethod() => await ExecuteAsyncMethodTest("TestMethodA", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts ignore to Skip")]
        public async Task ConvertTestMethodIgnoreA() => await ExecuteAsyncMethodTest("IgnoreA", Transformer.StripSurjectiveFactAttributes);
        
        [Fact(DisplayName = "Converts ignore to Skip")]
        public async Task ConvertTestMethodIgnoreB() => await ExecuteAsyncMethodTest("IgnoreB", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts ignore to Skip")]
        public async Task ConvertTestMethodIgnoreC() => await ExecuteAsyncMethodTest("IgnoreC", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts ignore to Skip")]
        public async Task ConvertTestMethodIgnoreD() => await ExecuteAsyncMethodTest("IgnoreD", Transformer.StripSurjectiveFactAttributes);
        
        [Fact(DisplayName = "Converts description to DisplayName")]
        public async Task ConvertTestMethodDescriptionA() => await ExecuteAsyncMethodTest("DescriptionA", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts description to DisplayName")]
        public async Task ConvertTestMethodDescriptionB() => await ExecuteAsyncMethodTest("DescriptionB", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts description to DisplayName")]
        public async Task ConvertTestMethodDescriptionC() => await ExecuteAsyncMethodTest("DescriptionC", Transformer.StripSurjectiveFactAttributes);
        
        [Fact(DisplayName = "Converts all three attributes to one")]
        public async Task ConvertTestMethodIgnoreAndDescriptionB() => await ExecuteAsyncMethodTest("TestMethodB", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Converts all three attributes to one")]
        public async Task ConvertTestMethodIgnoreAndDescriptionC() => await ExecuteAsyncMethodTest("TestMethodB", Transformer.StripSurjectiveFactAttributes);

        [Fact(DisplayName = "Ignores methods without target attributes")]
        public async Task ConvertPassesOverNormalMethods() => await ExecuteAsyncMethodTest("TestMethodX", Transformer.StripSurjectiveFactAttributes);
    }
}
