﻿namespace SRL
{
    partial class Setup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.tbDestination = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnBrowsSource = new System.Windows.Forms.Button();
            this.btnBrowsDes = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(709, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "پوشه فایل نصب";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(688, 107);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "محل نصب نرم افزار";
            // 
            // tbSource
            // 
            this.tbSource.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbSource.Location = new System.Drawing.Point(70, 48);
            this.tbSource.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(730, 26);
            this.tbSource.TabIndex = 2;
            // 
            // tbDestination
            // 
            this.tbDestination.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbDestination.Location = new System.Drawing.Point(70, 139);
            this.tbDestination.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbDestination.Name = "tbDestination";
            this.tbDestination.Size = new System.Drawing.Size(730, 26);
            this.tbDestination.TabIndex = 3;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(700, 271);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(100, 37);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "نصب";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnBrowsSource
            // 
            this.btnBrowsSource.Location = new System.Drawing.Point(25, 48);
            this.btnBrowsSource.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnBrowsSource.Name = "btnBrowsSource";
            this.btnBrowsSource.Size = new System.Drawing.Size(38, 26);
            this.btnBrowsSource.TabIndex = 5;
            this.btnBrowsSource.Text = "..";
            this.btnBrowsSource.UseVisualStyleBackColor = true;
            this.btnBrowsSource.Click += new System.EventHandler(this.btnBrowsSource_Click);
            // 
            // btnBrowsDes
            // 
            this.btnBrowsDes.Location = new System.Drawing.Point(25, 139);
            this.btnBrowsDes.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnBrowsDes.Name = "btnBrowsDes";
            this.btnBrowsDes.Size = new System.Drawing.Size(38, 26);
            this.btnBrowsDes.TabIndex = 6;
            this.btnBrowsDes.Text = "..";
            this.btnBrowsDes.UseVisualStyleBackColor = true;
            this.btnBrowsDes.Click += new System.EventHandler(this.btnBrowsDes_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(586, 271);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 37);
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "بروزرسانی";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(463, 271);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 37);
            this.button1.TabIndex = 8;
            this.button1.Text = "انصراف";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(70, 210);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(730, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 336);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnBrowsDes);
            this.Controls.Add(this.btnBrowsSource);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.tbDestination);
            this.Controls.Add(this.tbSource);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("B Koodak", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "Setup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "نصب کننده نرم افزار";
            this.Load += new System.EventHandler(this.Setup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.TextBox tbDestination;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnBrowsSource;
        private System.Windows.Forms.Button btnBrowsDes;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
    }
}
