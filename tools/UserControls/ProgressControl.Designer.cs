namespace SRL
{
    partial class ProgressControl
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
            this.llCancelProgess = new System.Windows.Forms.LinkLabel();
            this.lblPer = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // llCancelProgess
            // 
            this.llCancelProgess.AutoSize = true;
            this.llCancelProgess.Location = new System.Drawing.Point(414, 3);
            this.llCancelProgess.Name = "llCancelProgess";
            this.llCancelProgess.Size = new System.Drawing.Size(23, 23);
            this.llCancelProgess.TabIndex = 11;
            this.llCancelProgess.TabStop = true;
            this.llCancelProgess.Text = "لغو";
            this.llCancelProgess.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCancelProgess_LinkClicked);
            // 
            // lblPer
            // 
            this.lblPer.AutoSize = true;
            this.lblPer.Location = new System.Drawing.Point(377, 3);
            this.lblPer.Name = "lblPer";
            this.lblPer.Size = new System.Drawing.Size(17, 23);
            this.lblPer.TabIndex = 10;
            this.lblPer.Text = "%";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(393, 3);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(12, 23);
            this.lblProgress.TabIndex = 9;
            this.lblProgress.Text = ".";
            this.lblProgress.TextChanged += new System.EventHandler(this.lblProgress_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(68, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(306, 23);
            this.progressBar1.TabIndex = 8;
            this.progressBar1.VisibleChanged += new System.EventHandler(this.progressBar1_VisibleChanged);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("B Nazanin", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblCount.Location = new System.Drawing.Point(3, 6);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(13, 17);
            this.lblCount.TabIndex = 12;
            this.lblCount.Text = "0";
            // 
            // ProgressControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.llCancelProgess);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblPer);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("B Nazanin", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ProgressControl";
            this.Size = new System.Drawing.Size(440, 30);
            this.Load += new System.EventHandler(this.ProgressControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel llCancelProgess;
        private System.Windows.Forms.Label lblPer;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblCount;
    }
}
