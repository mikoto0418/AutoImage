using System.Net;
using System.Text.Json;
using SmartDiagram.Model;

namespace SmartDiagram.Tests;

public sealed class ModelClientTests
{
    [Fact]
    public void LocalModelSettingsLoader_reads_settings_from_json_file()
    {
        var directory = Path.Combine(Path.GetTempPath(), "smartdiagram-model-tests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "model.local.json");
        Directory.CreateDirectory(directory);
        File.WriteAllText(path, """
        {
          "provider": "siliconflow",
          "baseUrl": "https://api.siliconflow.cn/v1",
          "model": "deepseek-ai/DeepSeek-V4-Pro",
          "apiKey": "test-key",
          "timeoutSeconds": 45,
          "maxTokens": 2048,
          "temperature": 0.1
        }
        """);

        try
        {
            var settings = LocalModelSettingsLoader.Load(path);

            Assert.Equal("siliconflow", settings.Provider);
            Assert.Equal("https://api.siliconflow.cn/v1", settings.BaseUrl);
            Assert.Equal("deepseek-ai/DeepSeek-V4-Pro", settings.Model);
            Assert.Equal("test-key", settings.ApiKey);
            Assert.Equal(45, settings.TimeoutSeconds);
            Assert.Equal(2048, settings.MaxTokens);
            Assert.Equal(0.1, settings.Temperature);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task OpenAiCompatibleChatClient_posts_chat_completion_request()
    {
        var handler = new CapturingHandler();
        using var httpClient = new HttpClient(handler);
        var client = new OpenAiCompatibleChatClient(httpClient);
        var settings = new ModelClientSettings(
            "siliconflow",
            "https://api.siliconflow.cn/v1",
            "deepseek-ai/DeepSeek-V4-Pro",
            "test-key",
            30,
            1024,
            0.2);

        var result = await client.CompleteAsync(settings, [new ChatMessage("user", "生成流程图")], CancellationToken.None);

        Assert.Equal("{}", result);
        Assert.Equal(HttpMethod.Post, handler.Request?.Method);
        Assert.Equal("https://api.siliconflow.cn/v1/chat/completions", handler.Request?.RequestUri?.ToString());
        Assert.Equal("Bearer", handler.Request?.Headers.Authorization?.Scheme);
        Assert.Equal("test-key", handler.Request?.Headers.Authorization?.Parameter);

        using var payload = JsonDocument.Parse(handler.RequestBody);
        Assert.Equal("deepseek-ai/DeepSeek-V4-Pro", payload.RootElement.GetProperty("model").GetString());
        Assert.False(payload.RootElement.GetProperty("stream").GetBoolean());
        Assert.Equal(1024, payload.RootElement.GetProperty("max_tokens").GetInt32());
    }

    [Fact]
    public async Task DiagramGenerationService_parses_model_json_into_document()
    {
        var client = new StubChatClient("""
        {
          "diagram_id": "module",
          "diagram_type": "function_module",
          "title": "系统模块",
          "nodes": [
            { "id": "root", "label": "系统", "type": "module", "shape": "rounded_rectangle" },
            { "id": "export", "label": "导出", "type": "module", "shape": "rectangle" }
          ],
          "edges": [
            { "id": "e1", "from": "root", "to": "export", "line_type": "straight", "arrow": "none" }
          ]
        }
        """);
        var service = new DiagramGenerationService(client);

        var document = await service.GenerateAsync(
            ModelClientSettings.CreateMissing(),
            DiagramGenerationRequest.ForText(DiagramKind.FunctionModule, "生成系统模块图"),
            CancellationToken.None);

        Assert.Equal("module", document.DiagramId);
        Assert.Equal("系统模块", document.Title);
        Assert.Contains(document.Nodes, node => node.Id == "export");
    }

    private sealed class StubChatClient(string response) : IChatCompletionClient
    {
        public Task<string> CompleteAsync(ModelClientSettings settings, IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken)
        {
            Assert.Contains(messages, message => message.Role == "system");
            Assert.Contains(messages, message => message.Role == "user");
            return Task.FromResult(response);
        }
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string RequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "choices": [
                    {
                      "message": {
                        "role": "assistant",
                        "content": "{}"
                      }
                    }
                  ]
                }
                """)
            };
        }
    }
}
