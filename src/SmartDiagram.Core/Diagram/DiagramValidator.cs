namespace SmartDiagram.Core.Diagram;

public static class DiagramValidator
{
    public static DiagramValidationResult Validate(DiagramDocument document)
    {
        var errors = new List<DiagramValidationError>();

        if (string.IsNullOrWhiteSpace(document.DiagramId))
        {
            errors.Add(new DiagramValidationError("diagram.id_required", "diagram", "图 ID 不能为空。"));
        }

        var nodeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in document.Nodes)
        {
            if (string.IsNullOrWhiteSpace(node.Id))
            {
                errors.Add(new DiagramValidationError("node.id_required", node.Label, "节点 ID 不能为空。"));
                continue;
            }

            if (!nodeIds.Add(node.Id))
            {
                errors.Add(new DiagramValidationError("node.id_duplicate", node.Id, "节点 ID 重复。"));
            }

            ValidateChenErNode(document, node, errors);
        }

        var edgeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var edge in document.Edges)
        {
            if (!edgeIds.Add(edge.Id))
            {
                errors.Add(new DiagramValidationError("edge.id_duplicate", edge.Id, "连线 ID 重复。"));
            }

            if (!nodeIds.Contains(edge.From))
            {
                errors.Add(new DiagramValidationError("edge.from_missing", edge.Id, "连线起点节点不存在。"));
            }

            if (!nodeIds.Contains(edge.To))
            {
                errors.Add(new DiagramValidationError("edge.to_missing", edge.Id, "连线终点节点不存在。"));
            }

            if (document.DiagramType == DiagramType.ChenEr && edge.Arrow != DiagramArrow.None)
            {
                errors.Add(new DiagramValidationError("chen_er.edge_arrow", edge.Id, "Chen ER 图连线默认不使用箭头。"));
            }
        }

        return new DiagramValidationResult(errors);
    }

    private static void ValidateChenErNode(
        DiagramDocument document,
        DiagramNode node,
        ICollection<DiagramValidationError> errors)
    {
        if (document.DiagramType != DiagramType.ChenEr)
        {
            return;
        }

        if (node.Type == DiagramNodeType.Entity && node.Shape != DiagramShape.Rectangle)
        {
            errors.Add(new DiagramValidationError("chen_er.entity_shape", node.Id, "Chen ER 实体必须使用矩形。"));
        }

        if (node.Type == DiagramNodeType.Relationship && node.Shape != DiagramShape.Diamond)
        {
            errors.Add(new DiagramValidationError("chen_er.relationship_shape", node.Id, "Chen ER 关系必须使用菱形。"));
        }

        if (node.Type == DiagramNodeType.Attribute && node.Shape != DiagramShape.Ellipse)
        {
            errors.Add(new DiagramValidationError("chen_er.attribute_shape", node.Id, "Chen ER 属性必须使用椭圆。"));
        }
    }
}
