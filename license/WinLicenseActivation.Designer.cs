namespace SRL
{
    partial class WinLicenseActivation
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
            this.btnOK = new System.Windows.Forms.Button();
            this.tbUid = new System.Windows.Forms.TextBox();
            this.tbActivationCode = new System.Windows.Forms.TextBox();
            this.btnActivateApp = new System.Windows.Forms.Button();
            this.lblAppname = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMobile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.btnSenddata = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(32, 737);
            this.btnOK.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(125, 46);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // tbUid
            // 
            this.tbUid.BackColor = System.Drawing.SystemColors.Control;
            this.tbUid.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbUid.Location = new System.Drawing.Point(106, 93);
            this.tbUid.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbUid.Multiline = true;
            this.tbUid.Name = "tbUid";
            this.tbUid.ReadOnly = true;
            this.tbUid.Size = new System.Drawing.Size(351, 34);
            this.tbUid.TabIndex = 4;
            this.tbUid.Tag = "موبایل خود را بصورت صحیح وارد کنید";
            this.tbUid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbActivationCode
            // 
            this.tbActivationCode.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbActivationCode.Location = new System.Drawing.Point(14, 300);
            this.tbActivationCode.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbActivationCode.Multiline = true;
            this.tbActivationCode.Name = "tbActivationCode";
            this.tbActivationCode.Size = new System.Drawing.Size(570, 217);
            this.tbActivationCode.TabIndex = 6;
            // 
            // btnActivateApp
            // 
            this.btnActivateApp.Location = new System.Drawing.Point(238, 518);
            this.btnActivateApp.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnActivateApp.Name = "btnActivateApp";
            this.btnActivateApp.Size = new System.Drawing.Size(125, 46);
            this.btnActivateApp.TabIndex = 7;
            this.btnActivateApp.Text = "فعالسازی";
            this.btnActivateApp.UseVisualStyleBackColor = true;
            this.btnActivateApp.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblAppname
            // 
            this.lblAppname.AutoSize = true;
            this.lblAppname.Font = new System.Drawing.Font("B Koodak", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblAppname.Location = new System.Drawing.Point(-3, 575);
            this.lblAppname.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAppname.Name = "lblAppname";
            this.lblAppname.Size = new System.Drawing.Size(32, 21);
            this.lblAppname.TabIndex = 8;
            this.lblAppname.Text = "SN:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(467, 97);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 26);
            this.label2.TabIndex = 9;
            this.label2.Text = "شناسه محصول";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 268);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(572, 26);
            this.label1.TabIndex = 10;
            this.label1.Text = "اگر کدفعالسازی را در اختیار دارید در این قسمت وارد کنید در غیر اینصورت از روش بال" +
    "ا اقدام به فعالسازی نمایید\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(467, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 26);
            this.label3.TabIndex = 12;
            this.label3.Text = "موبایل";
            // 
            // tbMobile
            // 
            this.tbMobile.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbMobile.Location = new System.Drawing.Point(106, 10);
            this.tbMobile.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbMobile.Multiline = true;
            this.tbMobile.Name = "tbMobile";
            this.tbMobile.Size = new System.Drawing.Size(351, 34);
            this.tbMobile.TabIndex = 11;
            this.tbMobile.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMobile.TextChanged += new System.EventHandler(this.tbMobile_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(467, 56);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 26);
            this.label4.TabIndex = 14;
            this.label4.Text = "ایمیل";
            // 
            // tbEmail
            // 
            this.tbEmail.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbEmail.Location = new System.Drawing.Point(106, 52);
            this.tbEmail.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbEmail.Multiline = true;
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(351, 34);
            this.tbEmail.TabIndex = 13;
            this.tbEmail.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSenddata
            // 
            this.btnSenddata.Location = new System.Drawing.Point(126, 136);
            this.btnSenddata.Name = "btnSenddata";
            this.btnSenddata.Size = new System.Drawing.Size(316, 35);
            this.btnSenddata.TabIndex = 15;
            this.btnSenddata.Text = "ارسال مشخصات جهت دریافت کدفعالسازی";
            this.btnSenddata.UseVisualStyleBackColor = true;
            this.btnSenddata.Click += new System.EventHandler(this.btnSenddata_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // WinLicenseActivation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 592);
            this.Controls.Add(this.btnSenddata);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbMobile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblAppname);
            this.Controls.Add(this.btnActivateApp);
            this.Controls.Add(this.tbActivationCode);
            this.Controls.Add(this.tbUid);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("B Koodak", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "WinLicenseActivation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "فعالسازی";
            this.Load += new System.EventHandler(this.frmActivation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbUid;
        private System.Windows.Forms.TextBox tbActivationCode;
        private System.Windows.Forms.Button btnActivateApp;
        private System.Windows.Forms.Label lblAppname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMobile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Button btnSenddata;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}