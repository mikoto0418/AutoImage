using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;

namespace SmartDiagram.Tests;

public sealed class DiagramTableAdapterTests
{
    [Fact]
    public void NodeRows_maps_diagram_nodes_to_display_rows()
    {
        var rows = DiagramTableAdapter.NodeRows(SampleDiagramData.ChenErDocument).ToList();

        Assert.Equal(7, rows.Count);
        Assert.Contains(rows, row => row.Id == "student" && row.Label == "学生" && row.Type == "entity" && row.Shape == "rectangle");
        Assert.Contains(rows, row => row.Id == "enrollment" && row.Type == "relationship" && row.Shape == "diamond");
    }

    [Fact]
    public void EdgeRows_maps_diagram_edges_to_display_rows()
    {
        var rows = DiagramTableAdapter.EdgeRows(SampleDiagramData.ChenErDocument).ToList();

        Assert.Equal(6, rows.Count);
        Assert.Contains(rows, row => row.Id == "r4" && row.Label == "N:N" && row.From == "student" && row.To == "course");
    }
}
