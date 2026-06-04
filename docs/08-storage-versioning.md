# 存储与版本管理设计

## 目标

系统使用 SQLite 保存项目、图结构快照、Visio Shape 映射和用户操作历史。存储层支持自然语言增量修改、撤销、恢复和演示案例复现。

## 数据库文件

默认数据库路径：

```text
%AppData%/SmartDiagram/smartdiagram.db
```

用户可在设置中选择项目导出目录，但数据库默认位置由应用管理。

## 表结构

### diagram_project

保存项目基本信息。

| 字段 | 类型 | 说明 |
|---|---|---|
| id | TEXT PRIMARY KEY | 项目 ID |
| name | TEXT NOT NULL | 项目名称 |
| diagram_type | TEXT NOT NULL | 图类型 |
| created_at | TEXT NOT NULL | 创建时间 |
| updated_at | TEXT NOT NULL | 更新时间 |

### diagram_snapshot

保存图结构快照。

| 字段 | 类型 | 说明 |
|---|---|---|
| id | TEXT PRIMARY KEY | 快照 ID |
| project_id | TEXT NOT NULL | 项目 ID |
| json_content | TEXT NOT NULL | Diagram JSON 内容 |
| drawio_xml | TEXT | draw.io XML 内容 |
| version_no | INTEGER NOT NULL | 版本号 |
| created_at | TEXT NOT NULL | 创建时间 |

约束：

- `(project_id, version_no)` 唯一。

### visio_shape_mapping

保存 Diagram 节点和 Visio Shape 的映射。

| 字段 | 类型 | 说明 |
|---|---|---|
| id | TEXT PRIMARY KEY | 映射 ID |
| project_id | TEXT NOT NULL | 项目 ID |
| diagram_element_id | TEXT NOT NULL | 节点或边 ID |
| element_type | TEXT NOT NULL | node 或 edge |
| shape_id | TEXT NOT NULL | Visio Shape ID |
| shape_name | TEXT | Visio Shape 名称 |
| updated_at | TEXT NOT NULL | 更新时间 |

### operation_history

保存自然语言操作记录。

| 字段 | 类型 | 说明 |
|---|---|---|
| id | TEXT PRIMARY KEY | 操作 ID |
| project_id | TEXT NOT NULL | 项目 ID |
| user_input | TEXT NOT NULL | 用户输入 |
| operation_json | TEXT NOT NULL | 操作 JSON |
| before_snapshot_id | TEXT | 修改前快照 |
| after_snapshot_id | TEXT | 修改后快照 |
| created_at | TEXT NOT NULL | 创建时间 |

### app_settings

保存本地配置。

| 字段 | 类型 | 说明 |
|---|---|---|
| key | TEXT PRIMARY KEY | 配置键 |
| value | TEXT | 配置值 |
| updated_at | TEXT NOT NULL | 更新时间 |

敏感配置如 API Key 应优先使用 Windows 凭据管理器或 DPAPI 加密后保存。

## 版本策略

- 每次成功生成图形后保存快照。
- 每次自然语言修改前后各保存一次快照。
- 手动编辑 draw.io 后，用户点击保存版本才写入快照。
- 同步 Visio 不单独生成 Diagram 快照，除非 Diagram JSON 同时变化。

## 撤销与恢复

撤销：

- 查找当前项目上一个快照。
- 恢复 Diagram JSON。
- 重新生成 draw.io XML。
- 如 Visio 已连接，提示是否重绘当前页。

恢复历史版本：

- 用户在历史面板选择版本。
- 系统显示版本摘要。
- 用户确认后恢复。

## 数据一致性

- 快照保存成功后才更新项目 `updated_at`。
- Visio 映射只对当前 Diagram JSON 有效。
- 重绘 Visio 页面后应清空旧映射并写入新映射。
- 删除节点时必须删除相关边和映射。

## 验收标准

- 能创建项目记录。
- 能保存 Diagram JSON 快照。
- 能按项目读取最新快照。
- 能保存和读取 Visio Shape 映射。
- 能记录自然语言修改前后的操作历史。
- 数据库异常不会导致当前内存中的 Diagram JSON 丢失。

