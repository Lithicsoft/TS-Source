// License: Apache-2.0
/*
 * Form1.cs: Form for startup and project manager
 *
 * (C) Copyright 2024 Lithicsoft Organization
 * Author: Bui Nguyen Tan Sang <tansangbuinguyen52@gmail.com>
 */

namespace Lithicsoft_Trainer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.FormBorderStyle = FormBorderStyle.None;
                Startup startup = new Startup();
                startup.UserControlClosed += Startup_UserControlClosed;
                this.Controls.Add(startup);

                this.AutoSize = true;
                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                await WaitForUserControlToCloseAsync(startup);
                this.Controls.Clear();

                this.FormBorderStyle = FormBorderStyle.Sizable;
                Manager manager = new Manager();
                this.Controls.Add(manager);

                this.AutoSize = true;
                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting studio: {ex.Message}", "Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void Startup_UserControlClosed(object sender, EventArgs e)
        {
            if (sender is UserControl userControl)
            {
                this.Controls.Remove(userControl);
                userControl.Dispose();
            }
        }

        private Task WaitForUserControlToCloseAsync(Startup userControl)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = null;
            handler = (s, e) =>
            {
                userControl.UserControlClosed -= handler;
                tcs.SetResult(true);
            };
            userControl.UserControlClosed += handler;
            return tcs.Task;
        }
    }
}
