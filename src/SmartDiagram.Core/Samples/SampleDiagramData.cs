using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Core.Samples;

public static class SampleDiagramData
{
    public static DiagramDocument ChenErDocument { get; } = DiagramDocument.Create(
        "teaching_management_chen_er",
        DiagramType.ChenEr,
        "教学管理系统 Chen ER 图",
        [
            DiagramNode.Entity("dept", "院系"),
            DiagramNode.Entity("student", "学生"),
            DiagramNode.Entity("teacher", "教师"),
            DiagramNode.Entity("course", "课程"),
            DiagramNode.Relationship("enrollment", "选课"),
            DiagramNode.Entity("score", "成绩"),
            DiagramNode.Entity("classroom", "教室")
        ],
        [
            DiagramEdge.Line("r1", "student", "dept", "N:1"),
            DiagramEdge.Line("r2", "teacher", "dept", "N:1"),
            DiagramEdge.Line("r3", "teacher", "course", "1:N"),
            DiagramEdge.Line("r4", "student", "course", "N:N"),
            DiagramEdge.Line("r5", "student", "score", "N:1"),
            DiagramEdge.Line("r6", "course", "classroom", "N:1")
        ]);

    public static DiagramDocument FunctionModuleDocument { get; } = DiagramDocument.Create(
        "function_module_diagram",
        DiagramType.FunctionModule,
        "功能模块图",
        [
            new DiagramNode("designer", "智能图表设计器", DiagramNodeType.Module, DiagramShape.RoundedRectangle),
            new DiagramNode("project", "项目管理", DiagramNodeType.Module, DiagramShape.Rectangle),
            new DiagramNode("diagram", "图表设计", DiagramNodeType.Module, DiagramShape.Rectangle),
            new DiagramNode("data", "数据管理", DiagramNodeType.Module, DiagramShape.Rectangle),
            new DiagramNode("export", "导出管理", DiagramNodeType.Module, DiagramShape.Rectangle),
            new DiagramNode("settings", "系统设置", DiagramNodeType.Module, DiagramShape.Rectangle)
        ],
        [
            DiagramEdge.Line("edge_designer_project", "designer", "project"),
            DiagramEdge.Line("edge_designer_diagram", "designer", "diagram"),
            DiagramEdge.Line("edge_designer_data", "designer", "data"),
            DiagramEdge.Line("edge_designer_export", "designer", "export"),
            DiagramEdge.Line("edge_designer_settings", "designer", "settings")
        ]);

    public static SampleDiagram ChenEr { get; } = new(
        Nodes:
        [
            new("dept", "院系", "entity", "rectangle"),
            new("student", "学生", "entity", "rectangle"),
            new("teacher", "教师", "entity", "rectangle"),
            new("course", "课程", "entity", "rectangle"),
            new("enrollment", "选课", "relationship", "diamond"),
            new("score", "成绩", "entity", "rectangle"),
            new("classroom", "教室", "entity", "rectangle")
        ],
        Edges:
        [
            new("r1", "属于", "student", "dept", "N:1"),
            new("r2", "属于", "teacher", "dept", "N:1"),
            new("r3", "授课", "teacher", "course", "1:N"),
            new("r4", "选课", "student", "course", "N:N"),
            new("r5", "成绩", "student", "score", "N:1"),
            new("r6", "安排", "course", "classroom", "N:1")
        ],
        Json:
        """
        {
          "entities": [
            { "id": "dept", "name": "院系" },
            { "id": "student", "name": "学生" },
            { "id": "teacher", "name": "教师" },
            { "id": "course", "name": "课程" },
            { "id": "enrollment", "name": "选课" },
            { "id": "score", "name": "成绩" },
            { "id": "classroom", "name": "教室" }
          ],
          "relationships": [
            { "id": "r1", "name": "属于", "from": "student", "to": "dept", "cardinality": "N:1" },
            { "id": "r2", "name": "属于", "from": "teacher", "to": "dept", "cardinality": "N:1" },
            { "id": "r3", "name": "授课", "from": "teacher", "to": "course", "cardinality": "1:N" },
            { "id": "r4", "name": "选课", "from": "student", "to": "course", "cardinality": "N:N" },
            { "id": "r5", "name": "成绩", "from": "enrollment", "to": "score", "cardinality": "N:1" },
            { "id": "r6", "name": "安排", "from": "course", "to": "classroom", "cardinality": "N:1" }
          ]
        }
        """);
}
