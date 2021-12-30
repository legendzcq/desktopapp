using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Framework.Model
{
    [DataContract]
    public class RemoteLoginReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public RemoteLogin Result { get; set; }
    }

    [DataContract]
	public class RemoteLogin
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "msg")]
		public string Msg { get; set; }

        [DataMember(Name = "registerFlag")]
        public string RegisterFlag { get; set; }

        [DataMember(Name = "nickName")]
        public string NickName { get; set; }

        [DataMember(Name = "ssouid")]
		public int Ssouid { get; set; }

        [DataMember(Name = "sex")]
        public string Sex { get; set; }

        [DataMember(Name = "notify")]
        public string Notify { get; set; }

        [DataMember(Name = "username")]
		public string Username { get; set; }

		[DataMember(Name = "sid")]
		public string Sid { get; set; }

        [DataMember(Name = "isBindWechat")]
        public bool IsBindWechat { get; set; }

        [DataMember(Name = "mobilePhone")]
        public string MobilePhone { get; set; }

        [DataMember(Name = "schoolID")]
        public string SchoolID { get; set; }

        [DataMember(Name = "protalUserName")]
        public string ProtalUserName { get; set; }

        [DataMember(Name = "fullname")]
		public string FullName { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconUrl { get; set; }

        [DataMember(Name = "bindCode")]
        public string BindCode { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "mlist")]
        public ObservableCollection<LoginedDevice> Mlist { get; set; }

        //public List<LoginedDevice> LoginedDeviceList { get; set; }

        [DataContract]
        public class LoginedDevice
		{
            [DataMember(Name = "mid")]
            public string DeviceId { get; set; }

            [DataMember(Name = "mname")]
            public string DeviceName { get; set; }
		}
	}


}
