using System.Globalization;
using System.Xml.Linq;
using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Drawio;

public static class DrawioXmlGenerator
{
    public static string Generate(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var validation = DiagramValidator.Validate(document);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.ElementId}"));
            throw new InvalidOperationException($"Cannot export invalid diagram: {message}");
        }

        var bounds = DrawioLayoutEngine.Layout(document);
        var root = new XElement("root",
            new XElement("mxCell", new XAttribute("id", "0")),
            new XElement("mxCell", new XAttribute("id", "1"), new XAttribute("parent", "0")));

        foreach (var node in document.Nodes)
        {
            root.Add(CreateNodeCell(node, document.Style, bounds[node.Id]));
        }

        foreach (var edge in document.Edges)
        {
            root.Add(CreateEdgeCell(edge, document.Style));
        }

        var mxGraphModel = new XElement("mxGraphModel",
            new XAttribute("dx", "1200"),
            new XAttribute("dy", "900"),
            new XAttribute("grid", "1"),
            new XAttribute("gridSize", "10"),
            new XAttribute("guides", "1"),
            new XAttribute("tooltips", "1"),
            new XAttribute("connect", "1"),
            new XAttribute("arrows", "1"),
            new XAttribute("fold", "1"),
            new XAttribute("page", "1"),
            new XAttribute("pageScale", "1"),
            new XAttribute("pageWidth", "1169"),
            new XAttribute("pageHeight", "827"),
            new XAttribute("math", "0"),
            new XAttribute("shadow", document.Style.Shadow ? "1" : "0"),
            root);

        var mxfile = new XElement("mxfile",
            new XAttribute("host", "app.diagrams.net"),
            new XElement("diagram",
                new XAttribute("id", document.DiagramId),
                new XAttribute("name", document.Title),
                mxGraphModel));

        return new XDocument(mxfile).ToString(SaveOptions.DisableFormatting);
    }

    private static XElement CreateNodeCell(DiagramNode node, DiagramStyle style, DrawioNodeBounds bounds)
    {
        return new XElement("mxCell",
            new XAttribute("id", node.Id),
            new XAttribute("value", node.Label),
            new XAttribute("style", DrawioStyleMapper.NodeStyle(node, style)),
            new XAttribute("vertex", "1"),
            new XAttribute("parent", "1"),
            new XElement("mxGeometry",
                new XAttribute("x", FormatNumber(bounds.X)),
                new XAttribute("y", FormatNumber(bounds.Y)),
                new XAttribute("width", FormatNumber(bounds.Width)),
                new XAttribute("height", FormatNumber(bounds.Height)),
                new XAttribute("as", "geometry")));
    }

    private static XElement CreateEdgeCell(DiagramEdge edge, DiagramStyle style)
    {
        return new XElement("mxCell",
            new XAttribute("id", edge.Id),
            new XAttribute("value", edge.Label ?? string.Empty),
            new XAttribute("style", DrawioStyleMapper.EdgeStyle(edge, style)),
            new XAttribute("edge", "1"),
            new XAttribute("parent", "1"),
            new XAttribute("source", edge.From),
            new XAttribute("target", edge.To),
            new XElement("mxGeometry",
                new XAttribute("relative", "1"),
                new XAttribute("as", "geometry")));
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
