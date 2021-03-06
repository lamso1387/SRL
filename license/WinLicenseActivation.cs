﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Reflection;


namespace SRL
{
    public partial class WinLicenseActivation : Form
    {
        SRL.WinTools.UserControlValidation control_validation;
        SRL.LicenseClass lic_class;
        Assembly assembly = Assembly.GetExecutingAssembly();
        string license_file_name = "license.lic";
        Type license_obj_type;
        public bool is_activated = false;

        public WinLicenseActivation(SRL.LicenseClass lic, Assembly assembly_, Type license_obj_type_, string license_file_name_ = "license.lic")
        {
            InitializeComponent();
            lic_class = lic;
            assembly = assembly_;
            license_file_name = license_file_name_;
            license_obj_type = license_obj_type_;
        }

        private void frmActivation_Load(object sender, EventArgs e)
        {
            lblAppname.Text += assembly.GetName().Name;
            control_validation = new SRL.WinTools.UserControlValidation(this, errorProvider1, false);
            control_validation.ControlValidation(tbMobile, SRL.WinTools.UserControlValidation.ErrorTypes.NotNull_MobilePattern);
            control_validation.ControlValidation(tbEmail, WinTools.UserControlValidation.ErrorTypes.EmailPattern_NotNull);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            SRL.FileManagement.CopyToClipboard(tbUid.Text);
            tbUid.DeselectAll();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void tbMobile_TextChanged(object sender, EventArgs e)
        {
            tbUid.Text = lic_class.GetUID(assembly.GetName().Name, tbMobile.Text);
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            bool control_is_valid = false;
            control_is_valid = control_validation.CheckAllField(new List<Control> { tbMobile, tbEmail });
            if (control_is_valid)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("\n app_name: ");
                sb.Append(assembly.GetName().Name);
                sb.Append("\n uid: ");
                sb.Append(tbUid.Text);
                sb.Append("\n mobile: ");
                sb.Append(tbMobile.Text);
                sb.Append("\n email: ");
                sb.Append(tbEmail.Text);
                try
                {
                    SRL.Security.SendEmail("soheillamso@gmail.com", "فعالسازی نرم افزار", sb.ToString(), "KhaneBazaar@gmail.com", "2050130351");
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "خطا در ارسال:اتصال به اینترنت را بررسی کنید");
                }
                MessageBox.Show("ارسال با موفقیت انجام شد. بعد از تایید، کد به ایمیل شما ارسال میگردد");
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            bool control_is_valid = false;
            control_is_valid = control_validation.CheckAllField(new List<Control> { tbMobile, tbEmail });
            if (control_is_valid)
            {
                string message = "";
                //Call license control to validate the license string
                if (lic_class.ValidateLicense(tbActivationCode.Text, license_obj_type, out message))
                {
                    //If license if valid, save the license string into a local file
                    lic_class.SaveLicenseToFileInRoot(license_file_name, tbActivationCode.Text, out message);
                    is_activated = true;
                    this.Close();
                }
            }
        }
    }


}
