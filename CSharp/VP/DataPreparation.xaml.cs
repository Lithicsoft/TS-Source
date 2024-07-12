// License: Apache-2.0
/*
 * CSharp/VP/DataPreparation.xaml.cs: Back-end source code for data prepare value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
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
                openFileDialog.Filter = "Csv files (*.csv)|*.csv";
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

            try
            {
                if (CheckCsvFormat(path))
                {
                    MessageBox.Show("The csv structure is invalid", "Dataset Structure", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking csv file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private bool CheckCsvFormat(string filePath)
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
