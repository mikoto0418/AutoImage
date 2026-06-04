# 系统架构设计

## 总体架构

系统采用输入层、模型解析层、图结构中间层、绘图引擎层、导出层的分层架构。大模型只负责理解和结构化输出，程序负责校验、布局、绘图和持久化。

```text
用户输入
  -> 模型解析层
  -> Diagram JSON 中间层
  -> 布局与绘图引擎
  -> draw.io / Visio / 导出文件
```

## 客户端结构

WPF 客户端采用 MVVM 风格组织：

- View：主窗口、设置窗口、识别确认窗口、JSON 查看面板。
- ViewModel：承载命令、页面状态、输入输出绑定。
- Domain：Diagram JSON、操作 JSON、校验结果、布局结果。
- Services：模型调用、draw.io 生成、Visio COM、SQLite、文件导出。

## 推荐工程结构

```text
src/
  SmartDiagram.App/
    Views/
    ViewModels/
    App.xaml
    MainWindow.xaml
  SmartDiagram.Core/
    Diagram/
    Operations/
    Layout/
    Validation/
  SmartDiagram.ModelApi/
    OpenAICompatibleClient.cs
    PromptTemplates/
  SmartDiagram.Drawio/
    DrawioXmlGenerator.cs
    DrawioStyleMapper.cs
  SmartDiagram.Visio/
    VisioConnector.cs
    VisioRenderer.cs
  SmartDiagram.Storage/
    DatabaseContext.cs
    Repositories/
  SmartDiagram.Tests/
```

## 数据流

### 文本生成 draw.io

1. 用户在 WPF 左侧输入区提交文本。
2. ViewModel 调用模型解析服务。
3. 模型返回 Diagram JSON。
4. Core 层执行 JSON 反序列化和 Schema 校验。
5. Layout 模块计算节点坐标。
6. Drawio 模块生成 XML。
7. App 层将 XML 加载进 WebView2。
8. 用户导出 `.drawio` 文件。

### 文本生成 Visio

1. 用户在已有 Diagram JSON 基础上点击同步到 Visio。
2. Visio 服务检测运行实例。
3. 获取 ActivePage。
4. Layout 模块将坐标转换为 Visio 页面坐标。
5. VisioRenderer 创建 Shape 和连接线。
6. 保存 JSON 节点 ID 与 Visio Shape ID 映射。

### 自然语言修改

1. 用户输入修改指令。
2. 模型解析服务根据当前 Diagram JSON 生成操作 JSON。
3. Operations 模块执行操作序列。
4. Validation 模块校验更新后的 Diagram JSON。
5. Drawio 模块重新生成 XML。
6. Visio 模块按 Shape ID 执行增量更新。
7. Storage 模块保存快照和操作历史。

## 模块边界

- `SmartDiagram.Core` 不依赖 WPF、WebView2、Visio、SQLite。
- `SmartDiagram.Drawio` 只依赖 Core，输入 Diagram JSON，输出 XML。
- `SmartDiagram.Visio` 只负责 COM 连接和绘制，不调用大模型。
- `SmartDiagram.ModelApi` 只负责请求模型和解析响应，不直接绘图。
- `SmartDiagram.Storage` 只保存结构、快照、映射和配置。

## 错误处理原则

- 模型错误：返回可读错误和原始响应摘要，不进入绘图。
- JSON 校验失败：显示字段级错误，允许重新生成。
- draw.io 加载失败：允许导出 XML 文件并提示 WebView2 状态。
- Visio 不可用：提示降级，不影响 draw.io 功能。
- 图片识别不确定：生成 warnings，进入确认界面。

