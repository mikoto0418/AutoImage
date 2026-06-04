using System.Text.Json;

namespace SmartDiagram.App.Components;

public static class DrawioPreviewHtmlBuilder
{
    private const string DiagramsEmbedUrl = "https://embed.diagrams.net/?embed=1&proto=json&spin=1";

    public static string Build(string drawioXml)
    {
        ArgumentNullException.ThrowIfNull(drawioXml);

        var payload = JsonSerializer.Serialize(drawioXml);

        return $$"""
        <!doctype html>
        <html lang="zh-CN">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>SmartDiagram draw.io Preview</title>
          <style>
            html,
            body {
              width: 100%;
              height: 100%;
              margin: 0;
              overflow: hidden;
              background: #f8fafc;
              font-family: "Microsoft YaHei UI", "Segoe UI", sans-serif;
            }

            #drawio-frame {
              width: 100%;
              height: 100%;
              border: 0;
              background: #ffffff;
            }

            #drawio-status {
              position: fixed;
              left: 14px;
              bottom: 14px;
              z-index: 2;
              max-width: min(520px, calc(100vw - 28px));
              padding: 8px 11px;
              border: 1px solid #dbeafe;
              border-radius: 6px;
              background: rgba(239, 246, 255, 0.96);
              color: #1d4ed8;
              font-size: 12px;
              line-height: 1.5;
              box-shadow: 0 8px 20px rgba(15, 23, 42, 0.08);
            }

            #drawio-status[data-state="ready"] {
              color: #166534;
              border-color: #bbf7d0;
              background: rgba(240, 253, 244, 0.96);
            }

            #drawio-status[data-state="error"] {
              color: #991b1b;
              border-color: #fecaca;
              background: rgba(254, 242, 242, 0.96);
            }
          </style>
        </head>
        <body>
          <iframe id="drawio-frame" src="{{DiagramsEmbedUrl}}" title="diagrams.net preview"></iframe>
          <div id="drawio-status">正在加载 diagrams.net 预览。预览加载失败时仍可使用 WPF 简化预览和 draw.io 导出。</div>
          <script>
            const drawioXml = {{payload}};
            const frame = document.getElementById('drawio-frame');
            const status = document.getElementById('drawio-status');
            let loaded = false;

            function updateStatus(text, state) {
              status.textContent = text;
              status.dataset.state = state;
            }

            function sendLoad() {
              frame.contentWindow.postMessage(JSON.stringify({
                action: 'load',
                xml: drawioXml,
                autosave: 0,
                modified: false
              }), '*');
            }

            window.addEventListener('message', event => {
              let message;
              try {
                message = typeof event.data === 'string' ? JSON.parse(event.data) : event.data;
              } catch {
                return;
              }

              if (!message || !message.event) {
                return;
              }

              if (message.event === 'init') {
                sendLoad();
              }

              if (message.event === 'load') {
                loaded = true;
                updateStatus('diagrams.net 预览已加载', 'ready');
              }
            });

            frame.addEventListener('load', () => {
              window.setTimeout(() => {
                if (!loaded) {
                  updateStatus('正在等待 diagrams.net 初始化...', 'loading');
                }
              }, 1200);
            });

            window.setTimeout(() => {
              if (!loaded) {
                updateStatus('diagrams.net 预览暂未就绪，可继续使用 WPF 简化预览或直接导出 .drawio。', 'error');
              }
            }, 9000);
          </script>
        </body>
        </html>
        """;
    }
}
