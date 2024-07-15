// License: Apache-2.0
/*
 * CSharp/VP/ModelResult.xaml.cs: Back-end source code for model result value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
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
            if (File.Exists($"projects\\{projectName}\\outputs\\model.zip"))
            {
                button1.IsEnabled = true;
            }

            string code = @"
private MLContext mlContext;
private PredictionEngine<ModelInput, ModelOutput> predictionEngine;
mlContext = new MLContext();
DataViewSchema predictionPipelineSchema;
ITransformer predictionPipeline = mlContext.Model.Load($""model.zip"", out predictionPipelineSchema);
predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(predictionPipeline);
var sampleInput = new ModelInput
{
    A = 2.0f,
    B = 20.0f,
    C = 4.0f,
    D = 4.0f
};

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
    [ColumnName(""Score"")]
    public float Prediction { get; set; }
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
