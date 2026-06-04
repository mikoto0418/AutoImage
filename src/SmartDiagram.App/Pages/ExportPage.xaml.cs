using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SmartDiagram.App.Components;
using SmartDiagram.App.Workspace;
using SmartDiagram.Drawio;

namespace SmartDiagram.App.Pages;

public partial class ExportPage : UserControl
{
    private readonly WorkspaceDocumentStore _documents;

    public ExportPage(WorkspaceDocumentStore documents)
    {
        _documents = documents;
        InitializeComponent();
        PreviewHost.Content = DiagramPreviewFactory.Module();
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
            ? "功能模块图.drawio"
            : ExportFileNameTextBox.Text;
    }
}
