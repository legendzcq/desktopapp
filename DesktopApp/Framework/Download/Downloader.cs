using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Framework.Utility;

namespace Framework.Download
{
	public sealed class Downloader : IDownloader
	{
		/// <summary>
		/// 要下载的文件路径
		/// </summary>
		public string FileUrl { get; set; }

		/// <summary>
		/// 最终存放的本地路径
		/// </summary>
		public string FilePath { get; set; }

		/// <summary>
		/// 下载编号
		/// </summary>
		public long DownId { get; set; }

		public bool IsOver { get; set; }

		public bool UseMirrorDown { get; set; }

		/// <summary>
		/// 是否退出
		/// </summary>
		private bool _forceExist;

		/// <summary>
		/// 是否取消任务
		/// </summary>
		private bool _canceled;

		/// <summary>
		/// 下载进度事件
		/// </summary>
		public event DownloadProcessEventHandler DownloadProcess;

		/// <summary>
		/// 下载进度
		/// </summary>
		private void OnProcess(long downId, long fileSize, long offset, long downSpeed)
		{
			//Trace.WriteLine(string.Format("OnProcess:{0},{1},{2}", downId, fileSize, offset));
			DownloadProcessEventHandler handler = DownloadProcess;
			if (handler != null) handler(this, new DownloadProcessEventArgs(downId, fileSize, offset, downSpeed));
		}

		/// <summary>
		/// 下载开始事件
		/// </summary>
		public event DownloadStartedEventHandler DownloadStarted;

		private void OnStarted(long downId)
		{
			Trace.WriteLine("Download: " + FileUrl);
			DownloadStartedEventHandler handler = DownloadStarted;
			if (handler != null) handler(this, new DownloadEventArgs(downId));
		}

		/// <summary>
		/// 下载暂停事件
		/// </summary>
		public event DownloadPausedEventHandler DownloadPaused;

		private void OnPaused(long downId)
		{
			Trace.WriteLine("OnPaused: " + downId);
			DownloadPausedEventHandler handler = DownloadPaused;
			IsOver = true;
			if (handler != null) handler(this, new DownloadEventArgs(downId));
		}

		public event DownloadCanceledEventHandler DownloadCanceled;

		private void OnCanceled(long downId)
		{
			Trace.WriteLine("OnCanceled: " + downId);
			DownloadCanceledEventHandler handler = DownloadCanceled;
			IsOver = true;
			if (handler != null) handler(this, new DownloadEventArgs(downId));
		}

		/// <summary>
		/// 下载完成事件
		/// </summary>
		public event DownloadComplateEventHandler DownloadComplate;

		private void OnDownloadComplate(long downId)
		{
			Trace.WriteLine("DownloadComplate: " + downId);
			DownloadComplateEventHandler handler = DownloadComplate;
			IsOver = true;
			if (handler != null) handler(this, new DownloadComplateEventArgs(downId, FilePath));
		}

		public event DownloadErrorEventHandler DownloadError;

		private void OnDownloadError(long downId)
		{
			Trace.WriteLine("DownloadError: " + downId);
			DownloadErrorEventHandler handler = DownloadError;
			IsOver = true;
			if (handler != null) handler(this, new DownloadEventArgs(downId));
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fileUrl"></param>
		/// <param name="filePath"></param>
		/// <param name="fileId"></param>
		public Downloader(string fileUrl, string filePath, long fileId)
		{
			FileUrl = fileUrl;
			FilePath = filePath;
			string path = Path.GetDirectoryName(filePath);
			if (path != null && !Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			DownId = fileId;
		}

		/// <summary>
		/// 开始下载
		/// </summary>
		public void Start()
		{
			var skb = 0;
			_forceExist = false;
			long fileSize = 0;
			long offset = 0;
			var taskSpeed = new Task(() =>
			{
				while (!_forceExist)
				{
					OnProcess(DownId, fileSize, offset, skb);
					//Trace.WriteLine("speed:" + skb);
					skb = 0;
					Thread.Sleep(1000);
				}
			});

			SystemInfo.StartBackGroundThread("单线程下载线程", () =>
			{
				Thread.Sleep(1000);
				OnStarted(DownId);
				var tempFile = Path.GetDirectoryName(FilePath) + "\\" + Path.GetFileName(FilePath) + Util.DownloadFileExtension;
				FileStream fs;
				try
				{
					if (File.Exists(tempFile))
					{
						fs = File.OpenWrite(tempFile);
						offset = fs.Length;
						fs.Seek(offset, SeekOrigin.Current);
					}
					else
					{
						fs = File.Create(tempFile);
						offset = 0;
					}
				}
				catch
				{
					OnPaused(DownId);
					return;
				}
				bool isComplete = false;
				try
				{
					var webreq = (HttpWebRequest)WebRequest.Create(FileUrl);
					if (offset > 0) webreq.AddRange(offset);
					//如果设置了代理服务器，那么就设置代理
					if (Util.ProxyType != 0) webreq.Proxy = Network.GetWebProxy();
					var webRes = (HttpWebResponse)webreq.GetResponse();
					Stream webStream = webRes.GetResponseStream();
					string rangeStr = webRes.Headers["Content-Range"];
					fileSize = webRes.ContentLength;
					if (string.IsNullOrWhiteSpace(rangeStr))
					{
						offset = 0;
						fs.Seek(0, SeekOrigin.Current);
					}
					else
					{
						fileSize += offset;
					}
					taskSpeed.Start();
					var buffer = new byte[1024];
					int byr;
					while (webStream != null && ((byr = webStream.Read(buffer, 0, 1024)) > 0 && !_forceExist))
					{
						fs.Write(buffer, 0, byr);
						offset += byr;
						skb++;
						//Trace.WriteLine("total:" + skb);
					}
					if (webStream != null) webStream.Dispose();
					webRes.Close();
					if (!_forceExist)
					{
						isComplete = true;
					}
				}
				catch (WebException wex)
				{
					var res = (HttpWebResponse)wex.Response;
					if (res != null && res.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
					{
						isComplete = true;
					}
					else if (res != null && res.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
					{
						//Range不对
						Restart();
					}
					else
					{
						OnPaused(DownId);
					}
				}
				catch (Exception ex)
				{
					Log.RecordLog("error: " + ex);
					OnDownloadError(DownId);
				}
				fs.Close();
				//thSpeed.Abort();
				if (_forceExist)
				{
					if (_canceled)
					{
						OnCanceled(DownId);
						return;
					}
					OnPaused(DownId);
					return;
				}
				if (isComplete)
				{
					OnDownloadComplate(DownId);
				}
			});
		}

		/// <summary>
		/// 重新下载
		/// </summary>
		private void Restart()
		{
			var tempFile = Path.GetDirectoryName(FilePath) + "\\" + Path.GetFileName(FilePath) + Util.DownloadFileExtension;
			if (File.Exists(tempFile))
			{
				SystemInfo.TryDeleteFile(tempFile);
				Start();
			}
		}

		/// <summary>
		/// 取消下载
		/// </summary>
		public void Cancel()
		{
			_canceled = true;
			Stop();
		}

		/// <summary>
		/// 停止下载
		/// </summary>
		public void Stop()
		{
			_forceExist = true;
		}
	}
}
