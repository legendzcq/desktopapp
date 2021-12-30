using System;
using System.Runtime.Serialization;

namespace Framework.Push
{
	/// <summary>
	/// 推送消息
	/// </summary>
	[DataContract]
	public class PushMessage
	{
		/// <summary>
		/// 消息类型
		/// </summary>
		public int MessageType { get; set; }

		/// <summary>
		/// 消息体
		/// </summary>
		public string MessageBody { get; set; }

		public DateTime MessageTime { get; set; }

		/// <summary>
		/// 消息编号
		/// </summary>
		[DataMember(Name = "msgId")]
		public int MessageId { get; set; }

		/// <summary>
		/// 消息内容
		/// </summary>
		[DataMember(Name = "content")]
		public string MessageContent { get; set; }
	}
}