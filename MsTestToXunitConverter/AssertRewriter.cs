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
    /// 
    /// TODO rewrite to use:
    /// <see cref="Microsoft.CodeAnalysis.CSharp.SyntaxNodeExtensions">
    /// <![CDATA[public static TRoot ReplaceNodes<TRoot, TNode>(this TRoot root, IEnumerable<TNode> nodes, Func<TNode, TNode, SyntaxNode> computeReplacementNode)>]]>
    /// </see> 
    /// </summary>
    internal static class AssertRewriter
    {
        //TODO: rewrite to use SyntaxNodeExtensions. <se
        private static readonly Dictionary<string, string> AssertMapping = new Dictionary<string, string>
        {
            ["Assert.Fail"] = "True",
            ["Assert.Inconclusive"] = "Fail",
            ["Assert.AreEqual"] = "Equal",
            ["Assert.AreNotEqual"] = "NotEqual",
            ["Assert.AreNotSame"] = "NotSame",
            ["Assert.AreSame"] = "Same",
            ["Assert.IsFalse"] = "False",
            ["Assert.IsNotNull"] = "NotNull",
            ["Assert.IsNull"] = "Null",
            ["Assert.IsTrue"] = "True"
        };

        internal static ExpressionStatementSyntax RewriteMappedExpression(this ExpressionStatementSyntax expression,
            SemanticModel model)
        {
            if (!(expression.Expression is InvocationExpressionSyntax invocation))
            {
                return expression;
            }

            var from = invocation.Expression.ToString();

            if (AssertMapping.TryGetValue(from, out var to))
            {
                return expression.ExpressionRewriter(model: model, from: from, to: to);
            }
            else
            {
                return expression;
            }
        }
        

        private static ExpressionStatementSyntax ExpressionRewriter(
            this ExpressionStatementSyntax expression, SemanticModel model,
            string from = null, string to = null, IdentifierNameSyntax identifier = null,
            Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = null)
        {
            if (!(expression.Expression is InvocationExpressionSyntax invocation))
            {
                return expression;
            }

            if (invocation.Expression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
            {
                return expression;
            }

            if (!invocation.Expression.ToString().Equals(from, StringComparison.OrdinalIgnoreCase))
            {
                return expression;
            }

            if (invocation.Expression is MemberAccessExpressionSyntax mae)
            {
                if (identifier != null)
                {
                    identifier = identifier.WithLeadingTrivia(mae.Expression.GetLeadingTrivia());
                    expression = expression.ReplaceNode(expression.Expression,
                        expression.Expression.ReplaceNode(mae, mae.WithExpression(identifier)));
                }
                
                expression = expression.ReplaceNode(expression.Expression, invocation.ReplaceNode(mae, mae.WithName(IdentifierName(to))));
            }
            
            if (func != null)
            {
                expression = expression.ReplaceNode(expression.Expression, func((InvocationExpressionSyntax) expression.Expression));
            }
            
            expression = expression.RewriteAssertMessage(model);
            return expression;
        }

        private static ExpressionStatementSyntax RewriteAssertMessage(this ExpressionStatementSyntax expression, 
            SemanticModel model)
        {
            if (!(expression.Expression is InvocationExpressionSyntax invocation))
            {
                return expression;
            }

            model.TryGetSpeculativeSemanticModel(invocation.GetLocation())

            //TODO: the invocation has changed and so the semantic model isn't valid anymore
            if (!(model?.GetSymbolInfo(invocation).Symbol is IMethodSymbol method))
            {
                return expression;
            }

            var message = method.Parameters.FirstOrDefault(p => p.Name.Equals("message", StringComparison.OrdinalIgnoreCase));
            if (message == null)
            {
                return expression;
            }

            var parameter = invocation.ArgumentList.Arguments.ElementAt(message.Ordinal);
            var comment = Comment($"/*{parameter.Expression.ToFullString()}*/");

            expression = expression.InsertTriviaAfter(expression.SemicolonToken.TrailingTrivia.Last(), new[] {comment});
            expression = expression.ReplaceNode(invocation,
                invocation.WithArgumentList(
                    invocation.ArgumentList.WithArguments(
                        invocation.ArgumentList.Arguments.RemoveAt(message.Ordinal))));

            return expression;
        }

        #region Special Case Asserts
        
        internal static ExpressionStatementSyntax RewriteInconclusive(this ExpressionStatementSyntax expression)
        {
            return expression.ExpressionRewriter(model: null, from: "Assert.Inconclusive", to: "Fail").RewriteFail();
        }
        
        internal static ExpressionStatementSyntax RewriteFail(this ExpressionStatementSyntax expression)
        {
            InvocationExpressionSyntax Func(InvocationExpressionSyntax i)
            {
                var args = i.ArgumentList.Arguments.Insert(0, Argument(ParseExpression("false")));
                return i.WithArgumentList(i.ArgumentList.WithArguments(args));
            }

            expression = expression.ExpressionRewriter(model: null, from: "Assert.Fail", to: "True",
                func: (Func<InvocationExpressionSyntax, InvocationExpressionSyntax>) Func);

            //TODO: Using the Formatter.Annotation like this seems like a smell, I'd rather make it a parameter to this function.
            return expression.WithAdditionalAnnotations(Formatter.Annotation);
        }

        internal static ExpressionStatementSyntax RewriteContains(this ExpressionStatementSyntax expression)
        {
            return expression.ExpressionRewriter(model: null, from: "StringAssert.Contains", to: "Assert.Contains",
                identifier: IdentifierName("Assert"));
        }

        internal static ExpressionStatementSyntax RewriteDoesNotContain(this ExpressionStatementSyntax expression)
        {
            return expression; //Nothing special to do for now
        }

        private static ExpressionStatementSyntax RewriteOfType(this ExpressionStatementSyntax expression, string from,
            string to)
        {
            Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = (i) =>
            {
                if (!(i.Expression is MemberAccessExpressionSyntax mae)) return i;
                
                var oldArguments = i.ArgumentList.Arguments;

                //I promise oldArgs.OfType<TypeOfExpressionSyntax>() does not work... but not sure why. TODO: be more elegant
                var argument = oldArguments.First(t => t.ToString().StartsWith("typeof"));
                if (!(argument?.Expression is TypeOfExpressionSyntax typeofExpr))
                {
                    return i;
                }

                var genericName = GenericName(
                    ParseToken(to),
                    TypeArgumentList(SingletonSeparatedList(typeofExpr.Type)));

                var newArgs = i.ArgumentList.WithArguments(oldArguments.Remove(argument));
                return i.WithExpression(mae.WithName(genericName)).WithArgumentList(newArgs);
            };

            return expression.ExpressionRewriter(model: null, from: from, to: to, func: func);
        }

        internal static ExpressionStatementSyntax RewriteIsInstanceOfType(this ExpressionStatementSyntax expression,
            SemanticModel model)
        {
            return expression.RewriteOfType("Assert.IsInstanceOfType", "IsType");
        }

        internal static ExpressionStatementSyntax RewriteIsNotInstanceOfType(
            this ExpressionStatementSyntax expression, SemanticModel model)
        {
            return expression.RewriteOfType("Assert.IsNotInstanceOfType", "IsNotType");
        }

        #endregion
    }
}