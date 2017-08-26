using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace MsTestToXunitConverter
{
    internal class Converter : CSharpSyntaxRewriter
    {
        private static ImmutableDictionary<string, AttributeSyntax> AttributeMapping = new Dictionary<string, AttributeSyntax>()
        {
            ["TestClass"] = null,
            ["TestMethod"] = Attribute(IdentifierName("Fact")),
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

        private MethodDeclarationSyntax StripExpectedExceptionAttribute(MethodDeclarationSyntax method)
        {
            var target = method.AttributeLists.SelectMany(al => al.Attributes).FirstOrDefault(a => a.Name == IdentifierName("ExpectedException"));
            if (target == null) { return method; }

            var newbody = $"Assert.Throws<{target.ArgumentList.Arguments.First()}>({ParenthesizedLambdaExpression(method.Body)});";
            if (target.ArgumentList.Arguments.Count > 1)
            {
                newbody = $"var ex = {newbody}";
                newbody += Environment.NewLine;
                newbody += $"Assert.Equal(\"{target.ArgumentList.Arguments.ElementAt(1)}\", ex.Message);";
            }

            method = method.ReplaceNode(method.Body, ParseExpression(newbody));
            method = method.RemoveNode(target, SyntaxRemoveOptions.KeepNoTrivia);
            return method;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax method)
        {
            method = StripExpectedExceptionAttribute(method);
            return base.VisitMethodDeclaration(method);
        }
    }
}
