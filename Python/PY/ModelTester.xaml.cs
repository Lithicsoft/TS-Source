// License: Apache-2.0
/*
 * Python/PY/ModelTester.xaml.cs: Back-end source code for model tester image classification page
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
    /// Interaction logic for ModelTester.xaml
    /// </summary>
    public partial class ModelTester : Page
    {
        private string projectName = string.Empty;

        public ModelTester(string name)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = $"cmd.exe";
                start.Arguments = $"/K python \"{Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}\\tester.py")}\"";
                start.UseShellExecute = true;
                start.RedirectStandardOutput = false;

                Process process = Process.Start(start);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            button1.IsEnabled = true;
        }
    }
}
