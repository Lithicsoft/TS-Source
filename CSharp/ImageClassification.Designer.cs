namespace Lithicsoft_Trainer
{
    partial class ImageClassification
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
            button7 = new Button();
            button4 = new Button();
            button6 = new Button();
            label8 = new Label();
            label7 = new Label();
            pictureBox2 = new PictureBox();
            button5 = new Button();
            textBox2 = new TextBox();
            label6 = new Label();
            tabPage4 = new TabPage();
            label4 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
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
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage3.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Highlight;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(225, 32);
            label1.TabIndex = 0;
            label1.Text = "Image Classification";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(button7);
            tabPage5.Controls.Add(button4);
            tabPage5.Controls.Add(button6);
            tabPage5.Controls.Add(label8);
            tabPage5.Controls.Add(label7);
            tabPage5.Controls.Add(pictureBox2);
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
            // button7
            // 
            button7.Enabled = false;
            button7.Location = new Point(393, 64);
            button7.Name = "button7";
            button7.Size = new Size(105, 23);
            button7.TabIndex = 8;
            button7.Text = "Image file";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button4
            // 
            button4.Enabled = false;
            button4.Location = new Point(277, 64);
            button4.Name = "button4";
            button4.Size = new Size(105, 23);
            button4.TabIndex = 7;
            button4.Text = "Use camera";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button6
            // 
            button6.Enabled = false;
            button6.Location = new Point(277, 309);
            button6.Name = "button6";
            button6.Size = new Size(221, 23);
            button6.TabIndex = 6;
            button6.Text = "Predict";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(277, 291);
            label8.Name = "label8";
            label8.Size = new Size(79, 15);
            label8.TabIndex = 5;
            label8.Text = "Predict: None";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 46);
            label7.Name = "label7";
            label7.Size = new Size(64, 15);
            label7.TabIndex = 4;
            label7.Text = "Test model";
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Black;
            pictureBox2.Location = new Point(3, 64);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(268, 268);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
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
            label6.Size = new Size(60, 15);
            label6.TabIndex = 0;
            label6.Text = "Model file";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(label4);
            tabPage4.Controls.Add(label3);
            tabPage4.Controls.Add(pictureBox1);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(501, 335);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Report";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(323, 317);
            label4.Name = "label4";
            label4.Size = new Size(79, 15);
            label4.TabIndex = 2;
            label4.Text = "Predict: None";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(40, 15);
            label3.TabIndex = 1;
            label3.Text = "Image";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Location = new Point(3, 18);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(314, 314);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
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
            progressBar1.Location = new Point(6, 80);
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
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(3, 35);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(509, 363);
            tabControl1.TabIndex = 1;
            // 
            // ImageClassification
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ControlLightLight;
            Controls.Add(tabControl1);
            Controls.Add(label1);
            Name = "ImageClassification";
            Size = new Size(515, 401);
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
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
        private TabPage tabPage4;
        private TabPage tabPage3;
        private Label label3;
        private Label label6;
        private Button button5;
        private Button button6;
        private Label label7;
        private Label label8;
        private Button button7;
        private Button button4;
        private PictureBox pictureBox2;
        private static ProgressBar progressBar2;
        private static Label label4;
        private static TextBox textBox2;
        private static PictureBox pictureBox1;
        private static RichTextBox richTextBox2;
    }
}
