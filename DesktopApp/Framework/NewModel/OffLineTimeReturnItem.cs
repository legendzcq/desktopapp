using System.Runtime.Serialization;

namespace Framework.NewModel
{
	[DataContract]
	public class OffLineTimeReturnItem
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "result")]
		public int Result { get; set; }
	}
}