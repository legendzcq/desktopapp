using System.Runtime.Serialization;

namespace Framework.Push
{
	/// <summary>
	/// ���͵�������Ϣ
	/// </summary>
	[DataContract]
	public class PushLinkMessage : PushMessage
	{
		/// <summary>
		/// ���͵�����
		/// </summary>
		[DataMember(Name = "link")]
		public string LinkUrl { get; set; }
	}
}