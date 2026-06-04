namespace SmartDiagram.Core.Diagram;

public sealed record DiagramEditOperation(
    string Operation,
    string? Id = null,
    string? From = null,
    string? To = null,
    string? Label = null,
    string? Type = null,
    string? Shape = null,
    string? LineType = null,
    string? Arrow = null,
    string? Title = null,
    string? NewId = null);
