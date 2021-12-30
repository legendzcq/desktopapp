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
using System.Diagnostics;
using Framework.Utility;

namespace DesktopApp.ViewModel
{
	/// <summary>
	/// 讲义下载视图模型
	/// </summary>
	public class KcjyDownViewModel : NavigationViewModelBase
	{
		public KcjyDownViewModel()
		{
			InitCmd();
            ShowLoading();
		}

		#region 命令

		/// <summary>
		/// 浏览讲义下载命令
		/// </summary>
		public ICommand NavKcjyDownLoadCommand { get; private set; }

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

			NavKcjyDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item =>
			{
				if (item.IsOpen)
				{
					NavigationService.Navigate(new Uri("/Pages/KcjyDetail.xaml", UriKind.Relative), item);
				}
				else
				{
					CustomMessageBox.Show(item.CanDownload
						? string.Format("《{0}-{1}》 未开通!", item.CourseName, item.CWareClassName, " 未开通")
						: string.Format("《{0}-{1}》下载权限暂未开放。\r\n提示：开通课程满七天后自动开放下载权限，如果您开通课程已经满七天，请点击“更新列表”获取权限！",
							item.CourseName, item.CWareClassName));
				}
			});

			RefreshLocalCommand = new RelayCommand(() => BindData(StudentWareLogic.GetStudentCourseWareList()));
		}

		private void BindData(List<ViewStudentCourseWare> list)
		{
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
                BindData(StudentWareLogic.GetStudentCourseWareList());
                HideLoading();
                return;
            }
            //var list = StudentWareLogic.GetStudentCourseWareList();
            //BindData(list);

            SystemInfo.StartBackGroundThread("加载讲义数据", () => Dispatcher.Invoke(new Action(() =>
            {
                BindData(StudentWareLogic.GetStudentCourseWareList());
                HideLoading();

            })));
         
           
		}
	}
}