using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Tests;

public sealed class DiagramJsonSerializerTests
{
    [Fact]
    public void Parse_reads_fenced_model_json_as_diagram_document()
    {
        const string text = """
        ```json
        {
          "diagram_id": "student_flow",
          "diagram_type": "flowchart",
          "title": "学生申请流程",
          "nodes": [
            { "id": "start", "label": "开始", "type": "start", "shape": "ellipse" },
            { "id": "submit", "label": "提交申请", "type": "process", "shape": "rounded_rectangle" },
            { "id": "review", "label": "审核", "type": "decision", "shape": "diamond" }
          ],
          "edges": [
            { "id": "e1", "from": "start", "to": "submit", "line_type": "straight", "arrow": "classic" },
            { "id": "e2", "from": "submit", "to": "review", "label": "进入审核", "line_type": "orthogonal", "arrow": "classic" }
          ]
        }
        ```
        """;

        var document = DiagramJsonSerializer.Parse(text);

        Assert.Equal("student_flow", document.DiagramId);
        Assert.Equal(DiagramType.Flowchart, document.DiagramType);
        Assert.Equal("学生申请流程", document.Title);
        Assert.Equal(3, document.Nodes.Count);
        Assert.Equal(DiagramShape.RoundedRectangle, document.Nodes.Single(node => node.Id == "submit").Shape);
        Assert.Equal(DiagramLineType.Orthogonal, document.Edges.Single(edge => edge.Id == "e2").LineType);
    }

    [Fact]
    public void ToJson_writes_schema_names_expected_by_the_model()
    {
        var document = DiagramDocument.Create(
            "student_er",
            DiagramType.ChenEr,
            "学生选课",
            [
                DiagramNode.Entity("student", "学生"),
                DiagramNode.Relationship("enrollment", "选课")
            ],
            [DiagramEdge.Line("e1", "student", "enrollment", "N")]);

        var json = DiagramJsonSerializer.ToJson(document);

        Assert.Contains("\"diagram_id\"", json);
        Assert.Contains("\"diagram_type\": \"chen_er\"", json);
        Assert.Contains("\"shape\": \"diamond\"", json);
        Assert.Contains("\"arrow\": \"none\"", json);
    }
}
