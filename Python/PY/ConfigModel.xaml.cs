// License: Apache-2.0
/*
 * Python/PY/ConfigModel.xaml.cs: Back-end source code for config model python page
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Lithicsoft_Trainer_Studio.UserControls.Pages;
using Tensorflow;

namespace Lithicsoft_Trainer_Studio.Python.PY
{
    /// <summary>
    /// Interaction logic for ConfigModel.xaml
    /// </summary>
    public partial class ConfigModel : Page
    {
        private string projectName = string.Empty;

        public ConfigModel(string name)
        {
            InitializeComponent();

            projectName = name;
        }

        private ObservableCollection<VariableSelection> items;
        private Dictionary<string, string> trainParameters;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                trainParameters = (Dictionary<string, string>)DotEnv.Load($"projects\\{projectName}\\.env");

                items = new ObservableCollection<VariableSelection>();

                foreach (var kvp in trainParameters)
                {
                    items.Add(new VariableSelection { Variable = kvp.Key, Value = kvp.Value });
                }

                listView1.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading config: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in items)
                {
                    trainParameters[item.Variable] = item.Value;
                }

                DotEnv.Save($"projects\\{projectName}\\.env", trainParameters);
                MessageBox.Show("All changes have been saved.", "Saving Config", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class VariableSelection
        {
            public string Variable { get; set; }
            public string Value { get; set; }
        }
    }

    public static class DotEnv
    {
        public static IDictionary<string, string> Load(string filePath)
        {
            var envDictionary = new Dictionary<string, string>();

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                envDictionary[key] = value;
            }

            return envDictionary;
        }

        public static void Save(string filePath, IDictionary<string, string> envDictionary)
        {
            var lines = new List<string>();

            foreach (var kvp in envDictionary)
            {
                lines.Add($"{kvp.Key}={kvp.Value}");
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
