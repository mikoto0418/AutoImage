namespace SmartDiagram.Core.Diagram;

public sealed record DiagramValidationError(
    string Code,
    string ElementId,
    string Message);

public sealed record DiagramValidationResult(IReadOnlyList<DiagramValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;
}
