namespace SmartDiagram.Core.Diagram;

public static class DiagramTableAdapter
{
    public static IEnumerable<DiagramNodeRow> NodeRows(DiagramDocument document)
    {
        return document.Nodes.Select(node => new DiagramNodeRow(
            node.Id,
            node.Label,
            FormatNodeType(node.Type),
            FormatShape(node.Shape)));
    }

    public static IEnumerable<DiagramEdgeRow> EdgeRows(DiagramDocument document)
    {
        return document.Edges.Select(edge => new DiagramEdgeRow(
            edge.Id,
            edge.Label ?? string.Empty,
            edge.From,
            edge.To));
    }

    private static string FormatNodeType(DiagramNodeType value)
    {
        return value switch
        {
            DiagramNodeType.Process => "process",
            DiagramNodeType.Decision => "decision",
            DiagramNodeType.Start => "start",
            DiagramNodeType.End => "end",
            DiagramNodeType.Entity => "entity",
            DiagramNodeType.Relationship => "relationship",
            DiagramNodeType.Attribute => "attribute",
            DiagramNodeType.Actor => "actor",
            DiagramNodeType.Database => "database",
            DiagramNodeType.Class => "class",
            _ => "module"
        };
    }

    private static string FormatShape(DiagramShape value)
    {
        return value switch
        {
            DiagramShape.RoundedRectangle => "rounded_rectangle",
            DiagramShape.Diamond => "diamond",
            DiagramShape.Ellipse => "ellipse",
            DiagramShape.Parallelogram => "parallelogram",
            DiagramShape.Cylinder => "cylinder",
            DiagramShape.Swimlane => "swimlane",
            _ => "rectangle"
        };
    }
}

public sealed record DiagramNodeRow(string Id, string Label, string Type, string Shape);

public sealed record DiagramEdgeRow(string Id, string Label, string From, string To);
