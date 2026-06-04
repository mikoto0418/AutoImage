using System.Text.Json;

namespace SmartDiagram.Model;

public static class LocalModelSettingsLoader
{
    public static string DefaultPath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoImage",
            "model.local.json");

    public static ModelClientSettings LoadDefault()
    {
        return Load(DefaultPath);
    }

    public static ModelClientSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return ModelClientSettings.CreateMissing();
        }

        var json = File.ReadAllText(path);
        var settings = JsonSerializer.Deserialize<LocalModelSettings>(json, JsonOptions) ?? new LocalModelSettings();

        return new ModelClientSettings(
            settings.Provider ?? string.Empty,
            TrimTrailingSlash(settings.BaseUrl),
            settings.Model ?? string.Empty,
            settings.ApiKey ?? string.Empty,
            settings.TimeoutSeconds <= 0 ? 30 : settings.TimeoutSeconds,
            settings.MaxTokens <= 0 ? 2048 : settings.MaxTokens,
            settings.Temperature);
    }

    private static string TrimTrailingSlash(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().TrimEnd('/');
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class LocalModelSettings
    {
        public string? Provider { get; set; }
        public string? BaseUrl { get; set; }
        public string? Model { get; set; }
        public string? ApiKey { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxTokens { get; set; } = 2048;
        public double Temperature { get; set; } = 0.1;
    }
}
