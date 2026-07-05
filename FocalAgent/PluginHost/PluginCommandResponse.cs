using System.Text.Json;
using System.Text.Json.Serialization;

namespace FocalAgent.Plugins;

public sealed class PluginCommandResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public JsonElement? Data { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
