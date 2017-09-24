namespace SRL
{
    partial class WinLogin
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
            this.btnEnter = new System.Windows.Forms.Button();
            this.lblUsername = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblExit = new System.Windows.Forms.LinkLabel();
            this.pnlLoginForm = new System.Windows.Forms.Panel();
            this.pnlFoot = new System.Windows.Forms.Panel();
            this.lblFotNote = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlLoginForm.SuspendLayout();
            this.pnlFoot.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEnter
            // 
            this.btnEnter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEnter.Location = new System.Drawing.Point(5, 173);
            this.btnEnter.Margin = new System.Windows.Forms.Padding(5);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnEnter.Size = new System.Drawing.Size(164, 41);
            this.btnEnter.TabIndex = 2;
            this.btnEnter.Text = "ورود";
            this.btnEnter.UseVisualStyleBackColor = true;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblUsername.Location = new System.Drawing.Point(103, 11);
            this.lblUsername.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(66, 26);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "نام کاربری";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(5, 42);
            this.tbUsername.Margin = new System.Windows.Forms.Padding(5);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(164, 34);
            this.tbUsername.TabIndex = 0;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(5, 111);
            this.tbPassword.Margin = new System.Windows.Forms.Padding(5);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(164, 34);
            this.tbPassword.TabIndex = 1;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.BackColor = System.Drawing.Color.Transparent;
            this.lblPass.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPass.Location = new System.Drawing.Point(112, 80);
            this.lblPass.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(57, 26);
            this.lblPass.TabIndex = 5;
            this.lblPass.Text = "رمز عبور";
            // 
            // lblExit
            // 
            this.lblExit.AutoSize = true;
            this.lblExit.BackColor = System.Drawing.Color.Transparent;
            this.lblExit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblExit.LinkColor = System.Drawing.Color.White;
            this.lblExit.Location = new System.Drawing.Point(8, 9);
            this.lblExit.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblExit.Name = "lblExit";
            this.lblExit.Size = new System.Drawing.Size(43, 26);
            this.lblExit.TabIndex = 3;
            this.lblExit.TabStop = true;
            this.lblExit.Text = "خروج";
            this.lblExit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblExit_LinkClicked);
            // 
            // pnlLoginForm
            // 
            this.pnlLoginForm.BackColor = System.Drawing.Color.Transparent;
            this.pnlLoginForm.Controls.Add(this.btnEnter);
            this.pnlLoginForm.Controls.Add(this.lblUsername);
            this.pnlLoginForm.Controls.Add(this.lblPass);
            this.pnlLoginForm.Controls.Add(this.tbUsername);
            this.pnlLoginForm.Controls.Add(this.tbPassword);
            this.pnlLoginForm.Location = new System.Drawing.Point(104, 175);
            this.pnlLoginForm.Name = "pnlLoginForm";
            this.pnlLoginForm.Size = new System.Drawing.Size(178, 221);
            this.pnlLoginForm.TabIndex = 6;
            // 
            // pnlFoot
            // 
            this.pnlFoot.BackColor = System.Drawing.Color.Transparent;
            this.pnlFoot.Controls.Add(this.lblFotNote);
            this.pnlFoot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFoot.Location = new System.Drawing.Point(0, 419);
            this.pnlFoot.Name = "pnlFoot";
            this.pnlFoot.Size = new System.Drawing.Size(396, 43);
            this.pnlFoot.TabIndex = 7;
            // 
            // lblFotNote
            // 
            this.lblFotNote.AutoSize = true;
            this.lblFotNote.Font = new System.Drawing.Font("B Koodak", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblFotNote.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblFotNote.Location = new System.Drawing.Point(23, 8);
            this.lblFotNote.Name = "lblFotNote";
            this.lblFotNote.Size = new System.Drawing.Size(65, 21);
            this.lblFotNote.TabIndex = 0;
            this.lblFotNote.Text = "foot note";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.lblExit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(396, 43);
            this.panel2.TabIndex = 8;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("B Koodak", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTitle.Location = new System.Drawing.Point(251, 98);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(58, 36);
            this.lblTitle.TabIndex = 9;
            this.lblTitle.Text = "title";
            // 
            // WinLogin
            // 
            this.AcceptButton = this.btnEnter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 462);
            this.ControlBox = false;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlFoot);
            this.Controls.Add(this.pnlLoginForm);
            this.Font = new System.Drawing.Font("B Koodak", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WinLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ورود به نرم افزار";
            this.Load += new System.EventHandler(this.WinLogin_Load);
            this.pnlLoginForm.ResumeLayout(false);
            this.pnlLoginForm.PerformLayout();
            this.pnlFoot.ResumeLayout(false);
            this.pnlFoot.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.LinkLabel lblExit;
        private System.Windows.Forms.Panel pnlLoginForm;
        private System.Windows.Forms.Panel pnlFoot;
        private System.Windows.Forms.Label lblFotNote;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTitle;
    }
}