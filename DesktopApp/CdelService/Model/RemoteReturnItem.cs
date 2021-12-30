using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CdelService.NewModel
{
	[DataContract]
	public class RemoteReturnItem
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "msg")]
		public string Message { get; set; }
	}
}
