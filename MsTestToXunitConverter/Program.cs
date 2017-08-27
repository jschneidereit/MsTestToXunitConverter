using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTestToXunitConverter
{
    public class Program
    {
        static async Task<Compilation> CreateCompilation(string projectFilePath)
        {
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
            var project = await workspace.OpenProjectAsync(projectFilePath);
            var compilation = await project.GetCompilationAsync();
            return compilation;
        }

        private static void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            throw new NotImplementedException();
        }

        static async Task RewriteCompilation(Compilation compilation)
        {
            foreach (var st in compilation.SyntaxTrees)
            {
                var root = await st.GetRootAsync();
                if (ShouldRewriteSyntaxTree(root))
                {
                    Console.WriteLine("Found a test class: " + st.FilePath);
                }
            }
        }

        static bool ShouldRewriteSyntaxTree(SyntaxNode root)
        {
            foreach (var cd in root.ChildNodes().OfType<ClassDeclarationSyntax>())
            {
                foreach (var attr in cd.AttributeLists.SelectMany(al => al.Attributes))
                {
                    if (attr.Name is IdentifierNameSyntax id &&
                        id.Identifier.ValueText == "TestClass")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static async Task ConvertMsTest(string projectFilePath)
        {
            var comp = await CreateCompilation(projectFilePath);
            await RewriteCompilation(comp);
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: mstestconvert <test-csproj-file>");
                return 1;
            }

            ConvertMsTest(args[0]).Wait();

            return 0;
        }
    }
}
