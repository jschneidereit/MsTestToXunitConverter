﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MsTestToXunitConverter
{
    /// <summary>
    /// Implements the transformations described here: https://xunit.github.io/docs/comparisons.html
    /// </summary>
    internal static class AssertRewriter
    {
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

        internal static InvocationExpressionSyntax RewriteInconclusive(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.Inconclusive", "Fail").RewriteFail();
        }

        internal static InvocationExpressionSyntax RewriteAreEqual(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.AreEqual", "Equal");
        }

        internal static InvocationExpressionSyntax RewriteAreNotEqual(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.AreNotEqual", "NotEqual");
        }

        internal static InvocationExpressionSyntax RewriteAreSame(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.AreSame", "Same");
        }

        internal static InvocationExpressionSyntax RewriteContains(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("StringAssert.Contains", "Assert.Contains", IdentifierName("Assert"));
        }

        internal static InvocationExpressionSyntax RewriteDoesNotContain(this InvocationExpressionSyntax invocation)
        {
            return invocation; //Nothing special to do
        }

        internal static InvocationExpressionSyntax RewriteIsFalse(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsFalse", "False");
        }

        private static InvocationExpressionSyntax RewriteOfType(this InvocationExpressionSyntax invocation, string from, string to)
        {
            Func<InvocationExpressionSyntax, InvocationExpressionSyntax> func = (i) =>
            {
                if (i.Expression is MemberAccessExpressionSyntax mae)
                {
                    var oldArgs = i.ArgumentList.Arguments;
                    var typeArg = oldArgs.Last();
                    var typeofExpr = (TypeOfExpressionSyntax)typeArg.Expression;

                    var genericName = GenericName(
                        ParseToken(to),
                        TypeArgumentList(SingletonSeparatedList(typeofExpr.Type)));

                    var newArgs = i.ArgumentList.WithArguments(oldArgs.RemoveAt(oldArgs.Count - 1));

                    return i
                        .WithExpression(mae.WithName(genericName))
                        .WithArgumentList(newArgs);
                }

                return i;
            };

            return invocation.InvocationRewriter(from, to, func: func);
        }

        internal static InvocationExpressionSyntax RewriteIsInstanceOfType(this InvocationExpressionSyntax invocation)
        {
            return invocation.RewriteOfType("Assert.IsInstanceOfType", "IsType");
        }

        internal static InvocationExpressionSyntax RewriteIsNotInstanceOfType(this InvocationExpressionSyntax invocation)
        {
            return invocation.RewriteOfType("Assert.IsNotInstanceOfType", "IsNotType");
        }

        internal static InvocationExpressionSyntax RewriteIsNotNull(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsNotNull", "NotNull");
        }

        internal static InvocationExpressionSyntax RewriteIsNull(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsNull", "Null");
        }

        internal static InvocationExpressionSyntax RewriteIsTrue(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsTrue", "True");
        }
    }
}
