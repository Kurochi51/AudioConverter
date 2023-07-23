namespace AudioConverter
{
    partial class Window
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
            sourceLabel = new Label();
            sourceBox = new TextBox();
            sourceButton = new Button();
            outputButton = new Button();
            outputBox = new TextBox();
            destinationLabel = new Label();
            label2 = new Label();
            outputCombo = new ComboBox();
            convertButton = new Button();
            counterText = new Label();
            StyleIndicator = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)StyleIndicator).BeginInit();
            SuspendLayout();
            // 
            // sourceLabel
            // 
            sourceLabel.AutoSize = true;
            sourceLabel.Location = new Point(49, 64);
            sourceLabel.Name = "sourceLabel";
            sourceLabel.Size = new Size(54, 20);
            sourceLabel.TabIndex = 0;
            sourceLabel.Text = "Source";
            // 
            // sourceBox
            // 
            sourceBox.AllowDrop = true;
            sourceBox.BorderStyle = BorderStyle.FixedSingle;
            sourceBox.Location = new Point(109, 61);
            sourceBox.Name = "sourceBox";
            sourceBox.ReadOnly = true;
            sourceBox.Size = new Size(229, 27);
            sourceBox.TabIndex = 1;
            sourceBox.DragDrop += InputTextBox_DragDrop;
            sourceBox.DragEnter += InputTextBox_DragEnter;
            // 
            // sourceButton
            // 
            sourceButton.FlatAppearance.BorderSize = 0;
            sourceButton.FlatStyle = FlatStyle.System;
            sourceButton.Location = new Point(344, 60);
            sourceButton.Name = "sourceButton";
            sourceButton.Size = new Size(74, 29);
            sourceButton.TabIndex = 3;
            sourceButton.Text = "Locate";
            sourceButton.UseVisualStyleBackColor = true;
            sourceButton.Click += FolderPicker_Click;
            // 
            // outputButton
            // 
            outputButton.FlatAppearance.BorderSize = 0;
            outputButton.FlatStyle = FlatStyle.System;
            outputButton.Location = new Point(344, 123);
            outputButton.Name = "outputButton";
            outputButton.Size = new Size(74, 29);
            outputButton.TabIndex = 6;
            outputButton.Text = "Locate";
            outputButton.UseVisualStyleBackColor = true;
            outputButton.Click += FolderPicker_Click;
            // 
            // outputBox
            // 
            outputBox.AllowDrop = true;
            outputBox.BorderStyle = BorderStyle.FixedSingle;
            outputBox.Location = new Point(109, 124);
            outputBox.Name = "outputBox";
            outputBox.ReadOnly = true;
            outputBox.Size = new Size(229, 27);
            outputBox.TabIndex = 5;
            outputBox.DragDrop += InputTextBox_DragDrop;
            outputBox.DragEnter += InputTextBox_DragEnter;
            // 
            // destinationLabel
            // 
            destinationLabel.AutoSize = true;
            destinationLabel.Location = new Point(18, 127);
            destinationLabel.Name = "destinationLabel";
            destinationLabel.Size = new Size(85, 20);
            destinationLabel.TabIndex = 4;
            destinationLabel.Text = "Destination";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 184);
            label2.Name = "label2";
            label2.Size = new Size(90, 20);
            label2.TabIndex = 8;
            label2.Text = "Output Type";
            // 
            // outputCombo
            // 
            outputCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            outputCombo.FlatStyle = FlatStyle.System;
            outputCombo.FormattingEnabled = true;
            outputCombo.Location = new Point(109, 181);
            outputCombo.Name = "outputCombo";
            outputCombo.Size = new Size(109, 28);
            outputCombo.TabIndex = 9;
            // 
            // convertButton
            // 
            convertButton.FlatAppearance.BorderSize = 0;
            convertButton.FlatStyle = FlatStyle.System;
            convertButton.Location = new Point(167, 290);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(90, 37);
            convertButton.TabIndex = 10;
            convertButton.Text = "Convert";
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += ConvertButton_Click;
            // 
            // counterText
            // 
            counterText.AutoSize = true;
            counterText.Location = new Point(185, 232);
            counterText.Name = "counterText";
            counterText.Size = new Size(50, 20);
            counterText.TabIndex = 12;
            counterText.Text = "label3";
            // 
            // StyleIndicator
            // 
            StyleIndicator.BackgroundImage = Properties.Resources.moon;
            StyleIndicator.BackgroundImageLayout = ImageLayout.Zoom;
            StyleIndicator.InitialImage = null;
            StyleIndicator.Location = new Point(394, 291);
            StyleIndicator.Name = "StyleIndicator";
            StyleIndicator.Size = new Size(48, 48);
            StyleIndicator.TabIndex = 13;
            StyleIndicator.TabStop = false;
            StyleIndicator.WaitOnLoad = true;
            StyleIndicator.Click += DarkModeToggle_Click;
            // 
            // Window
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(454, 351);
            Controls.Add(StyleIndicator);
            Controls.Add(counterText);
            Controls.Add(convertButton);
            Controls.Add(outputCombo);
            Controls.Add(label2);
            Controls.Add(outputButton);
            Controls.Add(outputBox);
            Controls.Add(destinationLabel);
            Controls.Add(sourceButton);
            Controls.Add(sourceBox);
            Controls.Add(sourceLabel);
            MaximizeBox = false;
            Name = "Window";
            Text = "Audio Converter";
            ((System.ComponentModel.ISupportInitialize)StyleIndicator).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label sourceLabel;
        private TextBox sourceBox;
        private Button sourceButton;
        private Button outputButton;
        private TextBox outputBox;
        private Label destinationLabel;
        private Label label2;
        private ComboBox outputCombo;
        private Button convertButton;
        private Label counterText;
        private PictureBox StyleIndicator;
    }
}