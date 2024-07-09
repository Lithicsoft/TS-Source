namespace Lithicsoft_Trainer
{
    partial class Python
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            tabPage5 = new TabPage();
            button6 = new Button();
            textBox4 = new TextBox();
            label7 = new Label();
            button5 = new Button();
            textBox2 = new TextBox();
            label6 = new Label();
            tabPage3 = new TabPage();
            richTextBox2 = new RichTextBox();
            button3 = new Button();
            progressBar2 = new ProgressBar();
            tabPage1 = new TabPage();
            richTextBox1 = new RichTextBox();
            textBox1 = new TextBox();
            progressBar1 = new ProgressBar();
            button2 = new Button();
            button1 = new Button();
            label2 = new Label();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            button4 = new Button();
            textBox3 = new TextBox();
            listView1 = new ListView();
            tabPage5.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Highlight;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(170, 32);
            label1.TabIndex = 0;
            label1.Text = "Python project";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(button6);
            tabPage5.Controls.Add(textBox4);
            tabPage5.Controls.Add(label7);
            tabPage5.Controls.Add(button5);
            tabPage5.Controls.Add(textBox2);
            tabPage5.Controls.Add(label6);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(501, 335);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Result";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Enabled = false;
            button6.Location = new Point(423, 62);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 6;
            button6.Text = "Predict";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // textBox4
            // 
            textBox4.Enabled = false;
            textBox4.Location = new Point(3, 62);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(414, 23);
            textBox4.TabIndex = 5;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 44);
            label7.Name = "label7";
            label7.Size = new Size(249, 15);
            label7.TabIndex = 4;
            label7.Text = "Pipeline is not available for this model/project\r\n";
            // 
            // button5
            // 
            button5.Enabled = false;
            button5.Location = new Point(423, 18);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 2;
            button5.Text = "Show";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(3, 18);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(414, 23);
            textBox2.TabIndex = 1;
            textBox2.TextChanged += textBox3_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 0);
            label6.Name = "label6";
            label6.Size = new Size(73, 15);
            label6.TabIndex = 0;
            label6.Text = "Result folder";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(richTextBox2);
            tabPage3.Controls.Add(button3);
            tabPage3.Controls.Add(progressBar2);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(501, 335);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Train";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(3, 3);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.ReadOnly = true;
            richTextBox2.Size = new Size(495, 300);
            richTextBox2.TabIndex = 2;
            richTextBox2.Text = "Waiting...\n";
            // 
            // button3
            // 
            button3.Location = new Point(423, 309);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 1;
            button3.Text = "Train";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // progressBar2
            // 
            progressBar2.Location = new Point(3, 309);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(414, 23);
            progressBar2.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(richTextBox1);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(progressBar1);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(label2);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(501, 335);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Prepare Data";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(6, 109);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(489, 220);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "Waiting...";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(408, 23);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(9, 80);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(489, 23);
            progressBar1.TabIndex = 4;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(6, 51);
            button2.Name = "button2";
            button2.Size = new Size(489, 23);
            button2.TabIndex = 3;
            button2.Text = "Load Dataset";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(420, 22);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "Open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 4);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 1;
            label2.Text = "Zip file";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(3, 35);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(509, 363);
            tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(textBox3);
            tabPage2.Controls.Add(listView1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(501, 335);
            tabPage2.TabIndex = 5;
            tabPage2.Text = "Parameters";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(420, 306);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 2;
            button4.Text = "Apply";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(6, 306);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(408, 23);
            textBox3.TabIndex = 1;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // listView1
            // 
            listView1.Location = new Point(6, 6);
            listView1.Name = "listView1";
            listView1.Size = new Size(489, 293);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // Python
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ControlLightLight;
            Controls.Add(tabControl1);
            Controls.Add(label1);
            Name = "Python";
            Size = new Size(515, 401);
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TabPage tabPage5;
        private Button button3;
        private TabPage tabPage1;
        private RichTextBox richTextBox1;
        private TextBox textBox1;
        private ProgressBar progressBar1;
        private Button button2;
        private Button button1;
        private Label label2;
        private TabControl tabControl1;
        private TabPage tabPage3;
        private Label label6;
        private Button button5;
        private Label label7;
        private ProgressBar progressBar2;
        private TextBox textBox2;
        private RichTextBox richTextBox2;
        private TabPage tabPage2;
        private Button button4;
        private TextBox textBox3;
        private ListView listView1;
        private Button button6;
        private TextBox textBox4;
    }
}
