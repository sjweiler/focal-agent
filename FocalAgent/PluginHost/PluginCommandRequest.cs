using System.Text.Json.Serialization;

namespace FocalAgent.Plugins;

public sealed class PluginCommandRequest
{
    public PluginCommandRequest(string method, IReadOnlyDictionary<string, object?>? args = null)
    {
        Method = method;
        Args = args;
    }

    [JsonPropertyName("method")]
    public string Method { get; }

    [JsonPropertyName("args")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, object?>? Args { get; }
}
