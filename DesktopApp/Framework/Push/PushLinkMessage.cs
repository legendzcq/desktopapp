using System.Runtime.Serialization;

namespace Framework.Push
{
	/// <summary>
	/// 推送的链接消息
	/// </summary>
	[DataContract]
	public class PushLinkMessage : PushMessage
	{
		/// <summary>
		/// 推送的链接
		/// </summary>
		[DataMember(Name = "link")]
		public string LinkUrl { get; set; }
	}
}