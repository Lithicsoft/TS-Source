// License: Apache-2.0
/*
 * Python/PY/TestModel.xaml.cs: Back-end source code for model tester image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Diagnostics;
using System.IO;
using System.Windows;
using MessageBox = ModernWpf.MessageBox;
using Page = System.Windows.Controls.Page;

namespace Lithicsoft_Trainer_Studio.Python.PY
{
    /// <summary>
    /// Interaction logic for TestModel.xaml
    /// </summary>
    public partial class TestModel : Page
    {
        private readonly string projectName = string.Empty;

        public TestModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\tester.py") && Directory.Exists($"projects\\{projectName}\\outputs"))
            {
                button1.IsEnabled = true;
            }
            else
            {
                button1.IsEnabled = false;
                button1.Content = "Unavailable";
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            progressBar.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo start = new()
                    {
                        FileName = $"cmd.exe",
                        Arguments = $"/K conda activate \"{projectName}\" & python \"{Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}\\tester.py")}\" & conda deactivate & pause & exit",
                        UseShellExecute = true,
                        RedirectStandardOutput = false
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
                        MessageBox.Show($"Error testing model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });

            progressBar.Visibility = Visibility.Hidden;

            button1.IsEnabled = true;
        }
    }
}
