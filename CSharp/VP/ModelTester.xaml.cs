// License: Apache-2.0
/*
 * CSharp/VP/ModelTester.xaml.cs: Back-end source code for model tester value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Tensorflow;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
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
        private PredictionEngine<ModelInput, ModelOutput>? predictionEngine;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\outputs\\model.zip"))
            {
                button1.IsEnabled = true;
                ValueA.IsEnabled = true;
                ValueB.IsEnabled = true;
                ValueC.IsEnabled = true;
                ValueD.IsEnabled = true;
            }
        }

        ModelOutput prediction;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    InitModel();
                    var sampleInput = new ModelInput
                    {
                        A = float.Parse(ValueA.Text),
                        B = float.Parse(ValueB.Text),
                        C = float.Parse(ValueC.Text),
                        D = float.Parse(ValueD.Text)
                    };

                    if (predictionEngine != null)
                    {
                        var prediction = predictionEngine.Predict(sampleInput);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error testing model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            label1.Content = "Predict: " + prediction.Prediction;
            button1.IsEnabled = true;
        }

        private void InitModel()
        {
            try
            {
                mlContext = new MLContext();
                ITransformer predictionPipeline = mlContext.Model.Load($"projects\\{projectName}\\outputs\\model.zip", out DataViewSchema predictionPipelineSchema);
                predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(predictionPipeline);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class ModelInput
        {
            [LoadColumn(0)]
            public float A { get; set; }

            [LoadColumn(1)]
            public float B { get; set; }

            [LoadColumn(2)]
            public float C { get; set; }

            [LoadColumn(3)]
            public float D { get; set; }

            [LoadColumn(4)]
            public float Prediction { get; set; }
        }

        public class ModelOutput
        {
            [ColumnName("Score")]
            public float Prediction { get; set; }
        }
    }
}
