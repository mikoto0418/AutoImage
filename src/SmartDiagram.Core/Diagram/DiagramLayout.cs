namespace SmartDiagram.Core.Diagram;

public sealed record DiagramLayout(
    string Type,
    string Direction,
    int NodeGap,
    int LayerGap)
{
    public static DiagramLayout AutoTopToBottom { get; } =
        new("auto", "top_to_bottom", 80, 120);
}
