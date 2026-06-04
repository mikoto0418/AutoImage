using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Model;

public static class DiagramPromptBuilder
{
    public static IReadOnlyList<ChatMessage> Build(DiagramGenerationRequest request)
    {
        return
        [
            new ChatMessage("system", SystemPrompt),
            new ChatMessage("user", BuildUserPrompt(request))
        ];
    }

    private static string BuildUserPrompt(DiagramGenerationRequest request)
    {
        if (request.CurrentDocument is not null)
        {
            return $"""
            图类型：{MapKind(request.DiagramKind)}

            任务：增强当前图。
            必须保留当前图中已有的节点和连线；只能根据用户描述增加、重命名、修正或补充必要元素。
            除非用户明确要求删除，否则不要删除任何已有节点或连线。
            输出必须是增强后的完整 Diagram JSON。

            当前 Diagram JSON：
            {DiagramJsonSerializer.ToJson(request.CurrentDocument)}

            用户增强描述：
            {request.Prompt}

            只返回 JSON，不要 Markdown，不要解释。
            """;
        }

        return $"""
        图类型：{MapKind(request.DiagramKind)}

        用户描述：
        {request.Prompt}

        只返回 JSON，不要 Markdown，不要解释。
        """;
    }

    private static string MapKind(DiagramKind kind)
    {
        return kind switch
        {
            DiagramKind.FunctionModule => "function_module",
            DiagramKind.Flowchart => "flowchart",
            DiagramKind.ChenEr => "chen_er",
            _ => "function_module"
        };
    }

    private const string SystemPrompt = """
    你是论文绘图工具的 Diagram JSON 生成器。必须把用户描述转换成严格 JSON。

    输出 schema：
    {
      "diagram_id": "snake_case_id",
      "diagram_type": "function_module | flowchart | chen_er",
      "title": "短标题",
      "nodes": [
        { "id": "node_id", "label": "节点文本", "type": "module | process | decision | start | end | entity | relationship | attribute", "shape": "rectangle | rounded_rectangle | diamond | ellipse" }
      ],
      "edges": [
        { "id": "edge_id", "from": "source_node_id", "to": "target_node_id", "label": "可选文本", "line_type": "straight | orthogonal", "arrow": "none | classic" }
      ]
    }

    规则：
    - id 必须稳定、唯一、只用英文、数字和下划线。
    - function_module 使用 module 节点，主节点可用 rounded_rectangle。
    - flowchart 可用 start、process、decision、end，流程边默认 classic 箭头。
    - chen_er 实体用 entity + rectangle，关系用 relationship + diamond，属性用 attribute + ellipse，边必须 arrow=none。
    - 不要输出 draw.io XML，不要输出 Visio 指令，不要输出 Markdown。
    """;
}
