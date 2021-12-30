using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
	[DataContract]
	public class StudentUserData
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "msg")]
		public string Msg { get; set; }

		[DataMember(Name = "userdata")]
		public UserData UserDataItem { get; set; }

		[DataContract]
		public class UserData
		{
			[DataMember(Name = "address")]
			public string Address { get; set; }

			[DataMember(Name = "birthday")]
			public string Birthday { get; set; }

			[DataMember(Name = "city")]
			public string City { get; set; }

			[DataMember(Name = "eduLevel")]
			public string EduLevel { get; set; }

			[DataMember(Name = "email")]
			public string Email { get; set; }

			[DataMember(Name = "fullC")]
			public string FullC { get; set; }

			[DataMember(Name = "fullName")]
			public string FullName { get; set; }

			[DataMember(Name = "iconUrl")]
			public string IconUrl { get; set; }

			[DataMember(Name = "memberid")]
			public string Memberid { get; set; }

			[DataMember(Name = "mobilePhone")]
			public string MobilePhone { get; set; }

			[DataMember(Name = "nickName")]
			public string NickName { get; set; }

			[DataMember(Name = "province")]
			public string Province { get; set; }

			[DataMember(Name = "sex")]
			public string Sex { get; set; }

			[DataMember(Name = "sign")]
			public string Sign { get; set; }

			[DataMember(Name = "uid")]
			public int Uid { get; set; }
		}
	}
}
