// License: Apache-2.0
/*
 * CSharp/ValuePrediction.xaml.cs: Trainer usercontrol for value prediction
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.CSharp
{
    /// <summary>
    /// Interaction logic for ValuePrediction.xaml
    /// </summary>
    public partial class ValuePrediction : UserControl
    {
        private readonly string projectName = string.Empty;

        public ValuePrediction(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new VP.DataPreparation(projectName);
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new VP.TrainModel(projectName);
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new VP.ModelTester(projectName);
        }

        private void Button_Click_3(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new VP.ModelResult(projectName);
        }
    }
}
