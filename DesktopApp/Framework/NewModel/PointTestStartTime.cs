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
    public class PointTestStartTimeReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public PointTestStartTimeResult Result { get; set; }
    }
    [DataContract]
	public class PointTestStartTimeResult
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "pointTestTimeList")]
		public IEnumerable<PointTestStartTimeItem> ItemList { get; set; }
	}

	[DataContract]
	public class PointTestStartTimeItem
	{
		[DataMember(Name = "pointName")]
		public string PointName { get; set; }

		[DataMember(Name = "testID")]
		public int TestId { get; set; }

		[DataMember(Name = "pointTestStartTime")]
		public int PointTestStartTime { get; set; }

		[DataMember(Name = "pointOpenType")]
		public string PointOpenType { get; set; }

		[DataMember(Name = "backTime")]
		public int BackTime { get; set; }

		public int CwareId { get; set; }

		public string VideoId { get; set; }

		public DateTime LastUpdate { get; set; }
	}
}
