// License: Apache-2.0
/*
 * Python/PY/TrainModel.xaml.cs: Back-end source code for model trainer image classification page
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
    /// Interaction logic for TrainModel.xaml
    /// </summary>
    public partial class TrainModel : Page
    {
        private string projectName = string.Empty;

        public TrainModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private static TrainModel _instance;
        public bool isTraining = false;

        public static TrainModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TrainModel(string.Empty);
                return _instance;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists($"projects\\{projectName}\\datasets"))
            {
                label1.Content = "Ready for train your model";
                button1.IsEnabled = true;
            }
            else
            {
                label1.Content = "Please prepare your datasets before training model";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            TrainModel.Instance.isTraining = true;
            label1.Content = "Training your model...";
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = $"projects\\{projectName}\\python\\Scripts\\python.exe";
                start.Arguments = $"projects{projectName}\\trainer.py";
                start.UseShellExecute = true;
                start.RedirectStandardOutput = false;

                Process process = Process.Start(start);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error training model: {ex}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            label1.Content = "Done!";
            TrainModel.Instance.isTraining = true;
            button1.IsEnabled = true;
        }
    }
}
