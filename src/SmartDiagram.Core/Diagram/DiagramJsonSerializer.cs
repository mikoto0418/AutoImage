using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartDiagram.Core.Diagram;

public static class DiagramJsonSerializer
{
    public static DiagramDocument Parse(string text)
    {
        var json = ExtractJson(text);
        var model = JsonSerializer.Deserialize<DiagramModel>(json, JsonOptions)
            ?? throw new InvalidOperationException("Diagram JSON 为空。");

        return new DiagramDocument(
            model.DiagramId ?? string.Empty,
            ParseDiagramType(model.DiagramType),
            model.Title ?? string.Empty,
            DiagramStyle.PaperBlackWhite,
            DiagramLayout.AutoTopToBottom,
            model.Nodes.Select(ToNode).ToList(),
            model.Edges.Select(ToEdge).ToList());
    }

    public static string ToJson(DiagramDocument document)
    {
        var model = new DiagramModel
        {
            DiagramId = document.DiagramId,
            DiagramType = FormatDiagramType(document.DiagramType),
            Title = document.Title,
            Nodes = document.Nodes.Select(node => new NodeModel
            {
                Id = node.Id,
                Label = node.Label,
                Type = FormatNodeType(node.Type),
                Shape = FormatShape(node.Shape)
            }).ToList(),
            Edges = document.Edges.Select(edge => new EdgeModel
            {
                Id = edge.Id,
                From = edge.From,
                To = edge.To,
                Label = edge.Label,
                LineType = FormatLineType(edge.LineType),
                Arrow = FormatArrow(edge.Arrow)
            }).ToList()
        };

        return JsonSerializer.Serialize(model, JsonOptions);
    }

    private static string ExtractJson(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        var firstNewLine = trimmed.IndexOf('\n');
        var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        if (firstNewLine < 0 || lastFence <= firstNewLine)
        {
            return trimmed;
        }

        return trimmed[(firstNewLine + 1)..lastFence].Trim();
    }

    private static DiagramNode ToNode(NodeModel model)
    {
        return new DiagramNode(
            model.Id ?? string.Empty,
            model.Label ?? string.Empty,
            ParseNodeType(model.Type),
            ParseShape(model.Shape));
    }

    private static DiagramEdge ToEdge(EdgeModel model)
    {
        return new DiagramEdge(
            model.Id ?? string.Empty,
            model.From ?? string.Empty,
            model.To ?? string.Empty,
            model.Label,
            ParseLineType(model.LineType),
            ParseArrow(model.Arrow));
    }

    private static DiagramType ParseDiagramType(string? value)
    {
        return Normalize(value) switch
        {
            "flowchart" => DiagramType.Flowchart,
            "chener" => DiagramType.ChenEr,
            "chen_er" => DiagramType.ChenEr,
            "architecture" => DiagramType.Architecture,
            "hierarchy" => DiagramType.Hierarchy,
            "swimlane" => DiagramType.Swimlane,
            "dfd" => DiagramType.Dfd,
            "simpleclass" => DiagramType.SimpleClass,
            "simple_class" => DiagramType.SimpleClass,
            _ => DiagramType.FunctionModule
        };
    }

    private static DiagramNodeType ParseNodeType(string? value)
    {
        return Normalize(value) switch
        {
            "process" => DiagramNodeType.Process,
            "decision" => DiagramNodeType.Decision,
            "start" => DiagramNodeType.Start,
            "end" => DiagramNodeType.End,
            "entity" => DiagramNodeType.Entity,
            "relationship" => DiagramNodeType.Relationship,
            "attribute" => DiagramNodeType.Attribute,
            "actor" => DiagramNodeType.Actor,
            "database" => DiagramNodeType.Database,
            "class" => DiagramNodeType.Class,
            _ => DiagramNodeType.Module
        };
    }

    private static DiagramShape ParseShape(string? value)
    {
        return Normalize(value) switch
        {
            "roundedrectangle" => DiagramShape.RoundedRectangle,
            "rounded_rectangle" => DiagramShape.RoundedRectangle,
            "diamond" => DiagramShape.Diamond,
            "ellipse" => DiagramShape.Ellipse,
            "parallelogram" => DiagramShape.Parallelogram,
            "cylinder" => DiagramShape.Cylinder,
            "swimlane" => DiagramShape.Swimlane,
            _ => DiagramShape.Rectangle
        };
    }

    private static DiagramLineType ParseLineType(string? value)
    {
        return Normalize(value) == "orthogonal" ? DiagramLineType.Orthogonal : DiagramLineType.Straight;
    }

    private static DiagramArrow ParseArrow(string? value)
    {
        return Normalize(value) == "classic" ? DiagramArrow.Classic : DiagramArrow.None;
    }

    private static string FormatDiagramType(DiagramType value)
    {
        return value switch
        {
            DiagramType.Flowchart => "flowchart",
            DiagramType.ChenEr => "chen_er",
            DiagramType.Architecture => "architecture",
            DiagramType.Hierarchy => "hierarchy",
            DiagramType.Swimlane => "swimlane",
            DiagramType.Dfd => "dfd",
            DiagramType.SimpleClass => "simple_class",
            _ => "function_module"
        };
    }

    private static string FormatNodeType(DiagramNodeType value)
    {
        return value switch
        {
            DiagramNodeType.Process => "process",
            DiagramNodeType.Decision => "decision",
            DiagramNodeType.Start => "start",
            DiagramNodeType.End => "end",
            DiagramNodeType.Entity => "entity",
            DiagramNodeType.Relationship => "relationship",
            DiagramNodeType.Attribute => "attribute",
            DiagramNodeType.Actor => "actor",
            DiagramNodeType.Database => "database",
            DiagramNodeType.Class => "class",
            _ => "module"
        };
    }

    private static string FormatShape(DiagramShape value)
    {
        return value switch
        {
            DiagramShape.RoundedRectangle => "rounded_rectangle",
            DiagramShape.Diamond => "diamond",
            DiagramShape.Ellipse => "ellipse",
            DiagramShape.Parallelogram => "parallelogram",
            DiagramShape.Cylinder => "cylinder",
            DiagramShape.Swimlane => "swimlane",
            _ => "rectangle"
        };
    }

    private static string FormatLineType(DiagramLineType value)
    {
        return value == DiagramLineType.Orthogonal ? "orthogonal" : "straight";
    }

    private static string FormatArrow(DiagramArrow value)
    {
        return value == DiagramArrow.Classic ? "classic" : "none";
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().Replace("-", "_", StringComparison.Ordinal).ToLowerInvariant();
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class DiagramModel
    {
        [JsonPropertyName("diagram_id")]
        public string? DiagramId { get; set; }

        [JsonPropertyName("diagram_type")]
        public string? DiagramType { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("nodes")]
        public List<NodeModel> Nodes { get; set; } = [];

        [JsonPropertyName("edges")]
        public List<EdgeModel> Edges { get; set; } = [];
    }

    private sealed class NodeModel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("shape")]
        public string? Shape { get; set; }
    }

    private sealed class EdgeModel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("line_type")]
        public string? LineType { get; set; }

        [JsonPropertyName("arrow")]
        public string? Arrow { get; set; }
    }
}
