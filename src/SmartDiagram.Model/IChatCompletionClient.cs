namespace SmartDiagram.Model;

public interface IChatCompletionClient
{
    Task<string> CompleteAsync(
        ModelClientSettings settings,
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken);
}
