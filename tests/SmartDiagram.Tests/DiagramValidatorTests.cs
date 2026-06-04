using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Tests;

public sealed class DiagramValidatorTests
{
    [Fact]
    public void Validate_accepts_valid_chen_er_document()
    {
        var document = DiagramDocument.Create(
            "diagram_001",
            DiagramType.ChenEr,
            "学生选课系统 ER 图",
            [
                DiagramNode.Entity("student", "学生"),
                DiagramNode.Entity("course", "课程"),
                DiagramNode.Relationship("enrollment", "选课")
            ],
            [
                DiagramEdge.Line("edge_student_enrollment", "student", "enrollment", "N"),
                DiagramEdge.Line("edge_enrollment_course", "enrollment", "course", "M")
            ]);

        var result = DiagramValidator.Validate(document);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_rejects_edge_that_references_missing_node()
    {
        var document = DiagramDocument.Create(
            "diagram_001",
            DiagramType.ChenEr,
            "学生选课系统 ER 图",
            [DiagramNode.Entity("student", "学生")],
            [DiagramEdge.Line("edge_student_course", "student", "course", "N")]);

        var result = DiagramValidator.Validate(document);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Code == "edge.to_missing" && error.ElementId == "edge_student_course");
    }

    [Fact]
    public void Validate_rejects_chen_er_relationship_without_diamond_shape()
    {
        var document = DiagramDocument.Create(
            "diagram_001",
            DiagramType.ChenEr,
            "学生选课系统 ER 图",
            [
                DiagramNode.Entity("student", "学生"),
                new DiagramNode("enrollment", "选课", DiagramNodeType.Relationship, DiagramShape.Rectangle)
            ],
            [DiagramEdge.Line("edge_student_enrollment", "student", "enrollment", "N")]);

        var result = DiagramValidator.Validate(document);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Code == "chen_er.relationship_shape" && error.ElementId == "enrollment");
    }
}
