using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using SmartDiagram.Core.Diagram;
using SmartDiagram.App.Components;
using SmartDiagram.App.Workspace;
using SmartDiagram.Core.Samples;
using SmartDiagram.Core.Ui;
using SmartDiagram.Model;

namespace SmartDiagram.App.Pages;

public partial class EditorPage : UserControl
{
    private readonly Action<AppScreen> _navigate;
    private readonly WorkspaceDocumentStore _documents;

    public EditorPage(Action<AppScreen> navigate, WorkspaceDocumentStore documents)
    {
        _navigate = navigate;
        _documents = documents;
        InitializeComponent();
        ApplyDocument(_documents.CurrentDocument, "示例图已加载");
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.ImageReview);
    }

    private void JsonButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.JsonEditor);
    }

    private async void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var prompt = PromptTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(prompt))
        {
            GenerationStatusText.Text = "请输入图形描述。";
            return;
        }

        GenerateButton.IsEnabled = false;
        GenerationStatusText.Text = "正在调用本地模型...";

        try
        {
            using var httpClient = new HttpClient();
            var service = new DiagramGenerationService(new OpenAiCompatibleChatClient(httpClient));
            var document = await service.GenerateAsync(
                LocalModelSettingsLoader.LoadDefault(),
                DiagramGenerationRequest.ForText(ResolveDiagramKind(), prompt),
                CancellationToken.None);

            ApplyDocument(document, "生成完成");
        }
        catch (Exception ex)
        {
            GenerationStatusText.Text = $"生成失败：{ex.Message}";
        }
        finally
        {
            GenerateButton.IsEnabled = true;
        }
    }

    private DiagramKind ResolveDiagramKind()
    {
        return DiagramTypeComboBox.SelectedIndex switch
        {
            1 => DiagramKind.FunctionModule,
            2 => DiagramKind.Flowchart,
            _ => DiagramKind.ChenEr
        };
    }

    private void ApplyDocument(DiagramDocument document, string status)
    {
        _documents.SetCurrentDocument(document);
        PreviewHost.ShowDocument(document);
        JsonBox.Text = DiagramJsonSerializer.ToJson(document);
        NodeGrid.ItemsSource = DiagramTableAdapter.NodeRows(document).ToList();
        EdgeGrid.ItemsSource = DiagramTableAdapter.EdgeRows(document).ToList();
        NodeCountText.Text = $"节点（{document.Nodes.Count}）";
        EdgeCountText.Text = $"连线（{document.Edges.Count}）";
        GenerationStatusText.Text = status;
    }
}
