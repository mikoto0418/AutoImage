using System.Windows;
using System.Windows.Controls;
using SmartDiagram.Core.Diagram;
using SmartDiagram.Drawio;

namespace SmartDiagram.App.Components;

public partial class DrawioWebPreviewControl : UserControl
{
    private DiagramDocument? _pendingDocument;
    private bool _webViewReady;

    public DrawioWebPreviewControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public void ShowDocument(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        _pendingDocument = document;
        FallbackHost.Content = DiagramPreviewFactory.FromDocument(document);

        if (!_webViewReady)
        {
            return;
        }

        _ = NavigateAsync(document);
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        try
        {
            StatusText.Text = "正在启动 WebView2...";
            await Browser.EnsureCoreWebView2Async();
            Browser.NavigationCompleted += (_, args) =>
            {
                if (args.IsSuccess)
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                }
            };

            _webViewReady = true;
            WebHost.Visibility = Visibility.Visible;

            if (_pendingDocument is not null)
            {
                await NavigateAsync(_pendingDocument);
            }
        }
        catch (Exception ex)
        {
            _webViewReady = false;
            WebHost.Visibility = Visibility.Collapsed;
            FallbackHost.Content = BuildFallbackMessage(ex.Message, _pendingDocument);
        }
    }

    private async Task NavigateAsync(DiagramDocument document)
    {
        try
        {
            LoadingPanel.Visibility = Visibility.Visible;
            StatusText.Text = "正在加载 diagrams.net 预览...";
            var drawioXml = DrawioXmlGenerator.Generate(document);
            var html = DrawioPreviewHtmlBuilder.Build(drawioXml);
            Browser.NavigateToString(html);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            WebHost.Visibility = Visibility.Collapsed;
            FallbackHost.Content = BuildFallbackMessage(ex.Message, document);
        }
    }

    private static UIElement BuildFallbackMessage(string reason, DiagramDocument? document)
    {
        var root = new Grid();
        if (document is not null)
        {
            root.Children.Add(DiagramPreviewFactory.FromDocument(document));
        }

        root.Children.Add(new Border
        {
            Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F8FAFC")!,
            BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#E2E8F0")!,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(18),
            MaxWidth = 420,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = "WebView2 预览不可用",
                        FontSize = 16,
                        FontWeight = FontWeights.SemiBold,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 8)
                    },
                    new TextBlock
                    {
                        Text = "已降级为 WPF 简化预览；draw.io 导出仍可正常使用。",
                        Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#4B5563")!,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 8)
                    },
                    new TextBlock
                    {
                        Text = reason,
                        Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#991B1B")!,
                        FontSize = 12,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        });

        return root;
    }
}
