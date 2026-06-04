using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartDiagram.Model;

public sealed class OpenAiCompatibleChatClient(HttpClient httpClient) : IChatCompletionClient
{
    public async Task<string> CompleteAsync(
        ModelClientSettings settings,
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        if (!settings.IsConfigured)
        {
            throw new InvalidOperationException("模型配置不完整，请先配置本地 API Key。");
        }

        httpClient.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.BaseUrl}/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        var payload = new
        {
            model = settings.Model,
            messages = messages.Select(message => new { role = message.Role, content = message.Content }).ToArray(),
            temperature = settings.Temperature,
            max_tokens = settings.MaxTokens,
            stream = false
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json");

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"模型请求超时，请检查网络或调大 timeoutSeconds（当前 {settings.TimeoutSeconds} 秒）。", ex);
        }

        using (response)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"模型请求失败：{(int)response.StatusCode} {response.ReasonPhrase}");
            }

            using var document = JsonDocument.Parse(body);
            return document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
