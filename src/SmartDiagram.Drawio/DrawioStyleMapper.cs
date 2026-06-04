using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Drawio;

internal static class DrawioStyleMapper
{
    public static string NodeStyle(DiagramNode node, DiagramStyle style)
    {
        var shapeStyle = node.Shape switch
        {
            DiagramShape.Rectangle => "rounded=0;whiteSpace=wrap;html=1;",
            DiagramShape.RoundedRectangle => "rounded=1;whiteSpace=wrap;html=1;",
            DiagramShape.Diamond => "rhombus;whiteSpace=wrap;html=1;",
            DiagramShape.Ellipse => "ellipse;whiteSpace=wrap;html=1;",
            DiagramShape.Parallelogram => "shape=parallelogram;whiteSpace=wrap;html=1;",
            DiagramShape.Cylinder => "shape=cylinder3d;whiteSpace=wrap;html=1;boundedLbl=1;",
            DiagramShape.Swimlane => "swimlane;whiteSpace=wrap;html=1;",
            _ => "rounded=0;whiteSpace=wrap;html=1;"
        };

        return shapeStyle
            + $"fillColor={style.FillColor};"
            + $"strokeColor={style.LineColor};"
            + $"fontColor={style.LineColor};"
            + $"fontSize={style.FontSize};"
            + $"fontFamily={style.Font};"
            + $"shadow={(style.Shadow ? "1" : "0")};";
    }

    public static string EdgeStyle(DiagramEdge edge, DiagramStyle style)
    {
        var lineStyle = edge.LineType switch
        {
            DiagramLineType.Straight => "edgeStyle=none;html=1;",
            _ => "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;"
        };

        return lineStyle
            + $"endArrow={MapArrow(edge.Arrow)};"
            + $"strokeColor={style.LineColor};"
            + $"fontColor={style.LineColor};";
    }

    private static string MapArrow(DiagramArrow arrow)
    {
        return arrow switch
        {
            DiagramArrow.Classic => "classic",
            _ => "none"
        };
    }
}
