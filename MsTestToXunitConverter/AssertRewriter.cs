using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MSBuildWorkspaceTest
{
    internal static class AssertRewriter
    {
        private static InvocationExpressionSyntax InvocationRewriter(this InvocationExpressionSyntax invocation, string from, string to)
        {
            if (invocation.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression &&
            invocation.Expression.ToString().Equals(from) &&
            invocation.Expression is MemberAccessExpressionSyntax mae)
            {
                return invocation.ReplaceNode(mae, mae.WithName(IdentifierName(to)));
            }

            return invocation;
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
            return invocation.InvocationRewriter("StringAssert.Contains", "Assert.Contains");
        }

        internal static InvocationExpressionSyntax RewriteDoesNotContain(this InvocationExpressionSyntax invocation)
        {
            return invocation; //Nothing special to do
        }

        internal static InvocationExpressionSyntax RewriteInconclusive(this InvocationExpressionSyntax invocation)
        {
            throw new NotImplementedException("This should comment out this line");
        }

        internal static InvocationExpressionSyntax RewriteIsFalse(this InvocationExpressionSyntax invocation)
        {
            return invocation.InvocationRewriter("Assert.IsFalse", "False");
        }

        internal static InvocationExpressionSyntax RewriteIsInstanceOfType(this InvocationExpressionSyntax invocation)
        {
            throw new NotImplementedException("Requires a rewrite to <> type format");
            return invocation.InvocationRewriter("", "");
        }

        internal static InvocationExpressionSyntax RewriteIsNotInstanceOfType(this InvocationExpressionSyntax invocation)
        {
            throw new NotImplementedException("Requires a rewrite to <> type format");
            return invocation.InvocationRewriter("", "");
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
