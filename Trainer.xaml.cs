// License: Apache-2.0
/*
 * Trainer.xaml.cs: Back-end source code for trainer
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.CSharp;
using System.Windows;

namespace Lithicsoft_Trainer_Studio
{
    /// <summary>
    /// Interaction logic for Trainer.xaml
    /// </summary>
    public partial class Trainer : Window
    {
        private readonly string projectName = string.Empty, projectLanguage = string.Empty, projectType = string.Empty;

        public Trainer(string name, string language, string type)
        {
            InitializeComponent();

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Title = $"{name} - {language} - {type}";

            projectName = name;
            projectLanguage = language;
            projectType = type;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (projectLanguage == "CSharp")
                {
                    if (projectType == "Image Classification")
                    {
                        ImageClassification imageClassification = new(projectName);
                        this.MainStackPanel.Children.Add(imageClassification);
                    }
                    else if (projectType == "Value Prediction")
                    {
                        ValuePrediction valuePrediction = new(projectName);
                        this.MainStackPanel.Children.Add(valuePrediction);
                    }
                    else
                    {
                        MessageBox.Show($"Cannot open project {projectName} ({projectType})", "Project Open");
                    }
                }
                else
                {
                    Python.Python python = new(projectName);
                    this.MainStackPanel.Children.Add(python);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error dectecting project type: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CSharp.IC.TrainModel.Instance.isTraining || CSharp.VP.TrainModel.Instance.isTraining || Python.PY.TrainModel.Instance.isTraining)
            {
                MessageBox.Show("You can't not exit Trainer Studio while training model!", "Prevent Closing", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Cancel = true;
            }
            else
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
