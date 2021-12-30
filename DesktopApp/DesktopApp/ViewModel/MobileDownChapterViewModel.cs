using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Download;
using Framework.Local;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.ViewModel
{
	/// <summary>
	/// 
	/// </summary>
	public class MobileDownChapterViewModel : NavigationViewModelBase
	{

		private ViewStudentCourseWare _course;
		private int _downloadType;
		private List<ViewStudentWareDetail> _viewStudentWareDetails;

		public MobileDownChapterViewModel()
		{
			Messenger.Default.Register<string>(this, TokenManager.RefreshDownloadList, msg => BindLocalData());
			InitCmd();
		}

		#region ����

		private ListCollectionView _chapterList;
		private string _pageTitle;
		private ListCollectionView _undownloadList;
		private ObservableCollection<MobileDownSelectViewModel> _selectViewModels;
		private ObservableCollection<MobileDownDetailViewModel> _detailViewModels;
		private bool? _chkAllIsChecked;
		private ChapterPageState _state;

		public ChapterPageState State
		{
			get { return _state; }
			set
			{
				_state = value;
				RaisePropertyChanged(() => State);
			}
		}

		public string PageTitle
		{
			get { return _pageTitle; }
			set
			{
				_pageTitle = value;
				RaisePropertyChanged(() => PageTitle);
			}
		}

		public bool? ChkAllIsChecked
		{
			get { return _chkAllIsChecked; }
			set
			{
				_chkAllIsChecked = value;
				RaisePropertyChanged(() => ChkAllIsChecked);
			}
		}

		public ListCollectionView ChapterList
		{
			get { return _chapterList; }
			set
			{
				_chapterList = value;
				RaisePropertyChanged(() => ChapterList);
			}
		}

		public ListCollectionView UndownloadList
		{
			get { return _undownloadList; }
			set
			{
				_undownloadList = value;
				RaisePropertyChanged(() => UndownloadList);
			}
		}

		#endregion

		/// <summary>
		/// ѡ�����ذ�ť�¼�����
		/// </summary>
		public ICommand SelectDownloadCommand { get; private set; }
		/// <summary>
		/// ��ʼ���ذ�ť�¼�����
		/// </summary>
		public ICommand StartDownloadCommand { get; private set; }
		/// <summary>
		/// ��ʼ����ĳһ��ʱ������
		/// </summary>
		public ICommand StartDownloadOneCommand { get; private set; }
		/// <summary>
		/// ȫѡ�����¼�����
		/// </summary>
		public ICommand ChkAllClickCommand { get; private set; }
		/// <summary>
		/// ����ĵ����¼�����
		/// </summary>
		public ICommand ChkCommand { get; private set; }
		/// <summary>
		/// ɾ���γ��¼�����
		/// </summary>
		public ICommand DelCommand { get; private set; }
		/// <summary>
		/// ������������
		/// </summary>
		public ICommand RedownloadCommand { get; private set; }
		/// <summary>
		/// ������������
		/// </summary>
		public ICommand ContinueDownloadCommand { get; private set; }

		/// <summary>
		/// �鿴�ļ�����
		/// </summary>
		public ICommand NavFileCommand { get; private set; }

		private void InitCmd()
		{
			SelectDownloadCommand = new RelayCommand(() =>
			{
				if (!Util.IsOnline)
				{
					CustomMessageBox.Show("ϵͳ��⵽��û���������磬������������");
					_selectViewModels = null;
					return;
				}
				ChkAllIsChecked = false;
				BindUnSelectData();
				State = ChapterPageState.Select;
			});

			StartDownloadOneCommand = new RelayCommand<MobileDownDetailViewModel>(item =>
			{
				if (!CheckCanDownload()) return;
				item.VideoState = VideoState.Wait;
				item.DownId = App.Loc.DownloadCenter.AddToDownloadTask(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId, _downloadType, 0);
				App.Loc.DownloadCenter.BindData();
				App.Loc.DownloadCenter.StartNext();
			});
			StartDownloadCommand = new RelayCommand(() =>
			{
				if (!CheckCanDownload()) return;
				if (_selectViewModels == null)
					return;

				var list = _selectViewModels.Where(vm => vm.IsSelected && vm.IsCanSelect).ToList();
				if (!list.Any())
				{
					//CustomMessageBox.Show("��δ������������");
					return;
				}

				foreach (var item in list)
				{
					var detailVm = _detailViewModels.FirstOrDefault(x => x.DownId == item.ViewStudentWare.DownId);
					if (detailVm != null)
					{
						detailVm.VideoState = VideoState.Wait;// �ȴ�����
					}
					item.ViewStudentWare.DownId = App.Loc.DownloadCenter.AddToDownloadTask(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId, _downloadType, 0);
				}
				App.Loc.DownloadCenter.BindData();
				App.Loc.DownloadCenter.StartNext();
				BindLocalData();
				//Messenger.Default.Send("", TokenManager.DownStart);
			});

			ChkAllClickCommand = new RelayCommand<bool>(isChk =>
			{
				if (_selectViewModels != null)
				{
					_selectViewModels.Where(i => i.IsCanSelect).ToList().ForEach(s =>
					{
						s.IsSelected = isChk;
					});
				}
			});

			ChkCommand = new RelayCommand(() =>
			{
				var canDonwloads = _selectViewModels.Where(i => i.IsCanSelect).ToList();
				if (canDonwloads.Any(i => !i.IsSelected))
					ChkAllIsChecked = false;

				if (canDonwloads.All(i => i.IsSelected))
					ChkAllIsChecked = true;
			});

			DelCommand = new RelayCommand<MobileDownDetailViewModel>(vm =>
			{
				var msg = vm.VideoState == VideoState.Done ? "ȷ��ɾ�������ص���Ƶ����Ƶ����" : "ȷ��ȡ����������";
				if (CustomMessageBox.Show(msg, "����", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
					return;

				App.Loc.DownloadCenter.CancelDownloadTask(vm.DownId);
				vm.VideoState = VideoState.Undownload;
			});

			RedownloadCommand = new RelayCommand<MobileDownDetailViewModel>(vm =>
			{
				App.Loc.DownloadCenter.CancelDownloadTask(vm.DownId);
				vm.VideoState = VideoState.Wait;
				// ��ȡ�µ�DownId
				//todo ��ȡ������Ƶ����
				vm.DownId = App.Loc.DownloadCenter.AddToDownloadTask(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId, _downloadType, 0);
				App.Loc.DownloadCenter.StartNext();
			});

			ContinueDownloadCommand = new RelayCommand<MobileDownDetailViewModel>(vm =>
			{
				vm.VideoState = VideoState.Wait;
				new StudentWareData().UpdateCwareDownloadState(vm.DownId, 0);
				App.Loc.DownloadCenter.StartNext();
			});

			NavFileCommand = new RelayCommand<MobileDownDetailViewModel>(vm =>
			{
				if (vm.VideoState == VideoState.Done)
				{
					if (File.Exists(vm.ViewStudentWare.VideoPath))
					{
						var process = new Process
						{
							StartInfo =
							{
								FileName = "Explorer.exe",
								Arguments = "/select,\"" + vm.ViewStudentWare.VideoPath + "\""
							}
						};
						Trace.WriteLine(string.Format("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments));
						process.Start();
					}
					else
					{
						CustomMessageBox.Show("���ص��ļ������ڣ�����������");
					}
				}
			});
		}

		private bool CheckCanDownload()
		{
			if (Util.DownType == 0)
			{
				Util.DownType = CustomMessageBox.ShowSelectVideoType();
			}
			var size = SystemInfo.GetFolderFreeSpaceInMb(Util.VideoDownSavePath);
			if (size < 512)
			{
				CustomMessageBox.Show("����ָ��������·���ռ��Ѳ��㣬�޷����������\r\n���������·��������·������С���ÿռ�Ҫ��512M");
				Messenger.Default.Send(string.Empty, TokenManager.ShowSetting);
				return false;
			}
			State = ChapterPageState.View;

			if (!Util.IsOnline)
			{
				CustomMessageBox.Show("ϵͳ��⵽��û���������磬������������");
				return false;
			}
			return true;
		}

		private void BindData()
		{
			BindLocalData();
			if (_viewStudentWareDetails.Count == 0)
			{
				StudentWareLogic.GetWareDetailFromRemote(_course.CwId, "1", re =>
				{
					if (re.State)
					{
						Dispatcher.Invoke(new Action(BindLocalData));
					}
				});
			}
		}

		private void BindLocalData()
		{
			_viewStudentWareDetails = StudentWareLogic.GetMobileDownloadDetail(_course.CwareId, _downloadType).ToList();
			_detailViewModels = new ObservableCollection<MobileDownDetailViewModel>();
			_viewStudentWareDetails.ForEach(x =>
			{
				var vm = new MobileDownDetailViewModel();
				vm.FromModel(x);
				_detailViewModels.Add(vm);
			});

			// ���п�ͨ�½ھ�Ϊδ����״̬���л���ѡ��״̬��ȫ��ѡ��
			//var openList = _viewStudentWareDetails.Where(v => v.VideoState != (int)VideoState.NotOpen).ToList();
			//if (openList.Any() && openList.All(v => v.VideoState == (int)VideoState.Undownload))
			//{
			//	BindUnSelectData();
			//	State = ChapterPageState.Select;
			//	ChkAllIsChecked = true;
			//	ChkAllClickCommand.Execute(true);
			//}

			ChapterList = new ListCollectionView(_detailViewModels);
			if (ChapterList.GroupDescriptions != null)
				ChapterList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));

		}

		/// <summary>
		/// ��δ�����б�
		/// </summary>
		private void BindUnSelectData()
		{
			_selectViewModels = new ObservableCollection<MobileDownSelectViewModel>();
			_viewStudentWareDetails.ForEach(item => _selectViewModels.Add(new MobileDownSelectViewModel(item)));

			UndownloadList = new ListCollectionView(_selectViewModels);
			if (UndownloadList.GroupDescriptions != null)
				UndownloadList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));
		}

		#region �����¼�����

		private void DownloadStarting(object sender, DownloadFileEventArgs e)
		{
			if (_detailViewModels == null) return;
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.VideoState = VideoState.Downloading;// ��ʼ����
			}
		}

		private void DownloadComplete(object sender, DownloadFileEventArgs e)
		{
			if (_detailViewModels == null) return;
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.VideoState = VideoState.Done;// �������
				item.ViewStudentWare.VideoPath = e.DownloadItem.LocalFile;
			}
		}

		private void DownloadPaused(object sender, DownloadFileEventArgs e)
		{
			if (_detailViewModels == null) return;
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.Speed = string.Empty;
				item.VideoState = VideoState.Pause;
			}
		}

		private void DownloadGoWaiting(object sender, DownloadFileEventArgs e)
		{
			if (_detailViewModels == null) return;
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.Speed = string.Empty;
				item.VideoState = VideoState.Wait;
			}
		}

		private void DownloadCanceled(object sender, DownloadFileEventArgs e)
		{
			if (_detailViewModels == null) return;
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.Speed = string.Empty;
				item.VideoState = VideoState.Undownload;
			}
		}

		private void DownloadError(object sender, DownloadFileEventArgs e)
		{
			var item = _detailViewModels.FirstOrDefault(x => x.DownId == e.DownloadItem.DownId);
			if (item != null)
			{
				item.Speed = string.Empty;
				item.VideoState = VideoState.DownloadError;
			}
		}

		#endregion

		public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
		{
			State = ChapterPageState.View;
			// ע����������¼�

			App.Loc.DownloadCenter.BindData();

			App.Loc.DownloadCenter.DownloadFileStarted += DownloadStarting;
			App.Loc.DownloadCenter.DownloadFileCanceled += DownloadCanceled;
			App.Loc.DownloadCenter.DownloadFilePaused += DownloadPaused;
			App.Loc.DownloadCenter.DownloadFileGoWaiting += DownloadGoWaiting;
			App.Loc.DownloadCenter.DownloadFileComplete += DownloadComplete;
			App.Loc.DownloadCenter.DownloadFileError += DownloadError;

			if (mode == NavigationMode.Forward)
				return;
			if (mode == NavigationMode.New)
			{
				dynamic dy = e.ExtraData;

				ViewStudentCourseWare course = dy.Item;
				if (course == null)
					return;

				_course = course;
				_downloadType = dy.DownloadType;
				ChkAllIsChecked = false;
			}
			PageTitle = string.IsNullOrEmpty(_course.CourseWareName)
				? string.Format("{0} {1}({2})", _course.CourseName, _course.CWareClassName, _course.CTeacherName)
				: _course.CourseWareName;
			switch (_downloadType)
			{
				case 1:
					PageTitle += "  ����ƽ����Ƶ����";
					break;
				case 2:
					PageTitle += "  ������Ƶ����";
					break;
				case 3:
					PageTitle += "  �ֻ���Ƶ����";
					break;
				case 4:
					PageTitle += "  �ֻ���Ƶ����";
					break;
			}
			BindData();
		}

		public override void OnNavigateFrom(NavigatingCancelEventArgs e)
		{
			// ȡ��ע����������¼�
			App.Loc.DownloadCenter.DownloadFileStarted -= DownloadStarting;
			App.Loc.DownloadCenter.DownloadFileCanceled -= DownloadCanceled;
			App.Loc.DownloadCenter.DownloadFilePaused -= DownloadPaused;
			App.Loc.DownloadCenter.DownloadFileGoWaiting -= DownloadGoWaiting;
			App.Loc.DownloadCenter.DownloadFileComplete -= DownloadComplete;
			App.Loc.DownloadCenter.DownloadFileError -= DownloadError;
		}
	}
}