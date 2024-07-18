// License: Apache-2.0
/*
 * CSharpML/ImageClassification.cs: CSharp trainer for image classification
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Text;

namespace Lithicsoft_Trainer_Studio.CSharpML
{
    class ImageClassification
    {
        static string _assetsPath = Path.Combine(Environment.CurrentDirectory, ".");
        static string _imagesFolder = Path.Combine(_assetsPath, ".");
        static string _trainTagsTsv = Path.Combine(_imagesFolder, ".");
        static string _testTagsTsv = Path.Combine(_imagesFolder, ".");
        static string _inceptionTensorFlowModel = Path.Combine(_assetsPath, ".", ".");

        private static string projectName = string.Empty;

        public void Train(string name)
        {
            projectName = name;

            _assetsPath = Path.Combine(Environment.CurrentDirectory, $"projects\\{projectName}");
            _imagesFolder = Path.Combine(_assetsPath, "datasets");
            _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
            _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
            _inceptionTensorFlowModel = Path.Combine("inception", "tensorflow_inception_graph.pb");

            MLContext mlContext = new MLContext();

            ITransformer model = GenerateModel(mlContext);
        }

        public async Task DataPrepare(string rootPath, string projectName)
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

            var tsvContent = tsvData.ToString();
            var tagsPath = Path.Combine("projects", projectName, "datasets", "tags.tsv");
            var testTagsPath = Path.Combine("projects", projectName, "datasets", "test-tags.tsv");

            var writeTasks = new Task[]
            {
                File.WriteAllTextAsync(tagsPath, tsvContent),
                File.WriteAllTextAsync(testTagsPath, tsvContent)
            };

            await Task.WhenAll(writeTasks);
        }


        public static ITransformer GenerateModel(MLContext mlContext)
        {
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                            .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
                                ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                            .AppendCacheCheckpoint(mlContext);

            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);

            ITransformer model = pipeline.Fit(trainingData);

            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);

            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                  labelColumnName: "LabelKey",
                  predictedLabelColumnName: "PredictedLabel");

            mlContext.Model.Save(model, trainingData.Schema, $"projects\\{projectName}\\outputs\\model.zip");
            using FileStream stream = File.Create($"projects\\{projectName}\\outputs\\onnx_model.onnx");
            
            return model;
        }

        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
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
