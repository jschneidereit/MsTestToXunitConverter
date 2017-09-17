using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace MsTestToXunitConverter.xUnit
{
    internal static class ResourceHelper
    {
        private static ImmutableDictionary<string, string> Resources { get; } =
            Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true)
            .Cast<DictionaryEntry>()
            .ToImmutableDictionary(r => r.Key.ToString(), r => r.Value.ToString());

        private static string GetTestFile(string name) => Resources.First(t => t.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;

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

        internal static TestPod GetTestPod(string name)
        {
            var actualSrc = GetTestFile(name);
            var actualDoc = _baseProject.AddDocument(name: $"{name}.cs", text: actualSrc, filePath: $"{name}.cs");
            var actualRoot = actualDoc.GetSyntaxRootAsync().Result;

            var semanticModel = actualDoc.GetSemanticModelAsync().Result;

            var expectedSrc = GetTestFile($"{name}_out");
            var expectedDoc = _baseProject.AddDocument(name: $"{name}.out.cs", text: expectedSrc, filePath: $"{name}.out.cs");
            var expectedRoot = expectedDoc.GetSyntaxRootAsync().Result;

            return new TestPod(actualDocument: actualDoc, actualRoot: actualRoot, semanticModel: semanticModel, expectedDocument: expectedDoc, expectedRoot: expectedRoot);
        }

        internal static List<TestPod> GetTestPods() => Resources.Where(r => !r.Key.Contains("_out")).Select(r => GetTestPod(r.Key)).ToList();

        public static Func<TArg1, TResult> Apply<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func, TArg2 arg2) => arg1 => func(arg1, arg2);

        internal static InvocationExpressionSyntax GetInvocation(this SyntaxNode node)
        {
            return node.DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
        }

        internal static ClassDeclarationSyntax GetClass(this SyntaxNode node, string name)
        {
            return node.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        internal static MethodDeclarationSyntax GetMethod(this SyntaxNode node, string name)
        {
            return node.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
