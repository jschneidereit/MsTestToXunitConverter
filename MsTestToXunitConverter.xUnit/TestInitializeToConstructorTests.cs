using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class TestInitializeToConstructorTests
    {
        [Fact(DisplayName = "TestInitialize - converts to call to method from ctor")]
        public async Task ConvertTestInitialize()
        {
            const string targetname = "TestInitializeA";
            var tclass = ResourceHelper.GetTestClass(targetname);
            var doc = ResourceHelper.GetTestClasses();
            var annotation = new SyntaxAnnotation();

            var options = doc.Project.Solution.Workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.IndentBlock, true);

            var root = await doc.GetSyntaxRootAsync();
            var classes = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            var target = classes.Single(m => m.Identifier.ToString().Equals(targetname, System.StringComparison.OrdinalIgnoreCase));

            var actual = target.StripTestInitializerAttribute(annotation);
            root = root.ReplaceNode(target, actual);
            doc = doc.WithSyntaxRoot(root);

            var result = await Formatter.FormatAsync(doc, annotation: annotation);
            classes = (await result.GetSyntaxRootAsync()).DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            target = classes.Single(m => m.Identifier.ToString().Equals(targetname, System.StringComparison.OrdinalIgnoreCase));


            var actualstring = target.ToFullString();
            var expected = tclass.Item2.ToFullString();

            Assert.Equal(expected, actualstring);
        }

        [Fact(DisplayName = "TestInitialize - appends call to existing ctor")]
        public async Task AppendTestInitialize()
        {
            const string targetname = "TestInitializeB";
            var tclass = ResourceHelper.GetTestClass(targetname);
            var doc = ResourceHelper.GetTestClasses();
            var annotation = new SyntaxAnnotation();

            var options = doc.Project.Solution.Workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.IndentBlock, true);
            
            var root = await doc.GetSyntaxRootAsync();
            var classes = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            var target = classes.Single(m => m.Identifier.ToString().Equals(targetname, System.StringComparison.OrdinalIgnoreCase));
            
            var actual = target.StripTestInitializerAttribute(annotation);
            root = root.ReplaceNode(target, actual);
            doc = doc.WithSyntaxRoot(root);

            var result = await Formatter.FormatAsync(doc, annotation: annotation);
            classes = (await result.GetSyntaxRootAsync()).DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            target = classes.Single(m => m.Identifier.ToString().Equals(targetname, System.StringComparison.OrdinalIgnoreCase));


            var actualstring = target.ToFullString();
            var expected = tclass.Item2.ToFullString();

            Assert.Equal(expected, actualstring);
        }
    }
}
