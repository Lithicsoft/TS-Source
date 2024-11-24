// License: Apache-2.0
/*
 * Python/PY/TrainModel.xaml.cs: Back-end source code for model trainer image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.Utils;
using Notifications.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ModernWpf.MessageBox;

namespace Lithicsoft_Trainer_Studio.Python.PY
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
            if (Directory.Exists($"projects\\{projectName}\\datasets"))
            {
                label1.Content = "Ready for train your model";
            }
            else
            {
                label1.Content = "Load your datasets (if there is) before training model";
            }
            button1.IsEnabled = true;
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

                    ProcessStartInfo start = new()
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C conda activate \"{projectName}\" && python \"{Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}\\trainer.py")}\" && conda deactivate && pause && exit",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (Process process = new())
                    {
                        process.StartInfo = start;
                        process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                        process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error training model: {ex}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
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

            TrainModel.Instance.isTraining = false;
            button1.IsEnabled = true;
        }
    }
}
