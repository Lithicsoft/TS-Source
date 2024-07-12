// License: Apache-2.0
/*
 * Python/Python.xaml.cs: Trainer usercontrol for image classification
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.Python
{
    /// <summary>
    /// Interaction logic for Python.xaml
    /// </summary>
    public partial class Python : UserControl
    {
        private string projectName = string.Empty;

        public Python(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new PY.DataPreparation(projectName);
        }

        private void Button_Click1(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new PY.ConfigModel(projectName);
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new PY.TrainModel(projectName);
        }

        private void Button_Click_3(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new PY.ModelTester(projectName);
        }

        private void Button_Click_4(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new PY.ModelResult(projectName);
        }
    }
}
