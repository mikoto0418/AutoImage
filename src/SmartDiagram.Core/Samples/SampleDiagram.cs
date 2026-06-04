namespace SmartDiagram.Core.Samples;

public sealed record SampleDiagram(
    IReadOnlyList<SampleNode> Nodes,
    IReadOnlyList<SampleEdge> Edges,
    string Json);

public sealed record SampleNode(string Id, string Label, string Type, string Shape);

public sealed record SampleEdge(string Id, string Label, string From, string To, string Cardinality);
