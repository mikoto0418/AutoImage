using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;

namespace SmartDiagram.App.Workspace;

public sealed class WorkspaceDocumentStore
{
    public DiagramDocument CurrentDocument { get; private set; } = SampleDiagramData.ChenErDocument;

    public void SetCurrentDocument(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        CurrentDocument = document;
    }
}
