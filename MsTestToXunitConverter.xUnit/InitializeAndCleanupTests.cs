using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
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

            var document = pod.ActualDocument.WithSyntaxRoot(new_root);
            var result = await Formatter.FormatAsync(document, annotation: Annotation);

            Assert.Equal(result.ToString(), pod.ExpectedDocument.ToString());
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
