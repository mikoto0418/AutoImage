namespace SmartDiagram.Core.Ui;

public static class ScreenCatalog
{
    public static IReadOnlyList<ScreenDefinition> All { get; } =
    [
        new(AppScreen.Editor, "编辑器"),
        new(AppScreen.Empty, "空状态"),
        new(AppScreen.Export, "导出"),
        new(AppScreen.Settings, "设置"),
        new(AppScreen.Visio, "Visio"),
        new(AppScreen.JsonEditor, "JSON"),
        new(AppScreen.Versions, "版本"),
        new(AppScreen.Modifications, "修改"),
        new(AppScreen.ImageReview, "图片识别"),
        new(AppScreen.ExportPreview, "预览导出")
    ];
}
