﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFClientTest1.WCFServiceProxy {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Person", Namespace="http://schemas.datacontract.org/2004/07/WCFDemo1")]
    [System.SerializableAttribute()]
    public partial class Person : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AgeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SexField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Age {
            get {
                return this.AgeField;
            }
            set {
                if ((this.AgeField.Equals(value) != true)) {
                    this.AgeField = value;
                    this.RaisePropertyChanged("Age");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Sex {
            get {
                return this.SexField;
            }
            set {
                if ((object.ReferenceEquals(this.SexField, value) != true)) {
                    this.SexField = value;
                    this.RaisePropertyChanged("Sex");
                }
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WCFServiceProxy.IPersonManageService")]
    public interface IPersonManageService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonManageService/AddPerson", ReplyAction="http://tempuri.org/IPersonManageService/AddPersonResponse")]
        bool AddPerson(WCFClientTest1.WCFServiceProxy.Person info);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonManageService/DelPerson", ReplyAction="http://tempuri.org/IPersonManageService/DelPersonResponse")]
        bool DelPerson(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonManageService/UpdatePerson", ReplyAction="http://tempuri.org/IPersonManageService/UpdatePersonResponse")]
        bool UpdatePerson(WCFClientTest1.WCFServiceProxy.Person info);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonManageService/GetAllPerson", ReplyAction="http://tempuri.org/IPersonManageService/GetAllPersonResponse")]
        WCFClientTest1.WCFServiceProxy.Person[] GetAllPerson();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPersonManageServiceChannel : WCFClientTest1.WCFServiceProxy.IPersonManageService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PersonManageServiceClient : System.ServiceModel.ClientBase<WCFClientTest1.WCFServiceProxy.IPersonManageService>, WCFClientTest1.WCFServiceProxy.IPersonManageService {
        
        public PersonManageServiceClient() {
        }
        
        public PersonManageServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PersonManageServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PersonManageServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PersonManageServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool AddPerson(WCFClientTest1.WCFServiceProxy.Person info) {
            return base.Channel.AddPerson(info);
        }
        
        public bool DelPerson(int id) {
            return base.Channel.DelPerson(id);
        }
        
        public bool UpdatePerson(WCFClientTest1.WCFServiceProxy.Person info) {
            return base.Channel.UpdatePerson(info);
        }
        
        public WCFClientTest1.WCFServiceProxy.Person[] GetAllPerson() {
            return base.Channel.GetAllPerson();
        }
    }
}
