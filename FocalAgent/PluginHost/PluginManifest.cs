namespace FocalAgent.Plugins;

public sealed class PluginManifest
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Executable { get; init; } = string.Empty;

    public IReadOnlyList<PluginCommandManifest> Commands { get; init; } = [];
}
