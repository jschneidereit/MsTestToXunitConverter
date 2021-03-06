﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Formatting;
using System.Threading.Tasks;

namespace MsTestToXunitConverter
{
    /// <summary>
    /// Implements the transformations described here: https://xunit.github.io/docs/comparisons.html
    /// </summary>
    internal class TestClassRewriter : CSharpSyntaxRewriter
    {
        private readonly Document document;
        private readonly SyntaxAnnotation annotation = Formatter.Annotation;
        private readonly SemanticModel model;

        internal TestClassRewriter(Document doc)
        {
            document = doc;
            model = doc.GetSemanticModelAsync().Result;
        }

        public async Task<SyntaxNode> VisitSyntaxRoot(SyntaxNode node)
        {
            node = Visit(node);

            var formattedDocument = await Formatter.FormatAsync(document.WithSyntaxRoot(node), annotation: annotation);
            return await formattedDocument.GetSyntaxRootAsync();
        }

        private static ImmutableDictionary<string, AttributeSyntax> AttributeMapping = new Dictionary<string, AttributeSyntax>()
        {
            ["TestClass"] = null,
            ["TestProperty"] = Attribute(IdentifierName("Trait")),
            ["DataSource"] = Attribute(IdentifierName("Theory")),
        }.ToImmutableDictionary();

        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            //return the node instead of the base.VisitAttribute(node) cause we have nothing left to do
            return AttributeMapping.TryGetValue(node.Name.ToString(), out var result) ? result : node;
        }
        
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            node = node.StripTestInitializerAttribute(annotation);
            node = node.StripTestCleanupAttribute(annotation);

            node = (ClassDeclarationSyntax) base.VisitClassDeclaration(node);
            return node.Cleanup();
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax method)
        {
            method = method.StripSurjectiveFactAttributes(annotation);
            method = method.StripExpectedExceptionAttribute(annotation);

            return base.VisitMethodDeclaration(method);
        }

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            node = node.ReplaceUsing(oldUsing: Transformer.MSTEST_USING, newUsing: Transformer.XUNIT_USING);

            return base.VisitUsingDirective(node);
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            node = node.RewriteContains();
            node = node.RewriteDoesNotContain();
            
            return base.VisitExpressionStatement(node);
        }
    }
}
