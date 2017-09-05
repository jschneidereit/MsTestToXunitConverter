using Microsoft.CodeAnalysis;

namespace MsTestToXunitConverter.xUnit
{
    internal struct TestPod
    {
        internal Document ActualDocument { get; }

        internal SyntaxNode ActualRoot { get; }

        internal Document ExpectedDocument { get; }

        internal SyntaxNode ExpectedRoot { get; }

        internal TestPod(Document actualDocument, SyntaxNode actualRoot, Document expectedDocument, SyntaxNode expectedRoot)
        {
            ActualDocument = actualDocument;
            ActualRoot = actualRoot;

            ExpectedDocument = expectedDocument;
            ExpectedRoot = expectedRoot;
        }
    }
}
