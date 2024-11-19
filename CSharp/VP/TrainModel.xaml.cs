// License: Apache-2.0
/*
 * CSharp/VP/TrainModel.xaml.cs: Back-end source code for model trainer value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.Utils;
using Notifications.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
{
    /// <summary>
    /// Interaction logic for TrainModel.xaml
    /// </summary>
    public partial class TrainModel : Page
    {
        private readonly string projectName = string.Empty;

        public TrainModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private static TrainModel? _instance;
        public bool isTraining = false;

        public static TrainModel Instance
        {
            get
            {
                _instance ??= new TrainModel(string.Empty);
                return _instance;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\datasets\\dataset.csv"))
            {
                label1.Content = "Ready for train your model";
                button1.IsEnabled = true;
            }
            else
            {
                label1.Content = "Please prepare your datasets before training model";
            }
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            TrainModel.Instance.isTraining = true;
            label1.Content = "Training your model...";

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Hide();

            var loadingWindow = new LoadingWindow("Training your model...")
            {
                Owner = parentWindow
            };
            loadingWindow.Show();

            await Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists($"projects\\{projectName}\\outputs"))
                    {
                        Directory.CreateDirectory($"projects\\{projectName}\\outputs");
                    }

                    CSharpML.ValuePrediction valuePrediction = new();
                    valuePrediction.Train(projectName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error training model {ex}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            parentWindow?.Show();
            loadingWindow.Close();

            label1.Content = "Done!";

            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = "Completed training of the model",
                Message = "Return to Trainer Studio to test your new model.",
                Type = NotificationType.Information
            });

            TrainModel.Instance.isTraining = true;
            button1.IsEnabled = true;
        }
    }
}
