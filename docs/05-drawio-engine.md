# draw.io 引擎设计

## 目标

draw.io 引擎负责将 Diagram JSON 转换为 draw.io XML，并支持保存 `.drawio` 文件和 WebView2 预览。该模块是系统第一阶段的核心交付。

## 输入与输出

输入：

- `DiagramDocument`
- 布局后的节点坐标
- 全局样式配置

输出：

- draw.io XML 字符串
- `.drawio` 文件
- 可加载到 WebView2 的编辑页面消息

## XML 结构

draw.io 文件本质上是 `mxfile` 与 `mxGraphModel`：

```xml
<mxfile host="app.diagrams.net">
  <diagram name="Page-1">
    <mxGraphModel>
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```

每个节点生成一个 `mxCell`，每条边生成一个 `mxCell edge="1"`。

## 节点映射

| Diagram shape | draw.io style |
|---|---|
| rectangle | `rounded=0;whiteSpace=wrap;html=1;` |
| rounded_rectangle | `rounded=1;whiteSpace=wrap;html=1;` |
| diamond | `rhombus;whiteSpace=wrap;html=1;` |
| ellipse | `ellipse;whiteSpace=wrap;html=1;` |
| parallelogram | `shape=parallelogram;whiteSpace=wrap;html=1;` |
| cylinder | `shape=cylinder3d;whiteSpace=wrap;html=1;boundedLbl=1;` |

## 边映射

默认样式：

```text
edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;strokeColor=#000000;
```

当 `line_type` 为 `straight` 时：

```text
edgeStyle=none;html=1;endArrow=none;strokeColor=#000000;
```

当 `arrow` 为 `classic` 时：

```text
endArrow=classic;
```

Chen ER 图默认 `endArrow=none`。

## 论文黑白样式

节点样式必须包含：

```text
fillColor=#FFFFFF;strokeColor=#000000;fontColor=#000000;shadow=0;fontSize=12;
```

不使用渐变、阴影、彩色填充。除非用户明确选择其他主题，默认所有图形使用白底黑线。

## 布局策略

MVP1 使用规则布局：

- 功能模块图：父节点在上，子节点在下，同级水平排列。
- 流程图：从上到下排列，判断节点使用菱形。
- Chen ER 图：关系节点居中，实体围绕关系节点，属性围绕实体。

后续扩展：

- 分层布局。
- 正交线优化。
- 连线交叉减少。
- draw.io 自动布局命令集成。

## WebView2 集成

WebView2 加载 diagrams.net embed 页面。WPF 与页面通过 `postMessage` 传递 XML。

基础流程：

1. WebView2 初始化完成。
2. 加载 draw.io embed URL。
3. 页面发送 `init` 消息。
4. WPF 发送生成的 XML。
5. 用户编辑。
6. 用户保存时 WPF 请求导出当前 XML。

## 导出

必须支持：

- `.drawio`：MVP1 必做。

后续支持：

- SVG。
- PNG。
- PDF。

SVG、PNG、PDF 可以优先通过 draw.io embed 导出能力实现，不在 Core 层自行渲染。

## 验收标准

- 示例 Diagram JSON 可以生成合法 XML。
- 生成文件扩展名为 `.drawio`。
- 文件能被 diagrams.net 打开。
- 功能模块图、流程图、Chen ER 图节点形状正确。
- Chen ER 图无箭头、无 Crow's Foot。
- 默认样式为白底黑线。

