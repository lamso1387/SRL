namespace SRL
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
            this.tbappname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDb = new System.Windows.Forms.TextBox();
            this.btnDb = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(734, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "فایلهای نصبی";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(648, 107);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "محل نصب ( محل پیشنهادی)";
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
            this.btnCopy.Location = new System.Drawing.Point(700, 411);
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
            this.btnUpdate.Location = new System.Drawing.Point(586, 411);
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
            this.button1.Location = new System.Drawing.Point(463, 411);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 37);
            this.button1.TabIndex = 8;
            this.button1.Text = "انصراف";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbappname
            // 
            this.tbappname.Location = new System.Drawing.Point(564, 320);
            this.tbappname.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbappname.Name = "tbappname";
            this.tbappname.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tbappname.Size = new System.Drawing.Size(236, 34);
            this.tbappname.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(660, 288);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "نام نرم افزار جهت نمایش";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(734, 193);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 26);
            this.label4.TabIndex = 9;
            this.label4.Text = "پایگاه داده";
            // 
            // tbDb
            // 
            this.tbDb.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbDb.Location = new System.Drawing.Point(564, 225);
            this.tbDb.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbDb.Name = "tbDb";
            this.tbDb.Size = new System.Drawing.Size(236, 29);
            this.tbDb.TabIndex = 10;
            // 
            // btnDb
            // 
            this.btnDb.Location = new System.Drawing.Point(516, 225);
            this.btnDb.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDb.Name = "btnDb";
            this.btnDb.Size = new System.Drawing.Size(38, 29);
            this.btnDb.TabIndex = 11;
            this.btnDb.Text = "..";
            this.btnDb.UseVisualStyleBackColor = true;
            this.btnDb.Click += new System.EventHandler(this.btnDb_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 472);
            this.Controls.Add(this.btnDb);
            this.Controls.Add(this.tbDb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbappname);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.TextBox tbappname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbDb;
        private System.Windows.Forms.Button btnDb;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

