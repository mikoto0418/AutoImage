namespace SmartDiagram.Core.Diagram;

public sealed record DiagramNode(
    string Id,
    string Label,
    DiagramNodeType Type,
    DiagramShape Shape)
{
    public static DiagramNode Entity(string id, string label)
    {
        return new DiagramNode(id, label, DiagramNodeType.Entity, DiagramShape.Rectangle);
    }

    public static DiagramNode Relationship(string id, string label)
    {
        return new DiagramNode(id, label, DiagramNodeType.Relationship, DiagramShape.Diamond);
    }

    public static DiagramNode Attribute(string id, string label)
    {
        return new DiagramNode(id, label, DiagramNodeType.Attribute, DiagramShape.Ellipse);
    }
}
