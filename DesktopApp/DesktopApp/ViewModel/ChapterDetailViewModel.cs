using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class ChapterDetailViewModel : ViewModelBase
	{
		private int _downloadValue;
		private VideoState _videoState;
		private string _videoName;
		private string _chapterName;
		private string _speed;
		private string _downloadValueStr;
		private string _videoStateStr;
		/// <summary>
		/// 是否更新
		/// </summary>
		private bool _bolUpdate;
		public ViewStudentWareDetail ViewStudentWare { get; private set; }

		/// <summary>
		/// 下载Id
		/// </summary>
		public long DownId { get; set; }

		/// <summary>
		/// 下载进度（范围：0-100）
		/// </summary>
		public int DownloadValue
		{
			get { return _downloadValue; }
			set
			{
				if (value < 0 || value > 100)
					return;

				DownloadValueStr = value + "%";
				_downloadValue = value;
				RaisePropertyChanged(() => DownloadValue);
			}
		}
		/// <summary>
		/// 下载进度百分比（20%）
		/// </summary>
		public string DownloadValueStr
		{
			get { return _downloadValueStr; }
			set
			{
				_downloadValueStr = value;
				RaisePropertyChanged(() => DownloadValueStr);
			}
		}

		public string ChapterName
		{
			get { return _chapterName; }
			set
			{
				_chapterName = value;
				RaisePropertyChanged(() => ChapterName);
			}
		}

		public string VideoName
		{
			get { return _videoName; }
			set
			{
				_videoName = value;
				RaisePropertyChanged(() => VideoName);
			}
		}
		/// <summary>
		/// 是否更新
		/// </summary>
		public bool BolUpdate
		{
			get { return _bolUpdate; }
			set
			{
				_bolUpdate = value;
				RaisePropertyChanged(() => BolUpdate);
			}

		}
		public int ChapterId { get; set; }

		/// <summary>
		/// 状态
		/// -2，未开通下载，-1 未下载，0,等待中,1,正在下载，2，暂停，3，已完成
		/// </summary>
		/// <remarks>-2，未开通下载，-1 未下载，0,等待中,1,正在下载，2，暂停，3，已完成</remarks>
		public VideoState VideoState
		{
			get { return _videoState; }
			set
			{
				switch (value)
				{
					case VideoState.NotOpen:
						VideoStateStr = "未开通";
						break;
					case VideoState.Undownload:
						VideoStateStr = "未下载";
						break;
					case VideoState.Wait:
						VideoStateStr = "正在下载";
						break;
					case VideoState.Downloading:
						VideoStateStr = "下载进行中";
						break;
					case VideoState.Pause:
						VideoStateStr = "暂停";
						break;
					default:
						VideoStateStr = "";
						break;
				}
				_videoState = value;
				ViewStudentWare.VideoState = (int)value;
				RaisePropertyChanged(() => VideoState);
			}
		}

		public string VideoStateStr
		{
			get { return _videoStateStr; }
			set
			{
				_videoStateStr = value;
				RaisePropertyChanged(() => VideoStateStr);
			}
		}

		/// <summary>
		/// 视频长度
		/// </summary>
		public string VideoLength { get; set; }

		/// <summary>
		/// 下载速度
		/// </summary>
		public string Speed
		{
			get { return _speed; }
			set
			{
				_speed = value;
				RaisePropertyChanged(() => Speed);
			}
		}

		public void FromModel(ViewStudentWareDetail model)
		{
			if (model == null)
				return;
			ChapterId = model.ChapterId;
			ChapterName = model.ChapterName;
			VideoLength = model.VideoLength;
			VideoName = string.IsNullOrEmpty(model.VideoName) ? model.Title : model.VideoName;
			ViewStudentWare = model;
			VideoState = (VideoState)model.VideoState;
			BolUpdate = model.IsUpdate;
			if (VideoState == VideoState.NotOpen)
			{
				Speed = "未开通";
			}
			DownId = model.DownId;
		}
	}
}
