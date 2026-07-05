namespace FocalAgent.Plugins;

public sealed class PluginCommandManifest
{
    public string Method { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool RequiresPermission { get; init; } = true;
}
