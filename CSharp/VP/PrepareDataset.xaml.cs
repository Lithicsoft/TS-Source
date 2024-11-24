// License: Apache-2.0
/*
 * CSharp/VP/PrepareDataset.xaml.cs: Back-end source code for data prepare value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.Utils;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
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

            if (File.Exists($"projects\\{projectName}\\tree.txt"))
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
                    Filter = "Csv files (*.csv)|*.csv"
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
                try
                {
                    if (CheckCsvFormat(path))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("The csv structure is invalid", "Dataset Structure", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error checking csv file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                if (File.Exists(path))
                {
                    try
                    {
                        if (Directory.Exists($"projects\\{projectName}\\datasets"))
                        {
                            Directory.Delete($"projects\\{projectName}\\datasets", true);
                        }
                        Directory.CreateDirectory($"projects\\{projectName}\\datasets");
                        File.Copy(path, $"projects\\{projectName}\\datasets\\dataset.csv");

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

            if (File.Exists($"projects\\{projectName}\\tree.txt"))
            {
                DocCode.Document.Blocks.Clear();
                DocCode.Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText($"projects\\{projectName}\\tree.txt"))));
            }

            parentWindow?.Show();
            loadingWindow.Close();

            button1.IsEnabled = true;
            textBox1.IsEnabled = true;
        }

        private static bool CheckCsvFormat(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                return false;
            }

            foreach (var line in lines)
            {
                var columns = line.Split(',');

                if (columns.Length != 5)
                {
                    return false;
                }

                for (int cloumn = 0; cloumn < columns.Length; cloumn++)
                {
                    if (!float.TryParse(columns[cloumn], out _))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
