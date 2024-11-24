// License: Apache-2.0
/*
 * Python/PY/PrepareDataset.xaml.cs: Back-end source code for data prepare python page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.Utils;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.Python.PY
{
    /// <summary>
    /// Interaction logic for PrepareDataset.xaml
    /// </summary>
    public partial class PrepareDataset : Page
    {
        private readonly string projectName = string.Empty;

        public PrepareDataset(string name)
        {
            InitializeComponent();

            projectName = name;

            if (File.Exists($"projects\\{projectName}\\datasets\\tree.txt"))
            {
                DocCode.Document.Blocks.Clear();
                DocCode.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText($"projects\\{projectName}\\tree.txt"))));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Zip files (*.zip)|*.zip"
                };
                Nullable<bool> result = openFileDialog.ShowDialog();
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    textBox1.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening zip file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadDataset(textBox1.Text);
        }

        private async Task LoadDataset(string path)
        {
            button1.IsEnabled = false;
            textBox1.IsEnabled = false;

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Hide();

            var loadingWindow = new LoadingWindow("Preparing your dataset...")
            {
                Owner = parentWindow
            };
            loadingWindow.Show();

            await Task.Run(() =>
            {
                if (File.Exists(path))
                {
                    try
                    {
                        if (Directory.Exists($"projects\\{projectName}\\datasets"))
                        {
                            Directory.Delete($"projects\\{projectName}\\datasets", true);
                        }
                        Directory.CreateDirectory($"projects\\{projectName}\\datasets");
                        string extractPath = $"projects\\{projectName}\\datasets";
                        ZipFile.ExtractToDirectory(path, extractPath);

                        ProcessStartInfo start = new()
                        {
                            FileName = $"cmd.exe",
                            Arguments = $"/K tree projects\\{projectName}\\datasets /F /A > projects\\{projectName}\\tree.txt & exit",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };

                        using (var process = Process.Start(start))
                        {
                            if (process != null)
                            {
                                process.WaitForExit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error loading dataset: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"File not found!", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });

            if (File.Exists($"projects\\{projectName}\\datasets\\tree.txt"))
            {
                DocCode.Document.Blocks.Clear();
                DocCode.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText($"projects\\{projectName}\\tree.txt"))));
            }

            parentWindow?.Show();
            loadingWindow.Close();

            button1.IsEnabled = true;
            textBox1.IsEnabled = true;
        }
    }
}
