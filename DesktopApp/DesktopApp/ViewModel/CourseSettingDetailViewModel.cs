using System.Windows.Input;
using Framework.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
	public class CourseSettingDetailViewModel : ViewModelBase
	{
		public ViewStudentCwareSetting DataItem { get; set; }
		private string _cwareName;
		private bool _isShow;
		private int _sortOrder;

		public CourseSettingDetailViewModel(ViewStudentCwareSetting item)
		{
			DataItem = item;
			CwareName = item.CwareName;
			CwareId = item.CwareId;
			IsShow = item.UserHide == 0;
			SetCheckCommand = new RelayCommand(() =>
			{
				IsShow = !IsShow;
			});
		}

		public ICommand SetCheckCommand { get; set; }

		public string CwareName
		{
			get { return _cwareName; }
			set
			{
				_cwareName = value;
				RaisePropertyChanged(() => CwareName);
			}
		}

		public int CwareId { get; set; }


		public bool IsShow
		{
			get { return _isShow; }
			set
			{
				_isShow = value;
				DataItem.UserHide = value ? 0 : 1;
				RaisePropertyChanged(() => IsShow);
			}
		}
	}
}