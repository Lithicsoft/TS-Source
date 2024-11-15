// License: Apache-2.0
/*
 * MainWindow.xaml.cs: Back-end source code for main window
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.UserControls;
using Lithicsoft_Trainer_Studio.UserControls.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindows_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowStyle = WindowStyle.None;

                Startup startup = new();
                startup.UserControlClosed += Startup_UserControlClosed;
                MainStackPanel.Children.Add(startup);

                this.SizeToContent = SizeToContent.WidthAndHeight;

                await WaitForUserControlToCloseAsync(startup);
                MainStackPanel.Children.Clear();

                this.WindowStyle = WindowStyle.SingleBorderWindow;

                MainPage mainPage = new();
                MainStackPanel.Children.Add(mainPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting studio: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void Startup_UserControlClosed(object sender, EventArgs e)
        {
            if (sender is UserControl userControl)
            {
                MainStackPanel.Children.Remove(userControl);
            }
        }

        private Task<bool> WaitForUserControlToCloseAsync(Startup userControl)
        {
            var tcs = new TaskCompletionSource<bool>();
            void handler(object? s, EventArgs e)
            {
                userControl.UserControlClosed -= handler;
                tcs.SetResult(true);
            }

            userControl.UserControlClosed += handler;
            return tcs.Task;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Creator.Instance.isCreating)
            {
                MessageBox.Show("You can't not exit Trainer Studio while creating project!", "Prevent Closing", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Cancel = true;
            }
            else
            {
                if (this.Visibility != Visibility.Hidden)
                {
                    var result = MessageBox.Show("Do you want to exit Trainer Studio?", "Close Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                        System.Windows.Application.Current.Shutdown();
                    else if (result == MessageBoxResult.No)
                        e.Cancel = true;
                }
            }
        }
    }
}