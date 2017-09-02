﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;

namespace MsTestToXunitConverter.xUnit
{
    internal static class ResourceHelper
    {

        private static readonly ImmutableArray<MetadataReference> _coreReferences = ImmutableArray.Create<MetadataReference>(
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Xunit.Assert).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestTools.UnitTesting.Assert).Assembly.Location));

        private static readonly Project _baseProject = new AdhocWorkspace()
            .AddProject("Test", LanguageNames.CSharp)
            .AddMetadataReferences(_coreReferences);

        internal static Document GetTestClasses()
        {
            return _baseProject.AddDocument(name: "TestClasses.cs", text: Properties.Resources.TestClasses, filePath: "Resources/TestClasses.cs");
        }

        private static CompilationUnitSyntax GetCompilationUnit(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text, options: new CSharpParseOptions(kind: SourceCodeKind.Regular));
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
        internal static Tuple<CompilationUnitSyntax, CompilationUnitSyntax> GetTestCompilations(string file)
        {
            //Kinda lame... but for now it'll have to do
            var source = Properties.Resources.TestClasses;
            var result = Properties.Resources.TestClasses_out;

            return Tuple.Create(GetCompilationUnit(source), GetCompilationUnit(result));
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

        /// <summary>
        /// (Actual, Expected)
        /// </summary>
        internal static Tuple<InvocationExpressionSyntax, InvocationExpressionSyntax> GetTestInvocation(string name)
        {
            var source = Properties.Resources.TestInvocations;
            var result = Properties.Resources.TestInvocations_out;

            var actual = GetMethod(source, name).DescendantNodes().OfType<InvocationExpressionSyntax>().First();
            var expected = GetMethod(result, name).DescendantNodes().OfType<InvocationExpressionSyntax>().First();

            return Tuple.Create(actual, expected);
        }
    }
}
