using System.Text.Json;
using SmartDiagram.App.Components;

namespace SmartDiagram.Tests;

public sealed class DrawioPreviewHtmlBuilderTests
{
    [Fact]
    public void Build_embeds_diagrams_net_and_safely_serializes_drawio_xml()
    {
        const string xml = """
        <mxfile><diagram name="教学 &amp; 论文"></diagram></mxfile>
        """;

        var html = DrawioPreviewHtmlBuilder.Build(xml);

        Assert.Contains("https://embed.diagrams.net/?embed=1&proto=json&spin=1", html);
        Assert.Contains("JSON.stringify", html);
        Assert.Contains("action: 'load'", html);
        Assert.Contains("autosave: 0", html);
        Assert.DoesNotContain(xml, html);

        var encodedXml = JsonSerializer.Serialize(xml);
        Assert.Contains(encodedXml, html);
    }

    [Fact]
    public void Build_adds_status_and_fallback_markup()
    {
        var html = DrawioPreviewHtmlBuilder.Build("<mxfile />");

        Assert.Contains("drawio-status", html);
        Assert.Contains("正在加载 diagrams.net 预览", html);
        Assert.Contains("预览加载失败时仍可使用 WPF 简化预览和 draw.io 导出", html);
    }
}
