namespace FocalAgent.Plugins;

public sealed record InstalledPlugin(PluginManifest Manifest, string DirectoryPath)
{
    public string ExecutablePath
    {
        get
        {
            return Path.IsPathRooted(Manifest.Executable)
                ? Manifest.Executable
                : Path.GetFullPath(Path.Combine(DirectoryPath, Manifest.Executable));
        }
    }
}
