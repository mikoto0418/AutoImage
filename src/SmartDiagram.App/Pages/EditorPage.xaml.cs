using System.Windows;
using System.Windows.Controls;
using SmartDiagram.App.Components;
using SmartDiagram.Core.Samples;
using SmartDiagram.Core.Ui;

namespace SmartDiagram.App.Pages;

public partial class EditorPage : UserControl
{
    private readonly Action<AppScreen> _navigate;

    public EditorPage(Action<AppScreen> navigate)
    {
        _navigate = navigate;
        InitializeComponent();
        PreviewHost.Content = DiagramPreviewFactory.ChenEr();
        JsonBox.Text = SampleDiagramData.ChenEr.Json;
        NodeGrid.ItemsSource = SampleDiagramData.ChenEr.Nodes;
        EdgeGrid.ItemsSource = SampleDiagramData.ChenEr.Edges;
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.ImageReview);
    }

    private void JsonButton_Click(object sender, RoutedEventArgs e)
    {
        _navigate(AppScreen.JsonEditor);
    }
}
