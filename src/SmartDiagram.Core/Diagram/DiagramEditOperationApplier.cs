namespace SmartDiagram.Core.Diagram;

public static class DiagramEditOperationApplier
{
    public static DiagramDocument Apply(DiagramDocument document, IReadOnlyList<DiagramEditOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(operations);

        var title = document.Title;
        var nodes = document.Nodes.ToList();
        var edges = document.Edges.ToList();

        foreach (var operation in operations)
        {
            switch (Normalize(operation.Operation))
            {
                case "set_title":
                    title = Require(operation.Title, "title");
                    break;
                case "add_node":
                    AddNode(nodes, operation);
                    break;
                case "update_node":
                    UpdateNode(nodes, operation);
                    break;
                case "rename_node":
                    RenameNode(nodes, operation);
                    break;
                case "delete_node":
                    DeleteNode(nodes, edges, operation);
                    break;
                case "add_edge":
                    AddEdge(nodes, edges, operation);
                    break;
                case "update_edge":
                    UpdateEdge(nodes, edges, operation);
                    break;
                case "delete_edge":
                    DeleteEdge(edges, operation);
                    break;
                default:
                    throw new InvalidOperationException($"不支持的图编辑操作：{operation.Operation}");
            }
        }

        return new DiagramDocument(document.DiagramId, document.DiagramType, title, document.Style, document.Layout, nodes, edges);
    }

    private static void AddNode(List<DiagramNode> nodes, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        EnsureNodeMissing(nodes, id);

        nodes.Add(new DiagramNode(
            id,
            Require(operation.Label, "label"),
            ParseNodeType(operation.Type),
            ParseShape(operation.Shape)));
    }

    private static void UpdateNode(List<DiagramNode> nodes, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        var index = FindNodeIndex(nodes, id);
        var current = nodes[index];

        nodes[index] = current with
        {
            Label = operation.Label ?? current.Label,
            Type = operation.Type is null ? current.Type : ParseNodeType(operation.Type),
            Shape = operation.Shape is null ? current.Shape : ParseShape(operation.Shape)
        };
    }

    private static void RenameNode(List<DiagramNode> nodes, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        var index = FindNodeIndex(nodes, id);
        nodes[index] = nodes[index] with { Label = Require(operation.Label, "label") };
    }

    private static void DeleteNode(List<DiagramNode> nodes, List<DiagramEdge> edges, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        var index = FindNodeIndex(nodes, id);
        if (edges.Any(edge => edge.From == id || edge.To == id))
        {
            throw new InvalidOperationException($"节点仍被连线引用，不能删除：{id}");
        }

        nodes.RemoveAt(index);
    }

    private static void AddEdge(List<DiagramNode> nodes, List<DiagramEdge> edges, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        EnsureEdgeMissing(edges, id);
        var from = Require(operation.From, "from");
        var to = Require(operation.To, "to");
        EnsureNodeExists(nodes, from);
        EnsureNodeExists(nodes, to);

        edges.Add(new DiagramEdge(
            id,
            from,
            to,
            operation.Label,
            ParseLineType(operation.LineType),
            ParseArrow(operation.Arrow)));
    }

    private static void UpdateEdge(List<DiagramNode> nodes, List<DiagramEdge> edges, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        var index = FindEdgeIndex(edges, id);
        var current = edges[index];
        var from = operation.From ?? current.From;
        var to = operation.To ?? current.To;
        EnsureNodeExists(nodes, from);
        EnsureNodeExists(nodes, to);

        edges[index] = current with
        {
            From = from,
            To = to,
            Label = operation.Label ?? current.Label,
            LineType = operation.LineType is null ? current.LineType : ParseLineType(operation.LineType),
            Arrow = operation.Arrow is null ? current.Arrow : ParseArrow(operation.Arrow)
        };
    }

    private static void DeleteEdge(List<DiagramEdge> edges, DiagramEditOperation operation)
    {
        var id = Require(operation.Id, "id");
        edges.RemoveAt(FindEdgeIndex(edges, id));
    }

    private static int FindNodeIndex(List<DiagramNode> nodes, string id)
    {
        var index = nodes.FindIndex(node => node.Id == id);
        if (index < 0)
        {
            throw new InvalidOperationException($"节点不存在：{id}");
        }

        return index;
    }

    private static int FindEdgeIndex(List<DiagramEdge> edges, string id)
    {
        var index = edges.FindIndex(edge => edge.Id == id);
        if (index < 0)
        {
            throw new InvalidOperationException($"连线不存在：{id}");
        }

        return index;
    }

    private static void EnsureNodeExists(List<DiagramNode> nodes, string id)
    {
        if (nodes.All(node => node.Id != id))
        {
            throw new InvalidOperationException($"节点不存在：{id}");
        }
    }

    private static void EnsureNodeMissing(List<DiagramNode> nodes, string id)
    {
        if (nodes.Any(node => node.Id == id))
        {
            throw new InvalidOperationException($"节点已存在：{id}");
        }
    }

    private static void EnsureEdgeMissing(List<DiagramEdge> edges, string id)
    {
        if (edges.Any(edge => edge.Id == id))
        {
            throw new InvalidOperationException($"连线已存在：{id}");
        }
    }

    private static string Require(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"图编辑操作缺少字段：{fieldName}");
        }

        return value.Trim();
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

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().Replace("-", "_", StringComparison.Ordinal).ToLowerInvariant();
    }
}
