// License: Apache-2.0
/*
 * UserControls/Startup.xaml.cs: Back-end source code for launcher
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.UserControls
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : UserControl
    {
        public Startup()
        {
            InitializeComponent();
        }

        public event EventHandler? UserControlClosed;

        private void Startup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StartInitialization();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting Trainer Studio: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void StartInitialization()
        {
            await InitializeAsync();
            await Task.Delay(1500);
            OnUserControlClosed();
        }

        private static async Task InitializeAsync()
        {
            CreateProjectsFolder();
            await CheckAndDownloadFilesAsync();
        }

        protected virtual void OnUserControlClosed()
        {
            UserControlClosed?.Invoke(this, EventArgs.Empty);
        }

        private static void CreateProjectsFolder()
        {
            if (!Directory.Exists("projects"))
            {
                Directory.CreateDirectory("projects");
            }
        }

        private static async Task CheckAndDownloadFilesAsync()
        {
            try
            {

                if (!Directory.Exists("inception"))
                {
                    Directory.CreateDirectory("inception");
                }

                if (!File.Exists("inception\\tensorflow_inception_graph.pb"))
                {
                    string zipFile = "inception\\inception5h.zip";

                    using (HttpClient client = new())
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(new Uri("https://storage.googleapis.com/download.tensorflow.org/models/inception5h.zip"));
                            response.EnsureSuccessStatusCode();

                            using FileStream fs = new(zipFile, FileMode.Create, FileAccess.Write, FileShare.None);
                            await response.Content.CopyToAsync(fs);
                        }
                        catch (WebException ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(1);
                        }
                    }

                    try
                    {
                        string extractPath = "inception";
                        await Task.Run(() => ZipFile.ExtractToDirectory(zipFile, extractPath));
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Error extracting zip file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(1);
                    }
                }

                if (!File.Exists("sentiment_model\\saved_model.pb"))
                {
                    string zipFile = "sentiment_model.zip";

                    using (HttpClient client = new())
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(new Uri("https://github.com/dotnet/samples/blob/main/machine-learning/models/textclassificationtf/sentiment_model.zip?raw=true"));
                            response.EnsureSuccessStatusCode();

                            using FileStream fs = new(zipFile, FileMode.Create, FileAccess.Write, FileShare.None);
                            await response.Content.CopyToAsync(fs);
                        }
                        catch (WebException ex)
                        {
                            MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(1);
                        }
                    }

                    try
                    {
                        await Task.Run(() => ZipFile.ExtractToDirectory(zipFile, "."));
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Error extracting zip file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(1);
                    }
                }

                string[] filesToDelete = [
                    "inception\\inception5h.zip",
                    "sentiment_model.zip",
                ];

                foreach (string filePath in filesToDelete)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting studio: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
    }
}
