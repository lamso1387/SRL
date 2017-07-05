﻿using System;
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
        public string DateFormat { get; set; }

        public string DateTimeFormat { get; set; }
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



        public bool CheckLicense<LicenseT>(Assembly _assembly, string app_name, string license_cer__full_file_name,
             out byte[] _certPubicKeyData, out string license_) where LicenseT : LicenseEntity
        {
            license_ = "";
            //Initialize variables with default values
            LicenseEntity _lic = null;
            // AppLicenseClass _lic = null;
            string _msg = string.Empty;
            LicenseStatus _status = LicenseStatus.UNDEFINED;

            //Read public key from assembly
            //  Assembly _assembly = Assembly.GetExecutingAssembly();

            _certPubicKeyData = GetPubicKeyData(_assembly, app_name, license_cer__full_file_name);


            //Check if the XML license file exists
            if (System.IO.File.Exists("license.lic"))
            {
                _lic = (LicenseT)LicenseHandler.ParseLicenseFromBASE64String(
                    //  LicenseHandler.ParseLicenseFromBASE64String(
                    typeof(LicenseT),
                    System.IO.File.ReadAllText("license.lic"),
                    _certPubicKeyData,
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
                    license_ = ShowLicenseInfo(_lic, "");

                    return true;

                default:
                    //for the other status of license file, show the warning message
                    //and also popup the activation form for user to activate your application
                    MessageBox.Show(_msg, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
        }

        public byte[] GetPubicKeyData(Assembly _assembly_, string app_name_, string license_cer__full_file_name_)
        {
            using (MemoryStream _mem_ = new MemoryStream())
            {
                _assembly_.GetManifestResourceStream(app_name_ + "." + license_cer__full_file_name_).CopyTo(_mem_);

                return _mem_.ToArray();
            }

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

        class HardwareInfo
        {
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

            private static IEnumerable<string> SplitInParts(string input, int partLength)
            {
                if (input == null)
                    throw new ArgumentNullException("input");
                if (partLength <= 0)
                    throw new ArgumentException("Part length has to be positive.", "partLength");

                for (int i = 0; i < input.Length; i += partLength)
                    yield return input.Substring(i, Math.Min(partLength, input.Length - i));
            }

            /// <summary>
            /// Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
            /// </summary>
            /// <returns></returns>
            public static string GenerateUID(string appName)
            {
                //Combine the IDs and get bytes
                string _id = string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());
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


        public abstract class LicenseEntity
        {
            [Browsable(false)]
            [XmlIgnore]
            [ShowInLicenseInfo(false)]
            public string AppName { get; protected set; }

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
            public abstract LicenseStatus DoExtraValidation(out string validationMsg);

        }

        /// <summary>
        /// Usage Guide:
        /// Command for creating the certificate
        /// >> makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=<YourCertName>" -sky Signature
        /// Then export the cert with private key from key store with a password
        /// Also export another cert with only public key
        /// </summary>
        public class LicenseHandler
        {

            public static string GenerateUID(string appName)
            {
                return HardwareInfo.GenerateUID(appName);
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



            public static LicenseEntity ParseLicenseFromBASE64String(Type licenseObjType, string licenseString, byte[] certPubKeyData, out LicenseStatus licStatus, out string validationMsg)
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
                    X509Certificate2 cert = new X509Certificate2(certPubKeyData);
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

                        licStatus = _lic.DoExtraValidation(out validationMsg);
                    }
                    else
                    {
                        licStatus = LicenseStatus.INVALID;
                    }
                }
                catch
                {
                    licStatus = LicenseStatus.CRACKED;
                }

                return _lic;
            }

            // Sign an XML file. 
            // This document cannot be verified unless the verifying 
            // code has the key with which it was signed.
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

            // Verify the signature of an XML file against an asymmetric 
            // algorithm and return the result.
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

            public static bool ValidateUIDFormat(string UID)
            {
                return HardwareInfo.ValidateUIDFormat(UID);
            }
        }

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


        public class LicenseActivate
        {

            public bool ShowMessageAfterValidation { get; set; }

            public string LicenseBASE64String(string txtLicense)
            {
                return txtLicense.Trim();
            }

            public LicenseActivate(bool ShowMessageAfterValidation_)
            {

                ShowMessageAfterValidation = ShowMessageAfterValidation_;
            }

            public string ShowUID(string AppName)
            {
                return LicenseHandler.GenerateUID(AppName);
            }

            public bool ValidateLicense(string txtLicense, Type LicenseObjectType, byte[] CertificatePublicKeyData)
            {
                if (string.IsNullOrWhiteSpace(txtLicense))
                {
                    MessageBox.Show("Please input license", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                //Check the activation string
                LicenseStatus _licStatus = LicenseStatus.UNDEFINED;
                string _msg = string.Empty;
                LicenseEntity _lic = LicenseHandler.ParseLicenseFromBASE64String(LicenseObjectType, txtLicense.Trim(), CertificatePublicKeyData, out _licStatus, out _msg);
                switch (_licStatus)
                {
                    case LicenseStatus.VALID:
                        if (ShowMessageAfterValidation)
                        {
                            MessageBox.Show(_msg, "License is valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        return true;

                    case LicenseStatus.CRACKED:
                    case LicenseStatus.INVALID:
                    case LicenseStatus.UNDEFINED:
                        if (ShowMessageAfterValidation)
                        {
                            MessageBox.Show(_msg, "License is INVALID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return false;

                    default:
                        return false;
                }
            }

        }

        public class LicenseInfo
        {
            public string DateFormat { get; set; }

            public string DateTimeFormat { get; set; }

            public LicenseInfo()
            {
            }

            public string ShowTextOnly(string text)
            {
                return text.Trim();
            }

            public void ShowLicenseInfo(LicenseEntity license)
            {
                ShowLicenseInfo(license, string.Empty);
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
        }

        public class LicenseStringContainer
        {
            public string LicenseString { get; set; }


            public string LicenseStringset(string txtLicense)
            {
                LicenseString = txtLicense;
                return LicenseString;
            }

            public LicenseStringContainer()
            {

            }



        }

        public class LicenseSettingsValidatingEventArgs : EventArgs
        {
            public LicenseEntity License { get; set; }
            public bool CancelGenerating { get; set; }
        }

        public class LicenseGeneratedEventArgs
        {
            public string LicenseBASE64String { get; set; }
        }


        public delegate void LicenseSettingsValidatingHandler(object sender, LicenseSettingsValidatingEventArgs e);
        public delegate void LicenseGeneratedHandler(object sender, LicenseGeneratedEventArgs e);

        public class LicenseSettingsControl
        {
            PropertyGrid pgLicenseSettings;
            public byte[] CertificatePrivateKeyData { set; private get; }
            public LicenseSettingsControl(PropertyGrid pgLicenseSettings_, byte[] CertificatePrivateKeyData_)
            {
                pgLicenseSettings = pgLicenseSettings_;
                CertificatePrivateKeyData = CertificatePrivateKeyData_;
            }

            public event LicenseSettingsValidatingHandler OnLicenseSettingsValidating;
            public event LicenseGeneratedHandler OnLicenseGenerated;

            protected LicenseEntity _lic;

            public LicenseEntity License
            {
                set
                {
                    _lic = value;
                    pgLicenseSettings.SelectedObject = _lic;
                }
            }



            public SecureString CertificatePassword { set; private get; }

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

            public void GetLicense(LicenseTypes license_type, string UID_, out string licence)
            {
                licence = "";

                if (_lic == null) throw new ArgumentException("LicenseEntity is invalid");

                if (license_type == LicenseTypes.Single)
                {
                    if (LicenseHandler.ValidateUIDFormat(UID_.Trim()))
                    {
                        _lic.Type = LicenseTypes.Single;
                        _lic.UID = UID_.Trim();
                    }
                    else
                    {
                        MessageBox.Show("License UID is blank or invalid", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (license_type == LicenseTypes.Volume)
                {
                    _lic.Type = LicenseTypes.Volume;
                    _lic.UID = string.Empty;
                }

                _lic.CreateDateTime = DateTime.Now;

                if (OnLicenseSettingsValidating != null)
                {
                    LicenseSettingsValidatingEventArgs _args = new LicenseSettingsValidatingEventArgs() { License = _lic, CancelGenerating = false };

                    OnLicenseSettingsValidating(this, _args);

                    if (_args.CancelGenerating)
                    {
                        return;
                    }
                }

                string _licStr = LicenseHandler.GenerateLicenseBASE64String(_lic, CertificatePrivateKeyData, CertificatePassword);



                licence = _licStr;
                return;

            }
        }


        public class ActivationTool<LicenseT> where LicenseT : LicenseEntity
        {
            public byte[] _certPubicKeyData;
            private SecureString _certPwd;

            public ActivationTool(Assembly _assembly, LicenseT licenseT,
                SecureString _certPwd_, string activation_app_name)
            {

                //_certPwd.AppendChar('2');
                //_certPwd.AppendChar('0');
                //_certPwd.AppendChar('5');
                //_certPwd.AppendChar('0');
                //_certPwd.AppendChar('1');
                //_certPwd.AppendChar('3');
                //_certPwd.AppendChar('0');
                //_certPwd.AppendChar('3');
                //_certPwd.AppendChar('5');
                //_certPwd.AppendChar('1');

                _certPwd = _certPwd_;


                _certPubicKeyData = new SRL.LicenseClass().GetPubicKeyData(_assembly, activation_app_name, "LicenseSign.pfx");

            }

            private void btnGenSvrMgmLic_Click(LicenseStringContainer licString, LicenseT licenseT)
            {
                //Event raised when "Generate License" button is clicked. 
                //Call the core library to generate the license
                licString.LicenseString = LicenseHandler.GenerateLicenseBASE64String(
                   licenseT,
                    _certPubicKeyData,
                    _certPwd);
            }

        }


    }
}
