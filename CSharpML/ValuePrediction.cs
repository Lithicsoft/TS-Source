// License: Apache-2.0
/*
 * CSharpML/ValuePrediction.cs: CSharp trainer for values prediction
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Microsoft.ML;
using Microsoft.ML.Data;

namespace Lithicsoft_Trainer_Studio.CSharpML
{
    class ValuePrediction
    {
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

        static string? _trainDataPath;
        static string? _modelPath;

        private string? projectName;

        public void Train(string name)
        {
            projectName = name;
            _trainDataPath = $"projects\\{projectName}\\datasets\\dataset.csv";
            _modelPath = $"projects\\{projectName}\\outputs\\model.zip";

            MLContext mlContext = new(seed: 0);
            Train(mlContext, _trainDataPath);
        }
        
        private static TransformerChain<RegressionPredictionTransformer<Microsoft.ML.Trainers.FastTree.FastTreeRegressionModelParameters>> Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ModelInput.Prediction))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ModelInput.A), nameof(ModelInput.B), nameof(ModelInput.C), nameof(ModelInput.D)))
                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(dataView);

            mlContext.Model.Save(model, dataView.Schema, _modelPath);

            return model;
        }
    }
}
