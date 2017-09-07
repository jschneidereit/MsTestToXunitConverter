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
        internal const string MSTEST_USING = "Microsoft.VisualStudio.TestTools.UnitTesting";
        internal const string XUNIT_USING = "Xunit";

        private static AttributeArgumentListSyntax CreateArgumentList(string name, AttributeSyntax attribute, AttributeArgumentListSyntax other)
        {
            var argument = attribute.ArgumentList?.Arguments.FirstOrDefault();
            var expression = argument?.Expression?.ChildTokens().FirstOrDefault();

            var value = expression ?? Literal("");

            var attributeArgument = AttributeArgument(
                            AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(name),
                            LiteralExpression(SyntaxKind.StringLiteralExpression, value)));

            return other == null ? AttributeArgumentList(SeparatedList(new[] { attributeArgument })) : other.AddArguments(attributeArgument);
        }

        private static MethodDeclarationSyntax Cleanup(this MethodDeclarationSyntax method)
        {
            method = method.RemoveNodes(method.AttributeLists.Where(als => als.Attributes.Count == 0), SyntaxRemoveOptions.KeepExteriorTrivia);
            return method;
        }

        internal static ClassDeclarationSyntax Cleanup(this ClassDeclarationSyntax type)
        {
            type = type.RemoveNodes(type.AttributeLists.Where(als => als.Attributes.Count == 0), SyntaxRemoveOptions.KeepExteriorTrivia);
            foreach (var m in type.Members.OfType<MethodDeclarationSyntax>())
            {
                type = type.ReplaceNode(m, m.Cleanup());
            }

            return type;
        }

        private static AttributeSyntax GetTargetAttribute(this MethodDeclarationSyntax method, string target)
        {
            return method.AttributeLists.SelectMany(al => al.Attributes).SingleOrDefault(a => a.Name.ToString() == target);
        }

        internal static MethodDeclarationSyntax StripExpectedExceptionAttribute(this MethodDeclarationSyntax method, SyntaxAnnotation annotation)
        {
            var target = method.GetTargetAttribute("ExpectedException");
            if (target == null) { return method; }

            var arg = target.ArgumentList.Arguments.FirstOrDefault();
            var exceptionIdentifierName = arg?.ChildNodes().FirstOrDefault()?.ChildNodes().FirstOrDefault();

            if (exceptionIdentifierName == null) { return method; } //TODO: Throw exception? not valid MSTest if we get here

            var newbody = $"Assert.Throws<{exceptionIdentifierName}>({ParenthesizedLambdaExpression(method.Body)});";
            if (target.ArgumentList.Arguments.Count > 1)
            {
                newbody = $"var ex = {newbody}";
                newbody += Environment.NewLine;
                newbody += $"Assert.Equal(\"{target.ArgumentList.Arguments.ElementAt(1)}\", ex.Message);";
            }

            method = method.ReplaceNode(method.Body, Block(ParseStatement(newbody)).WithAdditionalAnnotations(annotation));

            //Refresh reference
            target = method.GetTargetAttribute("ExpectedException");
            method = method.RemoveNode(target, SyntaxRemoveOptions.KeepExteriorTrivia);

            return method.Cleanup();
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
            var attr = method.GetTargetAttribute("TestMethod");
            if (attr == null)
            {
                return method;
            }
            
            var factAttribute = attr.WithName(IdentifierName("Fact"));
            
            var description = method.GetTargetAttribute("Description");
            if (description != null)
            {
                factAttribute = factAttribute.WithArgumentList(CreateArgumentList("DisplayName", description, factAttribute.ArgumentList));
                method = method.RemoveNode(method.GetTargetAttribute("Description"), SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            var ignore = method.GetTargetAttribute("Ignore");
            if (ignore != null)
            {
                factAttribute = factAttribute.WithArgumentList(CreateArgumentList("Skip", ignore, factAttribute.ArgumentList));
                method = method.RemoveNode(method.GetTargetAttribute("Ignore"), SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            attr = method.GetTargetAttribute("TestMethod");
            method = method.ReplaceNode(attr, factAttribute);

            return method.Cleanup();
        }

        internal static ClassDeclarationSyntax StripTestInitializerAttribute(this ClassDeclarationSyntax type, SyntaxAnnotation annotation)
        {
            var target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestInitialize") != null);
            if (target == null) { return type; }

            var ctor = type.Members.OfType<ConstructorDeclarationSyntax>().SingleOrDefault(c => c.ParameterList.Parameters.Count == 0);
            
            var initializeStatement = ParseStatement($"{target.Identifier}();").WithAdditionalAnnotations(annotation);
            var replacementBody = ctor == null ? Block(initializeStatement) : Block(ctor.Body.Statements.Add(initializeStatement));
            var replacementCtor = ConstructorDeclaration(type.Identifier.WithoutTrivia()).WithBody(replacementBody).WithAdditionalAnnotations(annotation);

            type = ctor == null ? type.AddMembers(replacementCtor) : type.ReplaceNode(ctor, replacementCtor);

            //Refresh reference
            target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestInitialize") != null);
            type = type.ReplaceNode(target, target.RemoveNode(target.GetTargetAttribute("TestInitialize"), SyntaxRemoveOptions.KeepExteriorTrivia));

            return type.Cleanup();
        }

        internal static ClassDeclarationSyntax StripTestCleanupAttribute(this ClassDeclarationSyntax type, SyntaxAnnotation annotation)
        {
            BaseListSyntax CreateBaseList(string name, BaseListSyntax other)
            {
                if (other == null)
                {
                    return BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName(name))));
                }

                if (other.Types.Select(t => t.ToString()).Contains(name))
                {
                    return other;
                }

                return other.AddTypes(SimpleBaseType(IdentifierName(name)));
            }

            var target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestCleanup") != null);
            if (target == null) { return type; }

            var dispose = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.Identifier.ToString() == "Dispose");
            
            var cleanupStatement = ParseStatement($"{target.Identifier}();").WithAdditionalAnnotations(annotation);
            var replacementBody = dispose == null ? Block(cleanupStatement) : Block(dispose.Body.Statements.Insert(0, cleanupStatement));
            var replacementDisp = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "Dispose").WithBody(replacementBody).WithAdditionalAnnotations(annotation);

            if (dispose != null && dispose.Modifiers.Count > 0)
            {
                replacementDisp = replacementDisp.WithModifiers(dispose.Modifiers);
            }

            type = dispose == null ? type.AddMembers(replacementDisp) : type.ReplaceNode(dispose, replacementDisp);

            target = type.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.GetTargetAttribute("TestCleanup") != null);
            type = type.ReplaceNode(target, target.RemoveNode(target.GetTargetAttribute("TestCleanup"), SyntaxRemoveOptions.KeepExteriorTrivia));

            type = type.WithBaseList(CreateBaseList("IDisposable", type.BaseList));

            return type.Cleanup();
        }

        internal static UsingDirectiveSyntax ReplaceUsing(this UsingDirectiveSyntax node, string oldUsing, string newUsing)
        {
            if (node.Name.ToString().Equals(oldUsing, StringComparison.OrdinalIgnoreCase))
            {
                return node.WithName(IdentifierName(newUsing));
            }

            return node;
        }
    }
}
