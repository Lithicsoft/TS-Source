﻿// License: Apache-2.0
/*
 * CSharp/VP/TrainModel.xaml.cs: Back-end source code for model trainer value prediction page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.CSharp.VP
{
    /// <summary>
    /// Interaction logic for TrainModel.xaml
    /// </summary>
    public partial class TrainModel : Page
    {
        private string projectName = string.Empty;

        public TrainModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private static TrainModel _instance;
        public bool isTraining = false;

        public static TrainModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TrainModel(string.Empty);
                return _instance;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"projects\\{projectName}\\datasets\\dataset.tsv"))
            {
                label1.Content = "Ready for train your model";
                button1.IsEnabled = true;
            }
            else
            {
                label1.Content = "Please prepare your datasets before training model";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            TrainModel.Instance.isTraining = true;
            label1.Content = "Training your model...";
            CSharpML.ValuePrediction valuePrediction = new CSharpML.ValuePrediction();
            valuePrediction.Train(projectName);
            label1.Content = "Done!";
            TrainModel.Instance.isTraining = true;
            button1.IsEnabled = true;
        }
    }
}