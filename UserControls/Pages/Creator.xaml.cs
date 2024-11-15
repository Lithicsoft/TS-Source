// License: Apache-2.0
/*
 * UserControls/Pages/Creator.xaml.cs: Back-end source code for projects creator
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using Lithicsoft_Trainer_Studio.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Lithicsoft_Trainer_Studio.UserControls.Pages
{
    /// <summary>
    /// Interaction logic for Creator.xaml
    /// </summary>
    public partial class Creator : Page
    {
        public Creator()
        {
            InitializeComponent();
        }

        public event EventHandler? PageClosed;

        private ViewModelType? viewModelType;
        private ViewModel? viewModel;

        private static Creator? _instance;
        public bool isCreating = false;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new ViewModel();
            viewModelType = new ViewModelType();

            DataContext = this;

            comboBox1.DataContext = viewModel;
            comboBox2.DataContext = viewModelType;

            comboBox1.ItemsSource = viewModel.ListOfLanguages;
        }

        public static Creator Instance
        {
            get
            {
                _instance ??= new Creator();
                return _instance;
            }
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {

            button1.IsEnabled = false;
            textBox1.IsEnabled = false;
            comboBox1.IsEnabled = false;
            comboBox2.IsEnabled = false;
            Creator.Instance.isCreating = true;

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Hide();

            var loadingWindow = new LoadingWindow("Creating Project...")
            {
                Owner = parentWindow
            };
            loadingWindow.Show();

            await Task.Delay(1000);

            string ProjectName = textBox1.Text.Replace("\"", "").Replace("&", "").Replace("|", "").Replace(";", "");
            string? ProjectLanguage = comboBox1.SelectedItem.ToString();
            string? ProjectType = comboBox2.SelectedItem.ToString();

            await Task.Run(async () =>
            {
                try
                {
                    if (ProjectName != null && ProjectLanguage != null && ProjectType != null) {
                        Directory.CreateDirectory($"projects\\{ProjectName}");
                        await File.WriteAllLinesAsync($"projects\\{ProjectName}\\{ProjectName}.project", [ProjectLanguage, ProjectType]);
                        Directory.CreateDirectory($"projects\\{ProjectName}\\outputs");
                    }

                    if (ProjectLanguage != "CSharp")
                    {
                        try
                        {
                            await PythonSetup();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error installing dependencies package: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating project: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            loadingWindow.Hide();
            if (ProjectName != null && ProjectLanguage != null && ProjectType != null)
            {
                Trainer trainer = new(ProjectName, ProjectLanguage, ProjectType);
                trainer.Show();
            }
            
            Creator.Instance.isCreating = false;
        }

        private async Task PythonSetup()
        {
            await InstallPackageDependencies(comboBox1.SelectedItem.ToString());
        }

        private static bool GetCheckForDiscreteGPU()
        {
            bool discreteGPUFound = false;

            try
            {
                ManagementObjectSearcher searcher = new("select * from Win32_VideoController");
                ManagementObjectCollection moc = searcher.Get();

                foreach (ManagementObject mo in moc.Cast<ManagementObject>())
                {
                    string? adapterCompatibility = mo["AdapterCompatibility"]?.ToString();
                    string? description = mo["Description"]?.ToString();

                    if (adapterCompatibility != null &&
                        (adapterCompatibility.Contains("NVIDIA") || adapterCompatibility.Contains("AMD")))
                    {
                        discreteGPUFound = true;
                        string? gpuName = mo["Name"]?.ToString();
                    }
                }

                if (!discreteGPUFound)
                {
                    MessageBox.Show("No discrete GPU found, you will train your model with CPU", "No GPU Found! Using CPU", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking gpu: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return discreteGPUFound;

        }

        private async Task InstallPackageDependencies(string language)
        {
            ArgumentNullException.ThrowIfNull(language);

            if (!string.IsNullOrEmpty(comboBox2.SelectedItem.ToString()))
            {
                await DownloadKitFiles(textBox1.Text, comboBox2.SelectedItem.ToString(), GetCheckForDiscreteGPU());
            }

            try
            {
                string[] kitRequirements = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"projects\\{textBox1.Text}\\requirements.txt"));

                ProcessStartInfo start = new()
                {
                    FileName = $"cmd.exe",
                    Arguments = $"/K conda create -n \"{textBox1.Text}\" {kitRequirements[0]} & conda activate {textBox1.Text} & conda install {kitRequirements[1]} & python -m pip install python-dotenv & conda deactivate",
                    UseShellExecute = true,
                    RedirectStandardOutput = false
                };

                Process process = Process.Start(start);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error installing python packages: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private static async Task DownloadKitFiles(string projectPath, string projectType, bool usingGPU)
        {
            Dictionary<string, string> pythonTrainerKit = new()
            {
                {"Text Generation (RNN)", "rnn_text_generation"},
                {"Text Generation (LSTM)", "lstm_text_generation"}
            };

            try
            {
                using var client = new WebClient();

                var baseUri = $"https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/main/{pythonTrainerKit[projectType]}/";
                if (usingGPU)
                {
                    await client.DownloadFileTaskAsync(new Uri(baseUri + "requirements-gpu.txt"), $"projects\\{projectPath}\\requirements.txt");
                }
                else
                {
                    await client.DownloadFileTaskAsync(new Uri(baseUri + "requirements-cpu.txt"), $"projects\\{projectPath}\\requirements.txt");
                }
                await client.DownloadFileTaskAsync(new Uri(baseUri + "trainer.py"), $"projects\\{projectPath}\\trainer.py");
                await client.DownloadFileTaskAsync(new Uri(baseUri + "tester.py"), $"projects\\{projectPath}\\tester.py");
                await client.DownloadFileTaskAsync(new Uri(baseUri + ".env"), $"projects\\{projectPath}\\.env");
            }
            catch (WebException ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void CheckStation()
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || string.IsNullOrEmpty(textBox1.Text))
            {
                button1.IsEnabled = false;
            }
            else
            {
                button1.IsEnabled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckStation();
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModelType?.UpdateListOfTypes(comboBox1);

            CheckStation();
        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckStation();
        }

        public class ViewModel
        {
            public ObservableCollection<string> ListOfLanguages { get; private set; }

            public ViewModel()
            {
                ListOfLanguages =
                [
                    "CSharp",
                    "Python (TensorFlow)",
                    "Python (PyTorch)"
                ];
            }
        }

        public class ViewModelType : INotifyPropertyChanged
        {
            private ObservableCollection<string> _listOfTypes;
            public ObservableCollection<string> ListOfTypes
            {
                get { return _listOfTypes; }
                private set
                {
                    _listOfTypes = value;
                    OnPropertyChanged();
                }
            }

            public ViewModelType()
            {
                ListOfTypes = [];
            }

            public void UpdateListOfTypes(ComboBox comboBox1)
            {
                ObservableCollection<string> items = [];
                bool createAble = false;

                if (comboBox1.SelectedItem != null)
                {
                    string? selectedItem = comboBox1.SelectedItem.ToString();

                    if (selectedItem == "CSharp")
                    {
                        items = ["Image Classification", "Value Prediction"];
                        createAble = true;
                    }
                    else if (selectedItem == "Python (TensorFlow)")
                    {
                        items = ["Text Generation (RNN)"];
                        createAble = true;
                    }
                    else if (selectedItem == "Python (PyTorch)")
                    {
                        items = ["Text Generation (LSTM)"];
                        createAble = true;
                    }
                }

                ListOfTypes = createAble ? items : [];
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string? name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
