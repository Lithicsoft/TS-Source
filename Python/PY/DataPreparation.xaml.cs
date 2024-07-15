// License: Apache-2.0
/*
 * Python/PY/DataPreparation.xaml.cs: Back-end source code for data prepare python page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.Win32;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.Python.PY
{
    /// <summary>
    /// Interaction logic for DataPreparation.xaml
    /// </summary>
    public partial class DataPreparation : Page
    {
        private string projectName = string.Empty;

        public DataPreparation(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Zip files (*.zip)|*.zip";
                Nullable<bool> result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    textBox1.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening zip file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            loadDataset(textBox1.Text);
        }

        private void loadDataset(string path)
        {
            button1.IsEnabled = false;
            textBox1.IsEnabled = false;

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading dataset: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"File not found!", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            button1.IsEnabled = true;
            textBox1.IsEnabled = true;
        }
    }
}
