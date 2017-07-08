using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRL
{
    public partial class WinLicenseAdminActivator : Form
    {
        public  SRL.LicenseClass lic_class;
        Assembly assembly = Assembly.GetExecutingAssembly();
        string license_pfx_file_name = "LicenseSign.pfx";

        /// <summary>
        /// call this.SettingsControl-AppLicenseClass- after instanse created
        /// </summary>
        /// <param name="assembly_"></param>
        /// <param name="license_pfx_file_name_">LicenseSign.pfx must be Embedded Resource build action and do not copy to output</param>       
        public WinLicenseAdminActivator(bool is_mobile_uid_input, Assembly assembly_, string license_pfx_file_name_="LicenseSign.pfx")
        {
            InitializeComponent();
            assembly = assembly_;
            license_pfx_file_name=license_pfx_file_name_;

            lic_class = new SRL.LicenseClass(assembly, license_pfx_file_name, is_mobile_uid_input);
        }

        public void SettingControl<LicenseT>() where LicenseT: SRL.LicenseClass.LicenseEntity
        {
            lic_class.LicenseSettingsControl<LicenseT>(propertyGrid1);
        }

        private void activeLicense_Load(object sender, EventArgs e)
        {
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            lic_class.license_password = tbpass.Text;
            string lic = "";
            string message="";
            lic_class.GetLicense(SRL.LicenseClass.LicenseTypes.Single, textBox1.Text, out lic,out message);
            textBox2.Text = lic;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new SRL.FileManagement().CopyToClipboard(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new SRL.FileManagement().CopyToClipboard(textBox2.Text);
        }
    }
}
