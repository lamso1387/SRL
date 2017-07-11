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
            this.lblAppname = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMobile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnActivate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.tbUid.Location = new System.Drawing.Point(111, 193);
            this.tbUid.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbUid.Multiline = true;
            this.tbUid.Name = "tbUid";
            this.tbUid.ReadOnly = true;
            this.tbUid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbUid.Size = new System.Drawing.Size(351, 34);
            this.tbUid.TabIndex = 4;
            this.tbUid.Tag = "موبایل خود را بصورت صحیح وارد کنید";
            this.tbUid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbActivationCode
            // 
            this.tbActivationCode.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbActivationCode.Location = new System.Drawing.Point(109, 60);
            this.tbActivationCode.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbActivationCode.Multiline = true;
            this.tbActivationCode.Name = "tbActivationCode";
            this.tbActivationCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbActivationCode.Size = new System.Drawing.Size(351, 95);
            this.tbActivationCode.TabIndex = 6;
            // 
            // lblAppname
            // 
            this.lblAppname.AutoSize = true;
            this.lblAppname.Font = new System.Drawing.Font("B Koodak", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblAppname.Location = new System.Drawing.Point(1, 592);
            this.lblAppname.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAppname.Name = "lblAppname";
            this.lblAppname.Size = new System.Drawing.Size(32, 21);
            this.lblAppname.TabIndex = 8;
            this.lblAppname.Text = "SN:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(472, 197);
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
            this.label1.Location = new System.Drawing.Point(58, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(499, 26);
            this.label1.TabIndex = 10;
            this.label1.Text = "اگر کدفعالسازی را در اختیار دارید در قسمت زیر وارد کنید و سپس دکمه فعالسازی را ان" +
    "تخاب کنید";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(472, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 26);
            this.label3.TabIndex = 12;
            this.label3.Text = "موبایل";
            // 
            // tbMobile
            // 
            this.tbMobile.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbMobile.Location = new System.Drawing.Point(111, 110);
            this.tbMobile.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbMobile.MaxLength = 11;
            this.tbMobile.Multiline = true;
            this.tbMobile.Name = "tbMobile";
            this.tbMobile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbMobile.Size = new System.Drawing.Size(351, 34);
            this.tbMobile.TabIndex = 11;
            this.tbMobile.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbMobile.TextChanged += new System.EventHandler(this.tbMobile_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(472, 156);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 26);
            this.label4.TabIndex = 14;
            this.label4.Text = "ایمیل";
            // 
            // tbEmail
            // 
            this.tbEmail.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.tbEmail.Location = new System.Drawing.Point(111, 152);
            this.tbEmail.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbEmail.Multiline = true;
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbEmail.Size = new System.Drawing.Size(351, 34);
            this.tbEmail.TabIndex = 13;
            this.tbEmail.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSend);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbUid);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbEmail);
            this.groupBox1.Controls.Add(this.tbMobile);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(21, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(565, 282);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "روش اول: با استفاده از فایل فعالساز";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(65, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(494, 52);
            this.label5.TabIndex = 16;
            this.label5.Text = "ابتدا فیلدهای زیر را تکمیل و سپس دکمه ارسال را انتخاب کنید.\r\nبعد از تایید، یک فای" +
    "ل به ایمیل شما ارسال میگردد که باید ان را در پوشه نصب نرم افزار قرار دهید.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnActivate);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbActivationCode);
            this.groupBox2.Location = new System.Drawing.Point(21, 352);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox2.Size = new System.Drawing.Size(565, 214);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "روش دوم: با استفاده از کد فعالساز";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(228, 236);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(111, 35);
            this.btnSend.TabIndex = 17;
            this.btnSend.Text = "ارسال";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnActivate
            // 
            this.btnActivate.Location = new System.Drawing.Point(228, 164);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(111, 35);
            this.btnActivate.TabIndex = 11;
            this.btnActivate.Text = "فعالسازی";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // WinLicenseActivation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 607);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblAppname);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("B Koodak", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "WinLicenseActivation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "فعالسازی";
            this.Load += new System.EventHandler(this.frmActivation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbUid;
        private System.Windows.Forms.TextBox tbActivationCode;
        private System.Windows.Forms.Label lblAppname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMobile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnActivate;
    }
}