using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    /**新接口返回的数据反序列化对应的类
     * @author ChW
     * @date 2021-06-07
     */
    [DataContract]
    public class RemoteReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public RemoteReturnItem Result { get; set; }
    }

	[DataContract]
	public class RemoteReturnItem
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "msg")]
		public string Message { get; set; }
	}
}
