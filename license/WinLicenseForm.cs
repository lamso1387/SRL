using System;
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
    /// <summary>
    /// call CheckLicenseKey-AppLicenseClass- after instanse created then ShowDialog 
    /// example shows how to use in load form event:
    /// SRL.WinLicenseForm li_form=new SRL.WinLicenseForm(Assembly.GetExecutingAssembly(),true);
    /// li_form.CheckLicenseKey-AppLicenseClass-();
    /// if(!li_form.IsDisposed && !li_form.is_activated_before) li_form.ShowDialog();
    /// </summary>
    public partial class WinLicenseForm : Form
    {
        /* before  InitializeComponent(); use:
           SRL.WinLicenseForm li_form = new SRL.WinLicenseForm(Assembly.GetExecutingAssembly(), true);
            li_form.CheckLicenseKey<AppLicenseClass>();
           if (!li_form.IsDisposed && !li_form.is_activated_before) li_form.ShowDialog();*/



        string license_file_name = "license.lic";
        string license_cer_file_name = "LicenseVerify.cer";
        Assembly assembly = Assembly.GetExecutingAssembly();
        public bool is_activated = false;
        public bool is_activated_before = false;
        SRL.LicenseClass lic_class = new SRL.LicenseClass(true, true);

        /// <summary>
        /// license default file is "license.lic" and should be in app and executive root only. call this.CheckLicenseKey-AppLicenseClass- after instanse created then ShowDialog
        /// </summary>
        /// <param name="assembly_"></param>
        /// <param name="license_file_name_"></param>
        /// <param name="license_cer_file_name_">LicenseVerify.cer must be Embedded Resource build action and do not copy to output</param>
        public WinLicenseForm(Assembly assembly_, bool is_mobile_input, string license_file_name_ = "license.lic", string license_cer_file_name_ = "LicenseVerify.cer")
        {
            InitializeComponent();
            license_file_name = license_file_name_;
            license_cer_file_name = license_cer_file_name_;
            assembly = assembly_;
            lic_class.IsMobileUidInput = is_mobile_input;

        }

        /// <summary>
        ///call CheckLicenseKey after instanse created<LicenseT>
        /// </summary>
        public WinLicenseForm()
        {
            InitializeComponent();
        }

        public void CheckLicenseKey<LicenseT>() where LicenseT : SRL.LicenseClass.LicenseEntity
        {
            lblAppName.Text += assembly.GetName().Name;
            string license_key = "";
            if (!System.IO.File.Exists(license_file_name)) license_key = null;
            else license_key = System.IO.File.ReadAllText(license_file_name);
            if (lic_class.CheckLicense<LicenseT>(assembly, license_cer_file_name, license_key) == false)
            {

                var activator = new WinLicenseActivation(lic_class, assembly, typeof(LicenseT));
                activator.ShowDialog();
                if (is_activated = activator.is_activated) textBox1.Text = lic_class.LicenseInfo;
                else
                {
                    this.Close();
                    Environment.Exit(0);
                }

            }
            else is_activated = is_activated_before = true;  //textBox1.Text = lic_class.LicenseInfo;
        }



        private void WinLicenseForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }





}
