using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Drawio;

internal static class DrawioLayoutEngine
{
    public static IReadOnlyDictionary<string, DrawioNodeBounds> Layout(DiagramDocument document)
    {
        var nodeOrder = document.Nodes
            .Select((node, index) => new { node.Id, Index = index })
            .ToDictionary(item => item.Id, item => item.Index, StringComparer.OrdinalIgnoreCase);

        var depths = document.Nodes.ToDictionary(node => node.Id, _ => 0, StringComparer.OrdinalIgnoreCase);
        for (var pass = 0; pass < document.Nodes.Count; pass++)
        {
            var changed = false;
            foreach (var edge in document.Edges)
            {
                if (!depths.TryGetValue(edge.From, out var fromDepth) || !depths.ContainsKey(edge.To))
                {
                    continue;
                }

                var nextDepth = fromDepth + 1;
                if (nextDepth <= depths[edge.To])
                {
                    continue;
                }

                depths[edge.To] = nextDepth;
                changed = true;
            }

            if (!changed)
            {
                break;
            }
        }

        return document.Nodes
            .GroupBy(node => depths[node.Id])
            .SelectMany(layer =>
            {
                var orderedLayer = layer.OrderBy(node => nodeOrder[node.Id]).ToList();
                return orderedLayer.Select((node, index) =>
                {
                    var size = GetSize(node.Shape);
                    var x = 80 + index * (size.Width + document.Layout.NodeGap);
                    var y = 40 + layer.Key * (size.Height + document.Layout.LayerGap);
                    return new KeyValuePair<string, DrawioNodeBounds>(
                        node.Id,
                        new DrawioNodeBounds(x, y, size.Width, size.Height));
                });
            })
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);
    }

    private static DrawioSize GetSize(DiagramShape shape)
    {
        return shape switch
        {
            DiagramShape.Diamond => new DrawioSize(130, 80),
            DiagramShape.Ellipse => new DrawioSize(130, 60),
            DiagramShape.Cylinder => new DrawioSize(150, 70),
            DiagramShape.Swimlane => new DrawioSize(220, 120),
            _ => new DrawioSize(150, 60)
        };
    }
}

internal sealed record DrawioNodeBounds(double X, double Y, double Width, double Height);

internal sealed record DrawioSize(double Width, double Height);
