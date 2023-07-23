using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Text;
using static System.Windows.Forms.DataFormats;
using System.Windows.Forms;

namespace AudioConverter
{
    public partial class Window : Form
    {
        private int totalFiles, counter;
        private readonly string[] allOptions = { "AAC", "AC3", "FLAC", "M4A", "M4B", "MP3", "OGG", "WAV" };
        private bool isDarkModeEnabled;
        List<Task> conversionTasks = new List<Task>();

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
                    if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
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
            Task.Run(async () =>
            {
                if (File.Exists(directory))
                {
                    await ConvertAllAudioFilesParallel(new string[] { directory }, outputType);
                }
                else if (Directory.Exists(directory))
                {
                    string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    await ConvertAllAudioFilesParallel(files, outputType);
                }
                MessageBox.Show("Conversion complete.", "Conversion Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Invoke(new Action(() =>
                {
                    convertButton.Text = "Convert";
                    convertButton.Enabled = true;
                }));
            });
        }

        private void ConvertAudioFile(string inputFile, string outputFile, string outputFormat)
        {
            var formatMappings = new Dictionary<string, (Codec? codec, AudioQuality quality, string Extension)>
            {
                ["AAC"] = (AudioCodec.Aac, AudioQuality.Good, ".aac"),
                ["AC3"] = (null, AudioQuality.Low, ".ac3"),
                ["FLAC"] = (null, AudioQuality.Low, ".flac"),
                ["M4A"] = (AudioCodec.Aac, AudioQuality.Good, ".m4a"),
                ["M4B"] = (AudioCodec.Aac, AudioQuality.Good, ".m4b"),
                ["MP3"] = (AudioCodec.LibMp3Lame, AudioQuality.Good, ".mp3"),
                ["OGG"] = (AudioCodec.LibVorbis, AudioQuality.Ultra, ".ogg"),
                ["WAV"] = (null, AudioQuality.Low, ".wav")
            };

            if (!formatMappings.TryGetValue(outputFormat, out var format))
            {
                return;
            }
            var codec = format.codec;
            var quality = format.quality;
            var outputExtension = format.Extension;

            outputFile = Path.ChangeExtension(outputFile, outputExtension);

            if (codec == null || quality == AudioQuality.Low)
            {
                try
                {
                    FFMpegArguments
                        .FromFileInput(inputFile)
                        .OutputToFile(outputFile, true)
                        .ProcessSynchronously(true);
                    counter++;
                    UpdateCounterText();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    FFMpegArguments
                        .FromFileInput(inputFile)
                        .OutputToFile(outputFile, true, options => options
                            .WithAudioCodec(codec)
                            .WithAudioBitrate(quality))
                        .ProcessSynchronously(true);
                    counter++;
                    UpdateCounterText();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsAudioFile(string filePath)
        {
            bool found = false;
            string extension = Path.GetExtension(filePath).ToLower();
            foreach (var type in allOptions)
            {
                if ($".{type.ToLower()}" == extension)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        private async Task ConvertAllAudioFilesParallel(string[] inputFiles, string outputFormat)
        {
            var tasks = new List<Task>();
            UpdateCounterText();
            foreach (var inputFile in inputFiles)
            {
                string outputFile = Path.Combine(outputBox.Text.Trim(), Path.GetFileNameWithoutExtension(inputFile)) + $".{outputFormat.ToLower()}";
                tasks.Add(Task.Run(() => ConvertAudioFile(inputFile, outputFile, outputFormat)));
            }
            await Task.WhenAll(tasks);
        }

        private void DarkModeToggle_Click(object sender, EventArgs e)
        {
            isDarkModeEnabled = !isDarkModeEnabled;
            SetDarkMode(isDarkModeEnabled);
        }
    }
}