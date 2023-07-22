using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Text;

namespace AudioConverter
{
    public partial class Window : Form
    {
        private int totalFiles, counter;
        private readonly string[] allOptions = { "MP3", "WAV", "OGG" };
        private bool isDarkModeEnabled;

        public Window()
        {
            InitializeComponent();
            ControlsInitialize();
        }
        private void ControlsInitialize()
        {
            outputCombo.Items.AddRange(allOptions);
            outputCombo.SelectedItem = outputCombo.Items[0];
            convertButton.Left += this.Left + (this.Width / 2) - (convertButton.Left + (convertButton.Width / 2));
            counterText.Left += convertButton.Left + (convertButton.Width / 2) - (counterText.Left + (counterText.Width / 2));
            counterText.Top += convertButton.Top - counterText.Top - 30;
            counterText.Text = string.Empty;
            isDarkModeEnabled = true;
            SetDarkMode(isDarkModeEnabled);
        }

        private void FolderPicker_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (object.ReferenceEquals(sender, sourceButton))
                {
                    sourceBox.Text = folderBrowserDialog.SelectedPath;
                    if (!string.IsNullOrEmpty(sourceBox.Text))
                    {
                        totalFiles = Directory.EnumerateFiles(sourceBox.Text, "*", SearchOption.AllDirectories).Count();
                        counter = 0;
                        counterText.Text = $"{counter} out of {totalFiles} files";
                        counterText.Left += convertButton.Left + (convertButton.Width / 2) - (counterText.Left + (counterText.Width / 2));
                    }
                }
                if (object.ReferenceEquals(sender, outputButton))
                {
                    outputBox.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void InputTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void InputTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (files != null && files.Length > 0)
                    {
                        string path = files[0];
                        if (sender == sourceBox)
                        {
                            sourceBox.Text = path;
                            if (File.Exists(path))
                            {
                                if (!string.IsNullOrEmpty(sourceBox.Text))
                                {
                                    totalFiles = 1;
                                    counter = 0;
                                    counterText.Text = $"{counter} out of {totalFiles} files";
                                    counterText.Left += convertButton.Left + (convertButton.Width / 2) - (counterText.Left + (counterText.Width / 2));
                                }
                            }
                            else if (Directory.Exists(path))
                            {
                                if (!string.IsNullOrEmpty(sourceBox.Text))
                                {
                                    totalFiles = Directory.EnumerateFiles(sourceBox.Text, "*", SearchOption.AllDirectories).Count();
                                    counter = 0;
                                    counterText.Text = $"{counter} out of {totalFiles} files";
                                    counterText.Left += convertButton.Left + (convertButton.Width / 2) - (counterText.Left + (counterText.Width / 2));
                                }
                            }
                        }
                        else if (sender == outputBox)
                            outputBox.Text = path;
                    }
                }
            }
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            if (counter != 0)
            {
                counter = 0;
                if (!string.IsNullOrEmpty(sourceBox.Text))
                {
                    Invoke(new Action(() =>
                    {
                        counterText.Text = $"{counter} out of {totalFiles} files";
                        counterText.Left += (convertButton.Left + (convertButton.Width / 2)) - (counterText.Left + (counterText.Width / 2));
                    }));
                }
                else
                {
                    Invoke(new Action(() => counterText.Text = string.Empty));
                }
            }
            string directory = sourceBox.Text.Trim();
            string output = outputBox.Text.Trim();
            string outputType = outputCombo.Text.Trim();


            // Validate input
            if (string.IsNullOrEmpty(directory) || !(Directory.Exists(directory) || File.Exists(directory)))
            {
                MessageBox.Show("Please enter a valid source path.", "Invalid Source", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(output) || !Directory.Exists(output) || Directory.GetFiles(output).Length != 0)
            {
                if (!string.IsNullOrEmpty(output) && Directory.GetFiles(output).Length != 0)
                {
                    DialogResult result = MessageBox.Show("Output folder isn't empty! Do you wish to continue?", "Invalid Output", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel || result == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid output path.", "Invalid Output", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (outputCombo.SelectedItem == null)
            {
                MessageBox.Show("Please enter a valid output type.", "Invalid Output", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (File.Exists(directory))
            {
                if (!IsAudioFile(directory))
                {
                    MessageBox.Show("Unsupported file type.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (Directory.Exists(directory))
            {
                StringBuilder message = new();
                message.AppendLine("Unsupported file types.");
                foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    if (!IsAudioFile(file))
                    {
                        message.AppendLine(Path.GetFileName(file));
                    }
                }
                if (message.Length > 25)
                {
                    MessageBox.Show(message.ToString(), "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    message.Clear();
                    return;
                }
            }

            // Find and convert audio files
            Task.Run(() =>
            {
                if (File.Exists(directory)) // If the source is a single file
                {
                    string fileType = Path.GetExtension(directory);
                    ConvertAudioFile(directory, output, fileType, outputType);
                }
                else if (Directory.Exists(directory)) // If the source is a directory
                {
                    string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (IsAudioFile(file))
                        {
                            string fileType = Path.GetExtension(file);
                            string outputFile = Path.Combine(output, file.Substring(directory.Length + 1));
                            ConvertAudioFile(file, outputFile, fileType, outputType);
                        }
                    }
                }
                MessageBox.Show("Conversion complete.", "Conversion Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Invoke(new Action(() =>
                {
                    convertButton.Text = "Convert";
                    convertButton.Enabled = true;
                }));
            });
        }

        private void ConvertAudioFile(string inputFile, string outputFile, string inputFormat, string outputFormat)
        {
            Codec? codec = null;
            AudioQuality quality = AudioQuality.Low;
            string outputExtension = string.Empty;

            switch (outputFormat)
            {
                case "MP3":
                    codec = AudioCodec.LibMp3Lame;
                    quality = AudioQuality.Good;
                    outputExtension = ".mp3";
                    break;
                case "OGG":
                    codec = AudioCodec.LibVorbis;
                    quality = AudioQuality.Ultra;
                    outputExtension = ".ogg";
                    break;
                case "WAV":
                    outputExtension = ".wav";
                    break;
            }

            outputFile = Path.ChangeExtension(outputFile, outputExtension);

            if (codec == null || quality == AudioQuality.Low)
            {
                FFMpegArguments
                    .FromFileInput(inputFile)
                    .OutputToFile(outputFile, true)
                    .ProcessSynchronously();
            }
            else
            {
                FFMpegArguments
                    .FromFileInput(inputFile)
                    .OutputToFile(outputFile, true, options => options
                        .WithAudioCodec(codec)
                        .WithAudioBitrate(quality))
                    .ProcessSynchronously();
            }
            counter++;
            UpdateCounterText();
        }

        private bool IsAudioFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".mp3" || extension == ".wav" || extension == ".ogg";
        }

        private void UpdateCounterText()
        {
            Invoke(new Action(() =>
            {
                counterText.Text = $"{counter} out of {totalFiles} files";
                counterText.Left += (convertButton.Left + (convertButton.Width / 2)) - (counterText.Left + (counterText.Width / 2));
                convertButton.Text = "In progress";
                convertButton.Enabled = false;
            }));
        }

        private void SetDarkMode(bool enabled)
        {
            if (enabled)
            {
                StyleIndicator.BackgroundImage = Properties.Resources.sun;
                StyleIndicator.BackColor = Color.Transparent;
                this.BackColor = Color.FromArgb(30, 30, 30);
                this.ForeColor = Color.White;

                foreach (Control control in this.Controls)
                {
                    if (control is Button button)
                    {
                        button.BackColor = Color.FromArgb(97, 97, 97);
                        button.ForeColor = Color.White;
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                        button.FlatAppearance.BorderColor = Color.FromArgb(97, 97, 97);
                    }
                    if (control is TextBox textBox)
                    {
                        textBox.BackColor = Color.FromArgb(40, 40, 40);
                        textBox.ForeColor = Color.White;
                    }
                    if (control is ComboBox comboBox)
                    {
                        comboBox.BackColor = Color.FromArgb(97, 97, 97);
                        comboBox.ForeColor = Color.White;
                        comboBox.FlatStyle = FlatStyle.Flat;
                    }
                }
            }
            else
            {
                // Restore default light mode colors
                StyleIndicator.BackgroundImage = Properties.Resources.moon;
                StyleIndicator.BackColor = Color.Transparent;
                this.BackColor = SystemColors.Window;
                this.ForeColor = SystemColors.WindowText;

                foreach (Control control in this.Controls)
                {
                    if (control is Button button)
                    {
                        button.BackColor = Color.FromArgb(230, 230, 230);
                        button.ForeColor = SystemColors.WindowText;
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 0;
                        button.FlatAppearance.BorderColor = Color.FromArgb(230, 230, 230);
                    }
                    if (control is TextBox textBox)
                    {
                        textBox.BackColor = SystemColors.Window;
                        textBox.ForeColor = SystemColors.WindowText;
                    }
                    if (control is ComboBox comboBox)
                    {
                        comboBox.BackColor = Color.FromArgb(230, 230, 230);
                        comboBox.ForeColor = SystemColors.WindowText;
                        comboBox.FlatStyle = FlatStyle.System;
                    }
                }
            }
        }

        private void DarkModeToggle_Click(object sender, EventArgs e)
        {
            isDarkModeEnabled = !isDarkModeEnabled;
            SetDarkMode(isDarkModeEnabled);
        }
    }
}