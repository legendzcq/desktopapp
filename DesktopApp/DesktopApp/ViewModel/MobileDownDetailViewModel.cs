using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class MobileDownDetailViewModel : ViewModelBase
	{
		private int _downloadValue;
		private VideoState _videoState;
		private string _videoName;
		private string _chapterName;
		private string _speed;
		private string _downloadValueStr;
		private string _videoStateStr;

		public ViewStudentWareDetail ViewStudentWare { get; private set; }

		/// <summary>
		/// ����Id
		/// </summary>
		public long DownId { get; set; }

		/// <summary>
		/// ���ؽ��ȣ���Χ��0-100��
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
		/// ���ؽ��Ȱٷֱȣ�20%��
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

		public int ChapterId { get; set; }

		/// <summary>
		/// ״̬
		/// -2��δ��ͨ���أ�-1 δ���أ�0,�ȴ���,1,�������أ�2����ͣ��3�������
		/// </summary>
		/// <remarks>-2��δ��ͨ���أ�-1 δ���أ�0,�ȴ���,1,�������أ�2����ͣ��3�������</remarks>
		public VideoState VideoState
		{
			get { return _videoState; }
			set
			{
				switch (value)
				{
					case VideoState.NotOpen:
						VideoStateStr = "δ��ͨ";
						break;
					case VideoState.Undownload:
						VideoStateStr = "δ����";
						break;
					case VideoState.Wait:
						VideoStateStr = "�ȴ�����";
						break;
					case VideoState.Downloading:
						VideoStateStr = "��������";
						break;
					case VideoState.Pause:
						VideoStateStr = "��ͣ";
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
		/// ��Ƶ����
		/// </summary>
		public string VideoLength { get; set; }

		/// <summary>
		/// �����ٶ�
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
			if (VideoState == VideoState.NotOpen)
			{
				Speed = "δ��ͨ";
			}
			DownId = model.DownId;
		}
	}
}