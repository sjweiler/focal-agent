using System.Diagnostics;
using System.Text.Json;

namespace FocalAgent.Plugins;

public sealed class PluginProcessClient
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public async Task<PluginCommandResponse> InvokeAsync(
        InstalledPlugin plugin,
        string method,
        IReadOnlyDictionary<string, object?>? args = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(plugin.ExecutablePath))
        {
            throw new FileNotFoundException(
                $"Plugin executable was not found for {plugin.Manifest.Name}. Expected: {plugin.ExecutablePath}",
                plugin.ExecutablePath);
        }

        using var process = StartPluginProcess(plugin);
        var request = new PluginCommandRequest(method, args);
        var requestJson = JsonSerializer.Serialize(request, jsonOptions);

        await process.StandardInput.WriteLineAsync(requestJson.AsMemory(), cancellationToken);
        process.StandardInput.Close();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(DefaultTimeout);

        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            TryKill(process);
            throw new TimeoutException($"Plugin {plugin.Manifest.Name} did not respond within {DefaultTimeout.TotalSeconds:n0} seconds.");
        }

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(stderr)
                    ? $"Plugin {plugin.Manifest.Name} exited with code {process.ExitCode}."
                    : $"Plugin {plugin.Manifest.Name} exited with code {process.ExitCode}: {stderr.Trim()}");
        }

        if (string.IsNullOrWhiteSpace(stdout))
        {
            throw new InvalidOperationException($"Plugin {plugin.Manifest.Name} returned an empty response.");
        }

        var response = JsonSerializer.Deserialize<PluginCommandResponse>(stdout, jsonOptions)
            ?? throw new InvalidOperationException($"Plugin {plugin.Manifest.Name} returned an invalid response.");

        if (!response.Success)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(response.Error)
                    ? $"Plugin {plugin.Manifest.Name} reported an unknown error."
                    : response.Error);
        }

        return response;
    }

    private static Process StartPluginProcess(InstalledPlugin plugin)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = plugin.ExecutablePath,
            WorkingDirectory = plugin.DirectoryPath,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not start plugin {plugin.Manifest.Name}.");
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }
}
