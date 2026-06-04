using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartDiagram.Core.Diagram;

public static class DiagramEditOperationSerializer
{
    public static IReadOnlyList<DiagramEditOperation> Parse(string text)
    {
        var json = ExtractJson(text);
        var model = JsonSerializer.Deserialize<OperationEnvelope>(json, JsonOptions)
            ?? throw new InvalidOperationException("模型返回的操作列表为空。");

        if (model.Operations.Count == 0)
        {
            throw new InvalidOperationException("模型增强响应必须包含非空 operations 数组。");
        }

        return model.Operations.Select(operation => new DiagramEditOperation(
            operation.Operation ?? string.Empty,
            operation.Id,
            operation.From,
            operation.To,
            operation.Label,
            operation.Type,
            operation.Shape,
            operation.LineType,
            operation.Arrow,
            operation.Title,
            operation.NewId)).ToList();
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

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed class OperationEnvelope
    {
        [JsonPropertyName("operations")]
        public List<OperationModel> Operations { get; set; } = [];
    }

    private sealed class OperationModel
    {
        [JsonPropertyName("op")]
        public string? Operation { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("shape")]
        public string? Shape { get; set; }

        [JsonPropertyName("line_type")]
        public string? LineType { get; set; }

        [JsonPropertyName("arrow")]
        public string? Arrow { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("new_id")]
        public string? NewId { get; set; }
    }
}
