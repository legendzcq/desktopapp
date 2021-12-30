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
		/// �ļ�����·��
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
		/// �ļ���
		/// </summary>
		public string ShortName
		{
			get
			{
				return Path.GetFileName(_fileName);
			}
		}
		/// <summary>
		/// �ص���Ϣ
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
		/// ����״̬
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
		/// �Ƿ���ʾLoading����
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
		/// �Ƿ��Ѿ��������
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
		/// �ļ�����·��
		/// </summary>
		public string VideoSavePath { get; set; }
	}
}