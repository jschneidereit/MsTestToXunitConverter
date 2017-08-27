using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Formatting;

namespace MsTestToXunitConverter
{
    internal static class Transformer
    {
        private static AttributeSyntax GetTargetAttribute(this MethodDeclarationSyntax method, string target)
        {
            return method.AttributeLists.SelectMany(al => al.Attributes).SingleOrDefault(a => a.Name.ToString() == target);
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
            return method.NormalizeWhitespace(elasticTrivia: true);
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

            AttributeArgumentListSyntax CreateArgumentList(string name, AttributeSyntax attribute, AttributeArgumentListSyntax other)
            {
                var value = attribute.ArgumentList.Arguments.FirstOrDefault()?.Expression.ChildTokens().FirstOrDefault().Value.ToString() ?? string.Empty;
                
                var argument = AttributeArgument(
                                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(name), 
                                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value))));
                
                return other == null ? AttributeArgumentList(SeparatedList(new[] { argument })) : other.AddArguments(argument);
            }

            if (description != null)
            {
                factAttribute = factAttribute.WithArgumentList(CreateArgumentList("DisplayName", description, factAttribute.ArgumentList));
                method = method.RemoveNode(method.GetTargetAttribute("Description"), SyntaxRemoveOptions.KeepNoTrivia);
            }
            
            if (ignore != null)
            {
                factAttribute = factAttribute.WithArgumentList(CreateArgumentList("Skip", ignore, factAttribute.ArgumentList));
                method = method.RemoveNode(method.GetTargetAttribute("Ignore"), SyntaxRemoveOptions.KeepNoTrivia);
            }
            
            if (testmethod != null)
            {
                method = method.RemoveNode(method.GetTargetAttribute("TestMethod"), SyntaxRemoveOptions.KeepNoTrivia);
            }

            var attributeList = AttributeList(SingletonSeparatedList(factAttribute));
            var syntaxList = method.AttributeLists.Add(attributeList);

            method = method.WithAttributeLists(syntaxList);
            method = method.RemoveNodes(method.AttributeLists.Where(als => als.Attributes.Count == 0), SyntaxRemoveOptions.KeepNoTrivia);
            
            return method.NormalizeWhitespace(elasticTrivia: true);
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

            return type.NormalizeWhitespace(elasticTrivia: true);
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

            return type.NormalizeWhitespace(elasticTrivia: true);
        }
    }
}
