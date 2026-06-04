namespace SmartDiagram.Core.Diagram;

public sealed record DiagramStyle(
    string Theme,
    string Font,
    int FontSize,
    string LineColor,
    string FillColor,
    bool Shadow)
{
    public static DiagramStyle PaperBlackWhite { get; } =
        new("paper_black_white", "SimSun", 12, "#000000", "#FFFFFF", false);
}
