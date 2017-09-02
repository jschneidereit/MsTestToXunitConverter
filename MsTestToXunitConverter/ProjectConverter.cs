using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Threading.Tasks;

namespace MsTestToXunitConverter
{
    internal class ProjectConverter
    {
        internal static async Task ConvertProject(string projectFilePath)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectFilePath);
            foreach (var docId in project.DocumentIds)
            {
                var doc = project.GetDocument(docId);
                var converter = new TestClassRewriter(doc);
                var root = await doc.GetSyntaxRootAsync();
                var newRoot = converter.Visit(root);
                if (root != newRoot)
                {
                    project = doc
                        .WithSyntaxRoot(converter.Visit(root))
                        .Project;
                }
            }

            if (!workspace.TryApplyChanges(project.Solution))
            {
                Console.WriteLine("Failed to update project at " + projectFilePath);
            }
        }
    }
}
