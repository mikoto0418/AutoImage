using System.Globalization;
using System.Xml.Linq;
using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;
using SmartDiagram.Drawio;

namespace SmartDiagram.Tests;

public sealed class DrawioXmlGeneratorTests
{
    [Fact]
    public void Generate_creates_drawio_xml_with_nodes_edges_and_styles()
    {
        var document = CreateFlowchartDocument();

        var xml = DrawioXmlGenerator.Generate(document);

        var mxfile = XDocument.Parse(xml).Root;
        Assert.NotNull(mxfile);
        Assert.Equal("mxfile", mxfile!.Name.LocalName);
        Assert.Equal("app.diagrams.net", mxfile.Attribute("host")?.Value);

        var diagram = Assert.Single(mxfile.Elements("diagram"));
        Assert.Equal("Student flow", diagram.Attribute("name")?.Value);

        var root = diagram.Element("mxGraphModel")?.Element("root");
        Assert.NotNull(root);

        var vertices = root!.Elements("mxCell")
            .Where(cell => cell.Attribute("vertex")?.Value == "1")
            .ToList();
        Assert.Equal(3, vertices.Count);

        var collect = vertices.Single(cell => cell.Attribute("id")?.Value == "collect");
        Assert.Equal("Collect request", collect.Attribute("value")?.Value);
        Assert.Contains("rounded=1", collect.Attribute("style")?.Value);
        Assert.Contains("fillColor=#FFFFFF", collect.Attribute("style")?.Value);
        Assert.Contains("strokeColor=#000000", collect.Attribute("style")?.Value);
        Assert.Contains("fontSize=12", collect.Attribute("style")?.Value);
        Assert.True(ReadGeometryNumber(collect, "x") >= 0);
        Assert.True(ReadGeometryNumber(collect, "y") >= 0);

        var approve = vertices.Single(cell => cell.Attribute("id")?.Value == "approve");
        Assert.Contains("rhombus", approve.Attribute("style")?.Value);

        var edge = root.Elements("mxCell").Single(cell => cell.Attribute("id")?.Value == "edge_collect_approve");
        Assert.Equal("1", edge.Attribute("edge")?.Value);
        Assert.Equal("collect", edge.Attribute("source")?.Value);
        Assert.Equal("approve", edge.Attribute("target")?.Value);
        Assert.Equal("yes", edge.Attribute("value")?.Value);
        Assert.Contains("edgeStyle=orthogonalEdgeStyle", edge.Attribute("style")?.Value);
        Assert.Contains("endArrow=classic", edge.Attribute("style")?.Value);
    }

    [Fact]
    public void Save_writes_drawio_file_and_adds_extension_when_missing()
    {
        var document = CreateFlowchartDocument();
        var directory = Path.Combine(Path.GetTempPath(), "smartdiagram-drawio-tests", Guid.NewGuid().ToString("N"));
        var requestedPath = Path.Combine(directory, "student-flow");

        var savedPath = DrawioFileExporter.Save(document, requestedPath);

        try
        {
            Assert.EndsWith(".drawio", savedPath, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(savedPath));

            var mxfile = XDocument.Load(savedPath).Root;
            Assert.NotNull(mxfile);
            Assert.Equal("mxfile", mxfile!.Name.LocalName);
            Assert.Equal("Student flow", mxfile.Element("diagram")?.Attribute("name")?.Value);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [Fact]
    public void Generate_preserves_chen_er_shapes_and_arrowless_edges()
    {
        var xml = DrawioXmlGenerator.Generate(SampleDiagramData.ChenErDocument);

        var root = XDocument.Parse(xml).Root?
            .Element("diagram")?
            .Element("mxGraphModel")?
            .Element("root");
        Assert.NotNull(root);

        var student = root!.Elements("mxCell").Single(cell => cell.Attribute("id")?.Value == "student");
        Assert.Equal("学生", student.Attribute("value")?.Value);
        Assert.Contains("rounded=0", student.Attribute("style")?.Value);

        var enrollment = root.Elements("mxCell").Single(cell => cell.Attribute("id")?.Value == "enrollment");
        Assert.Equal("选课", enrollment.Attribute("value")?.Value);
        Assert.Contains("rhombus", enrollment.Attribute("style")?.Value);

        var relationship = root.Elements("mxCell").Single(cell => cell.Attribute("id")?.Value == "r4");
        Assert.Equal("1", relationship.Attribute("edge")?.Value);
        Assert.Equal("N:N", relationship.Attribute("value")?.Value);
        Assert.Contains("endArrow=none", relationship.Attribute("style")?.Value);
    }

    private static DiagramDocument CreateFlowchartDocument()
    {
        return new DiagramDocument(
            "student_flow",
            DiagramType.Flowchart,
            "Student flow",
            DiagramStyle.PaperBlackWhite,
            DiagramLayout.AutoTopToBottom,
            [
                new DiagramNode("start", "Start", DiagramNodeType.Start, DiagramShape.Ellipse),
                new DiagramNode("collect", "Collect request", DiagramNodeType.Process, DiagramShape.RoundedRectangle),
                new DiagramNode("approve", "Approved?", DiagramNodeType.Decision, DiagramShape.Diamond)
            ],
            [
                new DiagramEdge("edge_start_collect", "start", "collect", null, DiagramLineType.Straight, DiagramArrow.None),
                new DiagramEdge("edge_collect_approve", "collect", "approve", "yes", DiagramLineType.Orthogonal, DiagramArrow.Classic)
            ]);
    }

    private static double ReadGeometryNumber(XElement cell, string attributeName)
    {
        var geometry = cell.Element("mxGeometry");
        Assert.NotNull(geometry);

        var value = geometry!.Attribute(attributeName)?.Value;
        Assert.NotNull(value);
        return double.Parse(value!, CultureInfo.InvariantCulture);
    }
}
