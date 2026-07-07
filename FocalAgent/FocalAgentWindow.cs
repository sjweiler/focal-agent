using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FocalAgent.Plugins;
using NAudio.Wave;
using Vosk;

namespace FocalAgent
{
    public partial class frmFocalAgent : Form
    {
        private const string LocalModelLabel = "Local model";
        private const string OllamaModel = "llama3";
        private const string PermissionPrefix = "PERMISSION_REQUEST:";
        private const string VoskModelDirectoryName = "vosk-model-small-en-us-0.15";
        private const int SpeechSampleRate = 16000;
        private static readonly JsonSerializerOptions PrettyJsonOptions = new()
        {
            WriteIndented = true
        };
        private static readonly HttpClient OllamaClient = new()
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromMinutes(5)
        };
        private readonly PluginManager pluginManager = new();
        private readonly PluginProcessClient pluginClient = new();
        private readonly object speechLock = new();
        private Model? speechModel;
        private VoskRecognizer? speechRecognizer;
        private WaveInEvent? microphone;
        private bool isListening;

        public frmFocalAgent()
        {
            InitializeComponent();
            modelSelector.SelectedIndex = 0;
            toolStripMenuItem1.Click += ShowFocalAgent_Click;
            toolStripMenuItem2.Click += StartListening_Click;
            stopListeningToolStripMenuItem.Click += StopListening_Click;
            exitToolStripMenuItem.Click += Exit_Click;
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
            try
            {
                var plugins = pluginManager.LoadPlugins();

                if (plugins.Count == 0)
                {
                    MessageBox.Show(this, $"No plugins found in:{Environment.NewLine}{pluginManager.PluginsDirectory}", "Plugins", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ShowPluginMenu(plugins);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Plugins", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowPluginMenu(IReadOnlyList<InstalledPlugin> plugins)
        {
            var menu = new ContextMenuStrip();

            foreach (var plugin in plugins)
            {
                var pluginItem = new ToolStripMenuItem($"{plugin.Manifest.Name} {plugin.Manifest.Version}".Trim());

                foreach (var command in plugin.Manifest.Commands)
                {
                    var commandItem = new ToolStripMenuItem(command.Method)
                    {
                        ToolTipText = command.Description,
                        Tag = (plugin, command)
                    };
                    commandItem.Click += PluginCommand_Click;
                    pluginItem.DropDownItems.Add(commandItem);
                }

                menu.Items.Add(pluginItem);
            }

            menu.Show(pluginButton, new Point(0, pluginButton.Height));
        }

        private async void PluginCommand_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ValueTuple<InstalledPlugin, PluginCommandManifest> tag })
            {
                return;
            }

            await RunPluginCommandAsync(tag.Item1, tag.Item2);
        }

        private async Task RunPluginCommandAsync(InstalledPlugin plugin, PluginCommandManifest command)
        {
            if (command.RequiresPermission)
            {
                var decision = MessageBox.Show(
                    this,
                    $"{plugin.Manifest.Name} wants to run {command.Method}.{Environment.NewLine}{command.Description}",
                    "Run plugin command?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (decision != DialogResult.Yes)
                {
                    return;
                }
            }

            userQueryText.Text = $"plugin: {plugin.Manifest.Id}/{command.Method}";
            responseText.Text = "plugin: Running...";
            thinkingIndicator.Text = "Running plugin...";
            pluginButton.Enabled = false;

            try
            {
                var result = await pluginClient.InvokeAsync(plugin, command.Method);
                responseText.Text = $"plugin: {plugin.Manifest.Name}/{command.Method}{Environment.NewLine}{FormatPluginData(result.Data)}";
            }
            catch (Exception ex)
            {
                responseText.Text = $"plugin error: {ex.Message}";
            }
            finally
            {
                pluginButton.Enabled = true;
                thinkingIndicator.Text = "Ready";
            }
        }

        private static string FormatPluginData(JsonElement? data)
        {
            return data is null
                ? "(no data)"
                : JsonSerializer.Serialize(data.Value, PrettyJsonOptions);
        }

        private void VoiceButton_Click(object sender, EventArgs e)
        {
            if (isListening)
            {
                StopListening();
                return;
            }

            StartListening();
        }

        private void ShowFocalAgent_Click(object? sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void StartListening_Click(object? sender, EventArgs e)
        {
            StartListening();
        }

        private void StopListening_Click(object? sender, EventArgs e)
        {
            StopListening();
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void StartListening()
        {
            if (isListening)
            {
                return;
            }

            try
            {
                Vosk.Vosk.SetLogLevel(-1);
                var model = GetOrCreateSpeechModel();
                speechRecognizer = new VoskRecognizer(model, SpeechSampleRate);
                speechRecognizer.SetWords(true);

                microphone = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SpeechSampleRate, 1),
                    BufferMilliseconds = 50
                };
                microphone.DataAvailable += Microphone_DataAvailable;
                microphone.RecordingStopped += Microphone_RecordingStopped;
                microphone.StartRecording();

                isListening = true;
                voiceButton.Text = "Stop";
                toolStripMenuItem2.Enabled = false;
                stopListeningToolStripMenuItem.Enabled = true;
                thinkingIndicator.Text = "Listening...";
            }
            catch (Exception ex)
            {
                StopListening();
                MessageBox.Show(this, $"Could not start voice recognition:{Environment.NewLine}{ex.Message}", "Voice recognition", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StopListening()
        {
            if (!isListening && microphone is null && speechRecognizer is null)
            {
                return;
            }

            isListening = false;
            microphone?.StopRecording();
            DisposeMicrophone();
            DisposeRecognizer();
            voiceButton.Text = "Voice";
            toolStripMenuItem2.Enabled = true;
            stopListeningToolStripMenuItem.Enabled = false;
            thinkingIndicator.Text = "Ready";
        }

        private void Microphone_DataAvailable(object? sender, WaveInEventArgs e)
        {
            lock (speechLock)
            {
                if (speechRecognizer is null)
                {
                    return;
                }

                if (speechRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                {
                    AppendRecognizedText(GetRecognizedText(speechRecognizer.Result()));
                }
            }
        }

        private void Microphone_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception is not null && !IsDisposed)
            {
                BeginInvoke(() =>
                {
                    StopListening();
                    MessageBox.Show(this, e.Exception.Message, "Microphone stopped", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });
            }
        }

        private void AppendRecognizedText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || IsDisposed)
            {
                return;
            }

            BeginInvoke(() =>
            {
                promptTextBox.Text = string.IsNullOrWhiteSpace(promptTextBox.Text)
                    ? text
                    : $"{promptTextBox.Text.TrimEnd()} {text}";
                promptTextBox.SelectionStart = promptTextBox.TextLength;
                promptTextBox.Focus();
            });
        }

        private static string GetRecognizedText(string resultJson)
        {
            using var document = JsonDocument.Parse(resultJson);
            return document.RootElement.TryGetProperty("text", out var textElement)
                ? textElement.GetString() ?? string.Empty
                : string.Empty;
        }

        private Model GetOrCreateSpeechModel()
        {
            if (speechModel is not null)
            {
                return speechModel;
            }

            var modelPath = GetVoskModelPath();
            speechModel = new Model(modelPath);
            return speechModel;
        }

        private static string GetVoskModelPath()
        {
            var modelPath = Path.Combine(AppContext.BaseDirectory, "models", VoskModelDirectoryName);
            var nestedModelPath = Path.Combine(modelPath, VoskModelDirectoryName);

            if (IsVoskModelDirectory(modelPath))
            {
                return modelPath;
            }

            if (IsVoskModelDirectory(nestedModelPath))
            {
                return nestedModelPath;
            }

            throw new InvalidOperationException(
                $"Vosk model not found or not fully extracted. Expected folders like am, conf, and graph under:{Environment.NewLine}{modelPath}{Environment.NewLine}{Environment.NewLine}If your unzip created a nested folder, that is okay too; FocalAgent also checks:{Environment.NewLine}{nestedModelPath}");
        }

        private static bool IsVoskModelDirectory(string path)
        {
            return Directory.Exists(Path.Combine(path, "am"))
                && Directory.Exists(Path.Combine(path, "conf"))
                && Directory.Exists(Path.Combine(path, "graph"));
        }

        private void DisposeMicrophone()
        {
            if (microphone is null)
            {
                return;
            }

            microphone.DataAvailable -= Microphone_DataAvailable;
            microphone.RecordingStopped -= Microphone_RecordingStopped;
            microphone.Dispose();
            microphone = null;
        }

        private void DisposeRecognizer()
        {
            lock (speechLock)
            {
                speechRecognizer?.Dispose();
                speechRecognizer = null;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            StopListening();
            speechModel?.Dispose();
            speechModel = null;
            base.OnFormClosed(e);
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
