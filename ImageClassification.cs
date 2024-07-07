// License: Apache-2.0
/*
 * Form1.cs: UserControl for C# Image Classification
 *
 * (C) Copyright 2024 Lithicsoft Organization, Microsoft Learn
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 *         luisquintanilla
 *         gewarren
 *         jwood803
 *         BillWagner
 *         Youssef1313
 *         nschonni
 */

using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO.Compression;
using System.Text;

namespace Lithicsoft_Trainer
{
    public partial class ImageClassification : UserControl
    {
        static string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static string _imagesFolder = Path.Combine(_assetsPath, "images");
        static string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        static string _predictSingleImage = Path.Combine(_imagesFolder, "x.png");
        static string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");

        static string projectName = string.Empty;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private int currentCameraIndex = 0;

        public ImageClassification(string name)
        {
            InitializeComponent();
            label1.ForeColor = Color.FromArgb(0, 120, 215);

            this.HandleDestroyed += new EventHandler(UserControl_HandleDestroyed);
            projectName = name;

            if (File.Exists($"projects\\{projectName}\\model.zip"))
            {
                textBox2.Text = $"projects\\{projectName}\\model.zip";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Zip files (*.zip)|*.zip";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening zip file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DataPrepare(string rootPath)
        {
            try
            {
                var directories = Directory.GetDirectories(rootPath);

                var tsvData = new StringBuilder();

                foreach (var directory in directories)
                {
                    var label = Path.GetFileName(directory);

                    var files = Directory.GetFiles(directory, "*.*");

                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file).ToLower();
                        if (extension == ".png" || extension == ".jpg" || extension == ".webp")
                        {
                            var relativePath = Path.GetRelativePath(rootPath, file);

                            tsvData.AppendLine($"{relativePath}\t{label}");
                        }
                    }
                }

                File.WriteAllText($"projects\\{projectName}\\images\\tags.tsv", tsvData.ToString());
                File.WriteAllText($"projects\\{projectName}\\images\\test-tags.tsv", tsvData.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error preparing dataset: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(textBox1.Text))
                {
                    var validExtensions = new[] { ".png", ".jpg", ".webp" };

                    bool isValid = archive.Entries
                        .Where(e => !string.IsNullOrEmpty(Path.GetDirectoryName(e.FullName)))
                        .GroupBy(e => Path.GetDirectoryName(e.FullName))
                        .All(g => g.Any(e => validExtensions.Contains(Path.GetExtension(e.FullName))));

                    if (!isValid)
                    {
                        MessageBox.Show("The zip structure is invalid", "Dataset Structure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking zip file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                progressBar1.Value = 0;
                richTextBox1.Text += "\nUnzipping the file...\n";
                if (Directory.Exists($"projects\\{projectName}\\images"))
                {
                    Directory.Delete($"projects\\{projectName}\\images", true);
                }
                Directory.CreateDirectory($"projects\\{projectName}\\images");
                string zipPath = textBox1.Text;
                string extractPath = $"projects\\{projectName}\\images";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                progressBar1.Value = 30;

                richTextBox1.Text += "Generating TSV...\n";
                DataPrepare($"projects\\{projectName}\\images");
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

        string GetRandomImageFile(string folderPath)
        {
            try
            {
                string[] imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                               .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".webp"))
                                               .ToArray();

                Random rand = new Random();
                int index = rand.Next(imageFiles.Length);

                return imageFiles[index];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting random test file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                pictureBox2.Image = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating camera frame: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Couldn't find any cameras!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    button6.Enabled = false;
                    return;
                }

                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.NewFrame -= new NewFrameEventHandler(videoSource_NewFrame);
                }

                videoSource = new VideoCaptureDevice(videoDevices[currentCameraIndex].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening camera: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            string folderPath = $"projects\\{projectName}\\images";
            var validExtensions = new[] { ".png", ".jpg", ".webp" };

            try
            {
                var subdirectories = Directory.GetDirectories(folderPath);

                bool isValid = subdirectories
                    .Select(subdir => Directory.GetFiles(subdir))
                    .All(files => files.Any(file => validExtensions.Contains(Path.GetExtension(file).ToLower())));

                if (!isValid)
                {
                    MessageBox.Show("The dataset structure is invalid", "Dataset Structure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking dataset: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                _assetsPath = Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}");
                _imagesFolder = Path.Combine(_assetsPath, "images");
                _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
                _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
                _predictSingleImage = Path.Combine(_imagesFolder, GetRandomImageFile(_imagesFolder));
                _inceptionTensorFlowModel = Path.Combine("inception", "tensorflow_inception_graph.pb");
                progressBar2.Value = 6;

                // Create MLContext to be shared across the model creation workflow objects
                // <SnippetCreateMLContext>
                MLContext mlContext = new MLContext();
                progressBar2.Value = 7;
                // </SnippetCreateMLContext>

                // <SnippetCallGenerateModel>
                ITransformer model = GenerateModel(mlContext);
                // </SnippetCallGenerateModel>

                // <SnippetCallClassifySingleImage>
                ClassifySingleImage(mlContext, model);
                progressBar2.Value = 100;
                // </SnippetCallClassifySingleImage>
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error training model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            StartCamera();
            button3.Enabled = true;
        }

        private void UserControl_HandleDestroyed(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        // Build and train model
        public static ITransformer GenerateModel(MLContext mlContext)
        {
            // <SnippetImageTransforms>
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                            // The image transforms transform the images into the model's expected format.
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                            // </SnippetImageTransforms>
                            // The ScoreTensorFlowModel transform scores the TensorFlow model and allows communication
                            // <SnippetScoreTensorFlowModel>
                            .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
                                ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                            // </SnippetScoreTensorFlowModel>
                            // <SnippetMapValueToKey>
                            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                            // </SnippetMapValueToKey>
                            // <SnippetAddTrainer>
                            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                            // </SnippetAddTrainer>
                            // <SnippetMapKeyToValue>
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                            .AppendCacheCheckpoint(mlContext);
            // </SnippetMapKeyToValue>
            progressBar2.Value = 20;

            // <SnippetLoadData>
            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            progressBar2.Value = 30;
            // </SnippetLoadData>

            // Train the model
            WriteLine("=============== Training classification model ===============");
            // Create and train the model
            // <SnippetTrainModel>
            ITransformer model = pipeline.Fit(trainingData);
            progressBar2.Value = 60;
            // </SnippetTrainModel>

            // Generate predictions from the test data, to be evaluated
            // <SnippetLoadAndTransformTestData>
            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);
            progressBar2.Value = 70;

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);
            progressBar2.Value = 80;
            // </SnippetLoadAndTransformTestData>

            // Get performance metrics on the model using training data
            WriteLine("=============== Classification metrics ===============");

            // <SnippetEvaluate>
            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                  labelColumnName: "LabelKey",
                  predictedLabelColumnName: "PredictedLabel");
            // </SnippetEvaluate>

            //<SnippetDisplayMetrics>
            WriteLine($"LogLoss is: {metrics.LogLoss}");
            WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");
            progressBar2.Value = 90;
            //</SnippetDisplayMetrics>

            // <SnippetSaveModel>
            mlContext.Model.Save(model, trainingData.Schema, $"projects\\{projectName}\\model.zip");
            using FileStream stream = File.Create($"projects\\{projectName}\\onnx_model.onnx");
            mlContext.Model.ConvertToOnnx(model, trainingData, stream);
            textBox2.Text = $"projects\\{projectName}\\model.zip";
            progressBar2.Value = 95;
            // </SnippetSaveModel>

            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        public static void ClassifySingleImage(MLContext mlContext, ITransformer model)
        {
            // load the fully qualified image file name into ImageData
            // <SnippetLoadImageData>
            var imageData = new ImageData()
            {
                ImagePath = _predictSingleImage
            };
            // </SnippetLoadImageData>

            // <SnippetPredictSingle>
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);
            // </SnippetPredictSingle>

            WriteLine("=============== Making single image classification ===============");
            // <SnippetDisplayPrediction>
            WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            // </SnippetDisplayPrediction>

            pictureBox1.ImageLocation = _predictSingleImage;
            label4.Text = $"Predict: {prediction.PredictedLabelValue} ({prediction.Score.Max():P2})";
        }

        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            // <SnippetDisplayPredictions>
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
            // </SnippetDisplayPredictions>
        }

        // <SnippetInceptionSettings>
        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        // </SnippetInceptionSettings>

        // <SnippetDeclareImageData>
        public class ImageData
        {
            [LoadColumn(0)]
            public string ImagePath;

            [LoadColumn(1)]
            public string Label;
        }
        // </SnippetDeclareImageData>

        // <SnippetDeclareImagePrediction>
        public class ImagePrediction : ImageData
        {
            public float[] Score;

            public string PredictedLabelValue;
        }
        // </SnippetDeclareImagePrediction>

        private MLContext TestmlContext;
        private PredictionEngine<ImageData, ImagePrediction> TestpredictionEngine;

        private void InitModel()
        {
            try
            {
                TestmlContext = new MLContext();
                DataViewSchema predictionPipelineSchema;
                ITransformer predictionPipeline = TestmlContext.Model.Load($"projects\\{projectName}\\model.zip", out predictionPipelineSchema);
                TestpredictionEngine = TestmlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(predictionPipeline);
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

                string tempFilePath = Path.Combine(Path.GetTempPath(), "capturedImage.jpg");
                pictureBox2.Image.Save(tempFilePath);

                var imageData = new ImageData()
                {
                    ImagePath = tempFilePath
                };

                var prediction = TestpredictionEngine.Predict(imageData);

                label8.Text = $"Predict: {prediction.PredictedLabelValue} ({prediction.Score.Max():P2})";
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
                button6.Enabled = true;
                button4.Enabled = true;
                button7.Enabled = true;
                StartCamera();
            }
            else
            {
                button5.Enabled = false;
                button6.Enabled = false;
                button4.Enabled = false;
                button7.Enabled = false;
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

        private void button4_Click(object sender, EventArgs e)
        {
            StartCamera();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.ImageLocation = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening image file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}