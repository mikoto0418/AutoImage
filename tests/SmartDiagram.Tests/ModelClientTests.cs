using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using SmartDiagram.Core.Diagram;
using SmartDiagram.Core.Samples;
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

    [Fact]
    public void DiagramPromptBuilder_includes_current_diagram_when_enhancing()
    {
        var request = DiagramGenerationRequest.ForEnhancement(
            DiagramKind.ChenEr,
            "给学生增加联系方式属性",
            SampleDiagramData.ChenErDocument);

        var userPrompt = DiagramPromptBuilder.Build(request).Single(message => message.Role == "user").Content;

        Assert.Contains("增强当前图", userPrompt);
        Assert.Contains("必须保留当前图中已有的节点和连线", userPrompt);
        Assert.Contains("\"diagram_id\": \"teaching_management_chen_er\"", userPrompt);
        Assert.Contains("\"label\": \"学生\"", userPrompt);
        Assert.Contains("给学生增加联系方式属性", userPrompt);
    }

    [Fact]
    public async Task DiagramGenerationService_applies_model_edit_operations_when_enhancing()
    {
        var client = new StubChatClient("""
        {
          "operations": [
            {
              "op": "add_node",
              "id": "student_phone",
              "label": "联系电话",
              "type": "attribute",
              "shape": "ellipse"
            },
            {
              "op": "add_edge",
              "id": "edge_student_phone",
              "from": "student",
              "to": "student_phone",
              "label": "拥有",
              "line_type": "straight",
              "arrow": "none"
            }
          ]
        }
        """);
        var service = new DiagramGenerationService(client);

        var document = await service.GenerateAsync(
            ModelClientSettings.CreateMissing(),
            DiagramGenerationRequest.ForEnhancement(
                DiagramKind.ChenEr,
                "给学生增加联系电话属性",
                SampleDiagramData.ChenErDocument),
            CancellationToken.None);

        Assert.Contains(document.Nodes, node => node.Id == "student");
        Assert.Contains(document.Nodes, node => node.Id == "student_phone" && node.Label == "联系电话");
        Assert.Contains(document.Edges, edge => edge.Id == "edge_student_phone" && edge.From == "student" && edge.To == "student_phone");
    }

    [Fact]
    public void DiagramPromptBuilder_requests_edit_operations_when_enhancing()
    {
        var request = DiagramGenerationRequest.ForEnhancement(
            DiagramKind.ChenEr,
            "给学生增加联系电话属性",
            SampleDiagramData.ChenErDocument);

        var userPrompt = DiagramPromptBuilder.Build(request).Single(message => message.Role == "user").Content;

        Assert.Contains("\"operations\"", userPrompt);
        Assert.Contains("add_node", userPrompt);
        Assert.Contains("update_node", userPrompt);
        Assert.Contains("delete_edge", userPrompt);
        Assert.Contains("不要返回完整 Diagram JSON", userPrompt);
    }

    [Fact]
    public async Task DiagramGenerationService_rejects_full_diagram_json_when_enhancing()
    {
        var client = new StubChatClient("""
        {
          "diagram_id": "module",
          "diagram_type": "function_module",
          "title": "系统模块",
          "nodes": [],
          "edges": []
        }
        """);
        var service = new DiagramGenerationService(client);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GenerateAsync(
            ModelClientSettings.CreateMissing(),
            DiagramGenerationRequest.ForEnhancement(
                DiagramKind.ChenEr,
                "给学生增加联系电话属性",
                SampleDiagramData.ChenErDocument),
            CancellationToken.None));

        Assert.Contains("operations", exception.Message);
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
