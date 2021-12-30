using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using Framework.Import;
using Framework.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace DesktopApp.ViewModel
{
	/// <summary>
	/// 课程导入视图模型
	/// </summary>
	public class ImportViewModel : ViewModelBase
	{
		private readonly ImportZip _importZip;
		private int _index;
		private ObservableCollection<ImportItemViewModel> _items;

		//public event EventHandler ImportComplate;

		public ImportViewModel()
		{
			Items = new ObservableCollection<ImportItemViewModel>();
			ImportCommand = new RelayCommand(Import);
			StopImportCommand = new RelayCommand(() =>
			{
				if (Items != null)
				{
					for (int i = Items.Count - 1; i > 0; i--)
					{
						if (!Items[i].IsComplate && !Items[i].IsLoading)
						{
							Items.RemoveAt(i);
						}
					}
				}
			});
			_importZip = new ImportZip
			{
				ConfirmFun = msg => (bool)Application.Current.Dispatcher.Invoke(new Func<bool>(() => CustomMessageBox.Show(msg, "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)),
				MessageAction = (id, msg) => { Items.Single(i => i.Id == id).Message = msg; },
				StatusAction = (id, status) =>
				{
					try
					{
						Items.Single(i => i.Id == id).IsLoading = true;
						Items.Single(i => i.Id == id).Status = status;
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex);
					}
				},
				CompleteAction = id =>
				{
					var currentItem = Items.Single(i => i.Id == id);
					currentItem.IsLoading = false;
					currentItem.IsComplate = true;
					Messenger.Default.Send("", TokenManager.RefreshList);
					ImportNext();
				}
			};
		}

		#region 命令

		/// <summary>
		/// 导入按钮事件命令
		/// </summary>
		public ICommand ImportCommand { get; private set; }

		public ICommand StopImportCommand { get; private set; }

		#endregion

		public static bool IsImporting { get; private set; }

		public ObservableCollection<ImportItemViewModel> Items
		{
			get { return _items; }
			private set
			{
				_items = value;
				RaisePropertyChanged(() => Items);
			}
		}

		private void Import()
		{
			var size = SystemInfo.GetFolderFreeSpaceInMb(Util.VideoPath);
			if (size < 512)
			{
				CustomMessageBox.Show("您的指定的保存路径空间已不足，无法添加新任务。\r\n请更改保存路径。保存路径的最小可用空间要求512M");
				Messenger.Default.Send(string.Empty, TokenManager.ShowSetting);
				return;
			}
			if (!Util.IsOnline)
			{
				CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
				return;
			}
			App.Loc.DownloadCenter.StopDownloading();
			//DownloadManagerLogic.StopDownloading();
			//CustomMessageBox.Show("请选择您要导入视频的文件！");
			var ofd = new OpenFileDialog { Filter = "*.zip|*.zip", Multiselect = true };
			if (ofd.ShowDialog() == true)
			{
				var videoPath = Util.VideoPath;
				foreach (var filename in ofd.FileNames)
				{
					var item = new ImportItemViewModel
					{
						FileName = filename,
						VideoSavePath = videoPath,
					};
					Items.Add(item);
				}
				if (!IsImporting) ImportNext();
			}
		}

		internal void ImportNext()
		{
			for (_index = 0; _index < Items.Count; _index++)
			{
				if (!Items[_index].IsLoading && !Items[_index].IsComplate)
				{
					break;
				}
			}
			if (_index >= Items.Count)
			{
				IsImporting = false;
				Messenger.Default.Send(false, TokenManager.ImportState);
				Messenger.Default.Send(string.Empty, TokenManager.RefreshList);
				App.Loc.DownloadCenter.StartNext();
				return;
			}
			IsImporting = true;
			var current = Items[_index++];
			_importZip.ImportFileAsync(current.Id, current.FileName, current.VideoSavePath);
			Messenger.Default.Send(true, TokenManager.ImportState);
		}

		public override void Cleanup()
		{
			_index = 0;
			if (Items != null)
				Items.Clear();
		}

		public void CleanUpImported()
		{
			if (_items == null) return;
			var lst = _items.Where(x => x.IsComplate && (x.Status == "导入成功" || x.Status == "重复导入")).ToList();
			foreach (var item in lst)
			{
				_items.Remove(item);
			}
		}
	}
}
