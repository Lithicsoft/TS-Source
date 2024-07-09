// License: Apache-2.0
/*
 * Form1.cs: Project launcher for Trainer Studio
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.ComponentModel;

namespace Lithicsoft_Trainer
{
    public partial class Main : Form
    {
        public Main(string projectName, string projectLanguage, string ProjectType)
        {
            try
            {
                InitializeComponent();

                this.Closing += OnClosing;

                try
                {
                    if (projectLanguage == "C#")
                    {
                        if (ProjectType == "Image classification")
                        {
                            ImageClassification imageClassification = new ImageClassification(projectName);
                            this.Controls.Add(imageClassification);

                            this.AutoSize = true;
                            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        }
                        else if (ProjectType == "Text classification")
                        {
                            TextClassification textClassification = new TextClassification(projectName);
                            this.Controls.Add(textClassification);

                            this.AutoSize = true;
                            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        }
                        else
                        {
                            MessageBox.Show($"Cannot open project {projectName} ({ProjectType})", "Project Open");
                        }
                    }
                    else
                    {
                        Python python = new Python(projectName, projectLanguage, ProjectType);
                        this.Controls.Add(python);

                        this.AutoSize = true;
                        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error dectecting project type: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening project: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            string msg = "Do you want to exit Trainer Studio?";
            DialogResult result = MessageBox.Show(msg, "Close Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
            else if (result == DialogResult.No)
                cancelEventArgs.Cancel = false;

        }
    }
}
