// License: Apache-2.0
/*
 * Form1.cs: Startup, checker, downloader files for Trainer Studio, make sure everythings are fine for start the manager
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO.Compression;
using System.Net;

namespace Lithicsoft_Trainer
{
    public partial class Startup : UserControl
    {
        public event EventHandler UserControlClosed;

        public Startup()
        {
            InitializeComponent();

            Color highlight = Color.FromArgb(0, 120, 215);
            this.BackColor = highlight;
            label1.ForeColor = highlight;
            label2.ForeColor = highlight;
            label3.ForeColor = highlight;
            label4.ForeColor = highlight;

            label4.Text = "Starting studio...";
            StartInitialization();
        }

        private async void StartInitialization()
        {
            await InitializeAsync();
            await Task.Delay(1500);
            OnUserControlClosed();
        }

        private async Task InitializeAsync()
        {
            CreateProjectsFolder();
            await CheckAndDownloadFilesAsync();
        }

        protected virtual void OnUserControlClosed()
        {
            UserControlClosed?.Invoke(this, EventArgs.Empty);
        }

        private void CreateProjectsFolder()
        {
            label4.Text = "Checking for projects folder...";
            if (!Directory.Exists("projects"))
            {
                Directory.CreateDirectory("projects");
            }
            progressBar1.Value = 10;
        }

        private async Task CheckAndDownloadFilesAsync()
        {
            try
            {
                await UpdateLabelTextAsync("Checking for inception...");

                if (!Directory.Exists("inception"))
                {
                    Directory.CreateDirectory("inception");
                }

                await UpdateProgressBarAsync(30);

                await UpdateLabelTextAsync("Checking for tensorflow_inception_graph.pb...");
                if (!File.Exists("inception\\tensorflow_inception_graph.pb"))
                {
                    await UpdateProgressBarAsync(50);
                    await UpdateLabelTextAsync("Downloading inception5h.zip...");
                    string zipFile = "inception\\inception5h.zip";

                    using (var client = new WebClient())
                    {
                        try
                        {
                            await client.DownloadFileTaskAsync(new Uri("https://storage.googleapis.com/download.tensorflow.org/models/inception5h.zip"), zipFile);
                            await UpdateProgressBarAsync(70);
                        }
                        catch (WebException ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }
                    }

                    try
                    {
                        await UpdateLabelTextAsync("Unzipping inception5h.zip...");
                        string extractPath = "inception";
                        await Task.Run(() => ZipFile.ExtractToDirectory(zipFile, extractPath));
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Error extracting zip file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }

                await UpdateLabelTextAsync("Checking for saved_model.pb...");
                if (!File.Exists("sentiment_model\\saved_model.pb"))
                {
                    await UpdateProgressBarAsync(50);
                    await UpdateLabelTextAsync("Downloading sentiment_model.zip...");
                    string zipFile = "sentiment_model.zip";

                    using (var client = new WebClient())
                    {
                        try
                        {
                            await client.DownloadFileTaskAsync(new Uri("https://github.com/dotnet/samples/blob/main/machine-learning/models/textclassificationtf/sentiment_model.zip?raw=true"), zipFile);
                            await UpdateProgressBarAsync(70);
                        }
                        catch (WebException ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }
                    }

                    try
                    {
                        await UpdateLabelTextAsync("Unzipping sentiment_model.zip...");
                        await Task.Run(() => ZipFile.ExtractToDirectory(zipFile, "."));
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Error extracting zip file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }

                await UpdateLabelTextAsync("Checking for Python...");
                if (!File.Exists("Python.zip"))
                {
                    await UpdateProgressBarAsync(50);
                    await UpdateLabelTextAsync("Downloading Python.zip...");
                    string zipFile = "Python.zip";

                    using (var client = new WebClient())
                    {
                        try
                        {
                            await client.DownloadFileTaskAsync(new Uri("https://github.com/Lithicsoft/Lithicsoft-Trainer-Studio/raw/main/Python.zip"), zipFile);
                            await UpdateProgressBarAsync(70);
                        }
                        catch (WebException ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }
                    }
                }

                await UpdateProgressBarAsync(80);
                await UpdateLabelTextAsync("Cleaning files...");
                string[] filesToDelete = {
                    "inception\\inception5h.zip",
                    "sentiment_model.zip",
                };

                foreach (string filePath in filesToDelete)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                await UpdateProgressBarAsync(90);
                await UpdateLabelTextAsync("Finalizing setup...");
                await UpdateProgressBarAsync(100);
                await UpdateLabelTextAsync("Studio started successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting studio: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private Task UpdateLabelTextAsync(string text)
        {
            return Task.Run(() =>
            {
                if (label4.InvokeRequired)
                {
                    label4.Invoke(new Action(() => label4.Text = text));
                }
                else
                {
                    label4.Text = text;
                }
            });
        }

        private Task UpdateProgressBarAsync(int value)
        {
            return Task.Run(() =>
            {
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new Action(() => progressBar1.Value = value));
                }
                else
                {
                    progressBar1.Value = value;
                }
            });
        }
    }
}
