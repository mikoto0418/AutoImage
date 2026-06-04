namespace SmartDiagram.Model;

public sealed record ModelClientSettings(
    string Provider,
    string BaseUrl,
    string Model,
    string ApiKey,
    int TimeoutSeconds,
    int MaxTokens,
    double Temperature)
{
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(BaseUrl)
        && !string.IsNullOrWhiteSpace(Model)
        && !string.IsNullOrWhiteSpace(ApiKey);

    public static ModelClientSettings CreateMissing()
    {
        return new ModelClientSettings(string.Empty, string.Empty, string.Empty, string.Empty, 30, 2048, 0.1);
    }
}
