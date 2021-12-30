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
	/// 移动课程下载视图模型
	/// </summary>
	public class MobileDownViewModel : NavigationViewModelBase
	{
		public MobileDownViewModel()
		{
			InitCmd();
		}

		#region 命令

		/// <summary>
		/// 下载手机视频
		/// </summary>
		public ICommand NavMobileVideoDownLoadCommand { get; private set; }
		/// <summary>
		/// 下载手机音频
		/// </summary>
		public ICommand NavMobileAudioDownLoadCommand { get; private set; }
		/// <summary>
		/// 下载高清视频
		/// </summary>
		public ICommand NavHdVideoDownLoadCommand { get; private set; }
		/// <summary>
		/// 下载高清音频
		/// </summary>
		public ICommand NavHdAudioDownLoadCommand { get; private set; }

		/// <summary>
		/// 刷新本地数据命令
		/// </summary>
		public ICommand RefreshLocalCommand { get; set; }

		#endregion

		#region 属性

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

		#region 方法

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
					? string.Format("《{0}-{1}》 未开通!", item.CourseName, item.CWareClassName, " 未开通")
					: string.Format("《{0}-{1}》下载权限暂未开放。\r\n提示：开通课程满七天后自动开放下载权限，如果您开通课程已经满七天，请点击“更新列表”获取权限！",
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
            SystemInfo.StartBackGroundThread("加载移动数据", () => Dispatcher.Invoke(new Action(() =>
            {
                BindData();
                HideLoading();

            })));
		}
	}
}