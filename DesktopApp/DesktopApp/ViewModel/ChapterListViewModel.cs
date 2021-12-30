using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using DesktopApp.Pages;

using Framework.Local;
using Framework.Model;
using Framework.Utility;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 章节列表视图模型
    /// </summary>
    public class ChapterListViewModel : NavigationViewModelBase
    {
        private ViewStudentCourseWare _course;
        private List<ViewStudentWareDetail> _viewStudentWareDetails;

        public ChapterListViewModel()
        {
            InitCmd();
            // 从课程导入接收刷新消息
            Messenger.Default.Register<string>(this, TokenManager.RefreshList, s =>
            {
                if (_course != null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        _viewStudentWareDetails = StudentWareLogic.GetStudentWareDetail(_course.CwareId);
                        BindLocalData();
                    }));
                }
            });
        }

        #region 属性

        private ListCollectionView _chapterList;
        private string _pageTitle;
        private ListCollectionView _undownloadList;
        private ObservableCollection<ChapterSelectViewModel> _selectViewModels;
        private ObservableCollection<ChapterDetailViewModel> _detailViewModels;
        private bool? _chkAllIsChecked;
        private ChapterPageState _state;
        public ChapterPageState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);
            }
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public bool? ChkAllIsChecked
        {
            get => _chkAllIsChecked;
            set
            {
                _chkAllIsChecked = value;
                RaisePropertyChanged(() => ChkAllIsChecked);
            }
        }

        public ListCollectionView ChapterList
        {
            get => _chapterList;
            set
            {
                _chapterList = value;
                RaisePropertyChanged(() => ChapterList);
            }
        }

        public ListCollectionView UndownloadList
        {
            get => _undownloadList;
            set
            {
                _undownloadList = value;
                RaisePropertyChanged(() => UndownloadList);
            }
        }
        #endregion

        #region 命令

        /// <summary>
        /// 选择下载按钮事件命令
        /// </summary>
        public ICommand SelectDownloadCommand { get; private set; }
        /// <summary>
        /// 开始下载按钮事件命令
        /// </summary>
        public ICommand StartDownloadCommand { get; private set; }
        /// <summary>
        /// 开始下载某一个时间命令
        /// </summary>
        public ICommand StartDownloadOneCommand { get; private set; }
        /// <summary>
        /// 开始点播
        /// </summary>
        public ICommand StartVODCommand { get; private set; }
        /// <summary>
        /// 跳转看课命令
        /// </summary>
        public ICommand NavPlayPageCommand { get; private set; }
        /// <summary>
        /// 全选单击事件命令
        /// </summary>
        public ICommand ChkAllClickCommand { get; private set; }
        /// <summary>
        /// 子项的单击事件命令
        /// </summary>
        public ICommand ChkCommand { get; private set; }
        /// <summary>
        /// 删除课程事件命令
        /// </summary>
        public ICommand DelCommand { get; private set; }
        /// <summary>
        /// 重新下载命令
        /// </summary>
        public ICommand RedownloadCommand { get; private set; }
        /// <summary>
        /// 继续下载命令
        /// </summary>
        public ICommand ContinueDownloadCommand { get; private set; }

        /// <summary>
        /// 课件更新下载命令
        /// </summary>
        public ICommand UpdateDownloadCommand { get; private set; }
        /// <summary>
        /// 批量删除命令 dgh
        /// </summary>
        public ICommand MultDeleteCommand { get; private set; }
        /// <summary>
        /// 开始删除按钮事件命令 dgh
        /// </summary>
        public ICommand StartDeleteCommand { get; private set; }

        #endregion

        #region 方法

        private void InitCmd()
        {
            SelectDownloadCommand = new RelayCommand(() =>
            {
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    _selectViewModels = null;
                    return;
                }

                ChkAllIsChecked = false;
                BindUnSelectData();
                State = ChapterPageState.Select;
            });
            StartDownloadOneCommand = new RelayCommand<ChapterDetailViewModel>(item =>
            {
                if (!CheckCanDownload()) return;
                var noticeNext = true;
                var downNormal = false;

                Func<bool> funNotice = () =>
                {
                    //三分屏课程没有高清，直接下载标清课件
                    if (item.ViewStudentWare.VideoType == 2) return true;
                    if (noticeNext)
                    {
                        downNormal = CustomMessageBox.Show("当前班次《" + _course.CourseName + " " + _course.CWareClassName + " " + _course.CTeacherName + "》没有找到高清视频，是否下载标清视频？",
                            boxButton: MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                        noticeNext = false;
                    }
                    return downNormal;
                };
                item.VideoState = VideoState.Wait;
                item.DownId = App.Loc.DownloadCenter.AddToDownVideoTask(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId, Util.DownType == 1 ? 1 : 3, item.ViewStudentWare.ModTime, funNotice);
                App.Loc.DownloadCenter.BindData();
                App.Loc.DownloadCenter.StartNext();
            });
            StartVODCommand = new RelayCommand<ChapterDetailViewModel>(item =>
            {
                if (!CheckCanDownload()) return;
            });
            StartDownloadCommand = new RelayCommand(() =>
            {
                if (!CheckCanDownload()) return;
                if (_selectViewModels == null)
                    return;

                var list = _selectViewModels.Where(vm => vm.IsSelected && vm.IsCanSelect).ToList();
                if (!list.Any())
                {
                    //CustomMessageBox.Show("您未新增下载任务");
                    return;
                }

                var noticeNext = true;
                var downNormal = false;



                foreach (ChapterSelectViewModel item in list)
                {
                    ChapterSelectViewModel item1 = item;
                    Func<bool> funNotice = () =>
                    {
                        //三分屏课程没有高清，直接下载标清课件
                        if (item1.ViewStudentWare.VideoType == 2) return true;
                        if (noticeNext)
                        {
                            downNormal = CustomMessageBox.Show("当前班次《" + _course.CourseName + " " + _course.CWareClassName + " " + _course.CTeacherName + "》没有找到高清视频，是否下载标清视频？",
                                boxButton: MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                            noticeNext = false;
                        }
                        return downNormal;
                    };

                    ChapterDetailViewModel detailVm = _detailViewModels.FirstOrDefault(x => x.DownId == item.ViewStudentWare.DownId);
                    if (detailVm != null)
                    {
                        detailVm.VideoState = VideoState.Wait;// 等待下载
                    }
                    App.Loc.DownloadCenter.AddToDownloadTask(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId, Util.DownType == 1 ? 1 : 3, 1, item.ViewStudentWare.ModTime, funNotice);
                }
                App.Loc.DownloadCenter.BindData();
                App.Loc.DownloadCenter.StartNext();
                BindData();
                //Messenger.Default.Send(string.Empty, TokenManager.DownStart);
            });

            NavPlayPageCommand = new RelayCommand<ChapterDetailViewModel>(item =>
            {
                if (item != null && item.VideoState == VideoState.Done)
                {
                    ViewStudentWareDetail data = item.ViewStudentWare;
                    if (data != null)
                    {
                        var gotodown = false;
                        var error = string.Empty;
                        if (string.IsNullOrWhiteSpace(data.VideoPath))
                        {
                            gotodown = true;
                            error = "";
                        }
                        else if (!File.Exists(data.VideoPath))
                        {
                            gotodown = true;
                            error = "当前视频可能被删除，是否需要下载？";
                        }
                        if (gotodown)
                        {
                            if (error.Length > 0)
                            {
                                if (CustomMessageBox.Show(error, boxButton: MessageBoxButton.YesNo) !=
                                    MessageBoxResult.Yes)
                                {
                                    item.VideoState = VideoState.Undownload;
                                    item.BolUpdate = false;
                                    new StudentWareData().DeleteVideo(item.ViewStudentWare.DownId);
                                    App.Loc.DownloadCenter.CancelDownloadTaskForImport(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId);
                                    return;
                                }
                            }
                            new StudentWareData().DeleteVideo(item.ViewStudentWare.DownId);
                            App.Loc.DownloadCenter.CancelDownloadTaskForImport(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId);
                            var downId = App.Loc.DownloadCenter.AddToDownVideoTask(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId, Util.DownType == 1 ? 1 : 3);
                            if (downId > 0)
                            {
                                App.Loc.DownloadCenter.StartNext();
                                item.VideoState = 0;
                                return;
                            }
                            CustomMessageBox.Show("无法找到该课件的下载链接，请稍后重试!");
                        }
                        else
                        {
                            Window playWin;
                            if (item.ViewStudentWare.VideoType == 2)
                            {
                                playWin = new SFPlayWindow(PageTitle, item.ViewStudentWare, _course)
                                {
                                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                                };
                            }
                            else
                            {
                                //playWin = new PlayerWindow(new PlayerWindowViewModel()
                                //{
                                //    VideoItem = item.ViewStudentWare,
                                //    Course = _course
                                //});

                                playWin = new PlayWindow1(PageTitle, item.ViewStudentWare, _course);
                            }

                            App.CurrentMainWindow.ShowInTaskbar = false;
                            App.CurrentMainWindow.Hide();

                            playWin.ShowDialog();

                            App.CurrentMainWindow.ShowInTaskbar = true;
                            App.CurrentMainWindow.Show();
                        }
                    }
                }
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

            DelCommand = new RelayCommand<ChapterDetailViewModel>(vm =>
            {
                var msg = vm.VideoState == VideoState.Done ? "确定删除已下载的视频？" : "确定取消下载任务？";
                if (CustomMessageBox.Show(msg, "警告", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;
                new StudentWareData().DeleteVideo(vm.DownId);
                App.Loc.DownloadCenter.CancelDownloadTaskForImport(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId);
                vm.VideoState = VideoState.Undownload;
                vm.BolUpdate = false;
            });

            RedownloadCommand = new RelayCommand<ChapterDetailViewModel>(vm =>
            {
                new StudentWareData().DeleteVideo(vm.DownId);
                App.Loc.DownloadCenter.CancelDownloadTaskForImport(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId);
                vm.VideoState = VideoState.Wait;
                vm.DownId = App.Loc.DownloadCenter.AddToDownVideoTask(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId, Util.DownType == 1 ? 1 : 3, vm.ViewStudentWare.ModTime);
                App.Loc.DownloadCenter.StartNext();
            });
            ContinueDownloadCommand = new RelayCommand<ChapterDetailViewModel>(vm =>
            {
                vm.VideoState = VideoState.Wait;
                new StudentWareData().UpdateCwareDownloadState(vm.DownId, 0);
                App.Loc.DownloadCenter.StartNext();
            });
            UpdateDownloadCommand = new RelayCommand<ChapterDetailViewModel>(vm =>
            {
                var local = new StudentWareData();
                //删除旧课件(包括高清和标清)
                local.DeleteMultDonwload(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId);
                local.DeleteVideo(vm.DownId);
                vm.VideoState = VideoState.Wait;
                vm.BolUpdate = false;
                vm.DownId = App.Loc.DownloadCenter.AddToDownVideoTask(vm.ViewStudentWare.CwareId, vm.ViewStudentWare.VideoId, Util.DownType == 1 ? 1 : 3, vm.ViewStudentWare.ModTime);
                App.Loc.DownloadCenter.BindData();
                App.Loc.DownloadCenter.StartNext();
            });

            //批量删除 dgh
            MultDeleteCommand = new RelayCommand(() =>
            {
                ChkAllIsChecked = false;
                BindUnDeleteData();
                State = ChapterPageState.Delete;
            });

            //开始删除 dgh
            StartDeleteCommand = new RelayCommand(() =>
            {
                var list = _selectViewModels.Where(vm => vm.IsSelected && vm.IsCanSelect).ToList();
                if (!list.Any())
                {
                    //CustomMessageBox.Show("您没有下载的视频");
                    return;
                }
                if (CustomMessageBox.Show("您确定要删除这些已下载的视频吗？", boxButton: MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var local = new StudentWareData();
                    foreach (ChapterSelectViewModel item in list)
                    {
                        local.DeleteMultDonwload(item.ViewStudentWare.CwareId, item.ViewStudentWare.VideoId);
                        local.DeleteVideo(item.ViewStudentWare.DownId);
                        //item.ViewStudentWare.VideoState = -1;
                        ChapterDetailViewModel detailVm = _detailViewModels.FirstOrDefault(x => x.DownId == item.ViewStudentWare.DownId);
                        if (detailVm != null)
                        {
                            detailVm.VideoState = VideoState.Wait;// 等待下载
                        }
                    }
                    BindUnDeleteData();
                    App.Loc.DownloadCenter.BindData();
                    BindLocalData();
                    NavigationService.Navigate(new Uri("/Pages/ChapterPage.xaml", UriKind.Relative), _course);
                }


            });
        }

        //private void ShowKcjy(string chapterName)
        //{
        //	var jiangyiFile = _detailViewModels.First(x => x.ChapterName == chapterName).ViewStudentWare.JiangyiFile;
        //	if (string.IsNullOrEmpty(jiangyiFile))
        //	{
        //		CustomMessageBox.Show("本章讲义尚未提供");
        //		return;
        //	}
        //	var courseId = _course.CwareId;
        //	var path = Util.JiangyiSavePath + "\\" + courseId + "\\" + Path.GetFileName(jiangyiFile);
        //	var wordFile = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".doc";
        //	if (File.Exists(wordFile))
        //	{
        //		OpenWordFile(wordFile);
        //		return;
        //	}
        //	if (File.Exists(path))
        //	{
        //		ExtractRarFile(path);
        //		OpenWordFile(wordFile);
        //		return;
        //	}
        //	if (!Util.IsOnline)
        //	{
        //		CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
        //		return;
        //	}
        //	var url = "http://down.chnedu.com" + jiangyiFile;
        //	//加入下载任务;
        //	var sd = new Framework.Download.Downloader(url, path, 0);
        //	sd.DownloadComplate += id =>
        //	{
        //		//MessageBox.Show("讲义下载完毕");
        //		sd.Stop();
        //		var temp = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + Util.DownloadFileExtension;
        //		File.Move(temp, path);
        //		ExtractRarFile(path);
        //		OpenWordFile(wordFile);
        //	};
        //	sd.DownloadError += id => Dispatcher.Invoke(new Action(() => CustomMessageBox.Show("讲义下载失败，请重试")));
        //	sd.DownloadStarted += id => { };
        //	sd.DownloadProcess += (id, all, down) => Trace.WriteLine(string.Format("{0},{1},{2}", id, all, down));
        //	sd.Start();
        //}

        private bool CheckCanDownload()
        {
            if (Util.DownType == 0)
            {
                Util.DownType = CustomMessageBox.ShowSelectVideoType();
            }
            var size = SystemInfo.GetFolderFreeSpaceInMb(Util.VideoDownSavePath);
            if (size < 512)
            {
                CustomMessageBox.Show("您的指定的下载路径空间已不足，无法添加新任务。\r\n请更改下载路径。下载路径的最小可用空间要求512M");
                Messenger.Default.Send(string.Empty, TokenManager.ShowSetting);
                return false;
            }
            State = ChapterPageState.View;

            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return false;
            }
            return true;
        }

        private void BindData()
        {
            var cid = _course.CwareId;
            _viewStudentWareDetails = StudentWareLogic.GetStudentWareDetail(cid);

            if (_viewStudentWareDetails.Count == 0)
            {
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }
                StudentWareLogic.GetWareDetailFromRemote(_course.CwId, "1", re => Dispatcher.Invoke(new Action(() =>
                {
                    if (re.State)
                    {
                        _viewStudentWareDetails = StudentWareLogic.GetStudentWareDetail(cid);
                        BindLocalData();
                    }
                    else
                    {
                        CustomMessageBox.Show(re.Message);
                    }
                    HideLoading();
                })));
                ShowLoading();
            }
            else
            {
                BindLocalData();
                //每次打开。在线的话，就尝试更新一遍列表
                if (Util.IsOnline)
                {
                    StudentWareLogic.GetWareDetailFromRemote(_course.CwId, "1", re => Dispatcher.Invoke(new Action(() =>
                    {
                        if (re.State)
                        {
                            _viewStudentWareDetails = StudentWareLogic.GetStudentWareDetail(cid);
                            BindLocalData();
                        }
                    })));
                }
            }
            var isExistUpdate = _viewStudentWareDetails.Any(q => q.IsUpdate);
            if (isExistUpdate)
            {
                new AutoMessageBox().Show("已下载的课件有更新，敬请关注", "提示", 3000);
            }
        }

        private void BindLocalData()
        {
            _detailViewModels = new ObservableCollection<ChapterDetailViewModel>();
            _viewStudentWareDetails.ForEach(x =>
            {
                var vm = new ChapterDetailViewModel();
                vm.FromModel(x);
                _detailViewModels.Add(vm);
            });
            if (State == ChapterPageState.Select)
            {
                //// 所有开通章节均为未下载状态，切换至选择状态且全部选中
                var openList = _viewStudentWareDetails.Where(v => v.VideoState != (int)VideoState.NotOpen).ToList();
                if (openList.Any() && openList.All(v => v.VideoState == (int)VideoState.Undownload))
                {
                    BindUnSelectData();
                    ChkAllIsChecked = true;
                    ChkAllClickCommand.Execute(true);
                }
            }
            ChapterList = new ListCollectionView(_detailViewModels);
            if (ChapterList.GroupDescriptions != null)
                ChapterList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));
        }

        /// <summary>
        /// 绑定未下载列表
        /// </summary>
        private void BindUnSelectData()
        {
            _selectViewModels = new ObservableCollection<ChapterSelectViewModel>();
            _viewStudentWareDetails.ForEach(item => _selectViewModels.Add(new ChapterSelectViewModel(item)));

            UndownloadList = new ListCollectionView(_selectViewModels);
            if (UndownloadList.GroupDescriptions != null)
                UndownloadList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));
        }
        /// <summary>
        /// 绑定已完成列表 dgh
        /// </summary>
        private void BindUnDeleteData()
        {
            _selectViewModels = new ObservableCollection<ChapterSelectViewModel>();
            _viewStudentWareDetails.ForEach(item => _selectViewModels.Add(new ChapterSelectViewModel(item, 3)));

            UndownloadList = new ListCollectionView(_selectViewModels);
            if (UndownloadList.GroupDescriptions != null)
                UndownloadList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));
        }
        #region 下载事件处理

        private void DownloadProcessing(long downId, long fsize, long offset)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.DownId == downId);
            if (item != null)
            {
                var rate = (int)(offset / (double)fsize * 100);
                item.DownloadValue = rate;
            }
        }

        private void DownloadStarting(object sender, DownloadFileEventArgs e)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            if (item != null)
            {
                item.VideoState = VideoState.Downloading;// 开始下载
            }
        }

        private void DownloadComplete(object sender, DownloadFileEventArgs e)
        {
            //if (_detailViewModels == null) return;
            //var item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            //if (item != null)
            //{
            //	item.VideoState = VideoState.Done;// 下载完成
            //}
        }

        private void DownloadSpeed(long downId, int speed)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.DownId == downId);
            if (item != null)
            {
                item.Speed = speed + " KB/s";
            }
        }

        private void DownloadPaused(object sender, DownloadFileEventArgs e)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            if (item != null)
            {
                item.VideoState = VideoState.Pause;// 开始下载
            }
        }

        private void DownloadGoWaiting(object sender, DownloadFileEventArgs e)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            if (item != null)
            {
                item.VideoState = VideoState.Wait;// 开始下载
            }
        }

        private void DownloadCanceled(object sender, DownloadFileEventArgs e)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            if (item != null)
            {
                item.VideoState = VideoState.Undownload;
            }
        }

        private void DownloadError(object sender, DownloadFileEventArgs e)
        {
            if (_detailViewModels == null) return;
            ChapterDetailViewModel item = _detailViewModels.FirstOrDefault(x => x.ViewStudentWare.CwareId == e.DownloadItem.CwareId && x.ViewStudentWare.VideoId == e.DownloadItem.VideoId && e.DownloadItem.ForImport == 1);
            if (item != null)
            {
                item.VideoState = VideoState.Error;// 开始下载
            }
        }

        #endregion

        #endregion


        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (_detailViewModels != null) _detailViewModels.Clear();
            if (_viewStudentWareDetails != null) _viewStudentWareDetails.Clear();
            State = ChapterPageState.View;

            App.Loc.DownloadCenter.BindData();

            // 注册下载相关事件
            App.Loc.DownloadCenter.DownloadFileStarted += DownloadStarting;
            App.Loc.DownloadCenter.DownloadFileCanceled += DownloadCanceled;
            App.Loc.DownloadCenter.DownloadFilePaused += DownloadPaused;
            App.Loc.DownloadCenter.DownloadFileComplete += DownloadComplete;
            App.Loc.DownloadCenter.DownloadFileError += DownloadError;
            App.Loc.DownloadCenter.DownloadFileGoWaiting += DownloadGoWaiting;

            if (mode == NavigationMode.Forward)
            {
                BindData();
                return;
            }
            if (mode == NavigationMode.New)
            {
                var course = e.ExtraData as ViewStudentCourseWare;
                if (course == null)
                    return;

                _course = course;
                ChkAllIsChecked = false;
            }
            PageTitle = string.IsNullOrEmpty(_course.CourseWareName) ? string.Format("{0} {1}({2})", _course.CourseName, _course.CWareClassName, _course.CTeacherName) : _course.CourseWareName;
            BindData();
        }

        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            // 取消注册下载相关事件
            App.Loc.DownloadCenter.DownloadFileStarted -= DownloadStarting;
            App.Loc.DownloadCenter.DownloadFileCanceled -= DownloadCanceled;
            App.Loc.DownloadCenter.DownloadFilePaused -= DownloadPaused;
            App.Loc.DownloadCenter.DownloadFileGoWaiting -= DownloadGoWaiting;
            App.Loc.DownloadCenter.DownloadFileComplete -= DownloadComplete;
            App.Loc.DownloadCenter.DownloadFileError -= DownloadError;
            HideLoading();
        }
    }

    public enum ChapterPageState
    {
        View,
        Select,
        Delete
    }

    /// <summary>
    /// 课程视频下载状态
    /// </summary>
    public enum VideoState
    {
        /// <summary>
        /// 正在播放
        /// </summary>
        Playing = -3,

        /// <summary>
        /// 未开通下载
        /// </summary>
        NotOpen = -2,

        /// <summary>
        /// 未下载
        /// </summary>
        Undownload = -1,

        /// <summary>
        /// 等待中
        /// </summary>
        Wait = 0,

        /// <summary>
        /// 下载中
        /// </summary>
        Downloading = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 2,

        /// <summary>
        /// 已下载
        /// </summary>
        Done = 3,

        /// <summary>
        /// 失败
        /// </summary>
        Error = 4,
        /// <summary>
        /// 下载失败
        /// </summary>
        DownloadError = 5
    }
}
