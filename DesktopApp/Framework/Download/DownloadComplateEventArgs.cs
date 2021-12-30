using System;

namespace Framework.Download
{
	public class DownloadComplateEventArgs : EventArgs
	{
		public long DownId { get; private set; }

		public string LocalFile { get; private set; }

		public DownloadComplateEventArgs(long downId, string localFile)
		{
			DownId = downId;
			LocalFile = localFile;
		}
	}
}