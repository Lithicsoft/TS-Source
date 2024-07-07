// License: Apache-2.0
/*
 * Form1.cs: UserControl for project manager, also include project creator button, import project and infomation about the program
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Diagnostics;
using System.IO.Compression;

namespace Lithicsoft_Trainer
{
    public partial class Manager : UserControl
    {
        public Manager()
        {
            InitializeComponent();

            Color highlight = Color.FromArgb(0, 120, 215);
            label1.ForeColor = highlight;
            label2.ForeColor = highlight;
            button1.ForeColor = highlight;
            button2.ForeColor = highlight;
            button3.ForeColor = highlight;

            updateProjectList();
        }

        private void updateProjectList()
        {
            try
            {
                string[] projectFiles = Directory.GetFiles("projects", "*.project", SearchOption.AllDirectories);

                listView1.Items.Clear();


                foreach (string file in projectFiles)
                {
                    string[] projectConfig = File.ReadAllLines(file);
                    if (projectConfig[0].Length <= 0)
                    {
                        projectConfig[0] = "Unknown";
                    }
                    if (projectConfig[1].Length <= 0)
                    {
                        projectConfig[1] = "Unknown";
                    }
                    string projectName = Path.GetFileNameWithoutExtension(file);
                    ListViewItem item = new ListViewItem(projectName);
                    item.SubItems.Add(projectConfig[0]);
                    item.SubItems.Add(projectConfig[1]);
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating project list: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    string name = selectedItem.Text;
                    string language = selectedItem.SubItems[1].Text;
                    string type = selectedItem.SubItems[2].Text;

                    Main main = new Main(name, language, type);
                    main.Show();

                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening project: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Create create = new Create();
            create.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Zip files (*.zip)|*.zip";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string zipPath = openFileDialog.FileName;
                    string extractPath = "projects";
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    MessageBox.Show($"Imported '{openFileDialog.FileName}' successfully", "Import Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                updateProjectList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing project: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "https://lithicsoft.rf.gd/trainerstudio/",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening info: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
