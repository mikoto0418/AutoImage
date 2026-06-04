using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;

namespace SmartDiagram.App.Workspace;

public sealed class WorkspaceDocumentStore
{
    private readonly Stack<DiagramDocument> _undoStack = new();
    private readonly Stack<DiagramDocument> _redoStack = new();

    public DiagramDocument CurrentDocument { get; private set; } = SampleDiagramData.ChenErDocument;

    public bool CanUndo => _undoStack.Count > 0;

    public bool CanRedo => _redoStack.Count > 0;

    public void ApplyDocument(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _undoStack.Push(CurrentDocument);
        _redoStack.Clear();
        CurrentDocument = document;
    }

    public void ReplaceDocument(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        CurrentDocument = document;
    }

    public bool Undo()
    {
        if (!CanUndo)
        {
            return false;
        }

        _redoStack.Push(CurrentDocument);
        CurrentDocument = _undoStack.Pop();
        return true;
    }

    public bool Redo()
    {
        if (!CanRedo)
        {
            return false;
        }

        _undoStack.Push(CurrentDocument);
        CurrentDocument = _redoStack.Pop();
        return true;
    }

    public void ResetToSample()
    {
        CurrentDocument = SampleDiagramData.ChenErDocument;
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
