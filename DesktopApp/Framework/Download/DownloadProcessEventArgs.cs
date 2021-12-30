using System;

namespace Framework.Download
{
	/// <summary>
	/// 下载进度事件数据
	/// </summary>
	public class DownloadProcessEventArgs : EventArgs
	{
		public long DownId { get; private set; }

		public long FileSize { get; private set; }

		public long Offset { get; private set; }

		public long DownSpeed { get; private set; }

		public DownloadProcessEventArgs(long downId, long fileSize, long offset, long downSpeed)
		{
			DownId = downId;
			FileSize = fileSize;
			Offset = offset;
			DownSpeed = downSpeed;
		}
	}
}