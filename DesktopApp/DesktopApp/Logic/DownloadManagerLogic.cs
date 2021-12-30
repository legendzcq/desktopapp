using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using DesktopApp.Infrastructure;
using DesktopApp.ViewModel;
using Framework.Download;
using Framework.Local;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.Logic
{
	/// <summary>
	/// 下载业务逻辑
	/// </summary>
	//	internal static class DownloadManagerLogicOld
	//	{

	//		private const int MaxTaskCount = 3;

	//		/// <summary>
	//		/// 下载完毕之后界面要做的操做
	//		/// </summary>
	//		public static event Action<long, int> DownloadComplateEvent;

	//		/// <summary>
	//		/// 下载暂停的时候界面要做的操作
	//		/// </summary>
	//		public static event Action<long> DownloadPauseEvent;

	//		/// <summary>
	//		/// 下载取消的时候界面要做的操作
	//		/// </summary>
	//		public static event Action<long> DownloadCancelEvent;

	//		/// <summary>
	//		/// 下载开始的时候界面要做的操作
	//		/// </summary>
	//		public static event Action<long> DownloadStartEvent;

	//		/// <summary>
	//		/// 下载速度改变的时候的操作
	//		/// </summary>
	//		public static event Action<long, int> DownloadSpeedEvent;

	//		/// <summary>
	//		/// 下载进度改变时做的操作
	//		/// </summary>
	//		public static event Action<long, long, long> DownloadProcessEvent;

	//		/// <summary>
	//		/// 下载进度改变时做的操作
	//		/// </summary>
	//		public static event Action<long> DownloadErrorEvent;

	//		private static readonly Dictionary<long, int> ErrorDownList = new Dictionary<long, int>();

	//#if MULTYTASK
	//		private static readonly object LockObject = new object();

	//		private static readonly List<IDownloader> DownloaderList = new List<IDownloader>();
	//#else
	//			/// <summary>
	//			/// 下载器定义
	//			/// </summary>
	//			//private static MultiBlockDownloader _downloader;
	//			private static IDownloader _downloader;
	//#endif
	//		private static int _downType = -1;

	//		public static bool IsDownLoading
	//		{
	//			get
	//			{
	//#if MULTYTASK
	//				return DownloaderList.Count > 0;
	//#else
	//					return _downloader != null && !_downloader.IsOver;
	//#endif
	//			}
	//		}

	//		/// <summary>
	//		/// 添加到下载列表
	//		/// </summary>
	//		/// <returns></returns>
	//		public static long AddToDownloadTask(int cwareId, string videoId, int downType, Func<bool> noHdNotice)
	//		{
	//			Log.RecordData("DownloadVideo", cwareId, videoId);
	//			var local = new StudentWareData();
	//			var courseDetail = local.GetStudentCWareDetailItem(cwareId, videoId);
	//			if (courseDetail == null) return 0;
	//			string url;
	//			switch (downType)
	//			{
	//				case 1:
	//					url = courseDetail.VideoHdZipUrl;
	//					break;
	//				case 2:
	//					url = courseDetail.AudioZipUrl;
	//					break;
	//				case 3:
	//					url = courseDetail.VideoZipUrl;
	//					break;
	//				case 4:
	//					url = courseDetail.AudioZipUrl;
	//					break;
	//				default:
	//					url = string.Empty;
	//					break;
	//			}

	//			if (string.IsNullOrEmpty(url))
	//			{
	//				CustomMessageBox.Show("没有找到视频《" + courseDetail.VideoName + "》的下载链接");
	//				return 0;
	//			}
	//			var item = new StudentCwareDownload
	//			{
	//				CwareId = courseDetail.CWareId,
	//				DownState = 0,
	//				VideoId = courseDetail.VideoId,
	//				DownUrl = url,
	//				LocalFile = string.Empty,
	//				DownType = downType,
	//				ForImport = 0
	//			};
	//			var tmpfile = Util.VideoPath + "\\" + courseDetail.CWareId + "\\" + courseDetail.VideoId + Util.DownloadFileExtension;
	//			SystemInfo.TryDeleteFile(tmpfile);
	//			item.DownId = (int)local.AddCwareDownload(item);
	//			return item.DownId;
	//		}

	//		/// <summary>
	//		/// 创建一个下载器
	//		/// </summary>
	//		/// <param name="item"></param>
	//#if MULTYTASK
	//		private static IDownloader CreateDownloader(ViewStudentCwareDownLoad item)
	//		{
	//			IDownloader _downloader;
	//#else
	//			private static void CreateDownloader(ViewStudentCwareDown item)
	//			{
	//#endif
	//			if (item.LocalFile.Length == 0)
	//			{
	//				item.LocalFile = Util.VideoPath + "\\" + item.CwareId + "\\" + item.VideoId + Util.FormatExtension;
	//				bool beginCwareDown = new StudentWareData().BeginCwareDownload(item.DownId, item.LocalFile);
	//				Trace.WriteLine("update local:" + beginCwareDown);
	//			}
	//#if MULTYDOWN
	//			_downloader = new MultiBlockDownloader(item.DownUrl, item.LocalFile, item.DownId);
	//			if (ErrorDownList.ContainsKey(item.DownId)) _downloader.UseMirrorDown = true;
	//#else
	//				_downloader = new Downloader(item.Url, item.LocalFile, item.DownId);
	//#endif
	//			_downloader.DownloadStarted += ExecuteDownloadStarted;
	//			_downloader.DownloadComplate += ExecuteDownloadComplate;
	//			_downloader.DownloadPaused += (downer, downId) => ExecuteDownloadPaused(downId);
	//			_downloader.DownloadError += ExecuteDownloadError;
	//			int lastdownRate = 0;
	//			_downloader.DownloadProcess += (a, b, c) =>
	//			{
	//				var crate = c * 100 / b;
	//				if (crate != lastdownRate)
	//				{
	//					lastdownRate = (int)crate;
	//					SetTaskBarProgressValue(lastdownRate, 100);
	//				}
	//				if (DownloadProcessEvent != null) DownloadProcessEvent(a, b, c);
	//			};
	//			_downloader.DownloadSpeed += ExecuteDownloadSpeed;
	//			_downloader.DownloadCanceled += (downer, downId) => ExecuteDownloadCancel(downId);
	//#if MULTYTASK
	//			DownloaderList.Add(_downloader);
	//			return _downloader;
	//#endif
	//		}

	//		/// <summary>
	//		/// 下载取消回调
	//		/// </summary>
	//		/// <param name="downId"></param>
	//		private static void ExecuteDownloadCancel(long downId)
	//		{
	//			//Messenger.Default.Send(string.Empty, TokenManager.DownStart);
	//			//if (TaskbarManager.IsPlatformSupported)
	//			//{
	//			//    try
	//			//    {
	//			//        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
	//			//    }
	//			//    // ReSharper disable once EmptyGeneralCatchClause
	//			//    catch
	//			//    { }
	//			//}
	//			RemoveDownloadError(downId);
	//			if (DownloadCancelEvent != null) DownloadCancelEvent(downId);
	//		}

	//		private static void ExecuteDownloadSpeed(long downId, int speed)
	//		{
	//			if (DownloadSpeedEvent != null) DownloadSpeedEvent(downId, speed);
	//		}

	//		private static void ExecuteDownloadError(long downId)
	//		{
	//			var local = new StudentWareData();
	//#if MULTYTASK
	//			lock (LockObject)
	//			{
	//				var obj = DownloaderList.FirstOrDefault(x => x.DownId == downId);
	//				DownloaderList.Remove(obj);
	//			}
	//#endif
	//			int errcnt = AddDownloadError(downId);
	//			if (errcnt < 30)
	//			{
	//				local.MoveDownloadToLast(downId);
	//				local.UpdateCwareDownloadState(downId, 0);
	//				if (DownloadPauseEvent != null) DownloadPauseEvent(downId);
	//			}
	//			else
	//			{
	//				local.UpdateCwareDownloadState(downId, 5);
	//				if (DownloadErrorEvent != null) DownloadErrorEvent(downId);
	//			}
	//			//if (TaskbarManager.IsPlatformSupported)
	//			//{
	//			//    try
	//			//    {
	//			//        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
	//			//    }
	//			//    // ReSharper disable once EmptyGeneralCatchClause
	//			//    catch
	//			//    {
	//			//    }
	//			//}
	//			StartNext();
	//		}

	//		private static void ExecuteDownloadPaused(long downId)
	//		{
	//			if (DownloadPauseEvent != null) DownloadPauseEvent(downId);
	//			//if (TaskbarManager.IsPlatformSupported)
	//			//{
	//			//    try
	//			//    {
	//			//        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
	//			//    }
	//			//    // ReSharper disable once EmptyGeneralCatchClause
	//			//    catch
	//			//    {
	//			//    }
	//			//}
	//		}

	//		private static void ExecuteDownloadComplate(long downId)
	//		{
	//			long fid = downId;
	//			new StudentWareData().UpdateCwareDownloadState(downId, 3);
	//			//int state = DoDownloadFile(downId);
	//			RemoveDownloadError(downId);
	//			if (DownloadComplateEvent != null) DownloadComplateEvent(fid, 3);
	//			Application.Current.Dispatcher.Invoke(new Action(() => Messenger.Default.Send<string>(null, TokenManager.RefreshList)));
	//#if MULTYTASK
	//			lock (LockObject)
	//			{
	//				var obj = DownloaderList.FirstOrDefault(x => x.DownId == downId);
	//				DownloaderList.Remove(obj);
	//			}
	//#else
	//				if (_downloader != null) _downloader.IsOver = true;
	//#endif
	//			StartNext();
	//		}

	//		private static void ExecuteDownloadStarted(long downId)
	//		{
	//			var local = new StudentWareData();
	//			local.UpdateCwareDownloadState(downId, 1);
	//			try
	//			{
	//#if MULTYTASK
	//				if (DownloadStartEvent != null) DownloadStartEvent(downId);
	//#else
	//					if (DownloadStartEvent != null) DownloadStartEvent(_downloader.DownId);
	//#endif
	//			}
	//			catch (Exception ex)
	//			{
	//				Trace.WriteLine(ex.ToString());
	//			}
	//			//Messenger.Default.Send(item.VideoName, TokenManager.DownStart);
	//		}

	//		private static int AddDownloadError(long downId)
	//		{
	//			if (ErrorDownList.ContainsKey(downId))
	//			{
	//				ErrorDownList[downId]++;
	//				return ErrorDownList[downId];
	//			}
	//			ErrorDownList.Add(downId, 1);
	//			return 1;
	//		}

	//		private static void RemoveDownloadError(long downId)
	//		{
	//			if (ErrorDownList.ContainsKey(downId))
	//			{
	//				ErrorDownList.Remove(downId);
	//			}
	//		}

	//		/// <summary>
	//		/// win7 及以上设置任务栏图标的进度条
	//		/// </summary>
	//		/// <param name="currentValue"></param>
	//		/// <param name="maximumValue"></param>
	//		private static void SetTaskBarProgressValue(int currentValue, int maximumValue)
	//		{
	//			//try
	//			//{
	//			//    if (!TaskbarManager.IsPlatformSupported) return;
	//			//    TaskbarManager.Instance.SetProgressValue(currentValue, maximumValue);
	//			//}
	//			//// ReSharper disable once EmptyGeneralCatchClause
	//			//catch { }
	//		}

	//		/// <summary>
	//		/// 对下载完之后的文件做处理
	//		/// </summary>
	//		private static int DoDownloadFile(long downId)
	//		{
	//#if MULTYTASK
	//			var _downloader = DownloaderList.FirstOrDefault(x => x.DownId == downId);
	//			if (_downloader == null) return 3;
	//#endif
	//			var tempFile = Path.GetDirectoryName(_downloader.FilePath) + "\\" + Path.GetFileName(_downloader.FilePath) + Util.DownloadFileExtension;
	//			bool isFileError = false;
	//			var zip = new ImportZip
	//			{
	//				ConfirmFun = msg => true,
	//				CompleteAction = str => { },
	//				FileError = id =>
	//				{
	//					isFileError = true;
	//					SystemInfo.TryDeleteFile(tempFile);
	//				}
	//			};
	//			try
	//			{
	//				if (!zip.ImportFromFile(string.Empty, tempFile, Util.VideoPath))
	//				{
	//					int state = isFileError ? 4 : 5;
	//					var local = new StudentWareData();
	//					var item = local.GetCwareDownloadList().FirstOrDefault(x => x.DownId == _downloader.DownId);
	//					if (item != null && App.PlayedFile.Contains(item.CwareId + "-" + item.VideoId))
	//					{
	//						state = 2;
	//					}
	//					local.UpdateCwareDownloadState(_downloader.DownId, state);
	//					return state;
	//				}
	//				return 3;
	//			}
	//			finally
	//			{
	//				if (File.Exists(tempFile))
	//				{
	//					var zipfile = Path.GetDirectoryName(_downloader.FilePath) + "\\" + Path.GetFileName(_downloader.FileUrl);
	//					try
	//					{
	//						SystemInfo.TryDeleteFile(zipfile);
	//						File.Move(tempFile, zipfile);
	//					}
	//					catch (Exception ex)
	//					{
	//						Log.RecordLog(ex.ToString());
	//					}
	//				}
	//			}
	//		}

	//		/// <summary>
	//		/// 开始一个下载
	//		/// </summary>
	//		/// <param name="downItem"></param>
	//		private static void Start(ViewStudentCwareDownLoad downItem)
	//		{
	//#if !MULTYTASK
	//				Stop();
	//#endif
	//			var size = SystemInfo.GetFolderFreeSpaceInMb(Util.VideoPath);
	//			if (size < 512)
	//			{
	//				App.CurrentMainWindow.Dispatcher.Invoke(new Action(() => CustomMessageBox.Show("您的指定的下载路径空间已不足，下载任务已停止。\r\n请更改下载路径。下载路径的最小可用空间要求512M。")));
	//				Messenger.Default.Send(string.Empty, TokenManager.ShowSetting);
	//				return;
	//			}
	//#if MULTYTASK
	//			var downloader = CreateDownloader(downItem);
	//			Trace.WriteLine("Downloader Created");
	//			downloader.Start();
	//#else
	//				CreateDownloader(downItem);
	//				Trace.WriteLine("Downloader Created");
	//				_downloader.Start();
	//#endif
	//			Trace.WriteLine("Downloader Start");
	//		}

	//		/// <summary>
	//		/// 开始一个下载
	//		/// </summary>
	//		/// <param name="downId"></param>
	//		public static void Start(long downId)
	//		{
	//#if MULTYTASK
	//			if (DownloaderList.Count >= MaxTaskCount) return;
	//#else
	//				if (_downloader != null && !_downloader.IsOver) return;
	//#endif
	//			var local = new StudentWareData();
	//			var list = local.GetCwareNeedDownloadList();
	//			var item = list.FirstOrDefault(x => x.DownId == downId);
	//			if (item != null)
	//			{
	//				Start(item);
	//			}
	//		}

	//		/// <summary>
	//		/// 停止
	//		/// </summary>
	//		public static void Stop()
	//		{
	//#if MULTYTASK
	//			DownloaderList.ForEach(x =>
	//			{
	//				x.Stop();
	//				x.IsOver = true;
	//			});
	//			DownloaderList.Clear();
	//#else
	//				if (_downloader != null)
	//				{
	//					_downloader.Stop();
	//					_downloader.IsOver = true;
	//					_downloader = null;
	//				}
	//#endif
	//		}

	//		/// <summary>
	//		/// 寻找下一个下载并执行
	//		/// </summary>
	//		public static void StartNext()
	//		{
	//			if (!Util.IsOnline) return;
	//#if MULTYTASK
	//			if (DownloaderList.Count >= MaxTaskCount) return;
	//			lock (LockObject)
	//			{
	//				var local = new StudentWareData();
	//				var lst = local.GetCwareNeedDownloadList();
	//				var item = lst.FirstOrDefault(x => x.DownState != 3 && DownloaderList.All(y => y.DownId != x.DownId));
	//				if (item != null)
	//				{
	//					Trace.WriteLine("StartDownload:" + item.DownUrl);
	//					Start(item);
	//				}
	//				else
	//				{
	//					return;
	//				}
	//			}
	//			if (DownloaderList.Count < MaxTaskCount) StartNext();
	//#else
	//				if (_downloader != null && !_downloader.IsOver) return;
	//				var local = new StudentWareData();
	//				var lst = local.GetCwareNeedDownList();
	//				var item = lst.FirstOrDefault(x => x.State != 3);
	//				if (item != null)
	//				{
	//					Trace.WriteLine("StartDownload:" + item.Url);
	//					Start(item);
	//				}
	//#endif
	//		}

	//		public static void CancelDownload(long downId)
	//		{

	//		}

	//		/// <summary>
	//		/// 取消
	//		/// </summary>
	//		/// <param name="downId"></param>
	//		public static void Cancle(long downId)
	//		{
	//			bool isdowning = false;
	//#if MULTYTASK
	//			var downloader = DownloaderList.FirstOrDefault(x => x.DownId == downId);
	//			if (downloader != null && downloader.DownId == downId)
	//			{
	//				downloader.Cancel();
	//				downloader.IsOver = true;
	//				DownloaderList.Remove(downloader);
	//				isdowning = true;
	//			}
	//#else
	//				if (_downloader != null && _downloader.DownId == downId)
	//				{
	//					_downloader.Cancel();
	//					_downloader.IsOver = true;
	//					_downloader = null;
	//					isdowning = true;
	//				}
	//#endif
	//			var local = new StudentWareData();
	//			local.CancelDonwload(downId);
	//			if (isdowning) StartNext();
	//		}

	//		/// <summary>
	//		/// 暂停下载
	//		/// </summary>
	//		/// <param name="downId"></param>
	//		public static void Pause(long downId)
	//		{
	//#if MULTYTASK
	//			var downloader = DownloaderList.FirstOrDefault(x => x.DownId == downId);
	//			if (downloader != null && !downloader.IsOver && downloader.DownId == downId)
	//			{
	//				downloader.Stop();
	//				downloader.IsOver = true;
	//				DownloaderList.Remove(downloader);
	//			}
	//#else
	//				if (_downloader != null && !_downloader.IsOver && _downloader.DownId == downId)
	//				{
	//					_downloader.Stop();
	//					_downloader.IsOver = true;
	//					_downloader = null;
	//				}
	//#endif
	//			var local = new StudentWareData();
	//			local.UpdateCwareDownloadState(downId, 2);
	//			StartNext();
	//		}

	//		/// <summary>
	//		/// 继续下载
	//		/// </summary>
	//		/// <param name="downId"></param>
	//		public static void Continue(long downId)
	//		{
	//			var local = new StudentWareData();
	//			local.UpdateCwareDownloadState(downId, 0);
	//		}

	//		public static IEnumerable<ViewStudentCwareDownLoad> GetDownList()
	//		{
	//			var local = new StudentWareData();
	//			return local.GetCwareDownloadList();
	//		}
	//	}

	//internal class DownloadFileEventArgs : EventArgs
	//{
	//	public ViewStudentCwareDownLoad DownLoadItem { get; set; }
	//}

	//internal delegate void DownloadFileEventHandler(DownloadFileEventArgs e);

	//internal static class DownloadManagerLogic
	//{
	//	public static event DownloadFileEventHandler DownloadCanceled;
	//	public static event DownloadFileEventHandler DownloadComplate;
	//	public static event DownloadFileEventHandler DownloadError;
	//	public static event DownloadFileEventHandler DownloadPaused;
	//	public static event DownloadFileEventHandler DownloadProcess;
	//	public static event DownloadFileEventHandler DownloadStarted;

	//	/// <summary>
	//	/// 最多任务数
	//	/// </summary>
	//	private const int MaxTaskCount = 3;
	//	/// <summary>
	//	/// 锁对象
	//	/// </summary>
	//	private static readonly object LockObject = new object();
	//	/// <summary>
	//	/// 下载器列表
	//	/// </summary>
	//	private static readonly IList<IDownloader> DownLoaderList = new List<IDownloader>();

	//	private static readonly Dictionary<long, int> DownloadErrorList = new Dictionary<long, int>();

	//	/// <summary>
	//	/// 是否正在下载
	//	/// </summary>
	//	public static bool IsDownLoading
	//	{
	//		get
	//		{
	//			return DownLoaderList.Count > 0;
	//		}
	//	}

	//	/// <summary>
	//	/// 获取下载类型目录
	//	/// </summary>
	//	/// <param name="downType"></param>
	//	/// <returns></returns>
	//	private static string GetDownTypeDir(int downType)
	//	{
	//		switch (downType)
	//		{
	//			case 1:
	//				return "高清";
	//			case 2:
	//				return "高清音频";
	//			case 3:
	//				return "手机";
	//			case 4:
	//				return "手机音频";
	//		}
	//		return string.Empty;
	//	}

	//	public static long AddToDownVideoTask(int cwareId, string videoId, int downType,
	//		Func<bool> confirm = null)
	//	{
	//		AddToDownloadTask(cwareId, videoId, downType, 1, confirm);
	//		return new StudentWareData().GetCwareDownId(cwareId, videoId);
	//	}

	//	/// <summary>
	//	/// 添加下载任务
	//	/// </summary>
	//	public static long AddToDownloadTask(int cwareId, string videoId, int downType, int forImport, Func<bool> confirm = null)
	//	{
	//		var local = new StudentWareData();
	//		var vitem = local.GetStudentCWareDetailItem(cwareId, videoId);
	//		var downUrl = string.Empty;
	//		switch (downType)
	//		{
	//			case 1:
	//				downUrl = vitem.VideoHdZipUrl;
	//				break;
	//			case 2:
	//				downUrl = vitem.AudioZipUrl;
	//				break;
	//			case 3:
	//				downUrl = vitem.VideoZipUrl;
	//				break;
	//			case 4:
	//				downUrl = vitem.AudioZipUrl;
	//				break;
	//		}
	//		if (string.IsNullOrEmpty(downUrl))
	//		{
	//			if (forImport == 1 && downType == 1)
	//			{
	//				if (confirm != null && confirm())
	//				{
	//					downUrl = vitem.VideoZipUrl;
	//					downType = 3;
	//					if (string.IsNullOrEmpty(downUrl))
	//					{
	//						return 0;
	//					}
	//				}
	//			}
	//			else
	//			{
	//				return 0;
	//			}
	//		}
	//		var downloadItem = local.GetCwareDownloadItem(cwareId, videoId, downType);
	//		long id = 0;
	//		if (downloadItem == null)
	//		{
	//			var downItem = new StudentCwareDownload
	//			{
	//				CwareId = cwareId,
	//				DownState = 0,
	//				DownType = downType,
	//				DownUrl = downUrl,
	//				ForImport = forImport,
	//				LocalFile = string.Empty,
	//				VideoId = videoId
	//			};
	//			id = local.AddCwareDownload(downItem);
	//			if (id > 0)
	//			{
	//				var tmpFile = string.Format("{0}\\{1}\\{2}\\{3}{4}", Util.VideoDownSavePath, cwareId, GetDownTypeDir(downType), videoId, Util.DownloadFileExtension);
	//				SystemInfo.TryDeleteFile(tmpFile);
	//				var tmpCfgFile = string.Format("{0}\\{1}\\{2}\\{3}{4}", Util.VideoDownSavePath, cwareId, GetDownTypeDir(downType), videoId, Util.DownloadConfigExtension);
	//				SystemInfo.TryDeleteFile(tmpCfgFile);
	//			}
	//		}
	//		if (forImport == 1)
	//		{
	//			if (downloadItem != null)
	//			{
	//				if (downloadItem.DownState == 3 && File.Exists(downloadItem.LocalFile))
	//				{
	//					DoImport(downloadItem.LocalFile);
	//					return id;
	//				}
	//				local.SetDownloadForImport(downloadItem.DownId);
	//			}
	//			var item = new StudentCWareDown
	//			{
	//				CwareId = cwareId,
	//				VideoId = videoId,
	//				LocalFile = string.Empty,
	//				State = 0,
	//				Url = string.Empty,
	//				Rate = 0.0
	//			};
	//			local.AddCwareDown(item);
	//		}
	//		return id;
	//	}

	//	/// <summary>
	//	/// 取消下载
	//	/// </summary>
	//	/// <param name="downId"></param>
	//	/// <returns></returns>
	//	public static void CancelDownloadTask(long downId)
	//	{
	//		bool isDownloading = false;
	//		var downloader = DownLoaderList.FirstOrDefault(x => x.DownId == downId);
	//		if (downloader != null && !downloader.IsOver)
	//		{
	//			downloader.Cancel();
	//			downloader.IsOver = true;
	//			DownLoaderList.Remove(downloader);
	//			isDownloading = true;
	//		}
	//		new StudentWareData().CancelDonwload(downId);
	//		OnDownloadCanceled(null, new DownloadEventArgs(downId));
	//		if (isDownloading) StartNext();
	//	}

	//	/// <summary>
	//	/// 暂停下载任务
	//	/// </summary>
	//	/// <param name="downId"></param>
	//	/// <returns></returns>
	//	public static void PauseDownloadTask(long downId)
	//	{
	//		var downloader = DownLoaderList.FirstOrDefault(x => x.DownId == downId);
	//		if (downloader != null && !downloader.IsOver)
	//		{
	//			downloader.Stop();
	//			downloader.IsOver = true;
	//			DownLoaderList.Remove(downloader);
	//		}
	//		new StudentWareData().UpdateCwareDownloadState(downId, 2);
	//		StartNext();
	//	}

	//	/// <summary>
	//	/// 停止所有的下载
	//	/// </summary>
	//	public static void StopDownloading()
	//	{
	//		foreach (var downloader in DownLoaderList)
	//		{
	//			downloader.Stop();
	//			downloader.IsOver = true;
	//		}
	//		DownLoaderList.Clear();
	//	}

	//	/// <summary>
	//	/// 继续下载
	//	/// </summary>
	//	/// <param name="downId"></param>
	//	public static void ContinueDownloadTask(long downId)
	//	{
	//		new StudentWareData().UpdateCwareDownloadState(downId, 0);
	//	}

	//	/// <summary>
	//	/// 开始下载
	//	/// </summary>
	//	public static void StartNext()
	//	{
	//		if (!Util.IsOnline) return;
	//		if (DownLoaderList.Count >= MaxTaskCount) return;
	//		lock (LockObject)
	//		{
	//			var downlist = new StudentWareData().GetCwareNeedDownloadList();
	//			var item = downlist.FirstOrDefault(x => x.DownState < 2 && DownLoaderList.All(y => y.DownId != x.DownId));
	//			if (item != null)
	//			{
	//				var downloader = CreateDownloader(item);
	//				DownLoaderList.Add(downloader);
	//				downloader.Start();
	//			}
	//			else
	//			{
	//				return;
	//			}
	//		}
	//		StartNext();
	//	}

	//	public static IEnumerable<ViewStudentCwareDownLoad> GetDownloadTaskList()
	//	{
	//		return new StudentWareData().GetCwareDownloadList();
	//	}

	//	private static IDownloader CreateDownloader(ViewStudentCwareDownLoad item)
	//	{
	//		if (item.LocalFile.Length == 0)
	//		{
	//			string fileName = Path.GetFileName(item.DownUrl);
	//			item.LocalFile = string.Format("{0}\\{1}\\{2}\\{3}", Util.VideoDownSavePath, item.CwareId, GetDownTypeDir(item.DownType), fileName);
	//			new StudentWareData().BeginCwareDownload(item.DownId, item.LocalFile);
	//		}
	//		var downloader = new MultiBlockDownloader(item.DownUrl, item.LocalFile, item.DownId);
	//		downloader.DownloadStarted += OnDownloadStarted;
	//		downloader.DownloadProcess += OnDownloadProcess;
	//		downloader.DownloadPaused += OnDownloadPaused;
	//		downloader.DownloadError += OnDownloadError;
	//		downloader.DownloadComplate += OnDownloadComplate;
	//		downloader.DownloadCanceled += OnDownloadCanceled;
	//		return downloader;
	//	}

	//	static void OnDownloadCanceled(object sender, DownloadEventArgs e)
	//	{
	//		RemoveDownloadError(e.DownId);
	//		if (DownloadCanceled != null) DownloadCanceled(sender, e);
	//	}

	//	static void OnDownloadComplate(object sender, DownloadComplateEventArgs e)
	//	{
	//		RemoveDownloadError(e.DownId);
	//		if (DownloadComplate != null) DownloadComplate(sender, e);
	//		lock (LockObject)
	//		{
	//			new StudentWareData().UpdateCwareDownloadState(e.DownId, 3);
	//			var item = DownLoaderList.FirstOrDefault(x => x.DownId == e.DownId);
	//			if (item != null)
	//			{
	//				DownLoaderList.Remove(item);
	//			}
	//			var ditem = new StudentWareData().GetCwareDownloadItem(e.DownId);
	//			if (ditem.ForImport == 1)
	//			{
	//				DoImport(e.LocalFile);
	//			}
	//		}
	//		Messenger.Default.Send(e.DownId.ToString(CultureInfo.InvariantCulture), TokenManager.DownloadComplete);
	//		StartNext();
	//	}

	//	private static void DoImport(string localFile)
	//	{
	//		Application.Current.Dispatcher.Invoke(new Action(() =>
	//		{
	//			var impitem = new ImportItemViewModel
	//			{
	//				FileName = localFile,
	//				VideoSavePath = Util.VideoPath,
	//			};
	//			var loc = (ViewModelLocator)(Application.Current.Resources["Locator"]);
	//			loc.Import.Items.Add(impitem);
	//			loc.Import.ImportNext();
	//		}));
	//	}

	//	static void OnDownloadError(object sender, DownloadEventArgs e)
	//	{
	//		AddDownloadError(e.DownId);
	//		if (DownloadError != null) DownloadError(sender, e);
	//	}

	//	static void OnDownloadPaused(object sender, DownloadEventArgs e)
	//	{
	//		if (DownloadPaused != null) DownloadPaused(sender, e);
	//	}

	//	static void OnDownloadProcess(object sender, DownloadProcessEventArgs e)
	//	{
	//		if (DownloadProcess != null) DownloadProcess(sender, e);
	//	}

	//	static void OnDownloadStarted(object sender, DownloadEventArgs e)
	//	{
	//		if (DownloadStarted != null) DownloadStarted(sender, e);
	//	}

	//	private static void AddDownloadError(long downId)
	//	{
	//		if (DownloadErrorList.ContainsKey(downId))
	//		{
	//			DownloadErrorList[downId]++;
	//			return;
	//		}
	//		DownloadErrorList.Add(downId, 1);
	//	}

	//	private static void RemoveDownloadError(long downId)
	//	{
	//		if (DownloadErrorList.ContainsKey(downId))
	//		{
	//			DownloadErrorList.Remove(downId);
	//		}
	//	}
	//}
}

