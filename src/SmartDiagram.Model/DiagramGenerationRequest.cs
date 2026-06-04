using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Model;

public enum DiagramKind
{
    FunctionModule,
    Flowchart,
    ChenEr
}

public sealed record DiagramGenerationRequest(
    DiagramKind DiagramKind,
    string Prompt,
    DiagramDocument? CurrentDocument = null)
{
    public static DiagramGenerationRequest ForText(DiagramKind diagramKind, string prompt)
    {
        return new DiagramGenerationRequest(diagramKind, prompt);
    }

    public static DiagramGenerationRequest ForEnhancement(
        DiagramKind diagramKind,
        string prompt,
        DiagramDocument currentDocument)
    {
        ArgumentNullException.ThrowIfNull(currentDocument);
        return new DiagramGenerationRequest(diagramKind, prompt, currentDocument);
    }
}
