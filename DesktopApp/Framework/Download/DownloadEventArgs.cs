using System;

namespace Framework.Download
{
	/// <summary>
	/// 下载事件数据
	/// </summary>
	public class DownloadEventArgs : EventArgs
	{
		public long DownId { get; private set; }

		public DownloadEventArgs(long downId)
		{
			DownId = downId;
		}
	}
}