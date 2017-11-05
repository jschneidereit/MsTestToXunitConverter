using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MsTestToXunitConverter
{
    /// <summary>
    /// Implements the transformations described here: https://xunit.github.io/docs/comparisons.html
    /// </summary>
    internal static class AssertRewriter
    {
        private static Dictionary<string, string> AssertMappingDictionary = new Dictionary<string, string>
        {
            ["Assert.Fail"] = "True",
            ["Assert.Inconclusive"] = "Fail",
            ["Assert.AreEqual"] = "Equal",
            ["Assert.AreNotEqual"] = "NotEqual",
            ["Assert.AreNotSame"] = "NotSame",
            ["Assert.AreSame"] = "Same",
            ["Assert.IsFalse"] = "False",
            ["Assert."] = "",
            ["Assert."] = "",
            ["Assert."] = "",
            ["StringAssert.Contains"] = "",
        }

        private static InvocationExpressionSyntax InvocationRewriter(this InvocationExpressionSyntax invocation, string from, string to, IdentifierNameSyntax identifier = null, Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = null)
        {
            if (invocation.Expression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
            {
                return invocation;
            }

            if (!invocation.Expression.ToString().Equals(from))
            {
                return invocation;
            }

            if (invocation.Expression is MemberAccessExpressionSyntax mae)
            {
                if (identifier != null)
                {
                    identifier = identifier.WithLeadingTrivia(mae.Expression.GetLeadingTrivia());
                    invocation = invocation.ReplaceNode(mae, mae.WithExpression(identifier));
                }

                invocation = invocation.ReplaceNode(mae, mae.WithName(IdentifierName(to)));
            }

            if (func != null)
            {
                invocation = func(invocation);
            }

            return invocation;
        }

        internal static InvocationExpressionSyntax RewriteFail(this InvocationExpressionSyntax invocation)
        {
            Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = (i) =>
            {
                var args = i.ArgumentList.Arguments.Insert(0, Argument(ParseExpression("false")));
                return i.WithArgumentList(i.ArgumentList.WithArguments(args));
            };

            invocation = invocation.InvocationRewriter("Assert.Fail", "True", func: func);

            //TODO: Using the Formatter.Annotation like this seems like a smell, I'd rather make it a parameter to this function.
            return invocation.WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static ExpressionStatementSyntax RewriteAssert(this ExpressionStatementSyntax expression, SemanticModel model, string from, string to)
        {
            var invocation = expression.Expression as InvocationExpressionSyntax;

            var method = model?.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (method == null) { return expression; }

            var message = method.Parameters.FirstOrDefault(p => p.Name.Equals("message", StringComparison.OrdinalIgnoreCase));
            if (message == null) { return expression; }

            var parameter = invocation.ArgumentList.Arguments.ElementAt(message.Ordinal);
            var comment = Comment($"/*{parameter.Expression.ToFullString()}*/");

            expression = expression.InsertTriviaAfter(expression.SemicolonToken.TrailingTrivia.Last(), new[] { comment });
            expression = expression.ReplaceNode(invocation, invocation.WithArgumentList(invocation.ArgumentList.WithArguments(invocation.ArgumentList.Arguments.RemoveAt(message.Ordinal))));

            return expression;
        }

        internal static InvocationExpressionSyntax RewriteInconclusive(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.Inconclusive", "Fail").RewriteFail();
        }
        
        internal static InvocationExpressionSyntax RewriteAreEqual(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.AreEqual", "Equal");
        }

        internal static InvocationExpressionSyntax RewriteAreNotEqual(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.AreNotEqual", "NotEqual");
        }

        internal static InvocationExpressionSyntax RewriteAreNotSame(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.AreNotSame", "NotSame");
        }

        internal static InvocationExpressionSyntax RewriteAreSame(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.AreSame", "Same");
        }

        internal static InvocationExpressionSyntax RewriteIsFalse(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.IsFalse", "False");
        }

        internal static InvocationExpressionSyntax RewriteIsNotNull(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.IsNotNull", "NotNull");
        }

        internal static InvocationExpressionSyntax RewriteIsNull(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).InvocationRewriter("Assert.IsNull", "Null");
        }

        internal static InvocationExpressionSyntax RewriteIsTrue(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsTrue", "True");
        }

        #region Special Case Asserts
        internal static InvocationExpressionSyntax RewriteContains(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("StringAssert.Contains", "Assert.Contains", IdentifierName("Assert"));
        }

        internal static InvocationExpressionSyntax RewriteDoesNotContain(this InvocationExpressionSyntax invocation)
        {
            return invocation; //Nothing special to do
        }        
        
        private static InvocationExpressionSyntax RewriteOfType(this InvocationExpressionSyntax invocation, string from, string to)
        {
            Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = (i) =>
            {
                if (i.Expression is MemberAccessExpressionSyntax mae)
                {
                    //TypeOfExpression()
                    var oldArguments = i.ArgumentList.Arguments;

                    //I promise oldArgs.OfType<TypeOfExpressionSyntax>() does not work... but not sure why. TODO: be more elegant
                    var argument = oldArguments.First(t => t.ToString().StartsWith("typeof"));
                    var typeofExpr = argument?.Expression as TypeOfExpressionSyntax;
                    if (typeofExpr == null) { return i; }

                    var genericName = GenericName(
                        ParseToken(to),
                        TypeArgumentList(SingletonSeparatedList(typeofExpr.Type)));

                    var newArgs = i.ArgumentList.WithArguments(oldArguments.Remove(argument));
                    return i.WithExpression(mae.WithName(genericName)).WithArgumentList(newArgs);
                }

                return i;
            };

            return invocation.InvocationRewriter(from, to, func: func);
        }

        internal static InvocationExpressionSyntax RewriteIsInstanceOfType(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).RewriteOfType("Assert.IsInstanceOfType", "IsType");
        }

        internal static InvocationExpressionSyntax RewriteIsNotInstanceOfType(this InvocationExpressionSyntax invocation, SemanticModel model)
        {
            return invocation.RewriteMessage(model).RewriteOfType("Assert.IsNotInstanceOfType", "IsNotType");
        }
        #endregion



    }
}
