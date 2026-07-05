using System.Text.Json;

namespace FocalAgent.Plugins;

public sealed class PluginManager
{
    private const string PluginsDirectoryName = "plugins";
    private const string ManifestFileName = "plugin.json";
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public string PluginsDirectory { get; } = Path.Combine(AppContext.BaseDirectory, PluginsDirectoryName);

    public IReadOnlyList<InstalledPlugin> LoadPlugins()
    {
        if (!Directory.Exists(PluginsDirectory))
        {
            return [];
        }

        var plugins = new List<InstalledPlugin>();

        foreach (var manifestPath in Directory.EnumerateFiles(PluginsDirectory, ManifestFileName, SearchOption.AllDirectories))
        {
            var pluginDirectory = Path.GetDirectoryName(manifestPath);

            if (pluginDirectory is null)
            {
                continue;
            }

            var manifest = LoadManifest(manifestPath);
            ValidateManifest(manifest, manifestPath);
            plugins.Add(new InstalledPlugin(manifest, pluginDirectory));
        }

        return plugins
            .OrderBy(plugin => plugin.Manifest.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(plugin => plugin.Manifest.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private PluginManifest LoadManifest(string manifestPath)
    {
        using var stream = File.OpenRead(manifestPath);
        return JsonSerializer.Deserialize<PluginManifest>(stream, jsonOptions)
            ?? throw new InvalidOperationException($"Plugin manifest is empty: {manifestPath}");
    }

    private static void ValidateManifest(PluginManifest manifest, string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifest.Id))
        {
            throw new InvalidOperationException($"Plugin manifest is missing id: {manifestPath}");
        }

        if (string.IsNullOrWhiteSpace(manifest.Name))
        {
            throw new InvalidOperationException($"Plugin manifest is missing name: {manifestPath}");
        }

        if (string.IsNullOrWhiteSpace(manifest.Executable))
        {
            throw new InvalidOperationException($"Plugin manifest is missing executable: {manifestPath}");
        }

        if (manifest.Commands.Count == 0)
        {
            throw new InvalidOperationException($"Plugin manifest must declare at least one command: {manifestPath}");
        }

        if (manifest.Commands.Any(command => string.IsNullOrWhiteSpace(command.Method)))
        {
            throw new InvalidOperationException($"Plugin manifest has a command without a method: {manifestPath}");
        }
    }
}
