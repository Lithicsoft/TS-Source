// License: Apache-2.0
/*
 * Form1.cs: UserControl for Text Classification, still can't save model
 *
 * (C) Copyright 2024 Lithicsoft Organization, Microsoft Learn
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 *         luisquintanilla
 *         gewarren
 *         jwood803
 *         Youssef1313
 *         Thraka
 *         natke
 *         nschonni
 */

using AForge.Video.DirectShow;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using NumSharp;

namespace Lithicsoft_Trainer
{
    public partial class TextClassification : UserControl
    {
        static string projectName = string.Empty;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private int currentCameraIndex = 0;

        public TextClassification(string name)
        {
            try
            {
                InitializeComponent();
                label1.ForeColor = Color.FromArgb(0, 120, 215);

                projectName = name;

                if (File.Exists($"projects\\{projectName}\\model.zip"))
                {
                    textBox2.Text = $"projects\\{projectName}\\model.zip";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting project: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening csv file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckCsvFormat(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                return false;
            }

            foreach (var line in lines)
            {
                var columns = line.Split(',');

                if (columns.Length != 2)
                {
                    return false;
                }

                if (!int.TryParse(columns[1], out _))
                {
                    return false;
                }
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckCsvFormat(textBox1.Text))
                {
                    MessageBox.Show("The csv structure is invalid", "Dataset Structure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking csv file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                progressBar1.Value = 0;
                richTextBox1.Text += "\nCopying the file...\n";
                File.Copy(textBox1.Text, $"projects\\{projectName}\\dataset.csv");
                progressBar1.Value = 100;

                richTextBox1.Text += "Done!\n";
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dataset: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void WriteLine(string text)
        {
            richTextBox2.Text += text + "\n";
        }

        // <SnippetDeclareGlobalVariables>
        public const int FeatureLength = 600;
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "sentiment_model");
        // </SnippetDeclareGlobalVariables>

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            try
            {
                // Create MLContext to be shared across the model creation workflow objects
                // <SnippetCreateMLContext>
                MLContext mlContext = new MLContext();
                // </SnippetCreateMLContext>

                // Dictionary to encode words as integers.
                // <SnippetCreateLookupMap>
                var lookupMap = mlContext.Data.LoadFromTextFile(Path.Combine($"projects\\{projectName}\\dataset.csv"),
                    columns: new[]
                       {
                        new TextLoader.Column("Words", DataKind.String, 0),
                        new TextLoader.Column("Ids", DataKind.Int32, 1),
                       },
                    separatorChar: ','
                   );
                progressBar2.Value = 20;
                // </SnippetCreateLookupMap>

                // The model expects the input feature vector to be a fixed length vector.
                // This action resizes the variable length array generated by the lookup map
                // to a fixed length vector. If there are less than 600 words in the sentence,
                // the remaining indices will be filled with zeros. If there are more than
                // 600 words in the sentence, then the array is truncated at 600.
                // <SnippetResizeFeatures>
                Action<VariableLength, FixedLength> ResizeFeaturesAction = (s, f) =>
                {
                    var features = s.VariableLengthFeatures;
                    Array.Resize(ref features, FeatureLength);
                    f.Features = features;
                };
                // </SnippetResizeFeatures>

                // Load the TensorFlow model.
                // <SnippetLoadTensorFlowModel>
                TensorFlowModel tensorFlowModel = mlContext.Model.LoadTensorFlowModel(_modelPath);
                // </SnippetLoadTensorFlowModel>

                // <SnippetGetModelSchema>
                DataViewSchema schema = tensorFlowModel.GetModelSchema();
                Console.WriteLine(" =============== TensorFlow Model Schema =============== ");
                var featuresType = (VectorDataViewType)schema["Features"].Type;
                WriteLine($"Name: Features, Type: {featuresType.ItemType.RawType}, Size: ({featuresType.Dimensions[0]})");
                var predictionType = (VectorDataViewType)schema["Prediction/Softmax"].Type;
                WriteLine($"Name: Prediction/Softmax, Type: {predictionType.ItemType.RawType}, Size: ({predictionType.Dimensions[0]})");
                progressBar2.Value = 30;
                // </SnippetGetModelSchema>

                // <SnippetTokenizeIntoWords>
                IEstimator<ITransformer> pipeline =
                    // Split the text into individual words
                    mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "ReviewText")
                    // </SnippetTokenizeIntoWords>

                    // <SnippetMapValue>
                    // Map each word to an integer value. The array of integer makes up the input features.
                    .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap,
                        lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
                    // </SnippetMapValue>

                    // <SnippetCustomMapping>
                    // Resize variable length vector to fixed length vector.
                    .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
                    // </SnippetCustomMapping>

                    // <SnippetScoreTensorFlowModel>
                    // Passes the data to TensorFlow for scoring
                    .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
                    // </SnippetScoreTensorFlowModel>

                    // <SnippetCopyColumns>
                    // Retrieves the 'Prediction' from TensorFlow and and copies to a column
                    .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));
                // </SnippetCopyColumns>
                progressBar2.Value = 40;
                // <SnippetCreateModel>
                // Create an executable model from the estimator pipeline
                IDataView dataView = mlContext.Data.LoadFromEnumerable(new List<MovieReview>());
                ITransformer model = pipeline.Fit(dataView);
                progressBar2.Value = 90;
                // </SnippetCreateModel>

                // <SnippetSaveModel>
                mlContext.Model.Save(model, dataView.Schema, $"projects\\{projectName}\\model.zip");
                textBox2.Text = $"projects\\{projectName}\\model.zip";
                // using FileStream stream = File.Create($"projects\\{projectName}\\onnx_model.onnx");
                // mlContext.Model.ConvertToOnnx(model, dataView, stream);
                progressBar2.Value = 95;
                // </SnippetSaveModel>

                // <SnippetCallPredictSentiment>
                PredictSentiment(mlContext, model);
                progressBar2.Value = 100;
                // </SnippetCallPredictSentiment>
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error training model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button3.Enabled = true;
        }

        public static void PredictSentiment(MLContext mlContext, ITransformer model)
        {
            // <SnippetCreatePredictionEngine>
            var engine = mlContext.Model.CreatePredictionEngine<MovieReview, MovieReviewSentimentPrediction>(model);
            // </SnippetCreatePredictionEngine>

            // <SnippetCreateTestData>
            var review = new MovieReview()
            {
                ReviewText = textBox3.Text
            };
            // </SnippetCreateTestData>

            // Predict with TensorFlow pipeline.
            // <SnippetPredict>
            var sentimentPrediction = engine.Predict(review);
            // </SnippetPredict>

            // <SnippetDisplayPredictions>
            WriteLine("Number of classes: " + sentimentPrediction.Prediction.Length);
            WriteLine("Is sentiment/review positive? " + sentimentPrediction.Prediction[1]);
            label4.Text = $"Predict: {sentimentPrediction.Prediction[1]} ({sentimentPrediction.Prediction.Length})";
            // </SnippetDisplayPredictions>

            /////////////////////////////////// Expected output ///////////////////////////////////
            //
            // Name: Features, Type: System.Int32, Size: 600
            // Name: Prediction/Softmax, Type: System.Single, Size: 2
            //
            // Number of classes: 2
            // Is sentiment/review positive ? Yes
            // Prediction Confidence: 0.65
        }

        // <SnippetMovieReviewClass>
        /// <summary>
        /// Class to hold original sentiment data.
        /// </summary>
        public class MovieReview
        {
            public string ReviewText { get; set; }
        }
        //</SnippetMovieReviewClass>

        //<SnippetPrediction>
        /// <summary>
        /// Class to contain the output values from the transformation.
        /// </summary>
        public class MovieReviewSentimentPrediction
        {
            [VectorType(2)]
            public float[] Prediction { get; set; }
        }
        // </SnippetPrediction>

        // <SnippetVariableLengthFeatures>
        /// <summary>
        /// Class to hold the variable length feature vector. Used to define the
        /// column names used as input to the custom mapping action.
        /// </summary>
        public class VariableLength
        {
            /// <summary>
            /// This is a variable length vector designated by VectorType attribute.
            /// Variable length vectors are produced by applying operations such as 'TokenizeWords' on strings
            /// resulting in vectors of tokens of variable lengths.
            /// </summary>
            [VectorType]
            public int[] VariableLengthFeatures { get; set; }
        }
        // </SnippetVariableLengthFeatures>

        // <SnippetFixedLengthFeatures>
        /// <summary>
        /// Class to hold the fixed length feature vector. Used to define the
        /// column names used as output from the custom mapping action,
        /// </summary>
        public class FixedLength
        {
            /// <summary>
            /// This is a fixed length vector designated by VectorType attribute.
            /// </summary>
            [VectorType(FeatureLength)]
            public int[] Features { get; set; }
        }
        // </SnippetFixedLengthFeatures>

        private MLContext TestmlContext;
        private PredictionEngine<MovieReview, MovieReviewSentimentPrediction> TestpredictionEngine;

        private void InitModel()
        {
            try
            {
                TestmlContext = new MLContext();
                DataViewSchema predictionPipelineSchema;
                ITransformer predictionPipeline = TestmlContext.Model.Load($"projects\\{projectName}\\model.zip", out predictionPipelineSchema);
                TestpredictionEngine = TestmlContext.Model.CreatePredictionEngine<MovieReview, MovieReviewSentimentPrediction>(predictionPipeline);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                InitModel();

                var textData = new MovieReview()
                {
                    ReviewText = textBox4.Text
                };

                var prediction = TestpredictionEngine.Predict(textData);

                label8.Text = $"Predict: {prediction.Prediction[1]} ({prediction.Prediction.Length})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && File.Exists(textBox1.Text))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0 && File.Exists(textBox2.Text))
            {
                button5.Enabled = true;
                textBox4.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
                textBox4.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = textBox2.Text;
                if (!File.Exists(filePath))
                {
                    return;
                }

                string argument = "/select, \"" + filePath + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;
            }
        }
    }
}