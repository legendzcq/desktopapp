using System;
using System.IO;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class ImportItemViewModel : ViewModelBase
	{
		private string _fileName;
		private string _courseName;
		private string _status;
		private bool _isLoading;
		private bool _isComplate;

		public ImportItemViewModel()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; private set; }
		/// <summary>
		/// 文件完整路径
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set
			{
				_fileName = value;
				RaisePropertyChanged(() => FileName);
			}
		}
		/// <summary>
		/// 文件名
		/// </summary>
		public string ShortName
		{
			get
			{
				return Path.GetFileName(_fileName);
			}
		}
		/// <summary>
		/// 回调信息
		/// </summary>
		public string Message
		{
			get { return _courseName; }
			set
			{
				_courseName = value;
				RaisePropertyChanged(() => Message);
			}
		}
		/// <summary>
		/// 导入状态
		/// </summary>
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				RaisePropertyChanged(() => Status);
			}
		}
		/// <summary>
		/// 是否显示Loading动画
		/// </summary>
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				_isLoading = value;
				RaisePropertyChanged(() => IsLoading);
			}
		}

		/// <summary>
		/// 是否已经导入完成
		/// </summary>
		public bool IsComplate
		{
			get { return _isComplate; }
			set
			{
				_isComplate = value;
				RaisePropertyChanged(() => IsComplate);
			}
		}

		/// <summary>
		/// 文件保存路径
		/// </summary>
		public string VideoSavePath { get; set; }
	}
}