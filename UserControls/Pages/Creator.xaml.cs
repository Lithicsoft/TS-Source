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
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ModernWpf.MessageBox;

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
                    if (ProjectName != null && ProjectLanguage != null && ProjectType != null)
                    {
                        Directory.CreateDirectory($"projects\\{ProjectName}");
                        await File.WriteAllLinesAsync($"projects\\{ProjectName}\\{ProjectName}.project", [ProjectLanguage, ProjectType]);
                        Directory.CreateDirectory($"projects\\{ProjectName}\\outputs");
                    }

                    if (ProjectLanguage != "CSharp")
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(ProjectLanguage) && !string.IsNullOrEmpty(ProjectType))
                            {
                                await PythonSetup(ProjectName, ProjectLanguage, ProjectType);
                            }
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

        private static async Task PythonSetup(string ProjectName, string ProjectLanguage, string ProjectType)
        {
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(ProjectLanguage) && !string.IsNullOrEmpty(ProjectType))
            {
                await InstallPackageDependencies(ProjectName, ProjectLanguage, ProjectType);
            }
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

        private static async Task InstallPackageDependencies(string ProjectName, string ProjectLanguage, string ProjectType)
        {
            ArgumentNullException.ThrowIfNull(ProjectName);
            ArgumentNullException.ThrowIfNull(ProjectLanguage);
            ArgumentNullException.ThrowIfNull(ProjectType);

            if (!string.IsNullOrEmpty(ProjectLanguage) && !string.IsNullOrEmpty(ProjectType))
            {
                await DownloadKitFiles(ProjectName, ProjectType, GetCheckForDiscreteGPU());
            }

            try
            {
                string[] kitRequirements = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"projects\\{ProjectName}\\requirements.txt"));

                ProcessStartInfo start = new()
                {
                    FileName = $"cmd.exe",
                    Arguments = $"/K conda create -n \"{ProjectName}\" {kitRequirements[0]} -y & conda activate {ProjectName} & conda install {kitRequirements[1]} -y & python -m pip install python-dotenv -y & conda deactivate",
                    UseShellExecute = true,
                    RedirectStandardOutput = false
                };

                Process.Start(start);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error installing python packages: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        static async Task<Dictionary<string, string>> LoadDataFromWebAsync(string url)
        {
            using HttpClient client = new();
            string data = await client.GetStringAsync(url);

            Dictionary<string, string> result = [];

            string[] lines = data.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] parts = line.Split([':'], 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    result[parts[0]] = parts[1];
                }
            }

            return result;
        }

        private static async Task<bool> FileExistsAsync(string fileUri)
        {
            using HttpClient client = new();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, fileUri));
            return response.IsSuccessStatusCode;
        }

        private static async Task DownloadKitFiles(string projectPath, string projectType, bool usingGPU)
        {
            try
            {
                Dictionary<string, string> pythonTrainerKit = await LoadDataFromWebAsync("https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/refs/heads/main/dictionary.txt");

                using HttpClient client = new();

                async Task DownloadFileAsync(string sourceUrl, string destinationPath)
                {
                    HttpResponseMessage response = await client.GetAsync(sourceUrl);
                    response.EnsureSuccessStatusCode();

                    using FileStream fs = new(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);
                }

                var baseUri = $"https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/main/{pythonTrainerKit[projectType]}/";
                string requirementsFile = usingGPU ? "requirements-gpu.txt" : "requirements-cpu.txt";
                string projectBasePath = $"projects\\{projectPath}\\";

                await DownloadFileAsync(baseUri + requirementsFile, projectBasePath + "requirements.txt");

                string[] filesToDownload = ["trainer.py", "tester.py", "source.zip", ".env"];
                foreach (var file in filesToDownload)
                {
                    if (!await FileExistsAsync(baseUri + file))
                    {
                        continue;
                    }
                    await DownloadFileAsync(baseUri + file, projectBasePath + file);
                }

                if (File.Exists(projectBasePath + "src.zip"))
                {
                    ZipFile.ExtractToDirectory(projectBasePath + "source.zip", projectBasePath + "source");
                }
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

        private static async Task LoadItemsFromFileAsync(string url, ObservableCollection<string> items)
        {
            using HttpClient client = new();
            string data = await client.GetStringAsync(url);

            string[] lines = data.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                items.Add(line);
            }
        }

        public class ViewModelType : INotifyPropertyChanged
        {
            private ObservableCollection<string>? _listOfTypes;
            public ObservableCollection<string>? ListOfTypes
            {
                get { return _listOfTypes; }
                private set
                {
                    _listOfTypes = value;
                    OnPropertyChanged();
                }
            }

            public ViewModelType() => ListOfTypes = [];

            public async void UpdateListOfTypes(ComboBox comboBox1)
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
                        await LoadItemsFromFileAsync("https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/refs/heads/main/tensorflow.txt", items);
                        createAble = true;
                    }
                    else if (selectedItem == "Python (PyTorch)")
                    {
                        await LoadItemsFromFileAsync("https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/refs/heads/main/pytorch.txt", items);
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
