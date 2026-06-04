# 四阶段实施路线图

## 阶段一：draw.io 可演示闭环

目标：完成从文本到可编辑 draw.io 图的基础闭环。

任务：

1. 创建 C# WPF 解决方案和基础工程结构。
2. 创建 `DiagramDocument`、`DiagramNode`、`DiagramEdge`、`DiagramStyle`、`DiagramLayout` 类型。
3. 实现 Diagram JSON 反序列化和校验。
4. 实现 OpenAI 兼容模型配置和文本生成入口。
5. 编写功能模块图、流程图、Chen ER 图的提示词模板。
6. 实现三类图的规则布局。
7. 实现 Diagram JSON 到 draw.io XML 的转换。
8. 实现 `.drawio` 文件导出。
9. 集成 WebView2 预览。
10. 准备三个演示用例。

验收：

- 文本输入可以生成 Diagram JSON。
- 非法 JSON 会被拦截。
- 三类图能导出 `.drawio`。
- WebView2 能预览生成结果。

## 阶段二：Visio COM 自动绘图

目标：在当前 Visio 空白页自动绘制 Diagram JSON。

任务：

1. 添加 Visio COM 封装模块。
2. 实现 Visio 环境检测。
3. 实现 ActiveDocument 和 ActivePage 获取。
4. 实现 Diagram 坐标到 Visio 坐标转换。
5. 实现矩形、菱形、椭圆、圆角矩形绘制。
6. 实现连接线绘制。
7. 实现论文黑白样式。
8. 写入 Shape 自定义属性。
9. 保存 Shape 映射到 SQLite。
10. 在 UI 中加入同步 Visio 按钮和状态提示。

验收：

- Visio 不可用时能降级。
- Visio 可用时能绘制基础图形。
- Shape 中能查询到 Diagram 节点 ID。
- Chen ER 图边无箭头。

## 阶段三：自然语言增量修改

目标：支持基于当前图结构执行增量修改。

任务：

1. 定义操作 JSON 类型。
2. 实现操作 JSON 校验。
3. 实现 `add_node`、`delete_node`、`rename_node`。
4. 实现 `add_edge`、`delete_edge`、`update_edge`。
5. 实现 `update_style` 和 `relayout`。
6. 编写修改指令提示词模板。
7. 将修改前后快照保存到 SQLite。
8. 更新 draw.io XML 并刷新预览。
9. 基于 Shape 映射实现 Visio 增量修改。
10. UI 加入修改输入区和历史记录区。

验收：

- 重命名节点不丢失边。
- 删除节点会同步删除相关边。
- 全局样式修改能应用到 draw.io。
- Visio 可按节点 ID 定位并修改 Shape。
- 操作历史可追溯。

## 阶段四：多模态图片识别

目标：将图片或手绘图识别为可编辑 Diagram JSON。

任务：

1. 添加图片上传入口。
2. 支持本地图片预览。
3. 调用支持图片输入的大模型。
4. 将识别结果转换为 Diagram JSON。
5. 对识别结果执行校验。
6. 实现识别确认界面。
7. 允许用户修正节点和边。
8. 确认后生成 draw.io XML。
9. 可选同步 Visio。
10. 保存识别来源和 warnings。

验收：

- 图片识别结果不会直接覆盖当前项目。
- 用户确认后才生成图。
- 识别 warnings 可见。
- 确认后的 Diagram JSON 可导出 draw.io。

## 里程碑建议

| 里程碑 | 输出 |
|---|---|
| M1 | WPF 空工作台、设置页、示例 JSON 加载 |
| M2 | Diagram JSON 校验与三类图布局 |
| M3 | draw.io XML 导出与 WebView2 预览 |
| M4 | 模型生成接入 |
| M5 | Visio 自动绘图 |
| M6 | 增量修改与版本历史 |
| M7 | 多模态识别与确认界面 |

## 开发原则

- 每个阶段必须可独立演示。
- draw.io 路径始终保持可用。
- 复杂能力先整图重绘，再优化增量更新。
- 所有模型输出先校验后执行。
- 所有外部依赖失败都必须降级或给出明确提示。

