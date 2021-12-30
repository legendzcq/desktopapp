using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using static Framework.Model.RemoteLogin;

namespace Framework.Model
{
    [DataContract]
    public class DeviceListModel
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public DeviceListResultInfo Result { get; set; }
    }

    [DataContract]
    public class DeviceListResultInfo
    {
        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "ssouid")]
        public int Ssouid { get; set; }

        [DataMember(Name = "mlist")]
        public ObservableCollection<LoginedDevice> BindedDeviceList { get; set; }
    }

    ///**
    // * 保存设备的机器key、名字
    // */
    //[DataContract]
    //public class BindedDevice
    //{
    //    [DataMember(Name = "mid")]
    //    public string Mid { get; set; }

    //    [DataMember(Name = "mname")]
    //    public string Mname { get; set; }

    //}

    [DataContract]
    public class SendVerificationCodeReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public SendVerificationResult Result { get; set; }
    }

    [DataContract]
    public class SendVerificationResult
    {
        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "codeVerify")]
        public string CodeVerify { get; set; }
    }

    [DataContract]
    public class CheckUserIdentityByVerificationCodeReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public CheckUserIdentityByVerificationCodeResult Result { get; set; }
    }


    [DataContract]
    public class CheckUserIdentityByVerificationCodeResult
    {
        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }
    }

    [DataContract]
    public class UnbindDeviceReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public UnbindDeviceResult Result { get; set; }
    }


    [DataContract]
    public class UnbindDeviceResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
    }

    [DataContract]
    public class BindPhoneReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public BindPhoneResult Result { get; set; }
    }

    [DataContract]
    public class BindPhoneResult
    {
        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }
    }
}
