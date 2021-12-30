using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class DownloadItemViewModel : ViewModelBase
	{
		private double _downloadValue;
		private VideoState _videoState;
		private string _videoName;
		private string _videoStateStr;
		private string _speed;
		private string _downloadValueStr;
		private bool _isSelected;

		public ViewStudentCwareDownLoad ViewStudentCwareDown { get; private set; }

		/// <summary>
		/// 下载Id
		/// </summary>
		public long DownId { get; set; }

		/// <summary>
		/// 下载进度（范围：0-100）
		/// </summary>
		public double DownloadValue
		{
			get { return _downloadValue; }
			set
			{
				if (value < 0 || value > 100)
					return;

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
					case VideoState.Wait:
						DownloadValueStr = Speed = "";
						VideoStateStr = "等待下载";
						break;
					case VideoState.Pause:
						DownloadValueStr = Speed = "";
						VideoStateStr = "暂停";
						break;
					case VideoState.DownloadError:
						DownloadValueStr = Speed = "";
						VideoStateStr = "网络错误";
						break;
					case VideoState.Error:
						DownloadValueStr = Speed = "";
						VideoStateStr = "文件损坏或过期";
						break;
					default:
						VideoStateStr = "";
						break;
				}

				_videoState = value;
				ViewStudentCwareDown.DownState = (int)value;
				RaisePropertyChanged(() => VideoState);
			}
		}

		public string VideoType { get; private set; }

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
		/// <summary>
		/// 当前项是否选中
		/// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				RaisePropertyChanged(() => IsSelected);
			}
		}

		public void FromModel(ViewStudentCwareDownLoad model)
		{
			if (model == null)
				return;

			VideoName = model.VideoName;
			ViewStudentCwareDown = model;
			VideoState = (VideoState)model.DownState;
			switch (model.DownType)
			{
				case 1:
					VideoType = "高清平板视频";
					break;
				case 2:
					VideoType = "高清音频";
					break;
				case 3:
					VideoType = "手机视频";
					break;
				case 4:
					VideoType = "手机音频";
					break;
			}
			if (VideoState == VideoState.NotOpen)
			{
				Speed = "未开通";
			}
			DownId = model.DownId;
		}

	}
}
