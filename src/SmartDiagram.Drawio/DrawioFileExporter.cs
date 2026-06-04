using System.Text;
using SmartDiagram.Core.Diagram;

namespace SmartDiagram.Drawio;

public static class DrawioFileExporter
{
    public static string Save(DiagramDocument document, string requestedPath)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (string.IsNullOrWhiteSpace(requestedPath))
        {
            throw new ArgumentException("Export path cannot be empty.", nameof(requestedPath));
        }

        var targetPath = EnsureDrawioExtension(requestedPath);
        var directory = Path.GetDirectoryName(Path.GetFullPath(targetPath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(targetPath, DrawioXmlGenerator.Generate(document), Encoding.UTF8);
        return targetPath;
    }

    private static string EnsureDrawioExtension(string path)
    {
        return path.EndsWith(".drawio", StringComparison.OrdinalIgnoreCase)
            ? path
            : Path.ChangeExtension(path, ".drawio");
    }
}
