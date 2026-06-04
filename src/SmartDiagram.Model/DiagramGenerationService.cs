using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Model;

public sealed class DiagramGenerationService(IChatCompletionClient client)
{
    public async Task<DiagramDocument> GenerateAsync(
        ModelClientSettings settings,
        DiagramGenerationRequest request,
        CancellationToken cancellationToken)
    {
        var messages = DiagramPromptBuilder.Build(request);
        var response = await client.CompleteAsync(settings, messages, cancellationToken);
        var document = request.CurrentDocument is null
            ? DiagramJsonSerializer.Parse(response)
            : DiagramEditOperationApplier.Apply(request.CurrentDocument, DiagramEditOperationSerializer.Parse(response));
        var validation = DiagramValidator.Validate(document);

        if (!validation.IsValid)
        {
            var message = string.Join("；", validation.Errors.Select(error => $"{error.Code}:{error.ElementId}"));
            throw new InvalidOperationException($"模型返回的图结构无效：{message}");
        }

        return document;
    }
}
