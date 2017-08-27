using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace MsTestToXunitConverter.xUnit
{
    public static class ResourceHelper
    {
        private static CompilationUnitSyntax GetCompilationUnit(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text, options: new CSharpParseOptions(kind: SourceCodeKind.Script));
            return tree.GetRoot() as CompilationUnitSyntax;
        }

        private static MethodDeclarationSyntax GetMethod(string text, string functionname)
        {
            var root = GetCompilationUnit(text);
            var methods = root.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>();
            return methods.Single(m => m.Identifier.ToString().Equals(functionname, System.StringComparison.OrdinalIgnoreCase));
        }

        private static ClassDeclarationSyntax GetClass(string text, string classname)
        {
            var root = GetCompilationUnit(text);
            var classes = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            return classes.Single(m => m.Identifier.ToString().Equals(classname, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// (Actual, Expected)
        /// </summary>
        internal static Tuple<MethodDeclarationSyntax, MethodDeclarationSyntax> GetTestMethod(string name)
        {
            var source = Properties.Resources.TestMethods;
            var result = Properties.Resources.TestMethods_out;

            return Tuple.Create(GetMethod(source, name), GetMethod(result, name));
        }

        /// <summary>
        /// (Actual, Expected)
        /// </summary>
        internal static Tuple<ClassDeclarationSyntax, ClassDeclarationSyntax> GetTestClass(string name)
        {
            var source = Properties.Resources.TestClasses;
            var result = Properties.Resources.TestClasses_out;

            return Tuple.Create(GetClass(source, name), GetClass(result, name));
        }
    }
}
