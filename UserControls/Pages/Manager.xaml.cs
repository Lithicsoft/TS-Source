// License: Apache-2.0
/*
 * UserControls/Pages/Manager.xaml.cs: Back-end source code for projects manager
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lithicsoft_Trainer_Studio.UserControls.Pages
{
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class Manager : Page
    {
        public Manager()
        {
            InitializeComponent();
        }

        public event EventHandler PageClosed;

        private void Manager_Loaded(object sender, RoutedEventArgs e)
        {
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

                    listView1.Items.Add(new ProjectItems { ProjectName = projectName, ProjectLanguage = projectConfig[0], ProjectType = projectConfig[1] });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating project list: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    ProjectItems selectedProject = (ProjectItems)listView1.SelectedItems[0];

                    string projectName = selectedProject.ProjectName;
                    string projectLanguage = selectedProject.ProjectLanguage;
                    string projectType = selectedProject.ProjectType;

                    Trainer trainer = new Trainer(projectName, projectLanguage, projectType);
                    trainer.Show();

                    Window parentWindow = Window.GetWindow(this);
                    if (parentWindow != null)
                    {
                        parentWindow.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening project: {ex.Message}", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ProjectItems
    {
        public string ProjectName { get; set; }

        public string ProjectLanguage { get; set; }

        public string ProjectType { get; set; }
    }
}
