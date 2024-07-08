// License: Apache-2.0
/*
 * Form1.cs: Create form, project creator for Trainer Studio
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Net;

namespace Lithicsoft_Trainer
{
    public partial class Create : Form
    {
        public Create()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            try
            {
                Directory.CreateDirectory($"projects\\{textBox1.Text}");
                File.WriteAllLines($"projects\\{textBox1.Text}\\{textBox1.Text}.project", [comboBox2.Text, comboBox1.Text]);
                await UpdateProgressBarAsync(20);

                if (comboBox2.Text != "C#")
                {
                    Directory.CreateDirectory($"projects\\{textBox1.Text}\\python");
                    try
                    {
                        ZipFile.ExtractToDirectory("Python.zip", $"projects\\{textBox1.Text}");
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Error extracting zip file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }

                    await UpdateProgressBarAsync(30);

                    try
                    {
                        await PythonSetup();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error installing dependencies package: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
                await UpdateProgressBarAsync(100);

                Main main = new Main(textBox1.Text, comboBox2.Text, comboBox1.Text);
                main.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating project: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PythonSetup()
        {
            await InstallPackageDependencies(comboBox2.Text);
        }

        bool CheckForDiscreteGPU()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                ManagementObjectCollection moc = searcher.Get();

                bool discreteGPUFound = false;

                foreach (ManagementObject mo in moc)
                {
                    string adapterCompatibility = mo["AdapterCompatibility"]?.ToString();
                    string description = mo["Description"]?.ToString();

                    if (adapterCompatibility != null &&
                        (adapterCompatibility.Contains("NVIDIA") || adapterCompatibility.Contains("AMD")))
                    {
                        discreteGPUFound = true;
                        string gpuName = mo["Name"]?.ToString();
                        return true;
                    }
                }

                if (!discreteGPUFound)
                {
                    MessageBox.Show("No discrete GPU found, you will train your model with CPU", "No GPU Found! Using CPU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking gpu: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }


        private async Task InstallPackageDependencies(string language)
        {
            var packageRequirements = new List<string>();

            switch (language)
            {
                case "Python (Tensorflow)":
                    packageRequirements.Add(CheckForDiscreteGPU() ? "tensorflow" : "tensorflow");

                    if (comboBox1.Text == "Text generation (RNN)")
                    {
                        packageRequirements.Add("numpy");
                        await DownloadFilesForRNN(textBox1.Text);
                    }
                    break;

                case "Python (PyTorch)":
                    packageRequirements.Add("torch");
                    break;
            }
            await UpdateProgressBarAsync(35);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $"projects\\{textBox1.Text}\\python\\Scripts\\python.exe";
            start.Arguments = "-m pip install python-dotenv " + string.Join(" ", packageRequirements);
            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;

            await UpdateProgressBarAsync(40);
            Process process = Process.Start(start);

            await UpdateProgressBarAsync(50);
        }

        private async Task DownloadFilesForRNN(string projectPath)
        {
            using (var client = new WebClient())
            {
                try
                {
                    var baseUri = "https://raw.githubusercontent.com/Lithicsoft/Lithicsoft-Trainer-Studio/main/rnn_text_generation/";
                    await client.DownloadFileTaskAsync(new Uri(baseUri + "trainer.py"), $"projects\\{projectPath}\\trainer.py");
                    await client.DownloadFileTaskAsync(new Uri(baseUri + ".env"), $"projects\\{projectPath}\\.env");
                    await UpdateProgressBarAsync(20);
                }
                catch (WebException ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && comboBox1.Text.Length > 0 && comboBox2.Text.Length > 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && comboBox1.Text.Length > 0 && comboBox2.Text.Length > 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Text = string.Empty;
            if (comboBox2.Text == "C#")
            {
                comboBox1.Items.AddRange(["Image classification", "Text classification"]);
            }
            else if (comboBox2.Text == "Python (PyTorch)")
            {
                comboBox1.Items.AddRange(["Text generation"]);
            }
            else if (comboBox2.Text == "Python (Tensorflow)")
            {
                comboBox1.Items.AddRange(["Text generation (RNN)"]);
            }
        }

        private Task UpdateProgressBarAsync(int value)
        {
            return Task.Run(() =>
            {
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new Action(() => progressBar1.Value = value));
                }
                else
                {
                    progressBar1.Value = value;
                }
            });
        }
    }
}
