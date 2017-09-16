using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using MsTestToXunitConverter.xUnit.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class InitializeAndCleanupTests
    {
        private SyntaxAnnotation Annotation { get; } = Formatter.Annotation;

        private async Task ExecuteAsyncClassTest(string name, Func<ClassDeclarationSyntax, SyntaxAnnotation, ClassDeclarationSyntax> sut)
        {
            var pod = ResourceHelper.GetTestPod(name);
            var target = pod.ActualRoot.GetClass(name);

            var new_root = pod.ActualRoot.ReplaceNode(target, sut(target, Annotation));

            var actual = pod.ActualDocument.WithSyntaxRoot(new_root);
            actual = await Formatter.FormatAsync(actual, annotation: Annotation);
            var expected = pod.ExpectedDocument;

            var actual_string = actual.GetSyntaxRootAsync().Result.GetClass(name).ToEnvNewlineString();
            var expect_string = expected.GetSyntaxRootAsync().Result.GetClass(name).ToEnvNewlineString();
            
            Assert.Equal(expected: expect_string, actual: actual_string);
        }

        [Fact(DisplayName = "TestInitialize - converts to call to method from ctor")]
        public async Task ConvertTestInitialize() => await ExecuteAsyncClassTest("ConstructorA", Transformer.StripTestInitializerAttribute);
        
        [Fact(DisplayName = "TestInitialize - appends call to existing ctor")]
        public async Task AppendTestInitialize() => await ExecuteAsyncClassTest("ConstructorB", Transformer.StripTestInitializerAttribute);
        
        [Fact(DisplayName = "TestCleanup - converts to call to method from Dispose")]
        public async Task ConvertTestCleanup() => await ExecuteAsyncClassTest("DisposeA", Transformer.StripTestCleanupAttribute);

        [Fact(DisplayName = "TestCleanup - prepends call to existing Dispose")]
        public async Task AppendtestCleanup() => await ExecuteAsyncClassTest("DisposeB", Transformer.StripTestCleanupAttribute);

        [Fact(DisplayName = "TestCleanup - properly preserves existing basetypes")]
        public async Task PreserveBaseTypes() => await ExecuteAsyncClassTest("DisposeC", Transformer.StripTestCleanupAttribute);
    }
}
