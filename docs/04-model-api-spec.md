# 大模型接口规范

## 接口目标

模型接口负责将用户输入转换为结构化 JSON。模型只输出 Diagram JSON 或操作 JSON，不输出 draw.io XML、Visio 指令或 UI 状态。

接口兼容 OpenAI 风格 Chat Completions 或 Responses API。具体 SDK 可在实现阶段选择，但请求配置必须支持：

- Base URL。
- API Key。
- 模型名称。
- 温度。
- 超时时间。
- 是否启用图像输入。

## 文本生成图形

### 输入

```json
{
  "input_type": "text",
  "diagram_type": "chen_er",
  "content": "生成一个学生选课系统 ER 图，包括学生、课程、教师、选课记录。",
  "style": "paper_black_white"
}
```

### 输出

```json
{
  "success": true,
  "diagram_json": {
    "diagram_id": "diagram_001",
    "diagram_type": "chen_er",
    "title": "学生选课系统 ER 图",
    "style": {},
    "layout": {},
    "nodes": [],
    "edges": []
  },
  "message": "图结构生成成功"
}
```

### 系统提示词约束

模型必须遵守：

- 只输出合法 JSON。
- 不输出 Markdown 代码块。
- 不输出解释文字。
- 节点 ID 使用英文小写、数字和下划线。
- 中文显示文本放在 `label` 字段。
- Chen ER 图必须使用实体矩形、关系菱形、属性椭圆。
- Chen ER 图默认不使用 Crow's Foot，不使用箭头。

## 表结构生成 ER 图

### 输入

```json
{
  "input_type": "schema_text",
  "diagram_type": "chen_er",
  "content": "student(id, name, gender, major)\ncourse(id, name, credit)\nsc(student_id, course_id, grade)",
  "style": "paper_black_white"
}
```

### 解析要求

- 表名优先作为实体。
- `id` 或以 `_id` 结尾字段优先识别为主键或外键。
- 中间表可识别为关系节点。
- 多对多关系使用实体、关系、实体结构表达。

## 自然语言修改

### 输入

```json
{
  "current_diagram_json": {},
  "user_instruction": "把所有箭头改成直线，并把用户管理模块下面增加角色管理"
}
```

### 输出

```json
{
  "success": true,
  "operations": [
    {
      "operation": "add_node",
      "target_parent": "user_management",
      "node": {
        "id": "role_management",
        "label": "角色管理",
        "type": "module",
        "shape": "rectangle"
      }
    },
    {
      "operation": "update_style",
      "target": "all_edges",
      "style": {
        "arrow": "none",
        "line_type": "straight"
      }
    }
  ]
}
```

## 图片识别

### 输入

```json
{
  "input_type": "image",
  "image_path": "C:/Users/demo/Desktop/example.png",
  "diagram_type_hint": "flowchart"
}
```

### 输出

```json
{
  "success": true,
  "diagram_type": "flowchart",
  "diagram_json": {},
  "warnings": [
    "部分连接线方向识别不确定，建议用户确认"
  ]
}
```

### 识别要求

- 识别节点文字。
- 识别节点形状。
- 识别节点之间的连接关系。
- 识别整体布局方向。
- 不确定内容必须放入 `warnings`。
- 不得直接覆盖当前 Diagram JSON。

## 错误返回

```json
{
  "success": false,
  "message": "模型返回内容不是合法 JSON",
  "raw_response_excerpt": "前 500 个字符摘要"
}
```

常见错误：

- 模型超时。
- API Key 无效。
- 返回非 JSON。
- JSON 缺少必填字段。
- 节点或边引用不合法。

## 重试与修复策略

- 第一次失败：用原始响应触发 JSON 修复提示。
- 第二次失败：返回字段级错误给用户。
- 不允许无限重试。
- 不允许在校验失败时进入绘图引擎。

