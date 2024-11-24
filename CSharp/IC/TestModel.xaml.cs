// License: Apache-2.0
/*
 * CSharp/IC/TestModel.xaml.cs: Back-end source code for model tester image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Win32;
using ModernWpf.Controls;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MessageBox = ModernWpf.MessageBox;
using Page = System.Windows.Controls.Page;

namespace Lithicsoft_Trainer_Studio.CSharp.IC
{
    /// <summary>
    /// Interaction logic for TestModel.xaml
    /// </summary>
    public partial class TestModel : Page
    {
        private readonly string projectName = string.Empty;

        public TestModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private MLContext? mlContext;
        private PredictionEngine<ImageData, ImagePrediction>? predictionEngine;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\outputs\\model.zip"))
            {
                button1.IsEnabled = true;
            }
        }

        ImagePrediction? prediction;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png, *.jpg, *.webp)|*.png;*.jpg;*.webp"
            };
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                button1.IsEnabled = true;
                return;
            }

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
            bitmap.EndInit();

            image1.Source = bitmap;

            progressBar.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                try
                {
                    if (result == true)
                    {
                        InitModel();
                        var imageData = new ImageData()
                        {
                            ImagePath = openFileDialog.FileName
                        };

                        if (predictionEngine != null)
                        {
                            prediction = predictionEngine.Predict(imageData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error running model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });

            progressBar.Visibility = Visibility.Hidden;

            if (prediction != null)
            {
                label1.Content = "Predict: " + prediction.PredictedLabelValue;
            }
            button1.IsEnabled = true;
        }

        private void InitModel()
        {
            try
            {
                mlContext = new MLContext();
                ITransformer predictionPipeline = mlContext.Model.Load($"projects\\{projectName}\\outputs\\model.zip", out DataViewSchema predictionPipelineSchema);
                predictionEngine = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(predictionPipeline);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public class ImageData
        {
            [LoadColumn(0)]
            public string? ImagePath;

            [LoadColumn(1)]
            public string? Label;
        }

        public class ImagePrediction : ImageData
        {
            public float[]? Score;

            public string? PredictedLabelValue;
        }
    }
}
