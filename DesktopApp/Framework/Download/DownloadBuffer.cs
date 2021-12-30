using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Framework.Utility;

namespace Framework.Download
{
	/// <summary>
	/// 下载缓存
	/// </summary>
	public class DownloadBuffer
	{
		/// <summary>
		/// 已下载块结构
		/// </summary>
		private class DownloadedBlock
		{
			public long StartPosition { get; set; }
			public long EndPosition { get; set; }
			public DownloadedBlock NextBlock { get; set; }
		}

		#region 字段

		/// <summary>
		/// 下载根节点
		/// </summary>
		private DownloadedBlock _root;

		/// <summary>
		/// 
		/// </summary>
		private readonly string _configFileName;

		/// <summary>
		/// 缓存大小
		/// </summary>
		private const int BufferSize = 2048;

		/// <summary>
		/// 加锁对象
		/// </summary>
		private static readonly object LockObj = new object();

		/// <summary>
		/// 
		/// </summary>
		private readonly List<DownloadBlock> _blockList;

		//private byte[] _bitMap;

		private DateTime _lastModified;
		private long _fileSize;

		#endregion

		#region 属性

		/// <summary>
		/// 
		/// </summary>
		private string FileName { get; set; }

		/// <summary>
		/// 文件大小
		/// </summary>
		public long FileSize
		{
			get { return _fileSize; }
			set
			{
				if (_fileSize != value)
				{
					_root = null;
					_fileSize = value;
					SystemInfo.TryDeleteFile(FileName);
					//IntBitMap();
				}
			}
		}

		public DateTime LastModified
		{
			get { return _lastModified; }
			set
			{
				if (_lastModified != value)
				{
					_root = null;
					_lastModified = value;
					SystemInfo.TryDeleteFile(FileName);
					//IntBitMap();
				}
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public DownloadBuffer(string fileName, string configFile)
		{
			_lastModified = Util.GetNow();
			_blockList = new List<DownloadBlock>(BufferSize);
			FileName = fileName;
			_configFileName = configFile;
			ReadDownloadedBlocksFromFile();
		}

		#region 下载缓存处理

		/// <summary>
		/// 
		/// </summary>
		public void AddBufferBlock(DownloadBlock block)
		{
			lock (LockObj)
			{
				_blockList.Add(block);
				if (_blockList.Count == BufferSize)
				{
					Trace.WriteLine(string.Format("下载缓存已满！写入文件:{0}", FileName));
					WriteBufferToFile();
                    Trace.WriteLine("方法WriteBufferToFile通过");
					WriteDownloadedBlocksToFile();
                    Trace.WriteLine("方法WriteDownloadedBlocksToFile通过");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void WriteBufferToFile()
		{
			FileStream fs = !File.Exists(FileName) ? File.Create(FileName) : File.OpenWrite(FileName);
			using (fs)
			{
				foreach (DownloadBlock block in _blockList)
				{
					try
					{
						fs.Position = block.FileOffset;
						fs.Write(block.BlockBuffer, 0, block.BufferLen);
						AddDownloadedBlock(block.FileOffset, block.BufferLen);
					}
					catch (IOException ex)
					{
                        Trace.WriteLine(ex.Message);
					}
				}
				_blockList.Clear();
			}
			//BuildBitMap();
		}

		public void WriteAllBufferToFile()
		{
			lock (LockObj)
			{
				WriteBufferToFile();
				File.SetLastWriteTime(FileName, LastModified);
				WriteDownloadedBlocksToFile();
			}
		}

		public void DeleteConfigFile()
		{
			SystemInfo.TryDeleteFile(_configFileName);
		}

		#endregion

		#region 下载块信息

		/// <summary>
		/// 读取所有需要下载的块
		/// </summary>
		/// <param name="hasDownloadByte"></param>
		/// <returns></returns>
		public List<Tuple<long, long>> ReadNeededBlocks(ref long hasDownloadByte)
		{
			var lst = new List<Tuple<long, long>>();
			var point = _root;
			while (point != null)
			{
				lst.Add(point.NextBlock != null
					? new Tuple<long, long>(point.EndPosition, point.NextBlock.StartPosition)
					: new Tuple<long, long>(point.EndPosition, FileSize));
				hasDownloadByte += point.EndPosition - point.StartPosition;
				point = point.NextBlock;
			}
			return lst;
		}

		/// <summary>
		/// 从文件中读取块信息
		/// </summary>
		private void ReadDownloadedBlocksFromFile()
		{
			_root = null;
			if (!File.Exists(_configFileName)) return;
			var by = File.ReadAllBytes(_configFileName);
			if (by.Length == 0) return;
			_fileSize = BitConverter.ToInt32(by, 0);
			_lastModified = new DateTime(BitConverter.ToInt64(by, 4));
			if (_lastModified < new DateTime(2000, 1, 1)) _lastModified = Util.GetNow();
			//IntBitMap();
			int blockCount = BitConverter.ToInt32(by, 12);
			for (int i = 0; i < blockCount; i++)
			{
				long offset = BitConverter.ToInt32(by, (i + 2) * 8);
				long len = BitConverter.ToInt32(by, (i + 2) * 8 + 4);
				AddDownloadedBlock(offset, len);
			}
			//BuildBitMap();
		}

		/// <summary>
		/// 将块信息保存到文件
		/// </summary>
		private void WriteDownloadedBlocksToFile()
		{
			int blockCount = 0;
			var fs = File.Create(_configFileName);
			using (var ms = new MemoryStream())
			{
				var point = _root;
				while (point != null)
				{
					blockCount++;
					ms.Write(BitConverter.GetBytes((int)point.StartPosition), 0, 4);
					ms.Write(BitConverter.GetBytes((int)(point.EndPosition - point.StartPosition)), 0, 4);
					point = point.NextBlock;
				}
				fs.Write(BitConverter.GetBytes((int)FileSize), 0, 4);
				fs.Write(BitConverter.GetBytes(LastModified.Ticks), 0, 8);
				fs.Write(BitConverter.GetBytes(blockCount), 0, 4);
				var by = ms.ToArray();
				fs.Write(by, 0, by.Length);
			}
			fs.Close();
		}

		/// <summary>
		/// 在控制台上显示块信息
		/// </summary>
		public void DisplayDownloadedBlock()
		{
			var point = _root;
			while (point != null)
			{
				Trace.WriteLine(string.Format("{2}:{0}-{1}", point.StartPosition, point.EndPosition, point.GetHashCode()));
				point = point.NextBlock;
			}
		}

		/// <summary>
		/// 添加一个已下载块
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		private void AddDownloadedBlock(long offset, long length)
		{
			long offsetOver = offset + length;
			if (_root == null)
			{
				//根节点为空
				_root = new DownloadedBlock
				{
					StartPosition = offset,
					EndPosition = offsetOver
				};
				return;
			}
			DownloadedBlock point = _root;
			while (point != null)
			{
				if (point == _root && point.StartPosition > offset)
				{
					var block = new DownloadedBlock
					{
						StartPosition = offset,
						EndPosition = offset + length,
						NextBlock = _root
					};
					_root = block;
					point = block;
				}
				else
				{
					if (offset >= point.StartPosition && offsetOver <= point.EndPosition)
					{
						//在现有块内部，直接返回
						break;
					}
					if (offset >= point.StartPosition && offset <= point.EndPosition)
					{
						point.EndPosition = offsetOver;
					}
					else
					{
						if (point.NextBlock == null)
						{
							var block = new DownloadedBlock
							{
								StartPosition = offset,
								EndPosition = offset + length
							};
							point.NextBlock = block;
							break;
						}
						if (offset < point.NextBlock.StartPosition)
						{
							if (offsetOver < point.NextBlock.StartPosition)
							{
								var block = new DownloadedBlock
								{
									StartPosition = offset,
									EndPosition = offset + length,
									NextBlock = point.NextBlock
								};
								point.NextBlock = block;
								break;
							}
							point.NextBlock.StartPosition = offset;
							if (offsetOver > point.NextBlock.EndPosition)
							{
								point.NextBlock.EndPosition = offsetOver;
								point = point.NextBlock;
							}
						}
						else
						{
							point = point.NextBlock;
							continue;
						}
					}
				}
				//合并一下重叠或者连接的块
				while (point.NextBlock != null)
				{
					if (point.EndPosition >= point.NextBlock.StartPosition)
					{
						if (point.EndPosition < point.NextBlock.EndPosition)
							point.EndPosition = point.NextBlock.EndPosition;
						point.NextBlock = point.NextBlock.NextBlock;
					}
					else
					{
						break;
					}
				}
				break;
			}
		}

		/// <summary>
		/// 移除一个删除块
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void RemoveDownloadedBlock(long offset, long length)
		{
			if (length <= 0) return;
			long offsetOver = offset + length;
			var point = _root;
			if (point == null) return;
			while (point != null)
			{
				if (offsetOver <= point.StartPosition)
				{
					//在当前节点之前，那么肯定在
					break;
				}
				if (point == _root && offset <= _root.StartPosition && offsetOver >= _root.EndPosition)
				{
					//对越过根节点的切割有特殊性
					if (_root.NextBlock == null)
					{
						_root = null;
					}
					else
					{
						while (_root != null)
						{
							if (_root.NextBlock != null && offsetOver <= _root.NextBlock.StartPosition)
							{
								_root = _root.NextBlock;
								break;
							}
							if (_root.NextBlock != null && offsetOver < _root.NextBlock.EndPosition)
							{
								_root = _root.NextBlock;
								_root.StartPosition = offsetOver;
								break;
							}
							_root = _root.NextBlock;
						}
					}
					break;
				}
				if (offset <= point.StartPosition && offsetOver >= point.StartPosition && offsetOver < point.EndPosition)
				{
					//切掉当节点的前面一部分
					point.StartPosition = offsetOver;
					break;
					//不需要修复
				}
				if (offset > point.StartPosition && offsetOver < point.EndPosition)
				{
					//切掉当前节点的中间一部分
					var block = new DownloadedBlock
					{
						StartPosition = offsetOver,
						EndPosition = point.EndPosition,
						NextBlock = point.NextBlock
					};
					point.EndPosition = offset;
					point.NextBlock = block;
					break;
					//不需要修复
				}
				if (offset > point.StartPosition && offset <= point.EndPosition && offsetOver >= point.EndPosition)
				{
					point.EndPosition = offset;
					while (point.NextBlock != null)
					{
						if (point.NextBlock.StartPosition >= offsetOver)
						{
							break;
						}
						if (point.NextBlock.EndPosition > offsetOver)
						{
							point.NextBlock.StartPosition = offsetOver;
							break;
						}
						point.NextBlock = point.NextBlock.NextBlock;
					}
					break;
				}
				if (offset > point.EndPosition)
				{
					while (point.NextBlock != null && offset <= point.NextBlock.StartPosition && offsetOver >= point.NextBlock.EndPosition)
					{
						point.NextBlock = point.NextBlock.NextBlock;
					}
				}
				point = point.NextBlock;
			}
		}

		#endregion

		#region 位图索引
		/*
        /// <summary>
        /// 初始化位图索引
        /// </summary>
        private void IntBitMap()
        {
            var blockCount = (int)(FileSize % MultiBlockDownloader.MinBlockSize == 0
                ? FileSize / MultiBlockDownloader.MinBlockSize
                : FileSize / MultiBlockDownloader.MinBlockSize + 1);
            var bitMapSize = blockCount % 8 == 0 ? blockCount / 8 : blockCount / 8 + 1;
            _bitMap = new byte[bitMapSize];
            if (blockCount % 8 != 0) _bitMap[bitMapSize - 1] = (byte)(0xff >> blockCount % 8);
        }

        public void BuildBitMap()
        {
            var point = _root;
            while (point != null)
            {
                long offset = point.StartPosition;
                long offsetOver = point.EndPosition;
                if (offset % MultiBlockDownloader.MinBlockSize != 0)
                {
                    offset = (offset / MultiBlockDownloader.MinBlockSize + 1) * MultiBlockDownloader.MinBlockSize;
                }
                while (offset + MultiBlockDownloader.MinBlockSize <= offsetOver || (offset < FileSize && offsetOver == FileSize))
                {
                    var blockOffset = (int)(offset / MultiBlockDownloader.MinBlockSize);
                    var byteOffset = blockOffset / 8;
                    var bytevalue = 7 - blockOffset % 8;
                    _bitMap[byteOffset] = (byte)(_bitMap[byteOffset] | 1 << bytevalue);
                    offset += MultiBlockDownloader.MinBlockSize;
                }
                point = point.NextBlock;
            }
            //DisplayBitmap();
        }

        private void DisplayBitmap()
        {
            Trace.WriteLine("位图索引：");
            foreach (byte by in _bitMap)
            {
                for (int i = 1; i <= 8; i++)
                {
                    int j = (8 - i);
                    Trace.Write((by & 1 << j) >> j);
                }
                Trace.Write("  ");
            }
            Trace.WriteLine(string.Empty);
        }

        public string GetBitmap()
        {
            return Convert.ToBase64String(_bitMap);
        }
        */
		#endregion
	}
}