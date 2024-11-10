// License: Apache-2.0
/*
 * Utils/LoadingWindow.xaml: Waiting window
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

using System.Windows;

namespace Lithicsoft_Trainer_Studio.Utils
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow(string job)
        {
            InitializeComponent();
            this.JobName.Text = job;
        }
    }
}
