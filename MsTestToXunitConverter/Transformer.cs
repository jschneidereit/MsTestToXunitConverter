﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MsTestToXunitConverter
{
    internal static class Transformer
    {
        private static AttributeSyntax GetTargetAttribute(this MethodDeclarationSyntax method, string target)
        {
            return method.AttributeLists.SelectMany(al => al.Attributes).SingleOrDefault(a => a.Name == IdentifierName(target));
        }

        internal static MethodDeclarationSyntax StripExpectedExceptionAttribute(this MethodDeclarationSyntax method)
        {
            var target = method.GetTargetAttribute("ExpectedException");
            if (target == null) { return method; }

            var newbody = $"Assert.Throws<{target.ArgumentList.Arguments.First()}>({ParenthesizedLambdaExpression(method.Body)});";
            if (target.ArgumentList.Arguments.Count > 1)
            {
                newbody = $"var ex = {newbody}";
                newbody += Environment.NewLine;
                newbody += $"Assert.Equal(\"{target.ArgumentList.Arguments.ElementAt(1)}\", ex.Message);";
            }

            method = method.ReplaceNode(method.Body, ParseExpression(newbody));
            method = method.RemoveNode(target, SyntaxRemoveOptions.KeepNoTrivia); //TODO: determine what option I want here
            return method;
        }

        /// <summary>
        /// [TestMethod] -> [Fact]
        /// [Ignore("reason")] -> [Fact(Skip = "reason")]
        /// [Description("name")] -> [Fact(DisplayName = "name")]
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static MethodDeclarationSyntax StripSurjectiveFactAttributes(this MethodDeclarationSyntax method)
        {
            var testmethod = method.GetTargetAttribute("TestMethod");
            var ignore = method.GetTargetAttribute("Ignore");
            var description = method.GetTargetAttribute("Description");

            if (testmethod == null && ignore == null && description == null)
            {
                return method;
            }

            var factAttribute = Attribute(IdentifierName("Fact"));

            AttributeArgumentListSyntax CreateArgumentList(string name, string value)
            {
                var argument = AttributeArgument(
                                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(name), 
                                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value))));
                return AttributeArgumentList(SeparatedList(new[] { argument }));
            }

            if (description != null)
            {
                var value = description.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    factAttribute = factAttribute.WithArgumentList(CreateArgumentList("DisplayName", value));
                }
                
                method = method.RemoveNode(description, SyntaxRemoveOptions.KeepNoTrivia);
            }
            
            if (ignore != null)
            {
                var value = ignore.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    factAttribute = factAttribute.WithArgumentList(CreateArgumentList("Skip", value));
                }

                method = method.RemoveNode(ignore, SyntaxRemoveOptions.KeepNoTrivia);
            }
            
            if (testmethod != null)
            {
                return method.ReplaceNode(testmethod, factAttribute);
            }

            var attributeList = AttributeList(SingletonSeparatedList(factAttribute));
            var syntaxList = method.AttributeLists.Add(attributeList.NormalizeWhitespace());

            return method.WithAttributeLists(syntaxList);
        }

        internal static ClassDeclarationSyntax StripTestInitializerAttribute(this ClassDeclarationSyntax type)
        {
            var target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestInitialize") != null);
            if (target == null) { return type; }

            var ctor = type.Members.OfType<ConstructorDeclarationSyntax>().SingleOrDefault(c => c.ParameterList.Parameters.Count == 0);

            var initializeStatement = ParseStatement($"{target.Identifier}();");
            var replacementBody = ctor == null ? Block(initializeStatement) : Block(ctor.Body.Statements.Add(initializeStatement));
            var replacementCtor = ConstructorDeclaration(type.Identifier).WithBody(replacementBody);

            type = ctor == null ? type.AddMembers(replacementCtor) : type.ReplaceNode(ctor, replacementCtor);
            type = type.ReplaceNode(target, target.RemoveNode(target.GetTargetAttribute("TestInitialize"), SyntaxRemoveOptions.KeepNoTrivia));

            return type;
        }

        internal static ClassDeclarationSyntax StripTestCleanupAttribute(this ClassDeclarationSyntax type)
        {
            var target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestCleanup") != null);
            if (target == null) { return type; }

            var dispose = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.Identifier.ToString() == "Dispose");

            var cleanupStatement = ParseStatement($"{target.Identifier}();");
            var replacementBody = dispose == null ? Block(cleanupStatement) : Block(dispose.Body.Statements.Insert(0, cleanupStatement));
            var replacementDisp = MethodDeclaration(ParseName("void"), "Dispose").WithBody(replacementBody);
            
            type = dispose == null ? type.AddMembers(replacementDisp) : type.ReplaceNode(dispose, replacementDisp);
            type = type.ReplaceNode(target, target.RemoveNode(target.GetTargetAttribute("TestCleanup"), SyntaxRemoveOptions.KeepNoTrivia));

            return type;
        }
    }
}
