using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SmartDiagram.App.Components;
using SmartDiagram.App.Workspace;
using SmartDiagram.Core.Diagram;
using SmartDiagram.Drawio;

namespace SmartDiagram.App.Pages;

public partial class ExportPage : UserControl
{
    private readonly WorkspaceDocumentStore _documents;

    public ExportPage(WorkspaceDocumentStore documents)
    {
        _documents = documents;
        InitializeComponent();
        RefreshPreview();
        ExportPathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "导出 draw.io 文件",
            Filter = "draw.io 文件 (*.drawio)|*.drawio",
            DefaultExt = ".drawio",
            AddExtension = true,
            OverwritePrompt = true,
            InitialDirectory = ResolveExportDirectory(),
            FileName = ResolveExportFileName()
        };

        if (dialog.ShowDialog() != true)
        {
            ExportStatusText.Text = "已取消导出";
            return;
        }

        try
        {
            var savedPath = DrawioFileExporter.Save(_documents.CurrentDocument, dialog.FileName);
            ExportPathTextBox.Text = Path.GetDirectoryName(savedPath) ?? string.Empty;
            ExportFileNameTextBox.Text = Path.GetFileNameWithoutExtension(savedPath);
            ExportStatusText.Text = $"已导出：{Path.GetFileName(savedPath)}";
        }
        catch (Exception ex)
        {
            ExportStatusText.Text = $"导出失败：{ex.Message}";
        }
    }

    private string ResolveExportDirectory()
    {
        return Directory.Exists(ExportPathTextBox.Text)
            ? ExportPathTextBox.Text
            : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private string ResolveExportFileName()
    {
        return string.IsNullOrWhiteSpace(ExportFileNameTextBox.Text)
            ? $"{_documents.CurrentDocument.Title}.drawio"
            : ExportFileNameTextBox.Text;
    }

    private void RefreshPreview()
    {
        var document = _documents.CurrentDocument;
        PreviewHost.ShowDocument(document);
        PreviewFileNameTextBox.Text = $"{document.Title}.drawio";
        PreviewDiagramTypeTextBox.Text = DisplayDiagramType(document.DiagramType);
        PreviewNodeCountTextBox.Text = document.Nodes.Count.ToString();
        PreviewEdgeCountTextBox.Text = document.Edges.Count.ToString();
        ExportFileNameTextBox.Text = document.Title;
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
