using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTestToXunitConverter.xUnit.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Converts a syntax node to a string, ensuring that the appropriate
        /// new line characters for the current environment are used.
        /// </summary>
        internal static string ToEnvNewlineString(this SyntaxNode node)
        {
            var content = node.ToFullString();
            if (Environment.NewLine == "\n")
            {
                content = content.Replace("\r\n", "\n");
            }
            return content;
        }
    }
}
