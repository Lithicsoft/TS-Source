// License: Apache-2.0
/*
 * UserControls/MainPage.xaml.cs: Back-end source code for home page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.UserControls.Pages;
using System.Windows;
using System.Windows.Controls;

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

        private void button1_Click(object sender, RoutedEventArgs e)
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

        private void button4_Click(object sender, RoutedEventArgs e)
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
    }
}
