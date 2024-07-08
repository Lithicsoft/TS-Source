// License: Apache-2.0
/*
 * Form1.cs: UserControl for python project, support full project type and langguage
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using AForge.Video.DirectShow;
using System.Diagnostics;
using System.IO.Compression;

namespace Lithicsoft_Trainer
{
    public partial class Python : UserControl
    {
        static string projectName = string.Empty;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private int currentCameraIndex = 0;

        private IDictionary<string, string> trainParameters;

        public Python(string name, string language, string type)
        {
            InitializeComponent();
            label1.ForeColor = Color.FromArgb(0, 120, 215);

            projectName = name;
            label1.Text = $"{type} with {language}";

            if (Directory.Exists($"projects\\{projectName}\\model"))
            {
                textBox2.Text = $"projects\\{projectName}\\model";
            }

            trainParameters = DotEnv.Load($"projects\\{projectName}\\.env");
            listView1.View = View.Details;
            listView1.Columns.Add("Variable", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Value", -2, HorizontalAlignment.Left);

            foreach (var kvp in trainParameters)
            {
                var item = new ListViewItem(kvp.Key);
                item.SubItems.Add(kvp.Value);
                listView1.Items.Add(item);
            }

            listView1.LabelEdit = true;
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Value = 0;
                richTextBox1.Text += "\nUnzipping the file...\n";
                if (Directory.Exists($"projects\\{projectName}\\datasets"))
                {
                    Directory.Delete($"projects\\{projectName}\\datasets", true);
                }
                Directory.CreateDirectory($"projects\\{projectName}\\datasets");
                string zipPath = textBox1.Text;
                progressBar1.Value = 50;
                string extractPath = $"projects\\{projectName}\\datasets";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                progressBar1.Value = 100;

                richTextBox1.Text += "Done!\n";
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dataset: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action<string>(AppendText), text);
            }
            else
            {
                richTextBox2.AppendText(text);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            try
            {
                progressBar2.Value = 0;

                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = $"projects\\{projectName}\\python\\Scripts\\python.exe",
                    Arguments = $"projects\\{projectName}\\trainer.py",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                progressBar2.Value = 30;

                using (Process process = new Process())
                {
                    process.StartInfo = start;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            this.Invoke((Action)(() => AppendText(e.Data + Environment.NewLine)));
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            this.Invoke((Action)(() => AppendText("ERROR: " + e.Data + Environment.NewLine)));
                        }
                    };

                    await Task.Run(() =>
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    });

                    progressBar2.Value = 100;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error training model: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button3.Enabled = true;
        }

        private void InitModel()
        {
            try
            {
                // working
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

                //working
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
            if (textBox3.Tag != null)
            {
                ListViewItem selectedItem = (ListViewItem)textBox3.Tag;
                selectedItem.SubItems[1].Text = textBox3.Text;
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

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                textBox3.Text = selectedItem.SubItems[1].Text;
                textBox3.Tag = selectedItem;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                var key = item.Text;
                var value = item.SubItems[1].Text;
                trainParameters[key] = value;
            }

            DotEnv.Save($"projects\\{projectName}\\.env", trainParameters);
            MessageBox.Show("All changes have been saved.", "Saving Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
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