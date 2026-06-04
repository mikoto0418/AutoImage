using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartDiagram.App.Components;

public static class DiagramPreviewFactory
{
    private static readonly Brush CanvasBrush = BrushFrom("#FDFDFD");
    private static readonly Brush BlueBrush = BrushFrom("#0F6BFF");
    private static readonly Brush BorderBrush = BrushFrom("#E2E8F0");

    public static UIElement ChenEr(bool withHighlights = false)
    {
        var canvas = new Canvas { Width = 760, Height = 760, Background = CanvasBrush };
        AddRect(canvas, 350, 90, 110, 46, "院系");
        AddEllipse(canvas, 330, 30, 90, 34, "院系编号");
        AddEllipse(canvas, 210, 55, 96, 34, "院系名称");
        AddEllipse(canvas, 500, 58, 96, 34, "办公地点");
        AddLine(canvas, 405, 90, 405, 64);
        AddLine(canvas, 350, 110, 306, 72);
        AddLine(canvas, 460, 112, 500, 76);

        AddRect(canvas, 170, 300, 110, 46, "学生", withHighlights ? BlueBrush : null);
        AddEllipse(canvas, 40, 250, 92, 34, "学号");
        AddEllipse(canvas, 40, 290, 92, 34, "姓名");
        AddEllipse(canvas, 40, 330, 92, 34, "性别");
        AddEllipse(canvas, 40, 370, 96, 34, "入学年份");
        AddLine(canvas, 170, 318, 132, 267);
        AddLine(canvas, 170, 323, 132, 307);
        AddLine(canvas, 170, 330, 132, 347);
        AddLine(canvas, 170, 336, 136, 387);

        AddRect(canvas, 545, 300, 110, 46, "教师");
        AddEllipse(canvas, 685, 250, 82, 34, "工号");
        AddEllipse(canvas, 685, 290, 82, 34, "姓名", withHighlights ? BlueBrush : null);
        AddEllipse(canvas, 685, 330, 82, 34, "职称");
        AddEllipse(canvas, 685, 370, 96, 34, "联系电话");
        AddLine(canvas, 655, 315, 685, 267);
        AddLine(canvas, 655, 322, 685, 307);
        AddLine(canvas, 655, 330, 685, 347);
        AddLine(canvas, 655, 338, 685, 387);

        AddRect(canvas, 365, 475, 120, 46, "课程");
        AddEllipse(canvas, 540, 450, 92, 34, "课程号");
        AddEllipse(canvas, 540, 490, 100, 34, "课程名称");
        AddEllipse(canvas, 540, 530, 82, 34, "学分");
        AddLine(canvas, 485, 495, 540, 467);
        AddLine(canvas, 485, 502, 540, 507);
        AddLine(canvas, 485, 509, 540, 547);

        AddRect(canvas, 250, 610, 110, 46, "成绩");
        AddEllipse(canvas, 180, 695, 74, 32, "成绩ID");
        AddEllipse(canvas, 275, 695, 64, 32, "分数");
        AddEllipse(canvas, 365, 695, 64, 32, "绩点");
        AddLine(canvas, 280, 656, 218, 695);
        AddLine(canvas, 305, 656, 305, 695);
        AddLine(canvas, 335, 656, 392, 695);

        AddRect(canvas, 365, 650, 120, 46, "教室");
        AddEllipse(canvas, 300, 735, 76, 32, "教室号");
        AddEllipse(canvas, 400, 735, 92, 32, "教室名称");
        AddEllipse(canvas, 505, 735, 62, 32, "容量");
        AddEllipse(canvas, 585, 735, 62, 32, "位置", withHighlights ? BlueBrush : null);
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

    public static UIElement Module()
    {
        var canvas = new Canvas { Width = 900, Height = 680, Background = CanvasBrush };
        AddRect(canvas, 365, 55, 210, 58, "智能图谱设计器");
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
            AddRect(canvas, x, 190, 118, 48, heads[i]);
            AddLine(canvas, 470, 113, 470, 150);
            AddLine(canvas, 124, 150, 804, 150);
            AddLine(canvas, x + 59, 150, x + 59, 190);
            for (var j = 0; j < children[i].Length; j++)
            {
                var y = 285 + j * 64;
                AddRect(canvas, x, y, 118, 42, children[i][j]);
                AddLine(canvas, x + 59, 238, x + 59, y);
                AddLine(canvas, x + 35, y + 21, x, y + 21);
            }
        }
        return CenteredViewBox(canvas);
    }

    public static UIElement PaperPreview()
    {
        return new Border
        {
            Background = BrushFrom("#F3F4F6"),
            BorderBrush = BorderBrush,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(8),
            Child = ChenEr(true)
        };
    }

    private static SolidColorBrush BrushFrom(string color)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }

    private static Viewbox CenteredViewBox(Canvas canvas)
    {
        return new Viewbox
        {
            Stretch = Stretch.Uniform,
            Child = canvas,
            Margin = new Thickness(8)
        };
    }

    private static void AddRect(Canvas canvas, double x, double y, double width, double height, string label, Brush? accent = null)
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
        canvas.Children.Add(Position(Text(label, 16, FontWeights.SemiBold, HorizontalAlignment.Center), x, y + height / 2 - 11, width));
    }

    private static void AddEllipse(Canvas canvas, double x, double y, double width, double height, string label, Brush? accent = null)
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
        canvas.Children.Add(Position(Text(label, 13, FontWeights.Normal, HorizontalAlignment.Center), x, y + height / 2 - 9, width));
    }

    private static void AddDiamond(Canvas canvas, double x, double y, double width, double height, string label)
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
        canvas.Children.Add(Position(Text(label, 13, FontWeights.Normal, HorizontalAlignment.Center), x, y + height / 2 - 9, width));
    }

    private static void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, string? label = null)
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

    private static TextBlock Text(string text, double size, FontWeight? weight = null, HorizontalAlignment horizontal = HorizontalAlignment.Left)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = size,
            FontWeight = weight ?? FontWeights.Normal,
            FontFamily = new FontFamily("Microsoft YaHei UI"),
            HorizontalAlignment = horizontal,
            TextAlignment = TextAlignment.Center,
            Foreground = Brushes.Black
        };
    }

    private static FrameworkElement Position(FrameworkElement element, double x, double y, double? width = null)
    {
        if (width is not null)
        {
            element.Width = width.Value;
        }
        Canvas.SetLeft(element, x);
        Canvas.SetTop(element, y);
        return element;
    }
}
