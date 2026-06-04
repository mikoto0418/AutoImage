using SmartDiagram.Core.Samples;
using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Tests;

public sealed class SampleDiagramDataTests
{
    [Fact]
    public void ChenErSample_contains_reference_nodes_and_edges()
    {
        var sample = SampleDiagramData.ChenEr;

        Assert.Equal(7, sample.Nodes.Count);
        Assert.Equal(6, sample.Edges.Count);
        Assert.Contains(sample.Nodes, node => node.Id == "student" && node.Label == "学生");
        Assert.Contains(sample.Nodes, node => node.Id == "enrollment" && node.Shape == "diamond");
        Assert.Contains(sample.Edges, edge => edge.Id == "r4" && edge.Cardinality == "N:N");
    }

    [Fact]
    public void ChenErSample_exposes_json_for_the_json_editor()
    {
        var sample = SampleDiagramData.ChenEr;

        Assert.Contains("\"entities\"", sample.Json);
        Assert.Contains("\"relationships\"", sample.Json);
        Assert.Contains("\"classroom\"", sample.Json);
    }

    [Fact]
    public void ChenErDocument_matches_reference_sample_and_is_valid()
    {
        var document = SampleDiagramData.ChenErDocument;

        var result = DiagramValidator.Validate(document);

        Assert.True(result.IsValid);
        Assert.Equal(DiagramType.ChenEr, document.DiagramType);
        Assert.Equal(SampleDiagramData.ChenEr.Nodes.Count, document.Nodes.Count);
        Assert.Equal(SampleDiagramData.ChenEr.Edges.Count, document.Edges.Count);
        Assert.Contains(document.Nodes, node => node.Id == "enrollment" && node.Shape == DiagramShape.Diamond);
        Assert.All(document.Edges, edge => Assert.Equal(DiagramArrow.None, edge.Arrow));
    }

    [Fact]
    public void FunctionModuleDocument_is_valid_for_export()
    {
        var document = SampleDiagramData.FunctionModuleDocument;

        var result = DiagramValidator.Validate(document);

        Assert.True(result.IsValid);
        Assert.Equal(DiagramType.FunctionModule, document.DiagramType);
        Assert.Contains(document.Nodes, node => node.Id == "designer" && node.Label == "智能图表设计器");
        Assert.Contains(document.Edges, edge => edge.From == "designer" && edge.To == "export");
    }
}
