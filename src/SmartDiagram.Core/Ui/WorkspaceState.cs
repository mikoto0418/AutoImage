namespace SmartDiagram.Core.Ui;

public sealed record WorkspaceState(
    AppScreen CurrentScreen,
    string Version,
    bool IsSaved)
{
    public string SaveStatusText => IsSaved ? "已保存" : "未保存";

    public static WorkspaceState CreateDefault()
    {
        return new WorkspaceState(AppScreen.Editor, "v1.0.0", true);
    }
}
