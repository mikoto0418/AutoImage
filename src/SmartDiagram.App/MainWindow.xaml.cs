using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using SmartDiagram.App.Components;
using SmartDiagram.App.Pages;
using SmartDiagram.App.Workspace;
using SmartDiagram.Core.Samples;
using SmartDiagram.Core.Ui;

namespace SmartDiagram.App;

public partial class MainWindow : Window
{
    private readonly Brush _panel = BrushFrom("#FFFFFF");
    private readonly Brush _canvas = BrushFrom("#FDFDFD");
    private readonly Brush _border = BrushFrom("#E2E8F0");
    private readonly Brush _muted = BrushFrom("#64748B");
    private readonly Brush _blue = BrushFrom("#0F6BFF");
    private readonly Brush _green = BrushFrom("#16A34A");
    private readonly Brush _amber = BrushFrom("#F59E0B");
    private readonly Brush _red = BrushFrom("#EF4444");
    private readonly WorkspaceDocumentStore _documents = new();

    public MainWindow()
    {
        InitializeComponent();
        ShowScreen(AppScreen.Editor);
    }

    private static SolidColorBrush BrushFrom(string color)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }

    private void ShowEditor_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Editor);
    private void ShowEmpty_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Empty);
    private void ShowExport_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Export);
    private void ShowSettings_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Settings);
    private void ShowVisio_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Visio);
    private void ShowVersions_Click(object sender, RoutedEventArgs e) => ShowScreen(AppScreen.Versions);

    private void ShowScreen(AppScreen screen)
    {
        ContentHost.Content = screen switch
        {
            AppScreen.Empty => BuildEmptyPage(),
            AppScreen.Export => new ExportPage(_documents),
            AppScreen.Settings => BuildSettingsPage(),
            AppScreen.Visio => BuildVisioPage(),
            AppScreen.JsonEditor => BuildJsonEditorPage(),
            AppScreen.Versions => BuildVersionsPage(),
            AppScreen.Modifications => BuildModificationPage(),
            AppScreen.ImageReview => BuildImageReviewPage(),
            AppScreen.ExportPreview => BuildExportPreviewPage(),
            _ => new EditorPage(ShowScreen, _documents)
        };
    }

    private UIElement BuildEditorPage()
    {
        var grid = ThreeColumnGrid(306, 1, 414);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildInputPanel(true)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("预览", BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr()))), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildStructurePanel()), 2);
        return grid;
    }

    private UIElement BuildEmptyPage()
    {
        var grid = ThreeColumnGrid(306, 1, 414);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildInputPanel(false)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("", BuildEmptyCanvas())), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildEmptyStructurePanel()), 2);
        return grid;
    }

    private UIElement BuildExportPage()
    {
        var grid = ThreeColumnGrid(278, 1, 280);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildPreviewInfoPanel()), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("", BuildPreviewWithToolbar(DiagramPreviewFactory.Module()))), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildExportSettingsPanel()), 2);
        return grid;
    }

    private UIElement BuildSettingsPage()
    {
        var grid = TwoColumnGrid(320, 1);
        Grid.SetColumn(grid.Children.AddAndReturn(SettingsNav()), 0);

        var body = new StackPanel { Margin = new Thickness(18) };
        body.Children.Add(SettingsSection("模型",
            FormRow("Base URL", Input("https://api.example.com/v1")),
            FormRow("API Key", Input("••••••••••••••••••••••")),
            FormRow("文本模型", SelectBox("gpt-4o-mini")),
            FormRow("多模态模型", SelectBox("gpt-4o")),
            FormRow("超时时间", SplitRow(Input("60"), SelectBox("秒"))),
            RightAlignedButton("▣  保存", true)));
        body.Children.Add(SettingsSection("导出",
            FormRow("默认导出目录", Input(@"C:\Users\Public\Documents\GraphDesigner\Exports", "▣")),
            FormRow("默认格式", SelectBox("PNG"))));
        body.Children.Add(SettingsSection("Visio",
            ToggleRow("自动检测", true),
            ToggleRow("同步前确认", true),
            ToggleRow("写入 Shape 属性", true)));

        Grid.SetColumn(grid.Children.AddAndReturn(new ScrollViewer { Content = body, VerticalScrollBarVisibility = ScrollBarVisibility.Auto }), 1);
        return grid;
    }

    private UIElement BuildVisioPage()
    {
        var grid = TwoColumnGrid(1, 500);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("图形预览", BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr()))), 0);

        var right = new StackPanel();
        right.Children.Add(Card("Visio 状态", Stack(
            KeyValue("状态", Badge("已连接", _green)),
            KeyValue("当前文档", Text("教学管理系统.vsdx", 15)),
            KeyValue("当前页面", Text("Page-5", 15)),
            KeyValue("Shape 映射数量", Text("42", 15)),
            PrimaryWideButton("⟳  同步"),
            WideButton("⟳  重新检测"),
            ToggleRow("清空页面", false)
        )));
        right.Children.Add(Spacer(10));
        right.Children.Add(Card("同步日志", DataTable(
            new[] { ("时间", "Time"), ("操作", "Action"), ("结果", "Result") },
            LogRows())));

        Grid.SetColumn(grid.Children.AddAndReturn(right), 1);
        return grid;
    }

    private UIElement BuildJsonEditorPage()
    {
        var grid = ThreeColumnGrid(460, 1, 420);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("图表预览", BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr(true)))), 0);

        var editor = new Grid();
        editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        editor.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        var header = new DockPanel { Margin = new Thickness(0, 0, 0, 10) };
        header.Children.Add(Text("JSON 编辑器", 18, FontWeights.SemiBold));
        var formatButton = SmallButton("格式化");
        DockPanel.SetDock(formatButton, Dock.Right);
        header.Children.Add(formatButton);
        Grid.SetRow(editor.Children.AddAndReturn(header), 0);
        Grid.SetRow(editor.Children.AddAndReturn(JsonBox()), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("", editor)), 1);

        var validation = Stack(
            ValidationSummary(),
            Text("节点属性", 15, FontWeights.SemiBold, new Thickness(0, 20, 0, 8)),
            DataTable(new[] { ("id", "Id"), ("label", "Label"), ("type", "Type"), ("shape", "Shape") }, SampleDiagramData.ChenEr.Nodes)
        );
        Grid.SetColumn(grid.Children.AddAndReturn(Card("验证结果", validation)), 2);
        return grid;
    }

    private UIElement BuildVersionsPage()
    {
        var grid = ThreeColumnGrid(430, 1, 360);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("版本列表", DataTable(
            new[] { ("版本号", "Version"), ("图类型", "Type"), ("操作摘要", "Summary"), ("时间", "Time") },
            VersionRows()))), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("当前版本预览（v1.0.8）", BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr()))), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("差异", Stack(
            DiffBlock("节点新增 (1)", "位置", "属性", "教室位置", _green),
            DiffBlock("节点删除 (0)", "无删除节点", "", "", _red),
            DiffBlock("样式修改 (2)", "位置", "椭圆 → 虚线椭圆", "标签：(无) → (新)", _blue)
        ))), 2);
        return grid;
    }

    private UIElement BuildModificationPage()
    {
        var grid = TwoColumnGrid(1, 430);
        var left = new Grid();
        left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetRow(left.Children.AddAndReturn(BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr(true))), 0);
        Grid.SetRow(left.Children.AddAndReturn(CommandInput()), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("图形预览", left)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(BuildOperationQueue()), 1);
        return grid;
    }

    private UIElement BuildImageReviewPage()
    {
        var outer = new Grid();
        outer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        outer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var top = new DockPanel { Margin = new Thickness(8, 0, 8, 12) };
        top.Children.Add(Text("‹  7 / 图片识别确认", 18, FontWeights.SemiBold));
        var detect = SmallButton("⟳  重新识别");
        DockPanel.SetDock(detect, Dock.Right);
        top.Children.Add(detect);
        Grid.SetRow(outer.Children.AddAndReturn(top), 0);

        var grid = ThreeColumnGrid(1, 1, 410);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("上传预览", DiagramPreviewFactory.PaperPreview())), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("可编辑预览", BuildPreviewWithToolbar(DiagramPreviewFactory.ChenEr(true)))), 1);
        Grid.SetColumn(grid.Children.AddAndReturn(Card("识别结果", Stack(
            DataTable(new[] { ("节点", "Id"), ("类型", "Type"), ("文本", "Text"), ("置信度", "Score") }, RecognitionRows()),
            KeyValue("节点 25 个", Badge("整体置信度 0.98", _blue))
        ))), 2);
        Grid.SetRow(outer.Children.AddAndReturn(grid), 1);
        return outer;
    }

    private UIElement BuildExportPreviewPage()
    {
        return BuildExportPage();
    }

    private Border BuildInputPanel(bool filled)
    {
        var input = new StackPanel { Margin = new Thickness(14) };
        input.Children.Add(Label("图类型"));
        input.Children.Add(SelectBox(filled ? "Chen ER 图" : "选择图类型"));
        input.Children.Add(Separator());
        input.Children.Add(Label("输入"));
        input.Children.Add(TextArea(filled
            ? "教学管理系统的实体关系图，包含院系、学生、教师、课程、成绩、教室等实体及其关系。"
            : ""));
        input.Children.Add(Text(filled ? "38/2000" : "0/2000", 13, FontWeights.Normal, new Thickness(0, 6, 0, 18), HorizontalAlignment.Right));
        input.Children.Add(Label("上传参考图（可选）"));
        var upload = DashedBox("⇧  上传图片", "支持 JPG、PNG（≤5MB）");
        upload.MouseLeftButtonUp += (_, _) => ShowScreen(AppScreen.ImageReview);
        input.Children.Add(upload);
        input.Children.Add(OptionRow("高级选项", ""));
        input.Children.Add(OptionRow("识别设置", "自动识别"));
        input.Children.Add(OptionRow("布局方向", "自动布局"));
        var generate = PrimaryWideButton("✦  生成");
        if (!filled)
        {
            generate.IsEnabled = false;
        }
        input.Children.Add(generate);
        return BareCard(input);
    }

    private Border BuildStructurePanel()
    {
        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(210) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(210) });

        var json = new Grid();
        json.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        json.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(json.Children.AddAndReturn(Tabs(("JSON", true, () => ShowScreen(AppScreen.JsonEditor)), ("节点", false, null), ("连线", false, null))), 0);
        Grid.SetRow(json.Children.AddAndReturn(JsonBox()), 1);
        Grid.SetRow(root.Children.AddAndReturn(Card("", json)), 0);

        Grid.SetRow(root.Children.AddAndReturn(Card("节点（7）",
            DataTable(new[] { ("id", "Id"), ("标签", "Label"), ("类型", "Type"), ("形状", "Shape") }, SampleDiagramData.ChenEr.Nodes))), 1);
        Grid.SetRow(root.Children.AddAndReturn(Card("连线（6）",
            DataTable(new[] { ("id", "Id"), ("标签", "Label"), ("起点", "From"), ("终点", "To"), ("基数", "Cardinality") }, SampleDiagramData.ChenEr.Edges))), 2);
        return BareCard(root);
    }

    private Border BuildEmptyStructurePanel()
    {
        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(210) });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(210) });
        Grid.SetRow(root.Children.AddAndReturn(Card("", EmptyBox("▧", "暂无数据"))), 0);
        Grid.SetRow(root.Children.AddAndReturn(Card("节点（0）", EmptyTable())), 1);
        Grid.SetRow(root.Children.AddAndReturn(Card("连线（0）", EmptyTable())), 2);
        return BareCard(root);
    }

    private Border BuildPreviewInfoPanel()
    {
        var body = new StackPanel { Margin = new Thickness(16) };
        body.Children.Add(Text("预览信息", 16, FontWeights.SemiBold, new Thickness(0, 0, 0, 24)));
        body.Children.Add(Label("文件名"));
        body.Children.Add(Input("功能模块图.drawio"));
        body.Children.Add(Label("图类型"));
        body.Children.Add(Input("功能模块图"));
        body.Children.Add(Label("页面"));
        body.Children.Add(Input("1"));
        body.Children.Add(Label("图尺寸"));
        body.Children.Add(Input("1280 × 960"));
        body.Children.Add(Label("节点数量"));
        body.Children.Add(Input("19"));
        body.Children.Add(Label("连线数量"));
        body.Children.Add(Input("20"));
        body.Children.Add(Spacer(30));
        body.Children.Add(WideButton("⟳  刷新预览"));
        return BareCard(body);
    }

    private Border BuildExportSettingsPanel()
    {
        var body = new StackPanel { Margin = new Thickness(16) };
        body.Children.Add(Text("导出设置", 16, FontWeights.SemiBold, new Thickness(0, 0, 0, 22)));
        body.Children.Add(Label("格式选择"));
        body.Children.Add(ExportChoice("▧  draw.io（.drawio）", true));
        body.Children.Add(ExportChoice("▧  SVG（.svg）", false));
        body.Children.Add(ExportChoice("▧  PNG（.png）", false));
        body.Children.Add(ExportChoice("▧  PDF（.pdf）", false));
        body.Children.Add(Label("导出路径"));
        body.Children.Add(Input(@"C:\Users\Admin\Documents", "▣"));
        body.Children.Add(Label("文件名"));
        body.Children.Add(Input("功能模块图"));
        body.Children.Add(Label("导出范围"));
        body.Children.Add(Text("●  整个图      ○  选中内容", 14, FontWeights.Normal, new Thickness(0, 4, 0, 18)));
        body.Children.Add(Label("导出选项"));
        body.Children.Add(CheckRow("包含样式", true));
        body.Children.Add(CheckRow("包含背景", true));
        body.Children.Add(CheckRow("压缩文件", true));
        body.Children.Add(Spacer(18));
        body.Children.Add(PrimaryWideButton("▣  导出"));
        return BareCard(body);
    }

    private Border BuildOperationQueue()
    {
        var body = new StackPanel { Margin = new Thickness(16) };
        var top = new DockPanel { Margin = new Thickness(0, 0, 0, 12) };
        top.Children.Add(Text("操作队列", 17, FontWeights.SemiBold));
        var clear = SmallButton("▢  清空队列");
        DockPanel.SetDock(clear, Dock.Right);
        top.Children.Add(clear);
        body.Children.Add(top);
        body.Children.Add(OperationItem("1", "add_node", "在“学生”实体下添加属性“联系方式”", "目标：学生", _blue));
        body.Children.Add(OperationItem("2", "rename_node", "将“教师”实体的属性“姓名”重命名为“教师姓名”", "目标：教师.姓名", _green));
        body.Children.Add(OperationItem("3", "update_style", "将“学生”实体的填充色设为 #E8F1FF", "目标：学生", _amber));
        body.Children.Add(OperationItem("4", "move_node", "将“教室”的属性“位置”上移到第一位", "目标：教室.位置", BrushFrom("#8B5CF6")));
        body.Children.Add(OperationItem("5", "delete_edge", "删除“学生”与“成绩”之间的“选课”关系", "目标：学生 ↔ 成绩", _red));
        body.Children.Add(Spacer(18));
        body.Children.Add(PrimaryWideButton("✓  应用修改"));
        body.Children.Add(RowButtons("↶  撤销", "↷  重做"));
        return BareCard(body);
    }

    private UIElement BuildPreviewWithToolbar(UIElement diagram)
    {
        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetRow(root.Children.AddAndReturn(Toolbar()), 0);
        Grid.SetRow(root.Children.AddAndReturn(diagram), 1);
        Grid.SetRow(root.Children.AddAndReturn(ZoomBar()), 2);
        return root;
    }

    private UIElement BuildChenErDiagram(bool withHighlights = false)
    {
        var canvas = new Canvas { Width = 760, Height = 760, Background = _canvas };
        AddRect(canvas, 350, 90, 110, 46, "院系", withHighlights ? null : null);
        AddEllipse(canvas, 330, 30, 90, 34, "院系编号");
        AddEllipse(canvas, 210, 55, 96, 34, "院系名称");
        AddEllipse(canvas, 500, 58, 96, 34, "办公地点");
        AddLine(canvas, 405, 90, 405, 64);
        AddLine(canvas, 350, 110, 306, 72);
        AddLine(canvas, 460, 112, 500, 76);

        AddRect(canvas, 170, 300, 110, 46, "学生", withHighlights ? _blue : null);
        AddEllipse(canvas, 40, 250, 92, 34, "学号");
        AddEllipse(canvas, 40, 290, 92, 34, "姓名");
        AddEllipse(canvas, 40, 330, 92, 34, "性别");
        AddEllipse(canvas, 40, 370, 96, 34, "入学年份");
        AddLine(canvas, 170, 318, 132, 267);
        AddLine(canvas, 170, 323, 132, 307);
        AddLine(canvas, 170, 330, 132, 347);
        AddLine(canvas, 170, 336, 136, 387);

        AddRect(canvas, 545, 300, 110, 46, "教师", null);
        AddEllipse(canvas, 685, 250, 82, 34, "工号");
        AddEllipse(canvas, 685, 290, 82, 34, "姓名", withHighlights ? _blue : null);
        AddEllipse(canvas, 685, 330, 82, 34, "职称");
        AddEllipse(canvas, 685, 370, 96, 34, "联系电话");
        AddLine(canvas, 655, 315, 685, 267);
        AddLine(canvas, 655, 322, 685, 307);
        AddLine(canvas, 655, 330, 685, 347);
        AddLine(canvas, 655, 338, 685, 387);

        AddRect(canvas, 365, 475, 120, 46, "课程", null);
        AddEllipse(canvas, 540, 450, 92, 34, "课程号");
        AddEllipse(canvas, 540, 490, 100, 34, "课程名称");
        AddEllipse(canvas, 540, 530, 82, 34, "学分");
        AddLine(canvas, 485, 495, 540, 467);
        AddLine(canvas, 485, 502, 540, 507);
        AddLine(canvas, 485, 509, 540, 547);

        AddRect(canvas, 250, 610, 110, 46, "成绩", null);
        AddEllipse(canvas, 180, 695, 74, 32, "成绩ID");
        AddEllipse(canvas, 275, 695, 64, 32, "分数");
        AddEllipse(canvas, 365, 695, 64, 32, "绩点");
        AddLine(canvas, 280, 656, 218, 695);
        AddLine(canvas, 305, 656, 305, 695);
        AddLine(canvas, 335, 656, 392, 695);

        AddRect(canvas, 365, 650, 120, 46, "教室", null);
        AddEllipse(canvas, 300, 735, 76, 32, "教室号");
        AddEllipse(canvas, 400, 735, 92, 32, "教室名称");
        AddEllipse(canvas, 505, 735, 62, 32, "容量");
        AddEllipse(canvas, 585, 735, 62, 32, "位置", withHighlights ? _blue : null);
        AddLine(canvas, 390, 696, 338, 735);
        AddLine(canvas, 415, 696, 446, 735);
        AddLine(canvas, 450, 696, 536, 735);
        AddLine(canvas, 475, 696, 616, 735);

        AddDiamond(canvas, 255, 175, 72, 58, "属于");
        AddDiamond(canvas, 530, 175, 72, 58, "属于");
        AddDiamond(canvas, 285, 425, 72, 58, "选课");
        AddDiamond(canvas, 445, 335, 72, 58, "授课");
        AddDiamond(canvas, 420, 575, 72, 58, "安排");

        AddLine(canvas, 350, 136, 291, 175, "1");
        AddLine(canvas, 291, 233, 225, 300, "N");
        AddLine(canvas, 460, 136, 566, 175, "1");
        AddLine(canvas, 566, 233, 600, 300, "N");
        AddLine(canvas, 225, 346, 321, 425, "N");
        AddLine(canvas, 321, 483, 305, 610, "1");
        AddLine(canvas, 321, 454, 365, 498, "N");
        AddLine(canvas, 485, 498, 521, 498, "N");
        AddLine(canvas, 545, 346, 481, 335, "1");
        AddLine(canvas, 481, 393, 425, 475, "N");
        AddLine(canvas, 425, 521, 456, 575, "N");
        AddLine(canvas, 456, 633, 425, 650, "1");

        return CenteredViewBox(canvas);
    }

    private UIElement BuildModuleDiagram()
    {
        var canvas = new Canvas { Width = 900, Height = 680, Background = _canvas };
        AddRect(canvas, 365, 55, 210, 58, "智能图谱设计器", null);
        var heads = new[] { "项目管理", "图谱设计", "数据管理", "导出管理", "系统设置" };
        var children = new[]
        {
            new[] {"新建项目", "打开项目", "保存项目", "删除项目"},
            new[] {"选择图类型", "编辑节点", "编辑连线", "布局调整", "样式设置"},
            new[] {"导入数据", "数据映射", "数据校验", "数据预览"},
            new[] {"预览图谱", "选择格式", "导出设置", "导出文件"},
            new[] {"通用设置", "快捷键设置", "主题设置", "关于我们"}
        };
        for (var i = 0; i < heads.Length; i++)
        {
            var x = 65 + i * 170;
            AddRect(canvas, x, 190, 118, 48, heads[i], null);
            AddLine(canvas, 470, 113, 470, 150);
            AddLine(canvas, 124, 150, 804, 150);
            AddLine(canvas, x + 59, 150, x + 59, 190);
            for (var j = 0; j < children[i].Length; j++)
            {
                var y = 285 + j * 64;
                AddRect(canvas, x, y, 118, 42, children[i][j], null);
                AddLine(canvas, x + 59, 238, x + 59, y);
                AddLine(canvas, x + 35, y + 21, x, y + 21);
            }
        }
        return CenteredViewBox(canvas);
    }

    private UIElement BuildEmptyCanvas()
    {
        var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        stack.Children.Add(Text("◇", 84, FontWeights.Normal, new Thickness(0, 0, 0, 12), HorizontalAlignment.Center, _muted));
        var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        row.Children.Add(SizedButton("＋ 新建图形", 134, true));
        row.Children.Add(SizedButton("▣ 打开项目", 132, false, new Thickness(14, 0, 0, 0)));
        stack.Children.Add(row);
        stack.Children.Add(Text("请新建图形或打开已有项目开始设计", 14, FontWeights.Normal, new Thickness(0, 14, 0, 0), HorizontalAlignment.Center, _muted));
        return stack;
    }

    private UIElement BuildPaperImagePreview()
    {
        var border = new Border
        {
            Background = BrushFrom("#F3F4F6"),
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(8),
            Child = BuildChenErDiagram(true)
        };
        return border;
    }

    private UIElement Toolbar()
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
        foreach (var label in new[] { "⌖", "☝", "⊕", "−", "⛶", "⤢" })
        {
            row.Children.Add(SizedButton(label, 42, false, new Thickness(0, 0, 10, 0)));
        }
        return row;
    }

    private UIElement ZoomBar()
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 10, 0, 0) };
        row.Children.Add(SizedButton("▤", 44));
        row.Children.Add(SizedButton("−", 56, false, new Thickness(10, 0, 0, 0)));
        row.Children.Add(SizedButton("100%", 88));
        row.Children.Add(SizedButton("+", 56));
        row.Children.Add(SizedButton("⛶", 44, false, new Thickness(10, 0, 0, 0)));
        return row;
    }

    private UIElement CommandInput()
    {
        var grid = new Grid { Margin = new Thickness(0, 12, 0, 0) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var box = Input("输入修改指令");
        Grid.SetColumn(grid.Children.AddAndReturn(box), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(SizedButton("➤", 56, false, new Thickness(8, 0, 0, 0))), 1);
        return grid;
    }

    private UIElement JsonBox()
    {
        return new TextBox
        {
            Text = SampleDiagramData.ChenEr.Json,
            AcceptsReturn = true,
            AcceptsTab = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 14,
            Height = double.NaN,
            MinHeight = 300,
            TextWrapping = TextWrapping.NoWrap
        };
    }

    private UIElement ValidationSummary()
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 14) };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        AddSummaryCard(grid, 0, "●  通过", "34", _green);
        AddSummaryCard(grid, 1, "▲  警告", "0", _amber);
        AddSummaryCard(grid, 2, "●  错误", "0", _red);
        return grid;
    }

    private void AddSummaryCard(Grid grid, int col, string title, string count, Brush color)
    {
        var box = new Border
        {
            BorderBrush = color,
            BorderThickness = new Thickness(1),
            Background = BrushFrom("#FFFFFF"),
            CornerRadius = new CornerRadius(6),
            Margin = new Thickness(col == 0 ? 0 : 8, 0, 0, 0),
            Padding = new Thickness(14)
        };
        box.Child = Stack(
            Text(title, 14, FontWeights.SemiBold, brush: color),
            Text(count, 28, FontWeights.SemiBold, new Thickness(0, 8, 0, 0), HorizontalAlignment.Center)
        );
        Grid.SetColumn(box, col);
        grid.Children.Add(box);
    }

    private UIElement SettingsNav()
    {
        var nav = new StackPanel { Margin = new Thickness(16) };
        nav.Children.Add(NavItem("▧", "模型", true));
        nav.Children.Add(NavItem("⇧", "导出", false));
        nav.Children.Add(NavItem("▣", "Visio", false));
        nav.Children.Add(NavItem("◌", "外观", false));
        nav.Children.Add(NavItem("▤", "存储", false));
        return BareCard(nav);
    }

    private UIElement NavItem(string icon, string label, bool active)
    {
        var border = new Border
        {
            Background = active ? BrushFrom("#EFF6FF") : Brushes.Transparent,
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(18, 12, 18, 12),
            Margin = new Thickness(0, 0, 0, 8)
        };
        border.Child = Text($"{icon}   {label}", 18, active ? FontWeights.SemiBold : FontWeights.Normal, brush: active ? _blue : Brushes.Black);
        return border;
    }

    private UIElement SettingsSection(string title, params UIElement[] rows)
    {
        var stack = new StackPanel();
        stack.Children.Add(Text(title, 18, FontWeights.SemiBold, new Thickness(0, 0, 0, 18)));
        foreach (var row in rows)
        {
            stack.Children.Add(row);
        }
        return new Border
        {
            Background = _panel,
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(22),
            Margin = new Thickness(0, 0, 0, 12),
            Child = stack
        };
    }

    private UIElement FormRow(string label, UIElement input)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 14) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(210) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(grid.Children.AddAndReturn(Text(label, 15, FontWeights.Normal, vertical: VerticalAlignment.Center)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(input), 1);
        return grid;
    }

    private UIElement ToggleRow(string label, bool enabled)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 12) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(210) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(grid.Children.AddAndReturn(Text(label, 15, FontWeights.Normal, vertical: VerticalAlignment.Center)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Toggle(enabled)), 1);
        return grid;
    }

    private UIElement Toggle(bool enabled)
    {
        return new Border
        {
            Width = 48,
            Height = 26,
            CornerRadius = new CornerRadius(13),
            Background = enabled ? _blue : BrushFrom("#D1D5DB"),
            Child = new Border
            {
                Width = 22,
                Height = 22,
                Margin = enabled ? new Thickness(22, 2, 2, 2) : new Thickness(2),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(11)
            }
        };
    }

    private UIElement FormattedTextBlock(string title, string value, Brush color)
    {
        return Stack(Text(title, 14, FontWeights.SemiBold, brush: color), Text(value, 13, FontWeights.Normal, new Thickness(0, 6, 0, 0)));
    }

    private UIElement DiffBlock(string title, string a, string b, string c, Brush color)
    {
        var body = new StackPanel { Margin = new Thickness(0, 0, 0, 12) };
        body.Children.Add(Text($"●  {title}", 15, FontWeights.SemiBold, brush: color));
        body.Children.Add(DataTable(new[] { ("节点名", "A"), ("类型", "B"), ("说明", "C") }, new[] { new Triple(a, b, c) }, 120));
        return new Border
        {
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12),
            Margin = new Thickness(0, 0, 0, 12),
            Child = body
        };
    }

    private UIElement OperationItem(string no, string op, string desc, string target, Brush color)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(34) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(grid.Children.AddAndReturn(Text(no, 18, FontWeights.SemiBold, vertical: VerticalAlignment.Top)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(Stack(
            Badge(op, color),
            Text(desc, 14, FontWeights.Normal, new Thickness(0, 10, 0, 8)),
            Text(target, 13, FontWeights.Normal, brush: _muted)
        )), 1);
        return new Border
        {
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14),
            Margin = new Thickness(0, 0, 0, 10),
            Child = grid
        };
    }

    private UIElement OptionRow(string label, string value)
    {
        var dock = new DockPanel { Margin = new Thickness(0, 16, 0, 0) };
        dock.Children.Add(Text(label, 14, FontWeights.SemiBold));
        var right = Text(string.IsNullOrWhiteSpace(value) ? "›" : $"{value}  ›", 13, brush: _muted);
        DockPanel.SetDock(right, Dock.Right);
        dock.Children.Add(right);
        return dock;
    }

    private UIElement CheckRow(string label, bool selected)
    {
        return Text($"{(selected ? "☑" : "☐")}  {label}", 14, FontWeights.Normal, new Thickness(0, 8, 0, 0), brush: selected ? _blue : Brushes.Black);
    }

    private UIElement ExportChoice(string label, bool active)
    {
        return new Border
        {
            BorderBrush = active ? _blue : _border,
            BorderThickness = new Thickness(1),
            Background = active ? BrushFrom("#F8FBFF") : _panel,
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(14, 11, 14, 11),
            Margin = new Thickness(0, 0, 0, 8),
            Child = Text(label, 14, brush: active ? _blue : Brushes.Black)
        };
    }

    private UIElement DashedBox(string title, string subtitle)
    {
        return new Border
        {
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 4, 0, 12),
            Child = Stack(
                Text(title, 14, FontWeights.SemiBold, horizontal: HorizontalAlignment.Center),
                Text(subtitle, 12, FontWeights.Normal, new Thickness(0, 10, 0, 0), HorizontalAlignment.Center, _muted)
            )
        };
    }

    private UIElement EmptyBox(string icon, string text)
    {
        return Stack(
            Text(icon, 46, FontWeights.Normal, horizontal: HorizontalAlignment.Center, brush: _muted),
            Text(text, 14, FontWeights.Normal, new Thickness(0, 10, 0, 0), HorizontalAlignment.Center, _muted)
        );
    }

    private UIElement EmptyTable()
    {
        return EmptyBox("▱", "暂无数据");
    }

    private UIElement Label(string text)
    {
        return Text(text, 14, FontWeights.SemiBold, new Thickness(0, 12, 0, 8));
    }

    private UIElement Separator()
    {
        return new Border { Height = 1, Background = _border, Margin = new Thickness(0, 16, 0, 12) };
    }

    private UIElement Spacer(double height)
    {
        return new Border { Height = height, Background = Brushes.Transparent };
    }

    private TextBlock Text(string text, double size, FontWeight? weight = null, Thickness? margin = null, HorizontalAlignment horizontal = HorizontalAlignment.Left, Brush? brush = null, VerticalAlignment vertical = VerticalAlignment.Top)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = size,
            FontWeight = weight ?? FontWeights.Normal,
            Margin = margin ?? new Thickness(0),
            HorizontalAlignment = horizontal,
            VerticalAlignment = vertical,
            Foreground = brush ?? Brushes.Black,
            TextWrapping = TextWrapping.Wrap
        };
    }

    private TextBox Input(string text, string? suffix = null)
    {
        return new TextBox { Text = suffix is null ? text : $"{text}    {suffix}", Margin = new Thickness(0, 0, 0, 10) };
    }

    private ComboBox SelectBox(string selected)
    {
        var box = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
        box.Items.Add(selected);
        box.SelectedIndex = 0;
        return box;
    }

    private TextBox TextArea(string text)
    {
        return new TextBox
        {
            Text = text,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Height = 320,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(0, 0, 0, 0)
        };
    }

    private Button SmallButton(string label)
    {
        return SizedButton(label, 78);
    }

    private Button SizedButton(string label, double width, bool primary = false, Thickness? margin = null)
    {
        var button = new Button
        {
            Content = label,
            Width = width,
            Margin = margin ?? new Thickness(0)
        };
        if (primary)
        {
            button.Style = (Style)FindResource("PrimaryButton");
        }
        return button;
    }

    private Button WideButton(string label)
    {
        return new Button { Content = label, Height = 44, Margin = new Thickness(0, 8, 0, 0) };
    }

    private Button PrimaryWideButton(string label)
    {
        var button = new Button { Content = label, Height = 46, Margin = new Thickness(0, 12, 0, 0), Style = (Style)FindResource("PrimaryButton") };
        return button;
    }

    private UIElement RightAlignedButton(string label, bool primary)
    {
        var button = SizedButton(label, 110, primary);
        button.HorizontalAlignment = HorizontalAlignment.Right;
        return button;
    }

    private UIElement RowButtons(string left, string right)
    {
        var grid = new Grid { Margin = new Thickness(0, 10, 0, 0) };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        var b1 = WideButton(left);
        var b2 = WideButton(right);
        b1.Margin = new Thickness(0, 0, 6, 0);
        b2.Margin = new Thickness(6, 0, 0, 0);
        Grid.SetColumn(grid.Children.AddAndReturn(b1), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(b2), 1);
        return grid;
    }

    private UIElement SplitRow(UIElement left, UIElement right)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        Grid.SetColumn(grid.Children.AddAndReturn(left), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(right), 1);
        return grid;
    }

    private UIElement KeyValue(string key, UIElement value)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 14) };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        Grid.SetColumn(grid.Children.AddAndReturn(Text(key, 14, brush: _muted)), 0);
        Grid.SetColumn(grid.Children.AddAndReturn(value), 1);
        return grid;
    }

    private UIElement Badge(string label, Brush color)
    {
        return new Border
        {
            Background = WithOpacity(color, 0.10),
            BorderBrush = WithOpacity(color, 0.30),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(8, 3, 8, 3),
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = Text(label, 13, FontWeights.SemiBold, brush: color)
        };
    }

    private Brush WithOpacity(Brush brush, double opacity)
    {
        if (brush is SolidColorBrush solid)
        {
            return new SolidColorBrush(solid.Color) { Opacity = opacity };
        }
        return brush;
    }

    private UIElement Tabs(params (string label, bool active, Action? action)[] tabs)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
        foreach (var tab in tabs)
        {
            var button = new Button
            {
                Content = tab.label,
                Style = (Style)FindResource("GhostButton"),
                Foreground = tab.active ? _blue : Brushes.Black,
                FontWeight = tab.active ? FontWeights.SemiBold : FontWeights.Normal
            };
            if (tab.action is not null)
            {
                button.Click += (_, _) => tab.action();
            }
            row.Children.Add(button);
        }
        return row;
    }

    private Border Card(string title, UIElement content)
    {
        var grid = new Grid { Margin = new Thickness(12) };
        if (!string.IsNullOrWhiteSpace(title))
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(grid.Children.AddAndReturn(Text(title, 16, FontWeights.SemiBold, new Thickness(0, 0, 0, 12))), 0);
            Grid.SetRow(grid.Children.AddAndReturn(content), 1);
        }
        else
        {
            grid.Children.Add(content);
        }
        return BareCard(grid);
    }

    private Border BareCard(UIElement content)
    {
        return new Border
        {
            Background = _panel,
            BorderBrush = _border,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Child = content
        };
    }

    private Grid ThreeColumnGrid(double leftWidth, double centerWeight, double rightWidth)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(leftWidth) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(centerWeight, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(rightWidth) });
        SetColumnMargins(grid);
        return grid;
    }

    private Grid TwoColumnGrid(double left, double right)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = left == 1 ? new GridLength(1, GridUnitType.Star) : new GridLength(left) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = right == 1 ? new GridLength(1, GridUnitType.Star) : new GridLength(right) });
        SetColumnMargins(grid);
        return grid;
    }

    private static void SetColumnMargins(Grid grid)
    {
        grid.Loaded += (_, _) =>
        {
            foreach (var child in grid.Children.OfType<FrameworkElement>())
            {
                var column = Grid.GetColumn(child);
                child.Margin = new Thickness(column == 0 ? 0 : 8, 0, 0, 0);
            }
        };
    }

    private StackPanel Stack(params UIElement[] children)
    {
        var stack = new StackPanel();
        foreach (var child in children)
        {
            stack.Children.Add(child);
        }
        return stack;
    }

    private DataGrid DataTable<T>(IEnumerable<(string Header, string Binding)> columns, IEnumerable<T> rows, double? maxHeight = null)
    {
        var table = new DataGrid
        {
            ItemsSource = rows,
            Margin = new Thickness(0, 0, 0, 0),
            MaxHeight = maxHeight ?? double.PositiveInfinity
        };
        foreach (var col in columns)
        {
            table.Columns.Add(new DataGridTextColumn
            {
                Header = col.Header,
                Binding = new Binding(col.Binding),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }
        return table;
    }

    private Viewbox CenteredViewBox(Canvas canvas)
    {
        return new Viewbox
        {
            Stretch = Stretch.Uniform,
            Child = canvas,
            Margin = new Thickness(8)
        };
    }

    private void AddRect(Canvas canvas, double x, double y, double width, double height, string label, Brush? accent)
    {
        var borderBrush = accent ?? Brushes.Black;
        canvas.Children.Add(Position(new Rectangle
        {
            Width = width,
            Height = height,
            Stroke = borderBrush,
            StrokeThickness = accent is null ? 1.4 : 2.4,
            Fill = Brushes.White
        }, x, y));
        canvas.Children.Add(Position(Text(label, 16, FontWeights.SemiBold, horizontal: HorizontalAlignment.Center), x, y + height / 2 - 11, width));
    }

    private void AddEllipse(Canvas canvas, double x, double y, double width, double height, string label, Brush? accent = null)
    {
        var borderBrush = accent ?? Brushes.Black;
        canvas.Children.Add(Position(new Ellipse
        {
            Width = width,
            Height = height,
            Stroke = borderBrush,
            StrokeThickness = accent is null ? 1.4 : 2.4,
            Fill = Brushes.White
        }, x, y));
        canvas.Children.Add(Position(Text(label, 13, FontWeights.Normal, horizontal: HorizontalAlignment.Center), x, y + height / 2 - 9, width));
    }

    private void AddDiamond(Canvas canvas, double x, double y, double width, double height, string label)
    {
        var polygon = new Polygon
        {
            Points = new PointCollection
            {
                new(width / 2, 0),
                new(width, height / 2),
                new(width / 2, height),
                new(0, height / 2)
            },
            Stroke = Brushes.Black,
            StrokeThickness = 1.4,
            Fill = Brushes.White
        };
        canvas.Children.Add(Position(polygon, x, y));
        canvas.Children.Add(Position(Text(label, 13, FontWeights.Normal, horizontal: HorizontalAlignment.Center), x, y + height / 2 - 9, width));
    }

    private void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, string? label = null)
    {
        canvas.Children.Add(new Line
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = Brushes.Black,
            StrokeThickness = 1.2
        });
        if (!string.IsNullOrWhiteSpace(label))
        {
            canvas.Children.Add(Position(Text(label, 13), (x1 + x2) / 2 + 6, (y1 + y2) / 2 - 18));
        }
    }

    private FrameworkElement Position(FrameworkElement element, double x, double y, double? width = null)
    {
        if (width is not null)
        {
            element.Width = width.Value;
        }
        Canvas.SetLeft(element, x);
        Canvas.SetTop(element, y);
        return element;
    }

    private IEnumerable<NodeRow> NodeRows()
    {
        return
        [
            new("dept", "院系", "entity", "rectangle"),
            new("student", "学生", "entity", "rectangle"),
            new("teacher", "教师", "entity", "rectangle"),
            new("course", "课程", "entity", "rectangle"),
            new("enrollment", "选课", "relationship", "diamond"),
            new("score", "成绩", "entity", "rectangle"),
            new("classroom", "教室", "entity", "rectangle")
        ];
    }

    private IEnumerable<EdgeRow> EdgeRows()
    {
        return
        [
            new("r1", "属于", "student", "dept", "N:1"),
            new("r2", "属于", "teacher", "dept", "N:1"),
            new("r3", "授课", "teacher", "course", "1:N"),
            new("r4", "选课", "student", "course", "N:N"),
            new("r5", "成绩", "student", "score", "N:1"),
            new("r6", "安排", "course", "classroom", "N:1")
        ];
    }

    private IEnumerable<LogRow> LogRows()
    {
        return
        [
            new("2026-06-03 22:35:48", "连接 Visio", "成功"),
            new("2026-06-03 22:35:48", "读取文档信息", "成功"),
            new("2026-06-03 22:35:49", "读取页面 Page-5", "成功"),
            new("2026-06-03 22:35:49", "解析 Shape", "成功"),
            new("2026-06-03 22:35:49", "映射实体与关系", "成功"),
            new("2026-06-03 22:35:50", "同步图形", "成功"),
            new("2026-06-03 22:35:51", "同步完成", "成功")
        ];
    }

    private IEnumerable<VersionRow> VersionRows()
    {
        return Enumerable.Range(0, 16).Select(i => new VersionRow($"v1.0.{8 - i}", "Chen ER 图", i == 0 ? "新增教室位置属性" : "调整图形结构", $"06-03 {22 - i % 6}:10"));
    }

    private IEnumerable<RecognitionRow> RecognitionRows()
    {
        return
        [
            new("n1", "terminator", "院系编号", "0.98"),
            new("n2", "process", "院系", "0.99"),
            new("n3", "terminator", "院系名称", "0.97"),
            new("n4", "terminator", "办公地点", "0.97"),
            new("r1", "decision", "属于", "0.98"),
            new("n5", "process", "学生", "0.99"),
            new("n6", "terminator", "学号", "0.98"),
            new("n7", "terminator", "姓名", "0.98"),
            new("n8", "terminator", "性别", "0.97"),
            new("n9", "terminator", "入学年份", "0.97"),
            new("r2", "decision", "选课", "0.98"),
            new("n10", "process", "成绩", "0.98")
        ];
    }

    private const string SampleJson = """
{
  "entities": [
    { "id": "dept", "name": "院系" },
    { "id": "student", "name": "学生" },
    { "id": "teacher", "name": "教师" },
    { "id": "course", "name": "课程" },
    { "id": "enrollment", "name": "选课" },
    { "id": "score", "name": "成绩" },
    { "id": "classroom", "name": "教室" }
  ],
  "relationships": [
    { "id": "r1", "name": "属于", "from": "student", "to": "dept", "cardinality": "N:1" },
    { "id": "r2", "name": "属于", "from": "teacher", "to": "dept", "cardinality": "N:1" },
    { "id": "r3", "name": "授课", "from": "teacher", "to": "course", "cardinality": "1:N" },
    { "id": "r4", "name": "选课", "from": "student", "to": "course", "cardinality": "N:N" },
    { "id": "r5", "name": "成绩", "from": "enrollment", "to": "score", "cardinality": "N:1" },
    { "id": "r6", "name": "安排", "from": "course", "to": "classroom", "cardinality": "N:1" }
  ]
}
""";

    private sealed record NodeRow(string Id, string Label, string Type, string Shape);
    private sealed record EdgeRow(string Id, string Label, string From, string To, string Cardinality);
    private sealed record LogRow(string Time, string Action, string Result);
    private sealed record VersionRow(string Version, string Type, string Summary, string Time);
    private sealed record RecognitionRow(string Id, string Type, string Text, string Score);
    private sealed record Triple(string A, string B, string C);
}

internal static class UiElementCollectionExtensions
{
    public static T AddAndReturn<T>(this UIElementCollection children, T element) where T : UIElement
    {
        children.Add(element);
        return element;
    }
}
