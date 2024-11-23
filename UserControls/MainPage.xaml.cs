// License: Apache-2.0
/*
 * UserControls/MainPage.xaml.cs: Back-end source code for home page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.UserControls.Pages;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.UserControls
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Main.Content = new Manager();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening manager: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Main.Content = new Creator();
                button4.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening creator: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Main.Content = new Manager();
                button4.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening manager: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (result == true)
                {
                    string zipPath = openFileDialog.FileName;
                    string extractPath = "projects";
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    MessageBox.Show($"Imported '{openFileDialog.FileName}' successfully", "Import Project", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Main.Content = new Manager();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing project: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "https://lithicsoft.rf.gd/trainerstudio/",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening info: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
