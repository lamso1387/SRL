using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;

namespace SRL
{
    public class AppLicenseClass : SRL.LicenseClass.LicenseEntity
    {
        [DisplayName("app_version")]
        [Category("License Options")]
        [XmlElement("app_version")]
        [SRL.LicenseClass.ShowInLicenseInfo(true, "app_version", SRL.LicenseClass.ShowInLicenseInfoAttribute.FormatType.String)]
        public string app_version { get; set; }


        public AppLicenseClass()
        {
            this.AppName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public override SRL.LicenseClass.LicenseStatus DoExtraValidation(bool is_mobile_uid_input, out string validationMsg)
        {
            SRL.LicenseClass.LicenseStatus _licStatus = SRL.LicenseClass.LicenseStatus.UNDEFINED;
            validationMsg = string.Empty;

            switch (this.Type)
            {
                case SRL.LicenseClass.LicenseTypes.Single:
                    //For Single License, check whether UID is matched
                    string genereted_UID = SRL.LicenseClass.GenerateUID(this.AppName, is_mobile_uid_input ? this.UserMobile : null);
                    if (this.UID == genereted_UID)
                    {
                        _licStatus = SRL.LicenseClass.LicenseStatus.VALID;
                    }
                    else
                    {
                        validationMsg = "کلید امنیتی برای این کپی غیر مجاز است";
                        _licStatus = SRL.LicenseClass.LicenseStatus.INVALID;
                    }
                    break;
                case SRL.LicenseClass.LicenseTypes.Volume:
                    //No UID checking for Volume License
                    _licStatus = SRL.LicenseClass.LicenseStatus.VALID;
                    break;
                default:
                    validationMsg = "کلید امنیتی اشتباه است";
                    _licStatus = SRL.LicenseClass.LicenseStatus.INVALID;
                    break;
            }

            return _licStatus;
        }
    }
}
