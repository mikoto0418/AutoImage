# Diagram JSON 规范

## 设计目标

Diagram JSON 是系统唯一的图结构中间层。大模型、draw.io、Visio、存储和自然语言修改都围绕该结构工作。

模型不得直接输出 draw.io XML 或 Visio COM 指令。所有绘图引擎必须从 Diagram JSON 读取数据。

## 顶层结构

```json
{
  "diagram_id": "diagram_001",
  "diagram_type": "chen_er",
  "title": "学生选课系统 ER 图",
  "style": {},
  "layout": {},
  "nodes": [],
  "edges": []
}
```

必填字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| diagram_id | string | 图唯一 ID |
| diagram_type | string | 图类型 |
| title | string | 图标题 |
| style | object | 全局样式 |
| layout | object | 布局策略 |
| nodes | array | 节点集合 |
| edges | array | 边集合 |

## 图类型

允许值：

- `function_module`
- `flowchart`
- `chen_er`
- `architecture`
- `hierarchy`
- `swimlane`
- `dfd`
- `simple_class`

MVP1 只实现：

- `function_module`
- `flowchart`
- `chen_er`

## 样式结构

```json
{
  "theme": "paper_black_white",
  "font": "SimSun",
  "font_size": 12,
  "line_color": "#000000",
  "fill_color": "#FFFFFF",
  "shadow": false,
  "arrow": "none",
  "line_type": "straight"
}
```

默认论文风格：

- 白色背景。
- 黑色线条。
- 无阴影。
- 无渐变。
- 宋体或微软雅黑。
- 直线或正交线。
- Chen ER 图默认无箭头。

## 布局结构

```json
{
  "type": "auto",
  "direction": "top_to_bottom",
  "node_gap": 80,
  "layer_gap": 120
}
```

允许方向：

- `top_to_bottom`
- `left_to_right`
- `right_to_left`
- `bottom_to_top`

## 节点结构

```json
{
  "id": "student",
  "label": "学生",
  "type": "entity",
  "shape": "rectangle",
  "parent_id": null,
  "x": 120,
  "y": 80,
  "width": 120,
  "height": 50,
  "attributes": []
}
```

必填字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| id | string | 节点唯一 ID |
| label | string | 节点显示文本 |
| type | string | 节点语义类型 |
| shape | string | 绘图形状 |

节点类型建议：

- `module`
- `process`
- `decision`
- `start`
- `end`
- `entity`
- `relationship`
- `attribute`
- `actor`
- `database`
- `class`

形状允许值：

- `rectangle`
- `rounded_rectangle`
- `diamond`
- `ellipse`
- `parallelogram`
- `cylinder`
- `swimlane`

## ER 属性结构

Chen ER 图中，实体属性可以挂在实体节点下：

```json
{
  "id": "student_id",
  "label": "学号",
  "type": "primary_key",
  "shape": "ellipse"
}
```

属性类型：

- `primary_key`
- `normal`
- `multi_value`
- `derived`

MVP1 只实现 `primary_key` 和 `normal`。

## 边结构

```json
{
  "id": "edge_student_select",
  "from": "student",
  "to": "select",
  "label": "N",
  "line_type": "straight",
  "arrow": "none"
}
```

必填字段：

| 字段 | 类型 | 说明 |
|---|---|---|
| id | string | 边唯一 ID |
| from | string | 起点节点 ID |
| to | string | 终点节点 ID |

可选字段：

- `label`
- `line_type`
- `arrow`
- `cardinality`

## 操作 JSON

自然语言修改必须转换为操作 JSON。

```json
{
  "operations": [
    {
      "operation": "rename_node",
      "target": "user",
      "new_label": "学生用户"
    }
  ]
}
```

允许操作：

- `add_node`
- `delete_node`
- `rename_node`
- `add_edge`
- `delete_edge`
- `update_edge`
- `update_style`
- `relayout`

## 校验规则

- `diagram_id`、`diagram_type`、`nodes`、`edges` 必须存在。
- `diagram_type` 必须在允许值内。
- 节点 ID 必须唯一。
- 边 ID 必须唯一。
- 每条边的 `from` 和 `to` 必须能找到对应节点。
- Chen ER 图中的关系节点必须使用 `diamond`。
- Chen ER 图中的实体节点必须使用 `rectangle`。
- Chen ER 图中的属性节点必须使用 `ellipse`。
- Chen ER 图边默认 `arrow` 为 `none`。

