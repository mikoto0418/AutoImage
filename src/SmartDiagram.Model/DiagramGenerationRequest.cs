namespace SmartDiagram.Model;

public enum DiagramKind
{
    FunctionModule,
    Flowchart,
    ChenEr
}

public sealed record DiagramGenerationRequest(
    DiagramKind DiagramKind,
    string Prompt)
{
    public static DiagramGenerationRequest ForText(DiagramKind diagramKind, string prompt)
    {
        return new DiagramGenerationRequest(diagramKind, prompt);
    }
}
