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
using System.Windows.Controls;

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
            if (Directory.Exists($"projects\\{projectName}\\outputs"))
            {
                button1.IsEnabled = true;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo start = new()
                    {
                        FileName = $"cmd.exe",
                        Arguments = $"/K conda activate \"{projectName}\" & python \"{Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}\\tester.py")}\" & conda deactivate",
                        UseShellExecute = true,
                        RedirectStandardOutput = false
                    };

                    Process.Start(start);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error testing model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            button1.IsEnabled = true;
        }
    }
}
