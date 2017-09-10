using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Threading.Tasks;

namespace MsTestToXunitConverter
{
    internal class ProjectConverter
    {
        internal static async Task ConvertProject(string projectFilePath, bool dryRun = false)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectFilePath);
            foreach (var docId in project.DocumentIds)
            {
                var doc = project.GetDocument(docId);
                var converter = new TestClassRewriter(doc);
                var root = await doc.GetSyntaxRootAsync();
                var newRoot = await converter.VisitSyntaxRoot(root);
                if (root != newRoot)
                {
                    var visited = converter.Visit(root);
                    foreach(var diag in visited.GetDiagnostics())
                    {
                        Console.Error.WriteLine(diag);
                    }

                    project = doc
                        .WithSyntaxRoot(visited)
                        .Project;
                }
            }

            if (!dryRun && !workspace.TryApplyChanges(project.Solution))
            {
                Console.Error.WriteLine("Failed to update project at " + projectFilePath);
            }
        }
    }
}
