namespace SmartDiagram.Core.Diagram;

public sealed record DiagramEdge(
    string Id,
    string From,
    string To,
    string? Label,
    DiagramLineType LineType,
    DiagramArrow Arrow)
{
    public static DiagramEdge Line(string id, string from, string to, string? label = null)
    {
        return new DiagramEdge(id, from, to, label, DiagramLineType.Straight, DiagramArrow.None);
    }
}
