using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FocalAgent
{
    public partial class Form1 : Form
    {
        private const string LocalModelLabel = "Local model";
        private const string OllamaModel = "llama3";
        private const string PermissionPrefix = "PERMISSION_REQUEST:";
        private static readonly HttpClient OllamaClient = new()
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromMinutes(5)
        };

        public Form1()
        {
            InitializeComponent();
            modelSelector.SelectedIndex = 0;
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var prompt = promptTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(prompt))
            {
                promptTextBox.Focus();
                return;
            }

            userQueryText.Text = $"user: {prompt}";
            thinkingIndicator.Text = "Thinking...";
            promptTextBox.Clear();
            sendButton.Enabled = false;

            try
            {
                string aiResponse;

                if (IsLocalModelSelected())
                {
                    thinkingIndicator.Text = "Starting local model...";
                    aiResponse = await GenerateWithOllamaAsync(prompt);
                }
                else
                {
                    aiResponse = $"Using {modelSelector.SelectedItem}, I would respond to: {prompt}";
                }

                responseText.Text = $"ai: {aiResponse}";
                await HandlePermissionRequestAsync(prompt, aiResponse);
            }
            catch (HttpRequestException)
            {
                responseText.Text = "ai: Could not reach Ollama. Make sure Ollama is installed and can run the llama3 model.";
            }
            catch (TaskCanceledException)
            {
                responseText.Text = "ai: The local model request timed out.";
            }
            catch (InvalidOperationException ex)
            {
                responseText.Text = $"ai: {ex.Message}";
            }
            finally
            {
                sendButton.Enabled = true;
                thinkingIndicator.Text = "Ready";
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Settings will open here.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PluginButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Plugin controls will open here.", "Plugins", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static async Task<string> GenerateWithOllamaAsync(string prompt)
        {
            await EnsureOllamaIsReadyAsync();

            var request = new OllamaGenerateRequest(OllamaModel, BuildPrompt(prompt), false);
            using var response = await OllamaClient.PostAsJsonAsync("/api/generate", request);

            await EnsureOllamaSuccessAsync(response);

            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();
            return string.IsNullOrWhiteSpace(result?.Response)
                ? "Ollama returned an empty response."
                : result.Response;
        }

        private static async Task EnsureOllamaIsReadyAsync()
        {
            await EnsureOllamaIsRunningAsync();
            await EnsureOllamaModelIsAvailableAsync();
        }

        private static async Task EnsureOllamaIsRunningAsync()
        {
            if (await CanReachOllamaAsync())
            {
                return;
            }

            try
            {
                StartOllamaProcess("serve");
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                throw new InvalidOperationException("Could not start Ollama. Make sure the ollama command is installed and available on PATH.", ex);
            }

            for (var attempt = 0; attempt < 20; attempt++)
            {
                await Task.Delay(500);

                if (await CanReachOllamaAsync())
                {
                    return;
                }
            }

            throw new InvalidOperationException("Ollama was started, but the local API did not become available at http://localhost:11434.");
        }

        private static async Task EnsureOllamaModelIsAvailableAsync()
        {
            if (await HasOllamaModelAsync())
            {
                return;
            }

            var pullRequest = new OllamaPullRequest(OllamaModel, false);
            using var response = await OllamaClient.PostAsJsonAsync("/api/pull", pullRequest);
            await EnsureOllamaSuccessAsync(response);

            if (!await HasOllamaModelAsync())
            {
                throw new InvalidOperationException($"Ollama started, but the {OllamaModel} model is still not available.");
            }
        }

        private static async Task<bool> CanReachOllamaAsync()
        {
            try
            {
                using var response = await OllamaClient.GetAsync("/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        private static async Task<bool> HasOllamaModelAsync()
        {
            using var response = await OllamaClient.GetAsync("/api/tags");
            await EnsureOllamaSuccessAsync(response);

            var tags = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>();
            return tags?.Models.Any(model =>
                string.Equals(model.Name, OllamaModel, StringComparison.OrdinalIgnoreCase)
                || model.Name.StartsWith($"{OllamaModel}:", StringComparison.OrdinalIgnoreCase)) == true;
        }

        private static async Task EnsureOllamaSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? $"Ollama returned HTTP {(int)response.StatusCode}."
                : $"Ollama returned HTTP {(int)response.StatusCode}: {error}");
        }

        private static void StartOllamaProcess(string arguments)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private async Task HandlePermissionRequestAsync(string originalPrompt, string aiResponse)
        {
            var permissionRequest = GetPermissionRequest(aiResponse);

            if (permissionRequest is null)
            {
                return;
            }

            var result = MessageBox.Show(
                this,
                permissionRequest,
                "AI is asking for permission",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            var permissionDecision = result == DialogResult.Yes ? "granted" : "denied";
            responseText.Text = $"{responseText.Text}{Environment.NewLine}user permission: {permissionDecision}";

            if (!IsLocalModelSelected())
            {
                return;
            }

            thinkingIndicator.Text = "Continuing...";
            var followUpPrompt = $"""
                The user originally asked:
                {originalPrompt}

                You asked for permission:
                {permissionRequest}

                The user {permissionDecision} permission. Continue accordingly.
                """;
            var followUpResponse = await GenerateWithOllamaAsync(followUpPrompt);
            responseText.Text = $"{responseText.Text}{Environment.NewLine}ai: {followUpResponse}";
        }

        private static string? GetPermissionRequest(string aiResponse)
        {
            var trimmedResponse = aiResponse.Trim();

            return trimmedResponse.StartsWith(PermissionPrefix, StringComparison.OrdinalIgnoreCase)
                ? trimmedResponse[PermissionPrefix.Length..].Trim()
                : null;
        }

        private static string BuildPrompt(string prompt)
        {
            return $"""
                You are FocalAgent.
                If the user asks you to perform an action that needs user approval, such as changing files, running commands, installing software, using plugins, or accessing private data, ask first by replying exactly:
                {PermissionPrefix} <short reason>
                Otherwise answer normally.

                User:
                {prompt}
                """;
        }

        private bool IsLocalModelSelected()
        {
            return string.Equals(modelSelector.SelectedItem?.ToString(), LocalModelLabel, StringComparison.Ordinal);
        }

        private sealed record OllamaGenerateRequest(string Model, string Prompt, bool Stream);

        private sealed record OllamaGenerateResponse(string Response);

        private sealed record OllamaPullRequest(string Model, bool Stream);

        private sealed record OllamaTagsResponse(IReadOnlyList<OllamaModelInfo> Models);

        private sealed record OllamaModelInfo([property: JsonPropertyName("name")] string Name);
    }
}
