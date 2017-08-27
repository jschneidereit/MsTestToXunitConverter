using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using System.Collections.Generic;
using static MsTestToXunitConverter.Transformer;

namespace MsTestToXunitConverter
{
    internal class Converter : CSharpSyntaxRewriter
    {
        private static ImmutableDictionary<string, AttributeSyntax> AttributeMapping = new Dictionary<string, AttributeSyntax>()
        {
            ["TestClass"] = null,
            //["TestMethod"] = Attribute(IdentifierName("Fact")),
            ["TestProperty"] = Attribute(IdentifierName("Trait")),
            ["DataSource"] = Attribute(IdentifierName("Theory")),
        }.ToImmutableDictionary();

        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            if (AttributeMapping.TryGetValue(node.Name.ToString(), out var result))
            {
                return result;
            }

            //return the node instead of the base.VisitAttribute(node) cause we have nothing left to do
            return node;
        }
        
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            node = node.StripTestInitializerAttribute();
            node = node.StripTestCleanupAttribute();

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax method)
        {
            method = method.StripExpectedExceptionAttribute();

            return base.VisitMethodDeclaration(method);
        }
    }
}
