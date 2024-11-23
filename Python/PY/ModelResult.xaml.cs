// License: Apache-2.0
/*
 * Python/PY/ModelResult.xaml.cs: Back-end source code for model result image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.Python.PY
{
    /// <summary>
    /// Interaction logic for ModelResult.xaml
    /// </summary>
    public partial class ModelResult : Page
    {
        private readonly string projectName = string.Empty;

        public ModelResult(string name)
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

            string code = @"Fast document is not available for this project, please visit our wiki via website!";
            DocCode.Document.Blocks.Clear();
            DocCode.Document.Blocks.Add(new Paragraph(new Run(code)));
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"projects\\{projectName}\\outputs";
                if (!Directory.Exists(filePath))
                {
                    return;
                }

                string argument = "/select, \"" + filePath + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
