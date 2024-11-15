// License: Apache-2.0
/*
 * CSharp/IC/ModelTester.xaml.cs: Back-end source code for model tester image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Lithicsoft_Trainer_Studio.CSharp.IC
{
    /// <summary>
    /// Interaction logic for ModelTester.xaml
    /// </summary>
    public partial class ModelTester : Page
    {
        private readonly string projectName = string.Empty;

        public ModelTester(string name)
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

        ImagePrediction prediction;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png, *.jpg, *.webp)|*.png;*.jpg;*.webp"
            };
            Nullable<bool> result = openFileDialog.ShowDialog();

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
            bitmap.EndInit();

            image1.Source = bitmap;

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
                    MessageBox.Show($"Error running model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            label1.Content = "Predict: " + prediction.PredictedLabelValue;
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
