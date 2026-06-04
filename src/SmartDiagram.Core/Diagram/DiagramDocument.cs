namespace SmartDiagram.Core.Diagram;

public sealed record DiagramDocument(
    string DiagramId,
    DiagramType DiagramType,
    string Title,
    DiagramStyle Style,
    DiagramLayout Layout,
    IReadOnlyList<DiagramNode> Nodes,
    IReadOnlyList<DiagramEdge> Edges)
{
    public static DiagramDocument Create(
        string diagramId,
        DiagramType diagramType,
        string title,
        IReadOnlyList<DiagramNode> nodes,
        IReadOnlyList<DiagramEdge> edges)
    {
        return new DiagramDocument(
            diagramId,
            diagramType,
            title,
            DiagramStyle.PaperBlackWhite,
            DiagramLayout.AutoTopToBottom,
            nodes,
            edges);
    }
}
