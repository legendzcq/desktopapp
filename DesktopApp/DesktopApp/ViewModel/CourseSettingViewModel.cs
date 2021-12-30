using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using Framework.Local;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.ViewModel
{
	public class CourseSettingViewModel : NavigationViewModelBase
	{
		private ObservableCollection<CourseSettingDetailViewModel> _courseSettingList;

		public CourseSettingViewModel()
		{
			InitCmd();
		}

		public ObservableCollection<CourseSettingDetailViewModel> CourseSettingList
		{
			get { return _courseSettingList; }
			set
			{
				_courseSettingList = value;
				RaisePropertyChanged(() => CourseSettingList);
			}
		}

		public ICommand MoveUpCommand { get; set; }

		public ICommand MoveDownCommand { get; set; }

		public ICommand MoveTopCommand { get; set; }

		public ICommand SaveSettingCommand { get; set; }

		private void InitCmd()
		{
			MoveUpCommand = new RelayCommand<CourseSettingDetailViewModel>(item =>
			{
				var index = CourseSettingList.IndexOf(item);
				if (index == 0) return;
				CourseSettingList.Remove(item);
				CourseSettingList.Insert(index - 1, item);
			});
			MoveDownCommand = new RelayCommand<CourseSettingDetailViewModel>(item =>
			{
				var index = CourseSettingList.IndexOf(item);
				if (index == CourseSettingList.Count - 1) return;
				CourseSettingList.Remove(item);
				CourseSettingList.Insert(index + 1, item);
			});
			MoveTopCommand = new RelayCommand<CourseSettingDetailViewModel>(item =>
			{
				var index = CourseSettingList.IndexOf(item);
				if (index == 0) return;
				CourseSettingList.Remove(item);
				CourseSettingList.Insert(0, item);
			});

			SaveSettingCommand = new RelayCommand(() =>
			{
				if (CourseSettingList.Count(x => x.IsShow) == 0)
				{
					CustomMessageBox.Show("每个科目至少要有一个显示的班次");
					return;
				}
				var idx = 0;
				var lst = CourseSettingList.Select(x =>
				{
					x.DataItem.UserShowOrder = idx++;
					return x.DataItem;
				});
				new StudentWareData().UpdateStudentCwareSetting(lst);
				CustomMessageBox.Show("保存成功");
                App.CurrentCustomWindow.DialogResult = true;
				Messenger.Default.Send(string.Empty, TokenManager.CloseCustomWindow);
			});
		}

		public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
		{
			var subjectName = e.ExtraData as string;
			if (string.IsNullOrEmpty(subjectName)) return;
			var list = new StudentWareData().GetCwareSetting(subjectName).Select(x => new CourseSettingDetailViewModel(x));
			CourseSettingList = new ObservableCollection<CourseSettingDetailViewModel>(list);
		}
	}
}
