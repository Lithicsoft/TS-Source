// License: Apache-2.0
/*
 * CSharp/IC/ModelResult.xaml.cs: Back-end source code for model result image classification page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lithicsoft_Trainer_Studio.CSharp.IC
{
    /// <summary>
    /// Interaction logic for ModelResult.xaml
    /// </summary>
    public partial class ModelResult : Page
    {
        private string projectName = string.Empty;

        public ModelResult(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"project\\{projectName}\\outputs\\model.zip"))
            {
                button1.IsEnabled = true;
            }

            string code = @"
private MLContext mlContext;
private PredictionEngine<ImageData, ImagePrediction> predictionEngine;
mlContext = new MLContext();
DataViewSchema predictionPipelineSchema;
ITransformer predictionPipeline = mlContext.Model.Load($""model.zip"", out predictionPipelineSchema);
predictionEngine = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(predictionPipeline);
var imageData = new ImageData()
{
    ImagePath = ""your_image_file_path.png""
};

var prediction = predictionEngine.Predict(imageData);
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
}";
            DocCode.Document.Blocks.Clear();
            DocCode.Document.Blocks.Add(new Paragraph(new Run(code)));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"projects\\{projectName}\\outputs";
                if (!Directory.Exists(filePath))
                {
                    return;
                }

                string argument = "/select, \"" + filePath + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening model: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
