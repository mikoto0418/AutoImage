# Visio COM 引擎设计

## 目标

Visio COM 引擎负责连接当前运行的 Microsoft Visio，并将 Diagram JSON 自动绘制到当前 ActivePage。该能力作为第二阶段实现，不影响第一阶段 draw.io 主流程。

## 环境要求

- Windows 桌面系统。
- 本机安装 Microsoft Visio。
- 用户已打开 Visio。
- 当前存在 ActiveDocument 和 ActivePage。

如果任一条件不满足，系统必须提示降级到 draw.io 模式。

## 模块职责

### VisioConnector

负责：

- 检测 Visio COM 类型是否可用。
- 获取当前运行的 Visio Application。
- 获取 ActiveDocument。
- 获取 ActivePage。
- 返回环境检测结果。

### VisioRenderer

负责：

- 根据 Diagram JSON 创建 Shape。
- 设置 Shape 文本。
- 设置线条、填充、字体。
- 创建连接线。
- 写入自定义属性。
- 返回 JSON 节点 ID 到 Visio Shape ID 的映射。

### VisioShapeMapper

负责：

- 将 `rectangle` 映射为矩形。
- 将 `diamond` 映射为菱形。
- 将 `ellipse` 映射为椭圆。
- 将 `rounded_rectangle` 映射为圆角矩形。
- 将边映射为连接器。

## 绘图流程

```text
检测 Visio 环境
  -> 获取 ActivePage
  -> 将 Diagram 坐标转换为 Visio 坐标
  -> 创建节点 Shape
  -> 写入节点文本和样式
  -> 写入 Shape 自定义属性 DiagramNodeId
  -> 创建连接线
  -> 保存 Shape 映射
```

## 坐标转换

Diagram JSON 使用像素式坐标，draw.io 与 WPF 更自然。Visio 页面使用英寸坐标。

实现时应定义转换器：

```text
visio_x = diagram_x / 96
visio_y = page_height - diagram_y / 96
visio_width = diagram_width / 96
visio_height = diagram_height / 96
```

实际实现中以页面高度和缩放比例为准，统一封装，避免散落在绘制逻辑中。

## 样式规则

默认论文风格：

- `LineColor` 黑色。
- `FillForegnd` 白色。
- 无阴影。
- 字号 12。
- 字体优先宋体或微软雅黑。
- Chen ER 边不使用箭头。

## Shape 自定义属性

每个节点 Shape 必须写入：

- `DiagramNodeId`
- `DiagramNodeType`
- `DiagramProjectId`

每条边 Shape 必须写入：

- `DiagramEdgeId`
- `DiagramFromNodeId`
- `DiagramToNodeId`

这些属性用于第三阶段自然语言增量修改定位。

## 增量更新能力

第三阶段需要支持：

- 根据 Shape ID 查找节点并重命名。
- 根据节点 ID 新增 Shape。
- 删除节点时删除 Shape 和相关连接线。
- 更新边样式。
- 全局应用论文黑白样式。

MVP2 可先实现整页重绘，第三阶段再实现真正增量更新。

## 错误处理

- Visio 未安装：提示“未检测到 Visio，请使用 draw.io 模式”。
- Visio 未运行：提示“请先打开 Visio 空白文档”。
- 无 ActivePage：提示“当前 Visio 没有可绘制页面”。
- COM 调用异常：记录异常信息，保留当前 Diagram JSON。

## 验收标准

- 未安装或未打开 Visio 时不会影响 draw.io 导出。
- 已打开 Visio 空白页时可以绘制矩形、菱形、椭圆和连接线。
- Chen ER 图绘制结果无箭头。
- 每个 Shape 可以找到对应 Diagram 节点 ID。
- 绘制失败时不会丢失当前项目状态。

