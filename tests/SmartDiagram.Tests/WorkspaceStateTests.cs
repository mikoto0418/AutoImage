using SmartDiagram.Core.Ui;

namespace SmartDiagram.Tests;

public sealed class WorkspaceStateTests
{
    [Fact]
    public void CreateDefault_uses_editor_screen_and_saved_status()
    {
        var state = WorkspaceState.CreateDefault();

        Assert.Equal(AppScreen.Editor, state.CurrentScreen);
        Assert.Equal("v1.0.0", state.Version);
        Assert.True(state.IsSaved);
        Assert.Equal("已保存", state.SaveStatusText);
    }

    [Fact]
    public void ScreenCatalog_contains_the_reference_ui_set()
    {
        var screens = ScreenCatalog.All;

        Assert.Equal(10, screens.Count);
        Assert.Contains(screens, screen => screen.Id == AppScreen.Editor && screen.Label == "编辑器");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Empty && screen.Label == "空状态");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Export && screen.Label == "导出");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Settings && screen.Label == "设置");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Visio && screen.Label == "Visio");
        Assert.Contains(screens, screen => screen.Id == AppScreen.JsonEditor && screen.Label == "JSON");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Versions && screen.Label == "版本");
        Assert.Contains(screens, screen => screen.Id == AppScreen.Modifications && screen.Label == "修改");
        Assert.Contains(screens, screen => screen.Id == AppScreen.ImageReview && screen.Label == "图片识别");
        Assert.Contains(screens, screen => screen.Id == AppScreen.ExportPreview && screen.Label == "预览导出");
    }
}
