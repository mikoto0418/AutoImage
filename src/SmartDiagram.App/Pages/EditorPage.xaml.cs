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
        RenderDocument(_documents.CurrentDocument, "示例模板已加载");
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.ImageReview);
    }

    private void JsonButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.JsonEditor);
    }

    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        if (_documents.Undo())
        {
            RenderDocument(_documents.CurrentDocument, "已撤销上一次增强");
        }
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        if (_documents.Redo())
        {
            RenderDocument(_documents.CurrentDocument, "已重做增强");
        }
    }

    private void RefreshPreviewButton_Click(object sender, RoutedEventArgs e)
    {
        RenderDocument(_documents.CurrentDocument, "预览已刷新");
    }

    private void ResetSampleButton_Click(object sender, RoutedEventArgs e)
    {
        _documents.ResetToSample();
        DiagramTypeComboBox.SelectedIndex = 0;
        RenderDocument(_documents.CurrentDocument, "示例模板已重置");
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
                DiagramGenerationRequest.ForEnhancement(ResolveDiagramKind(_documents.CurrentDocument.DiagramType), prompt, _documents.CurrentDocument),
                CancellationToken.None);

            _documents.ApplyDocument(document);
            RenderDocument(document, "已基于当前图增强");
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

    private static DiagramKind ResolveDiagramKind(DiagramType diagramType)
    {
        return diagramType switch
        {
            DiagramType.FunctionModule => DiagramKind.FunctionModule,
            DiagramType.Flowchart => DiagramKind.Flowchart,
            _ => DiagramKind.ChenEr
        };
    }

    private void RenderDocument(DiagramDocument document, string status)
    {
        PreviewHost.ShowDocument(document);
        JsonBox.Text = DiagramJsonSerializer.ToJson(document);
        NodeGrid.ItemsSource = DiagramTableAdapter.NodeRows(document).ToList();
        EdgeGrid.ItemsSource = DiagramTableAdapter.EdgeRows(document).ToList();
        PreviewTitleText.Text = document.Title;
        DiagramSummaryText.Text = $"{DisplayDiagramType(document.DiagramType)} · {document.DiagramId}";
        NodeCountText.Text = $"节点（{document.Nodes.Count}）";
        EdgeCountText.Text = $"连线（{document.Edges.Count}）";
        NodeCountBadgeText.Text = $"节点 {document.Nodes.Count}";
        EdgeCountBadgeText.Text = $"连线 {document.Edges.Count}";
        UndoButton.IsEnabled = _documents.CanUndo;
        RedoButton.IsEnabled = _documents.CanRedo;
        GenerationStatusText.Text = status;
    }

    private static string DisplayDiagramType(DiagramType diagramType)
    {
        return diagramType switch
        {
            DiagramType.ChenEr => "Chen ER 图",
            DiagramType.FunctionModule => "功能模块图",
            DiagramType.Flowchart => "流程图",
            DiagramType.Architecture => "系统架构图",
            DiagramType.Hierarchy => "层级结构图",
            DiagramType.Swimlane => "泳道图",
            DiagramType.Dfd => "数据流图",
            DiagramType.SimpleClass => "简易类图",
            _ => diagramType.ToString()
        };
    }
}
