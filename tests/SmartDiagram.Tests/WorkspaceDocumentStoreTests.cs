using SmartDiagram.App.Workspace;
using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;

namespace SmartDiagram.Tests;

public sealed class WorkspaceDocumentStoreTests
{
    [Fact]
    public void ApplyDocument_records_history_for_undo_and_redo()
    {
        var store = new WorkspaceDocumentStore();
        var original = store.CurrentDocument;
        var changed = DiagramDocument.Create(
            "changed",
            DiagramType.ChenEr,
            "增强后的图",
            [.. original.Nodes, DiagramNode.Attribute("student_phone", "联系方式")],
            original.Edges);

        store.ApplyDocument(changed);

        Assert.Equal(changed, store.CurrentDocument);
        Assert.True(store.CanUndo);
        Assert.False(store.CanRedo);

        Assert.True(store.Undo());
        Assert.Equal(original, store.CurrentDocument);
        Assert.False(store.CanUndo);
        Assert.True(store.CanRedo);

        Assert.True(store.Redo());
        Assert.Equal(changed, store.CurrentDocument);
    }

    [Fact]
    public void ResetToSample_returns_to_debug_template_and_clears_history()
    {
        var store = new WorkspaceDocumentStore();
        store.ApplyDocument(DiagramDocument.Create(
            "changed",
            DiagramType.ChenEr,
            "增强后的图",
            [.. SampleDiagramData.ChenErDocument.Nodes, DiagramNode.Attribute("student_phone", "联系方式")],
            SampleDiagramData.ChenErDocument.Edges));

        store.ResetToSample();

        Assert.Equal(SampleDiagramData.ChenErDocument, store.CurrentDocument);
        Assert.False(store.CanUndo);
        Assert.False(store.CanRedo);
    }
}
