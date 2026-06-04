# AutoImage / SmartDiagram

Windows 本地智能绘图工具原型，面向论文、课程设计、系统设计文档和项目汇报场景。它把自然语言或结构化描述转换为统一的 `DiagramDocument`，再生成可编辑的 draw.io 文件，并在 WPF 桌面端提供预览、JSON、节点表、连线表和导出闭环。

> 当前阶段：MVP draw.io 闭环。Visio COM、WebView2 深度编辑、多模态识别会在后续阶段接入。

## 亮点

- **可编辑输出**：生成 `.drawio`，不是一次性的静态图片。
- **统一图模型**：`DiagramDocument` 贯穿模型生成、校验、预览、表格和导出。
- **本地密钥优先**：模型配置放在本地 `model.local.json`，默认不会进入 Git。
- **论文友好样式**：优先支持黑白、简洁、可打印的 Chen ER 图、流程图、功能模块图。
- **动态预览**：WPF 预览区根据当前 `DiagramDocument` 绘制节点与连线，生成结果和导出结果保持一致。

## 当前能力

| 模块 | 说明 |
|---|---|
| `SmartDiagram.Core` | 图结构模型、样例数据、JSON 序列化、结构校验、节点/连线表格适配 |
| `SmartDiagram.Model` | OpenAI 兼容接口调用、本地模型配置加载、提示词构建、模型响应解析 |
| `SmartDiagram.Drawio` | `DiagramDocument` 到 draw.io XML 的转换与 `.drawio` 文件导出 |
| `SmartDiagram.App` | WPF 桌面端原型：输入、生成、动态预览、JSON、节点表、连线表、导出 |
| `SmartDiagram.Tests` | 样例数据、校验器、序列化、模型客户端、draw.io 导出、动态预览测试 |

## 项目结构

```text
src/
  SmartDiagram.App/      WPF 桌面应用
  SmartDiagram.Core/     图模型、校验、样例和 UI 状态
  SmartDiagram.Drawio/   draw.io XML 生成与文件导出
  SmartDiagram.Model/    OpenAI 兼容模型调用
tests/
  SmartDiagram.Tests/    xUnit 测试
docs/                    需求、架构、规范和路线图
config/
  model.local.example.json
```

## 快速开始

### 1. 环境要求

- Windows 10/11
- .NET SDK 8.x
- Visual Studio 2022 或 `dotnet` CLI

### 2. 配置本地模型

复制示例配置：

```powershell
Copy-Item .\config\model.local.example.json .\model.local.json
```

编辑 `model.local.json`：

```json
{
  "provider": "siliconflow",
  "baseUrl": "https://api.siliconflow.cn/v1",
  "model": "deepseek-ai/DeepSeek-V4-Pro",
  "apiKey": "sk-your-local-key",
  "timeoutSeconds": 60,
  "maxTokens": 2048,
  "temperature": 0.1
}
```

`model.local.json` 和 `*.local.json` 已被 `.gitignore` 忽略，请不要提交真实密钥。

### 3. 构建与测试

```powershell
dotnet restore .\SmartDiagram.sln
dotnet build .\SmartDiagram.sln --no-restore
dotnet test .\SmartDiagram.sln --no-restore
```

### 4. 启动应用

```powershell
dotnet run --project .\src\SmartDiagram.App\SmartDiagram.App.csproj
```

## 典型流程

1. 在编辑页选择图类型，例如 Chen ER 图、功能模块图或流程图。
2. 输入论文/系统设计描述。
3. 点击生成，模型返回 Diagram JSON。
4. 系统校验结构，更新 WPF 动态预览、JSON、节点表和连线表。
5. 在导出页保存为 `.drawio`，继续用 diagrams.net 编辑。

## DiagramDocument 示例

```json
{
  "diagram_id": "teaching_management_chen_er",
  "diagram_type": "chen_er",
  "title": "教学管理系统 Chen ER 图",
  "nodes": [
    { "id": "student", "label": "学生", "type": "entity", "shape": "rectangle" },
    { "id": "course", "label": "课程", "type": "entity", "shape": "rectangle" },
    { "id": "enrollment", "label": "选课", "type": "relationship", "shape": "diamond" }
  ],
  "edges": [
    { "id": "r1", "from": "student", "to": "enrollment", "label": "N", "line_type": "straight", "arrow": "none" },
    { "id": "r2", "from": "enrollment", "to": "course", "label": "N", "line_type": "straight", "arrow": "none" }
  ]
}
```

## 路线图

- [x] WPF 原型界面与基础导航
- [x] `DiagramDocument` 核心模型与校验
- [x] OpenAI 兼容模型生成流程
- [x] WPF 动态预览当前图结构
- [x] draw.io XML 生成与 `.drawio` 导出
- [ ] WebView2 / diagrams.net 内嵌预览与编辑
- [ ] Visio COM 自动绘图
- [ ] 自然语言增量修改
- [ ] 图片识别后重绘为可编辑图
- [ ] SQLite 项目与版本管理

## 文档

- [项目总览](docs/00-project-overview.md)
- [需求说明](docs/01-requirements.md)
- [系统架构](docs/02-architecture.md)
- [Diagram JSON 规范](docs/03-diagram-json-spec.md)
- [模型 API 规范](docs/04-model-api-spec.md)
- [draw.io 引擎](docs/05-drawio-engine.md)
- [Visio COM 引擎](docs/06-visio-com-engine.md)
- [WPF UI 规范](docs/07-wpf-ui-spec.md)
- [存储与版本](docs/08-storage-versioning.md)
- [实施路线](docs/09-implementation-roadmap.md)
- [测试计划](docs/10-test-plan.md)

## 仓库说明建议

```text
Windows WPF 智能绘图原型：自然语言生成 DiagramDocument，动态预览并导出可编辑 draw.io 图。
```
