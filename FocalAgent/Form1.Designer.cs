namespace FocalAgent
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            stopListeningToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            headerPanel = new TableLayoutPanel();
            modelSelector = new ComboBox();
            thinkingIndicator = new Label();
            settingsButton = new Button();
            pluginButton = new Button();
            mainPanel = new TableLayoutPanel();
            userCard = new Panel();
            userCardLayout = new TableLayoutPanel();
            userCardTitle = new Label();
            userQueryText = new Label();
            responseCard = new Panel();
            responseCardLayout = new TableLayoutPanel();
            responseHeaderLayout = new TableLayoutPanel();
            responseCardTitle = new Label();
            responseText = new TextBox();
            composerPanel = new TableLayoutPanel();
            promptTextBox = new TextBox();
            voiceButton = new Button();
            sendButton = new Button();
            contextMenuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            headerPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            userCard.SuspendLayout();
            userCardLayout.SuspendLayout();
            responseCard.SuspendLayout();
            responseCardLayout.SuspendLayout();
            responseHeaderLayout.SuspendLayout();
            composerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "FocalAgent";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, stopListeningToolStripMenuItem, settingsToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(223, 164);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(222, 32);
            toolStripMenuItem1.Text = "Show FocalAgent";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(222, 32);
            toolStripMenuItem2.Text = "Start Listening";
            // 
            // stopListeningToolStripMenuItem
            // 
            stopListeningToolStripMenuItem.Name = "stopListeningToolStripMenuItem";
            stopListeningToolStripMenuItem.Size = new Size(222, 32);
            stopListeningToolStripMenuItem.Text = "Stop Listening";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(222, 32);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(222, 32);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(244, 246, 248);
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(headerPanel, 0, 0);
            tableLayoutPanel1.Controls.Add(mainPanel, 0, 1);
            tableLayoutPanel1.Controls.Add(composerPanel, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(16);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
            tableLayoutPanel1.Size = new Size(980, 620);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.ColumnCount = 4;
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104F));
            headerPanel.Controls.Add(modelSelector, 0, 0);
            headerPanel.Controls.Add(thinkingIndicator, 1, 0);
            headerPanel.Controls.Add(settingsButton, 2, 0);
            headerPanel.Controls.Add(pluginButton, 3, 0);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(16, 16);
            headerPanel.Margin = new Padding(0, 0, 0, 12);
            headerPanel.Name = "headerPanel";
            headerPanel.RowCount = 1;
            headerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            headerPanel.Size = new Size(948, 40);
            headerPanel.TabIndex = 0;
            // 
            // modelSelector
            // 
            modelSelector.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            modelSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            modelSelector.FormattingEnabled = true;
            modelSelector.Items.AddRange(new object[] { "GPT-5", "GPT-5 mini", "Local model" });
            modelSelector.Location = new Point(0, 3);
            modelSelector.Margin = new Padding(0, 0, 12, 0);
            modelSelector.Name = "modelSelector";
            modelSelector.Size = new Size(208, 33);
            modelSelector.TabIndex = 0;
            // 
            // thinkingIndicator
            // 
            thinkingIndicator.Anchor = AnchorStyles.Left;
            thinkingIndicator.AutoSize = true;
            thinkingIndicator.ForeColor = Color.FromArgb(89, 99, 110);
            thinkingIndicator.Location = new Point(220, 7);
            thinkingIndicator.Margin = new Padding(0);
            thinkingIndicator.Name = "thinkingIndicator";
            thinkingIndicator.Size = new Size(113, 25);
            thinkingIndicator.TabIndex = 1;
            thinkingIndicator.Text = "Ready";
            // 
            // settingsButton
            // 
            settingsButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            settingsButton.Location = new Point(735, 0);
            settingsButton.Margin = new Padding(0, 0, 8, 0);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(104, 40);
            settingsButton.TabIndex = 2;
            settingsButton.Text = "Settings";
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += SettingsButton_Click;
            // 
            // pluginButton
            // 
            pluginButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            pluginButton.Location = new Point(847, 0);
            pluginButton.Margin = new Padding(0);
            pluginButton.Name = "pluginButton";
            pluginButton.Size = new Size(101, 40);
            pluginButton.TabIndex = 3;
            pluginButton.Text = "Plugins";
            pluginButton.UseVisualStyleBackColor = true;
            pluginButton.Click += PluginButton_Click;
            // 
            // mainPanel
            // 
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(userCard, 0, 0);
            mainPanel.Controls.Add(responseCard, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(16, 68);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
            mainPanel.Size = new Size(948, 452);
            mainPanel.TabIndex = 1;
            // 
            // userCard
            // 
            userCard.BackColor = Color.White;
            userCard.BorderStyle = BorderStyle.FixedSingle;
            userCard.Controls.Add(userCardLayout);
            userCard.Dock = DockStyle.Fill;
            userCard.Location = new Point(0, 0);
            userCard.Margin = new Padding(0, 0, 0, 12);
            userCard.Name = "userCard";
            userCard.Padding = new Padding(16);
            userCard.Size = new Size(948, 177);
            userCard.TabIndex = 0;
            // 
            // userCardLayout
            // 
            userCardLayout.ColumnCount = 1;
            userCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            userCardLayout.Controls.Add(userCardTitle, 0, 0);
            userCardLayout.Controls.Add(userQueryText, 0, 1);
            userCardLayout.Dock = DockStyle.Fill;
            userCardLayout.Location = new Point(16, 16);
            userCardLayout.Name = "userCardLayout";
            userCardLayout.RowCount = 2;
            userCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            userCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            userCardLayout.Size = new Size(914, 143);
            userCardLayout.TabIndex = 0;
            // 
            // userCardTitle
            // 
            userCardTitle.AutoSize = true;
            userCardTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            userCardTitle.ForeColor = Color.FromArgb(43, 50, 58);
            userCardTitle.Location = new Point(0, 0);
            userCardTitle.Margin = new Padding(0);
            userCardTitle.Name = "userCardTitle";
            userCardTitle.Size = new Size(100, 25);
            userCardTitle.TabIndex = 0;
            userCardTitle.Text = "User query";
            // 
            // userQueryText
            // 
            userQueryText.Dock = DockStyle.Fill;
            userQueryText.ForeColor = Color.FromArgb(43, 50, 58);
            userQueryText.Location = new Point(0, 32);
            userQueryText.Margin = new Padding(0);
            userQueryText.Name = "userQueryText";
            userQueryText.Size = new Size(914, 111);
            userQueryText.TabIndex = 1;
            userQueryText.Text = "Ask FocalAgent for help with the current task.";
            // 
            // responseCard
            // 
            responseCard.BackColor = Color.White;
            responseCard.BorderStyle = BorderStyle.FixedSingle;
            responseCard.Controls.Add(responseCardLayout);
            responseCard.Dock = DockStyle.Fill;
            responseCard.Location = new Point(0, 189);
            responseCard.Margin = new Padding(0);
            responseCard.Name = "responseCard";
            responseCard.Padding = new Padding(16);
            responseCard.Size = new Size(948, 263);
            responseCard.TabIndex = 1;
            // 
            // responseCardLayout
            // 
            responseCardLayout.ColumnCount = 1;
            responseCardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            responseCardLayout.Controls.Add(responseHeaderLayout, 0, 0);
            responseCardLayout.Controls.Add(responseText, 0, 1);
            responseCardLayout.Dock = DockStyle.Fill;
            responseCardLayout.Location = new Point(16, 16);
            responseCardLayout.Name = "responseCardLayout";
            responseCardLayout.RowCount = 2;
            responseCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            responseCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            responseCardLayout.Size = new Size(914, 229);
            responseCardLayout.TabIndex = 0;
            // 
            // responseHeaderLayout
            // 
            responseHeaderLayout.ColumnCount = 1;
            responseHeaderLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            responseHeaderLayout.Controls.Add(responseCardTitle, 0, 0);
            responseHeaderLayout.Dock = DockStyle.Fill;
            responseHeaderLayout.Location = new Point(0, 0);
            responseHeaderLayout.Margin = new Padding(0);
            responseHeaderLayout.Name = "responseHeaderLayout";
            responseHeaderLayout.RowCount = 1;
            responseHeaderLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            responseHeaderLayout.Size = new Size(914, 44);
            responseHeaderLayout.TabIndex = 0;
            // 
            // responseCardTitle
            // 
            responseCardTitle.Anchor = AnchorStyles.Left;
            responseCardTitle.AutoSize = true;
            responseCardTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            responseCardTitle.ForeColor = Color.FromArgb(43, 50, 58);
            responseCardTitle.Location = new Point(0, 9);
            responseCardTitle.Margin = new Padding(0);
            responseCardTitle.Name = "responseCardTitle";
            responseCardTitle.Size = new Size(113, 25);
            responseCardTitle.TabIndex = 0;
            responseCardTitle.Text = "AI response";
            // 
            // responseText
            // 
            responseText.BackColor = Color.White;
            responseText.BorderStyle = BorderStyle.None;
            responseText.Dock = DockStyle.Fill;
            responseText.ForeColor = Color.FromArgb(43, 50, 58);
            responseText.Location = new Point(0, 44);
            responseText.Margin = new Padding(0);
            responseText.Multiline = true;
            responseText.Name = "responseText";
            responseText.ReadOnly = true;
            responseText.ScrollBars = ScrollBars.Vertical;
            responseText.Size = new Size(914, 185);
            responseText.TabIndex = 1;
            responseText.Text = "The assistant response will appear here. Permission requests can open a confirmation dialog before continuing.";
            // 
            // composerPanel
            // 
            composerPanel.ColumnCount = 3;
            composerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            composerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            composerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            composerPanel.Controls.Add(promptTextBox, 0, 0);
            composerPanel.Controls.Add(voiceButton, 1, 0);
            composerPanel.Controls.Add(sendButton, 2, 0);
            composerPanel.Dock = DockStyle.Fill;
            composerPanel.Location = new Point(16, 536);
            composerPanel.Margin = new Padding(0, 16, 0, 0);
            composerPanel.Name = "composerPanel";
            composerPanel.RowCount = 1;
            composerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            composerPanel.Size = new Size(948, 68);
            composerPanel.TabIndex = 2;
            // 
            // promptTextBox
            // 
            promptTextBox.AcceptsReturn = true;
            promptTextBox.Dock = DockStyle.Fill;
            promptTextBox.Location = new Point(0, 0);
            promptTextBox.Margin = new Padding(0, 0, 12, 0);
            promptTextBox.Multiline = true;
            promptTextBox.Name = "promptTextBox";
            promptTextBox.PlaceholderText = "Type a prompt...";
            promptTextBox.Size = new Size(712, 68);
            promptTextBox.TabIndex = 0;
            // 
            // voiceButton
            // 
            voiceButton.Dock = DockStyle.Fill;
            voiceButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            voiceButton.Location = new Point(724, 0);
            voiceButton.Margin = new Padding(0, 0, 12, 0);
            voiceButton.Name = "voiceButton";
            voiceButton.Size = new Size(100, 68);
            voiceButton.TabIndex = 1;
            voiceButton.Text = "Voice";
            voiceButton.UseVisualStyleBackColor = true;
            voiceButton.Click += VoiceButton_Click;
            // 
            // sendButton
            // 
            sendButton.Dock = DockStyle.Fill;
            sendButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            sendButton.Location = new Point(836, 0);
            sendButton.Margin = new Padding(0);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(112, 68);
            sendButton.TabIndex = 2;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += SendButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 620);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new Size(760, 480);
            Name = "Form1";
            Text = "Focal Agent";
            contextMenuStrip1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            mainPanel.ResumeLayout(false);
            userCard.ResumeLayout(false);
            userCardLayout.ResumeLayout(false);
            userCardLayout.PerformLayout();
            responseCard.ResumeLayout(false);
            responseCardLayout.ResumeLayout(false);
            responseHeaderLayout.ResumeLayout(false);
            responseHeaderLayout.PerformLayout();
            composerPanel.ResumeLayout(false);
            composerPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem stopListeningToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel headerPanel;
        private ComboBox modelSelector;
        private Label thinkingIndicator;
        private Button settingsButton;
        private Button pluginButton;
        private TableLayoutPanel mainPanel;
        private Panel userCard;
        private TableLayoutPanel userCardLayout;
        private Label userCardTitle;
        private Label userQueryText;
        private Panel responseCard;
        private TableLayoutPanel responseCardLayout;
        private TableLayoutPanel responseHeaderLayout;
        private Label responseCardTitle;
        private TextBox responseText;
        private TableLayoutPanel composerPanel;
        private TextBox promptTextBox;
        private Button voiceButton;
        private Button sendButton;
    }
}
