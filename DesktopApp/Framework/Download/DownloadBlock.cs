using System;

namespace Framework.Download
{
	/// <summary>
	/// 下载块
	/// </summary>
	public class DownloadBlock
	{
		/// <summary>
		/// 当前块在整个文件中的偏移量
		/// </summary>
		public long FileOffset { get; private set; }

		/// <summary>
		/// 当前块的数据
		/// </summary>
		public byte[] BlockBuffer { get; private set; }

		/// <summary>
		/// 当前块的长度
		/// </summary>
		public int BufferLen { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public DownloadBlock()
		{
			BlockBuffer = new byte[MultiBlockDownloader.PackSize];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="size"></param>
		/// <param name="offset"></param>
		public void WriteBlock(byte[] buffer, int size, long offset)
		{
			BufferLen = size;
			if (BufferLen > MultiBlockDownloader.PackSize) throw new ArgumentOutOfRangeException("buffer");
			FileOffset = offset;
			Buffer.BlockCopy(buffer, 0, BlockBuffer, 0, size);
		}
	}
}