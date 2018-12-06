﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SRL.CixGetPersonInfoServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CixGetPersonInfoServiceReference.GetPersonInfoPortType")]
    public interface GetPersonInfoPortType {
        
        // CODEGEN: Parameter 'getPersonInfoSAHA96MResult' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISabtAhvalSAHA/getPersonInfoSAHA96M", ReplyAction="//tempuri.org/ISabtAhvalSAHA/getPersonInfoSAHA96MResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse getPersonInfoSAHA96M(SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISabtAhvalSAHA/getPersonInfoSAHA96M", ReplyAction="//tempuri.org/ISabtAhvalSAHA/getPersonInfoSAHA96MResponse")]
        System.Threading.Tasks.Task<SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse> getPersonInfoSAHA96MAsync(SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SabtAhvalSAHA.PersonInfoStract", Namespace="http://schemas.datacontract.org/2004/07/NewSabtAhvalSAHA")]
    public partial class SabtAhvalSAHAPersonInfoStract : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string birthDateField;
        
        private string cityField;
        
        private string deathDateField;
        
        private int errorCodeField;
        
        private bool errorCodeFieldSpecified;
        
        private string errorDescriptionField;
        
        private string fatherNameField;
        
        private string firstNameField;
        
        private string genderField;
        
        private string identityNoField;
        
        private string identitySerialField;
        
        private string identitySeriesField;
        
        private string isLiveField;
        
        private string lastNameField;
        
        private string nationalCodeField;
        
        private string supervisorNationalCodeField;
        
        private string townField;
        
        private string vilageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=0)]
        public string BirthDate {
            get {
                return this.birthDateField;
            }
            set {
                this.birthDateField = value;
                this.RaisePropertyChanged("BirthDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=1)]
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
                this.RaisePropertyChanged("City");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string DeathDate {
            get {
                return this.deathDateField;
            }
            set {
                this.deathDateField = value;
                this.RaisePropertyChanged("DeathDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public int ErrorCode {
            get {
                return this.errorCodeField;
            }
            set {
                this.errorCodeField = value;
                this.RaisePropertyChanged("ErrorCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ErrorCodeSpecified {
            get {
                return this.errorCodeFieldSpecified;
            }
            set {
                this.errorCodeFieldSpecified = value;
                this.RaisePropertyChanged("ErrorCodeSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string ErrorDescription {
            get {
                return this.errorDescriptionField;
            }
            set {
                this.errorDescriptionField = value;
                this.RaisePropertyChanged("ErrorDescription");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string FatherName {
            get {
                return this.fatherNameField;
            }
            set {
                this.fatherNameField = value;
                this.RaisePropertyChanged("FatherName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=6)]
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
                this.RaisePropertyChanged("FirstName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=7)]
        public string Gender {
            get {
                return this.genderField;
            }
            set {
                this.genderField = value;
                this.RaisePropertyChanged("Gender");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=8)]
        public string IdentityNo {
            get {
                return this.identityNoField;
            }
            set {
                this.identityNoField = value;
                this.RaisePropertyChanged("IdentityNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=9)]
        public string IdentitySerial {
            get {
                return this.identitySerialField;
            }
            set {
                this.identitySerialField = value;
                this.RaisePropertyChanged("IdentitySerial");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=10)]
        public string IdentitySeries {
            get {
                return this.identitySeriesField;
            }
            set {
                this.identitySeriesField = value;
                this.RaisePropertyChanged("IdentitySeries");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=11)]
        public string IsLive {
            get {
                return this.isLiveField;
            }
            set {
                this.isLiveField = value;
                this.RaisePropertyChanged("IsLive");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=12)]
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
                this.RaisePropertyChanged("LastName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=13)]
        public string NationalCode {
            get {
                return this.nationalCodeField;
            }
            set {
                this.nationalCodeField = value;
                this.RaisePropertyChanged("NationalCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=14)]
        public string SupervisorNationalCode {
            get {
                return this.supervisorNationalCodeField;
            }
            set {
                this.supervisorNationalCodeField = value;
                this.RaisePropertyChanged("SupervisorNationalCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=15)]
        public string Town {
            get {
                return this.townField;
            }
            set {
                this.townField = value;
                this.RaisePropertyChanged("Town");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=16)]
        public string Vilage {
            get {
                return this.vilageField;
            }
            set {
                this.vilageField = value;
                this.RaisePropertyChanged("Vilage");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getPersonInfoSAHA96M", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class getPersonInfoSAHA96MRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string nationalCode;
        
        public getPersonInfoSAHA96MRequest() {
        }
        
        public getPersonInfoSAHA96MRequest(string nationalCode) {
            this.nationalCode = nationalCode;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getPersonInfoSAHA96MResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class getPersonInfoSAHA96MResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public SRL.CixGetPersonInfoServiceReference.SabtAhvalSAHAPersonInfoStract getPersonInfoSAHA96MResult;
        
        public getPersonInfoSAHA96MResponse() {
        }
        
        public getPersonInfoSAHA96MResponse(SRL.CixGetPersonInfoServiceReference.SabtAhvalSAHAPersonInfoStract getPersonInfoSAHA96MResult) {
            this.getPersonInfoSAHA96MResult = getPersonInfoSAHA96MResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface GetPersonInfoPortTypeChannel : SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetPersonInfoPortTypeClient : System.ServiceModel.ClientBase<SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType>, SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType {
        
        public GetPersonInfoPortTypeClient() {
        }
        
        public GetPersonInfoPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GetPersonInfoPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetPersonInfoPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetPersonInfoPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType.getPersonInfoSAHA96M(SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest request) {
            return base.Channel.getPersonInfoSAHA96M(request);
        }
        
        public SRL.CixGetPersonInfoServiceReference.SabtAhvalSAHAPersonInfoStract getPersonInfoSAHA96M(string nationalCode) {
            SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest inValue = new SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest();
            inValue.nationalCode = nationalCode;
            SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse retVal = ((SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType)(this)).getPersonInfoSAHA96M(inValue);
            return retVal.getPersonInfoSAHA96MResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse> SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType.getPersonInfoSAHA96MAsync(SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest request) {
            return base.Channel.getPersonInfoSAHA96MAsync(request);
        }
        
        public System.Threading.Tasks.Task<SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MResponse> getPersonInfoSAHA96MAsync(string nationalCode) {
            SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest inValue = new SRL.CixGetPersonInfoServiceReference.getPersonInfoSAHA96MRequest();
            inValue.nationalCode = nationalCode;
            return ((SRL.CixGetPersonInfoServiceReference.GetPersonInfoPortType)(this)).getPersonInfoSAHA96MAsync(inValue);
        }
    }
}
