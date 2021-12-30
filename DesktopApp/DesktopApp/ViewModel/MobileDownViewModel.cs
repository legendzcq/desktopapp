using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Model;
using GalaSoft.MvvmLight.Command;
using Framework.Utility;

namespace DesktopApp.ViewModel
{
	/// <summary>
	/// �ƶ��γ�������ͼģ��
	/// </summary>
	public class MobileDownViewModel : NavigationViewModelBase
	{
		public MobileDownViewModel()
		{
			InitCmd();
		}

		#region ����

		/// <summary>
		/// �����ֻ���Ƶ
		/// </summary>
		public ICommand NavMobileVideoDownLoadCommand { get; private set; }
		/// <summary>
		/// �����ֻ���Ƶ
		/// </summary>
		public ICommand NavMobileAudioDownLoadCommand { get; private set; }
		/// <summary>
		/// ���ظ�����Ƶ
		/// </summary>
		public ICommand NavHdVideoDownLoadCommand { get; private set; }
		/// <summary>
		/// ���ظ�����Ƶ
		/// </summary>
		public ICommand NavHdAudioDownLoadCommand { get; private set; }

		/// <summary>
		/// ˢ�±�����������
		/// </summary>
		public ICommand RefreshLocalCommand { get; set; }

		#endregion

		#region ����

		private ListCollectionView _courseList;
		private bool _isShowNoData;

		public ListCollectionView CourseList
		{
			get { return _courseList; }
			set
			{
				_courseList = value;
				RaisePropertyChanged(() => CourseList);
			}
		}

		public bool IsShowNoData
		{
			get { return _isShowNoData; }
			set
			{
				_isShowNoData = value;
				RaisePropertyChanged(() => IsShowNoData);
			}
		}

		#endregion

		#region ����

		private void InitCmd()
		{
			NavMobileVideoDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item => ShowDetailPage(item, 3));
			NavMobileAudioDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item => ShowDetailPage(item, 4));
			NavHdVideoDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item => ShowDetailPage(item, 1));
			NavHdAudioDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item => ShowDetailPage(item, 2));

			RefreshLocalCommand = new RelayCommand(BindData);
		}

		private void ShowDetailPage(ViewStudentCourseWare item, int downloadType)
		{
			if (item.IsOpen)
			{
				NavigationService.Navigate(new Uri("/Pages/MobileDownloadDetail.xaml", UriKind.Relative), new { Item = item, DownloadType = downloadType });
			}
			else
			{
				CustomMessageBox.Show(item.CanDownload
					? string.Format("��{0}-{1}�� δ��ͨ!", item.CourseName, item.CWareClassName, " δ��ͨ")
					: string.Format("��{0}-{1}������Ȩ����δ���š�\r\n��ʾ����ͨ�γ���������Զ���������Ȩ�ޣ��������ͨ�γ��Ѿ������죬�����������б���ȡȨ�ޣ�",
						item.CourseName, item.CWareClassName));
			}
		}

		private void BindData()
		{
			List<ViewStudentCourseWare> list = StudentWareLogic.GetStudentCourseWareList();
			IsShowNoData = !list.Any();
			CourseList = new ListCollectionView(list);

			if (CourseList.GroupDescriptions != null)
				CourseList.GroupDescriptions.Add(new PropertyGroupDescription("CourseName"));

		}

		#endregion

		public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
		{
            ShowLoading();
			if (mode == NavigationMode.Back)
			{
				BindData();
                HideLoading();
				return;
			}
            //BindData();
            SystemInfo.StartBackGroundThread("�����ƶ�����", () => Dispatcher.Invoke(new Action(() =>
            {
                BindData();
                HideLoading();

            })));
		}
	}
}