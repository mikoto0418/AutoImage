# AutoImage / SmartDiagram

面向论文与工程文档绘图的 Windows 本地智能绘图工具原型。当前仓库已经完成 MVP1 基础骨架：WPF 桌面界面、统一 DiagramDocument 模型、Chen ER / 功能模块图样例、draw.io XML 生成与 `.drawio` 文件导出。

## 当前里程碑

- `SmartDiagram.Core`：图结构模型、样例数据、校验器。
- `SmartDiagram.Drawio`：DiagramDocument 到 draw.io XML 的转换与文件导出。
- `SmartDiagram.App`：Shadcn 风格 WPF 原型界面与导出入口。
- `SmartDiagram.Tests`：图结构校验、样例数据、draw.io 生成和导出测试。

## 验证命令

```powershell
dotnet build .\SmartDiagram.sln --no-restore
dotnet test .\SmartDiagram.sln --no-restore
```

## 下一步

- 将编辑页输入接入真实 DiagramDocument 状态。
- 接入模型 API 生成 DiagramDocument。
- 嵌入 WebView2 / diagrams.net 预览与编辑。
- 增加 Visio COM 自动绘图模块。
