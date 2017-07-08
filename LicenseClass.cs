using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Data.Entity;
using Microsoft.Reporting.WinForms;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using IWshRuntimeLibrary;

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml.Serialization;
using System.Security;
using System.Management;
using System.Security.Cryptography.X509Certificates;

namespace SRL
{
    public class LicenseClass
    {
        public byte[] CertificatePublicKeyData { get; set; }
        public byte[] CertificatePrivateKeyData { set; get; }
        public SecureString CertificatePassword = new SecureString();
        public string license_password
        {
            set
            {
                SetCertificatePassword(value);
            }
        }
        public bool ShowMessageAfterValidation { get; set; }
        public bool IsMobileUidInput { get; set; }

        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }



        PropertyGrid pgLicenseSettings;

        public LicenseEntity License { get; set; }

        public string LicenseInfo { get; set; }

        public delegate void LicenseSettingsValidatingHandler(object sender, LicenseSettingsValidatingEventArgs e);

        public event LicenseSettingsValidatingHandler OnLicenseSettingsValidating;
        public event LicenseGeneratedHandler OnLicenseGenerated;

        /// <summary>
        /// use it for checking license
        /// </summary>
        /// <param name="ShowMessageAfterValidation_"></param>
        public LicenseClass(bool ShowMessageAfterValidation_, bool is_mobile_uid_input_)
        {
            ShowMessageAfterValidation = ShowMessageAfterValidation_;
            IsMobileUidInput = is_mobile_uid_input_;
        }


        /// <summary>
        /// use it for activating license
        /// </summary>
        /// <param name="_assembly"></param>
        /// <param name="license_full_file_name">e.g. "LicenseSign.pfx"</param>
        /// <param name="license_password"></param>
        public LicenseClass(Assembly _assembly, string license_pfx_full_file_name, bool is_mobile_uid_input_, string license_password = null)
        {
            IsMobileUidInput = is_mobile_uid_input_;
            SetCertificatePassword(license_password);
            CertificatePrivateKeyData = GetPubicKeyData(_assembly, license_pfx_full_file_name);

        }

        private void SetCertificatePassword(string license_password)
        {
            if (license_password == null) return;
            SecureString secure_pass = new SecureString();
            foreach (var item in license_password.ToCharArray())
            {
                secure_pass.AppendChar(item);
            }

            CertificatePassword = secure_pass;
        }
        #region check license is valid

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="LicenseT">LicenseT must inherite : LicenseEntity . see examples</typeparam>
        /// <param name="_assembly">Assembly.GetExecutingAssembly()</param>
        /// <param name="app_name">e.g. "hesabdari"</param>
        /// <param name="license_cer_full_file_name">e.g "LicenseVerify.cer" file must be Embedded Resource Build Action and Do not copy to output directory</param>
        /// <param name="license_key"> get it from a file or db. send null if license file or license column not exist </param>
        /// <returns></returns>
        public bool CheckLicense<LicenseT>(Assembly _assembly, string license_cer_full_file_name, string license_key) where LicenseT : LicenseEntity
        {
            LicenseInfo = "";
            //Initialize variables with default values
            LicenseT _lic = null;
            string _msg = string.Empty;
            LicenseStatus _status = LicenseStatus.UNDEFINED;

            //Read public key from assembly
            CertificatePublicKeyData = GetPubicKeyData(_assembly, license_cer_full_file_name);


            //Check if the XML license file exists
            if (!string.IsNullOrWhiteSpace(license_key))
            {
                _lic = (LicenseT)ParseLicenseFromBASE64String(
                    typeof(LicenseT),
                    license_key,
                    out _status,
                    out _msg);
            }
            else
            {
                _status = LicenseStatus.INVALID;
                _msg = "Your copy of this application is not activated";
            }

            switch (_status)
            {
                case LicenseStatus.VALID:

                    //TODO: If license is valid, you can do extra checking here
                    //TODO: E.g., check license expiry date if you have added expiry date property to your license entity
                    //TODO: Also, you can set feature switch here based on the different properties you added to your license entity 

                    //Here for demo, just show the license information and RETURN without additional checking       
                    LicenseInfo = ShowLicenseInfo(_lic, "");

                    return true;

                default:
                    //for the other status of license file, show the warning message
                    //and also popup the activation form for user to activate your application
                    MessageBox.Show(_msg, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
        }


        /// <summary>
        /// app should has a class for example AppLicenseClass that inherites from LicenseEntity. see AppLicenseExample1 and AppLicenseExample2
        /// </summary>
        public abstract class LicenseEntity
        {
            [DisplayName("UserMobile")]
            [Category("License Options")]
            [XmlElement("UserMobile")]
            [ShowInLicenseInfo(true, "UserMobile", ShowInLicenseInfoAttribute.FormatType.String)]
            public string UserMobile { get; set; }

            [Browsable(false)]
            [XmlIgnore]
            [ShowInLicenseInfo(false)]
            public string AppName { get; set; }

            [Browsable(false)]
            [XmlIgnore]
            [ShowInLicenseInfo(false)]
            public string UserEmail { get; set; }

            [Browsable(false)]
            [XmlElement("UID")]
            [ShowInLicenseInfo(false)]
            public string UID { get; set; }

            [Browsable(false)]
            [XmlElement("Type")]
            [ShowInLicenseInfo(true, "Type", ShowInLicenseInfoAttribute.FormatType.EnumDescription)]
            public LicenseTypes Type { get; set; }

            [Browsable(false)]
            [XmlElement("CreateDateTime")]
            [ShowInLicenseInfo(true, "Creation Time", ShowInLicenseInfoAttribute.FormatType.DateTime)]
            public DateTime CreateDateTime { get; set; }

            /// <summary>
            /// For child class to do extra validation for those extended properties
            /// </summary>
            /// <param name="validationMsg"></param>
            /// <returns></returns>
            public abstract LicenseStatus DoExtraValidation(bool is_mobile_uid_input, out string validationMsg);

        }


        /// <summary>
        /// returns public key if .cer or private key if .pfx is given
        /// </summary>
        /// <param name="_assembly_"></param>
        /// <param name="app_name_"></param>
        /// <param name="license_full_file_name_">e.g. "LicenseSign.pfx" or "LicenseVerify.cer"</param>
        /// <returns></returns>    
        public byte[] GetPubicKeyData(Assembly _assembly_, string license_full_file_name_)
        {
            using (MemoryStream _mem_ = new MemoryStream())
            {
                _assembly_.GetManifestResourceStream(_assembly_.GetName().Name + "." + license_full_file_name_).CopyTo(_mem_);

                return _mem_.ToArray();
            }

        }

        public LicenseEntity ParseLicenseFromBASE64String(Type licenseObjType, string licenseString, out LicenseStatus licStatus, out string validationMsg)
        {
            validationMsg = string.Empty;
            licStatus = LicenseStatus.UNDEFINED;

            if (string.IsNullOrWhiteSpace(licenseString))
            {
                licStatus = LicenseStatus.CRACKED;
                return null;
            }

            string _licXML = string.Empty;
            LicenseEntity _lic = null;

            try
            {
                //Get RSA key from certificate
                X509Certificate2 cert = new X509Certificate2(CertificatePublicKeyData);
                RSACryptoServiceProvider rsaKey = (RSACryptoServiceProvider)cert.PublicKey.Key;

                XmlDocument xmlDoc = new XmlDocument();

                // Load an XML file into the XmlDocument object.
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(licenseString)));

                // Verify the signature of the signed XML.            
                if (VerifyXml(xmlDoc, rsaKey))
                {
                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
                    xmlDoc.DocumentElement.RemoveChild(nodeList[0]);

                    _licXML = xmlDoc.OuterXml;

                    //Deserialize license
                    XmlSerializer _serializer = new XmlSerializer(typeof(LicenseEntity), new Type[] { licenseObjType });
                    using (StringReader _reader = new StringReader(_licXML))
                    {
                        _lic = (LicenseEntity)_serializer.Deserialize(_reader);
                    }

                    licStatus = _lic.DoExtraValidation(IsMobileUidInput, out validationMsg);
                }
                else
                {
                    licStatus = LicenseStatus.INVALID;
                }
            }
            catch (Exception exc)
            {
                validationMsg = exc.Message;
                licStatus = LicenseStatus.CRACKED;
            }

            return _lic;
        }

        /// <summary>
        /// Verify the signature of an XML file against an asymmetric algorithm and return the result.
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static Boolean VerifyXml(XmlDocument Doc, RSA Key)
        {
            // Check arguments.
            if (Doc == null)
                throw new ArgumentException("Doc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(Doc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            return signedXml.CheckSignature(Key);
        }

        public string ShowLicenseInfo(LicenseEntity license, string additionalInfo)
        {
            try
            {
                StringBuilder _sb = new StringBuilder(512);

                Type _typeLic = license.GetType();
                PropertyInfo[] _props = _typeLic.GetProperties();

                object _value = null;
                string _formatedValue = string.Empty;
                foreach (PropertyInfo _p in _props)
                {
                    try
                    {
                        ShowInLicenseInfoAttribute _showAttr = (ShowInLicenseInfoAttribute)Attribute.GetCustomAttribute(_p, typeof(ShowInLicenseInfoAttribute));
                        if (_showAttr != null && _showAttr.ShowInLicenseInfo)
                        {
                            _value = _p.GetValue(license, null);
                            _sb.Append(_showAttr.DisplayAs);
                            _sb.Append(": ");

                            //Append value and apply the format   
                            if (_value != null)
                            {
                                switch (_showAttr.DataFormatType)
                                {
                                    case ShowInLicenseInfoAttribute.FormatType.String:
                                        _formatedValue = _value.ToString();
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.Date:
                                        if (_p.PropertyType == typeof(DateTime) && !string.IsNullOrWhiteSpace(DateFormat))
                                        {
                                            _formatedValue = ((DateTime)_value).ToString(DateFormat);
                                        }
                                        else
                                        {
                                            _formatedValue = _value.ToString();
                                        }
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.DateTime:
                                        if (_p.PropertyType == typeof(DateTime) && !string.IsNullOrWhiteSpace(DateTimeFormat))
                                        {
                                            _formatedValue = ((DateTime)_value).ToString(DateTimeFormat);
                                        }
                                        else
                                        {
                                            _formatedValue = _value.ToString();
                                        }
                                        break;
                                    case ShowInLicenseInfoAttribute.FormatType.EnumDescription:
                                        string _name = Enum.GetName(_p.PropertyType, _value);
                                        if (_name != null)
                                        {
                                            FieldInfo _fi = _p.PropertyType.GetField(_name);
                                            DescriptionAttribute _dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_fi, typeof(DescriptionAttribute));
                                            if (_dna != null)
                                                _formatedValue = _dna.Description;
                                            else
                                                _formatedValue = _value.ToString();
                                        }
                                        else
                                        {
                                            _formatedValue = _value.ToString();
                                        }
                                        break;
                                }

                                _sb.Append(_formatedValue);
                            }

                            _sb.Append("\r\n");
                        }
                    }
                    catch
                    {
                        //Ignore exeption
                    }
                }


                if (string.IsNullOrWhiteSpace(additionalInfo))
                {
                    _sb.Append(additionalInfo.Trim());
                }

                return _sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool ValidateLicense(string txtLicense, Type LicenseObjectType, out string message)
        {
            message = "";
            if (string.IsNullOrWhiteSpace(txtLicense))
            {
                message = "Please input license";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Check the activation string
            LicenseStatus _licStatus = LicenseStatus.UNDEFINED;
            string _msg = string.Empty;

            LicenseEntity _lic = ParseLicenseFromBASE64String(LicenseObjectType, txtLicense.Trim(), out _licStatus, out _msg);
            switch (_licStatus)
            {
                case LicenseStatus.VALID:
                    if (ShowMessageAfterValidation)
                    {
                        message = "License is valid .";
                        MessageBox.Show(_msg, message, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        message += _msg;
                    }
                    LicenseInfo = ShowLicenseInfo(_lic, "");
                    return true;

                case LicenseStatus.CRACKED:
                case LicenseStatus.INVALID:
                case LicenseStatus.UNDEFINED:
                    if (ShowMessageAfterValidation)
                    {
                        message = "License is INVALID .";
                        MessageBox.Show(_msg, message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        message += _msg;
                    }

                    return false;

                default:
                    return false;
            }
        }

        public void SaveLicenseToFileInRoot(string file_name, string license_key, out string message)
        {
            message = "License accepted, the application will be close. Please restart it later";
            new FileManagement().SaveToFile(Path.Combine(Application.StartupPath, file_name), license_key.Trim());
            MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// This attribute defines whether the property of LicenseEntity object will be shown in LicenseInfoControl
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class ShowInLicenseInfoAttribute : Attribute
        {
            public enum FormatType
            {
                String,
                Date,
                DateTime,
                EnumDescription,
            }

            protected bool _showInLicenseInfo = true;
            protected string _displayAs = string.Empty;
            protected FormatType _formatType = FormatType.String;

            public ShowInLicenseInfoAttribute()
            {
            }

            public ShowInLicenseInfoAttribute(bool showInLicenseInfo)
            {
                if (showInLicenseInfo)
                {
                    throw new Exception("When ShowInLicenseInfo is True, DisplayAs MUST have a value");
                }
                _showInLicenseInfo = showInLicenseInfo;
            }

            public ShowInLicenseInfoAttribute(bool showInLicenseInfo, string displayAs)
            {
                _showInLicenseInfo = showInLicenseInfo;
                _displayAs = displayAs;
            }
            public ShowInLicenseInfoAttribute(bool showInLicenseInfo, string displayAs, FormatType dataFormatType)
            {
                _showInLicenseInfo = showInLicenseInfo;
                _displayAs = displayAs;
                _formatType = dataFormatType;
            }

            public bool ShowInLicenseInfo
            {
                get
                {
                    return _showInLicenseInfo;
                }
            }

            public string DisplayAs
            {
                get
                {
                    return _displayAs;
                }
            }

            public FormatType DataFormatType
            {
                get
                {
                    return _formatType;
                }
            }
        }


        /// <summary>
        /// Usage Guide:
        /// Command for creating the certificate
        /// >> makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=<YourCertName>" -sky Signature
        /// Then export the cert with private key from key store with a password
        /// Also export another cert with only public key
        /// </summary>
        public enum LicenseStatus
        {
            UNDEFINED = 0,
            VALID = 1,
            INVALID = 2,
            CRACKED = 4
        }

        public enum LicenseTypes
        {
            [Description("Unknown")]
            Unknown = 0,
            [Description("Single")]
            Single = 1,
            [Description("Volume")]
            Volume = 2
        }

        public class LicenseGeneratedEventArgs
        {
            public string LicenseBASE64String { get; set; }
        }
        public delegate void LicenseGeneratedHandler(object sender, LicenseGeneratedEventArgs e);


        #endregion

        #region get uid


        public string GetUID(string app_name, string mobile)
        {
            if (mobile.Length == 11)
            {
                return GenerateUID(app_name, IsMobileUidInput ? mobile : null);
            }
            else return "";
        }


        /// <summary>
        /// Combine appName, CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
        /// if user_mobile is set, it is first input for device Id
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="user_mobile">if set, it is first input for device Id</param>
        /// <returns></returns>
        public static string GenerateUID(string appName, string user_mobile = null)
        {
            //Combine the IDs and get bytes
            string _id = user_mobile == null ?
                string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber()) :
            string.Concat(user_mobile, appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());

            byte[] _byteIds = Encoding.UTF8.GetBytes(_id);

            //Use MD5 to get the fixed length checksum of the ID string
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            byte[] _checksum = _md5.ComputeHash(_byteIds);

            //Convert checksum into 4 ulong parts and use BASE36 to encode both
            string _part1Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 0));
            string _part2Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 4));
            string _part3Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 8));
            string _part4Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 12));

            //Concat these 4 part into one string
            string uid = string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
            return uid;
        }


        /// <summary>
        /// Get volume serial number of drive C
        /// </summary>
        /// <returns></returns>
        private static string GetDiskVolumeSerialNumber()
        {
            try
            {
                ManagementObject _disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                _disk.Get();
                return _disk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get CPU ID
        /// </summary>
        /// <returns></returns>
        private static string GetProcessorId()
        {
            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["ProcessorId"].ToString();
                    break;
                }

                return _id;

            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Get motherboard serial number
        /// </summary>
        /// <returns></returns>
        private static string GetMotherboardID()
        {

            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["SerialNumber"].ToString();
                    break;
                }

                return _id;
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// not usable in main license proccess
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static byte[] GetUIDInBytes(string UID)
        {
            //Split 4 part Id into 4 ulong
            string[] _ids = UID.Split('-');

            if (_ids.Length != 4) throw new ArgumentException("Wrong UID");

            //Combine 4 part Id into one byte array
            byte[] _value = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[0])), 0, _value, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[1])), 0, _value, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[2])), 0, _value, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[3])), 0, _value, 24, 8);

            return _value;
        }


        class BASE36
        {
            private const string _charList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            private static readonly char[] _charArray = _charList.ToCharArray();

            public static long Decode(string input)
            {
                long _result = 0;
                double _pow = 0;
                for (int _i = input.Length - 1; _i >= 0; _i--)
                {
                    char _c = input[_i];
                    int pos = _charList.IndexOf(_c);
                    if (pos > -1)
                        _result += pos * (long)Math.Pow(_charList.Length, _pow);
                    else
                        return -1;
                    _pow++;
                }
                return _result;
            }

            public static string Encode(ulong input)
            {
                StringBuilder _sb = new StringBuilder();
                do
                {
                    _sb.Append(_charArray[input % (ulong)_charList.Length]);
                    input /= (ulong)_charList.Length;
                } while (input != 0);

                return Reverse(_sb.ToString());
            }

            private static string Reverse(string s)
            {
                var charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
        }
        #endregion

        #region activate license

        public void LicenseSettingsControl<LicenseT>(PropertyGrid pgLicenseSettings_) where LicenseT : LicenseEntity
        {
            pgLicenseSettings = pgLicenseSettings_;
            var class_mgnt = new SRL.ClassManagement<LicenseT>();
            License = class_mgnt.CreateInstance();
            pgLicenseSettings.SelectedObject = License;
        }

        public void GetLicense(LicenseTypes license_type, string UID_, out string licence, out string message)
        {
            licence = "";
            message = "";

            if (License == null) throw new ArgumentException("LicenseEntity is invalid");

            if (license_type == LicenseTypes.Single)
            {
                if (ValidateUIDFormat(UID_.Trim()))
                {
                    License.Type = LicenseTypes.Single;
                    License.UID = UID_.Trim();
                }
                else
                {
                    message = "License UID is blank or invalid";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (license_type == LicenseTypes.Volume)
            {
                License.Type = LicenseTypes.Volume;
                License.UID = string.Empty;
            }

            License.CreateDateTime = DateTime.Now;

            if (OnLicenseSettingsValidating != null)
            {
                LicenseSettingsValidatingEventArgs _args = new LicenseSettingsValidatingEventArgs() { License_ = License, CancelGenerating = false };

                OnLicenseSettingsValidating(this, _args);

                if (_args.CancelGenerating)
                {
                    return;
                }
            }

            string _licStr = GenerateLicenseBASE64String(License, CertificatePrivateKeyData, CertificatePassword);

            licence = _licStr;
            return;

        }

        public static bool ValidateUIDFormat(string UID)
        {
            if (!string.IsNullOrWhiteSpace(UID))
            {
                string[] _ids = UID.Split('-');

                return (_ids.Length == 4);
            }
            else
            {
                return false;
            }

        }

        public class LicenseSettingsValidatingEventArgs : EventArgs
        {
            public LicenseEntity License_ { get; set; }
            public bool CancelGenerating { get; set; }
        }

        public static string GenerateLicenseBASE64String(LicenseEntity lic, byte[] certPrivateKeyData, SecureString certFilePwd)
        {
            //Serialize license object into XML                    
            XmlDocument _licenseObject = new XmlDocument();
            using (StringWriter _writer = new StringWriter())
            {
                XmlSerializer _serializer = new XmlSerializer(typeof(LicenseEntity), new Type[] { lic.GetType() });

                _serializer.Serialize(_writer, lic);

                _licenseObject.LoadXml(_writer.ToString());
            }

            //Get RSA key from certificate
            X509Certificate2 cert = new X509Certificate2(certPrivateKeyData, certFilePwd);

            RSACryptoServiceProvider rsaKey = (RSACryptoServiceProvider)cert.PrivateKey;

            //Sign the XML
            SignXML(_licenseObject, rsaKey);

            //Convert the signed XML into BASE64 string            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(_licenseObject.OuterXml));
        }

        /// <summary>
        /// Sign an XML file. This document cannot be verified unless the verifying code has the key with which it was signed.
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="Key"></param>
        private static void SignXML(XmlDocument xmlDoc, RSA Key)
        {
            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = Key;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

        }


        #endregion

        #region volume License

        public bool AllowVolumeLicenseSet(bool value, GroupBox grpbxLicenseType, RadioButton rdoSingleLicense)
        {

            if (!value)
            {
                rdoSingleLicense.Checked = true;
            }

            grpbxLicenseType.Enabled = value;


            return grpbxLicenseType.Enabled;
        }

        private void LicenseTypeRadioButtonsCheckedChanged(TextBox txtUID, RadioButton rdoSingleLicense)
        {
            txtUID.Text = string.Empty;

            txtUID.Enabled = rdoSingleLicense.Checked;
        }


        #endregion

        #region examples

        public class AppLicenseExample1 : LicenseEntity
        {
            [DisplayName("license_key")]
            [Category("License Options")]
            [XmlElement("license_key")]
            [ShowInLicenseInfo(true, "license_key", ShowInLicenseInfoAttribute.FormatType.String)]
            public string license_key { get; set; }


            public AppLicenseExample1()
            {
                //Initialize app name for the license
                this.AppName = Assembly.GetExecutingAssembly().GetName().Name; ;
            }

            public override LicenseStatus DoExtraValidation(bool is_mobile_uid_input, out string validationMsg)
            {
                LicenseStatus _licStatus = LicenseStatus.UNDEFINED;
                validationMsg = string.Empty;

                switch (this.Type)
                {
                    case SRL.LicenseClass.LicenseTypes.Single:
                        //For Single License, check whether UID is matched

                        string genereted_UID = GenerateUID(this.AppName, is_mobile_uid_input ? this.UserMobile : null);
                        if (this.UID == genereted_UID)
                        {
                            _licStatus = LicenseStatus.VALID;
                        }
                        else
                        {
                            validationMsg = "کلید امنیتی برای این کپی غیر مجاز است";
                            _licStatus = LicenseStatus.INVALID;
                        }
                        break;
                    case LicenseTypes.Volume:
                        //No UID checking for Volume License
                        _licStatus = LicenseStatus.VALID;
                        break;
                    default:
                        validationMsg = "کلید امنیتی اشتباه است";
                        _licStatus = LicenseStatus.INVALID;
                        break;
                }

                return _licStatus;
            }
        }

        public class AppLicenseExample2 : LicenseEntity
        {
            [DisplayName("Enable Feature 01")]
            [Category("License Options")]
            [XmlElement("EnableFeature01")]
            [ShowInLicenseInfo(true, "Enable Feature 01", ShowInLicenseInfoAttribute.FormatType.String)]
            public bool EnableFeature01 { get; set; }

            [DisplayName("Enable Feature 02")]
            [Category("License Options")]
            [XmlElement("EnableFeature02")]
            [ShowInLicenseInfo(true, "Enable Feature 02", ShowInLicenseInfoAttribute.FormatType.String)]
            public bool EnableFeature02 { get; set; }


            [DisplayName("Enable Feature 03")]
            [Category("License Options")]
            [XmlElement("EnableFeature03")]
            [ShowInLicenseInfo(true, "Enable Feature 03", ShowInLicenseInfoAttribute.FormatType.String)]
            public bool EnableFeature03 { get; set; }

            public AppLicenseExample2()
            {
                //Initialize app name for the license
                this.AppName = Assembly.GetExecutingAssembly().GetName().Name;
            }

            public override LicenseStatus DoExtraValidation(bool is_mobile_uid_input, out string validationMsg)
            {
                LicenseStatus _licStatus = LicenseStatus.UNDEFINED;
                validationMsg = string.Empty;

                switch (this.Type)
                {
                    case LicenseTypes.Single:
                        //For Single License, check whether UID is matched

                        if (this.UID == GenerateUID(this.AppName, is_mobile_uid_input ? this.UserMobile : null))
                        {
                            _licStatus = LicenseStatus.VALID;
                        }
                        else
                        {
                            validationMsg = "The license is NOT for this copy!";
                            _licStatus = LicenseStatus.INVALID;
                        }
                        break;
                    case LicenseTypes.Volume:
                        //No UID checking for Volume License
                        _licStatus = LicenseStatus.VALID;
                        break;
                    default:
                        validationMsg = "Invalid license";
                        _licStatus = LicenseStatus.INVALID;
                        break;
                }

                return _licStatus;
            }
        }

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

        #endregion


    }
}
