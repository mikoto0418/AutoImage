using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Tests;

public sealed class DiagramEditOperationApplierTests
{
    [Fact]
    public void Apply_adds_node_and_edge_without_replacing_current_document()
    {
        var document = DiagramDocument.Create(
            "course_er",
            DiagramType.ChenEr,
            "Course ER",
            [DiagramNode.Entity("student", "Student")],
            []);
        var operations = DiagramEditOperationSerializer.Parse("""
        {
          "operations": [
            {
              "op": "add_node",
              "id": "student_phone",
              "label": "Phone",
              "type": "attribute",
              "shape": "ellipse"
            },
            {
              "op": "add_edge",
              "id": "edge_student_phone",
              "from": "student",
              "to": "student_phone",
              "label": "has",
              "line_type": "straight",
              "arrow": "none"
            }
          ]
        }
        """);

        var result = DiagramEditOperationApplier.Apply(document, operations);

        Assert.Single(document.Nodes);
        Assert.Contains(result.Nodes, node => node.Id == "student_phone" && node.Label == "Phone");
        Assert.Contains(result.Edges, edge => edge.Id == "edge_student_phone" && edge.From == "student" && edge.To == "student_phone");
        Assert.Equal(document.DiagramId, result.DiagramId);
        Assert.Equal(document.Title, result.Title);
    }

    [Fact]
    public void Apply_rejects_edge_that_references_missing_node()
    {
        var document = DiagramDocument.Create(
            "course_er",
            DiagramType.ChenEr,
            "Course ER",
            [DiagramNode.Entity("student", "Student")],
            []);
        var operations = DiagramEditOperationSerializer.Parse("""
        {
          "operations": [
            {
              "op": "add_edge",
              "id": "edge_missing",
              "from": "student",
              "to": "missing",
              "line_type": "straight",
              "arrow": "none"
            }
          ]
        }
        """);

        var exception = Assert.Throws<InvalidOperationException>(() => DiagramEditOperationApplier.Apply(document, operations));

        Assert.Contains("missing", exception.Message);
    }

    [Fact]
    public void Apply_updates_and_deletes_existing_elements()
    {
        var document = DiagramDocument.Create(
            "flow",
            DiagramType.Flowchart,
            "Old title",
            [
                new DiagramNode("start", "Start", DiagramNodeType.Start, DiagramShape.RoundedRectangle),
                new DiagramNode("review", "Review", DiagramNodeType.Process, DiagramShape.Rectangle)
            ],
            [new DiagramEdge("edge_start_review", "start", "review", null, DiagramLineType.Straight, DiagramArrow.Classic)]);
        var operations = DiagramEditOperationSerializer.Parse("""
        {
          "operations": [
            { "op": "set_title", "title": "Updated title" },
            { "op": "update_node", "id": "review", "label": "Manual review", "type": "process", "shape": "rounded_rectangle" },
            { "op": "delete_edge", "id": "edge_start_review" }
          ]
        }
        """);

        var result = DiagramEditOperationApplier.Apply(document, operations);

        Assert.Equal("Updated title", result.Title);
        Assert.Contains(result.Nodes, node => node.Id == "review" && node.Label == "Manual review" && node.Shape == DiagramShape.RoundedRectangle);
        Assert.Empty(result.Edges);
    }
}
