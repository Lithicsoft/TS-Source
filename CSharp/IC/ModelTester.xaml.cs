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
        private string projectName = string.Empty;

        public ModelTester(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private MLContext mlContext;
        private PredictionEngine<ImageData, ImagePrediction> predictionEngine;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\outputs\\model.zip"))
            {
                button1.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png, *.jpg, *.webp)|*.png;*.jpg;*.webp";
                Nullable<bool> result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmap.EndInit();

                    image1.Source = bitmap;

                    InitModel();
                    var imageData = new ImageData()
                    {
                        ImagePath = openFileDialog.FileName
                    };

                    var prediction = predictionEngine.Predict(imageData);

                    label1.Content = "Predict: " + prediction.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening zip file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            button1.IsEnabled = true;
        }

        private void InitModel()
        {
            try
            {
                mlContext = new MLContext();
                DataViewSchema predictionPipelineSchema;
                ITransformer predictionPipeline = mlContext.Model.Load($"projects\\{projectName}\\outputs\\model.zip", out predictionPipelineSchema);
                predictionEngine = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(predictionPipeline);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class ImageData
        {
            [LoadColumn(0)]
            public string ImagePath;

            [LoadColumn(1)]
            public string Label;
        }

        public class ImagePrediction : ImageData
        {
            public float[] Score;

            public string PredictedLabelValue;
        }
    }
}
