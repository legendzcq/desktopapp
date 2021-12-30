using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using Framework.Download;
using Framework.Local;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.ViewModel
{
    public class DownloadCenterViewModel : NavigationViewModelBase
    {
        public DownloadCenterViewModel()
        {
            Items = new ObservableCollection<DownloadItemViewModel>();
            Items.CollectionChanged += (s, e) => { DownloadCount = Items.Count; };
            InitCmd();
            Util.DownloadTaskCountChanged += (s, e) =>
            {
                if (Util.DownloadTaskCount < _downLoaderList.Count)
                {
                    for (int i = _downLoaderList.Count - 1; i >= Util.DownloadTaskCount; i--)
                    {
                        var downloader = _downLoaderList[i];
                        downloader.Stop();
                        downloader.IsOver = true;
                        _taskCountChangeDownId.Add(downloader.DownId);
                        _downLoaderList.Remove(downloader);
                    }
                }
                StartNext();
            };
        }

        #region 属性

        //当下载任务数量更改时，需要暂停下载的DownId
        private readonly List<long> _taskCountChangeDownId = new List<long>();
        private ObservableCollection<DownloadItemViewModel> _items;
        private int _downloadCount;

        public ObservableCollection<DownloadItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }
        /// <summary>
        /// 下载任务数
        /// </summary>
        public int DownloadCount
        {
            get { return _downloadCount; }
            set
            {
                _downloadCount = value;
                RaisePropertyChanged(() => DownloadCount);
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 开始下载任务命令
        /// </summary>
        public ICommand StartCommand { get; private set; }
        /// <summary>
        /// 暂停下载任务命令
        /// </summary>
        public ICommand PauseCommand { get; private set; }
        /// <summary>
        /// 删除下载任务命令
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
        /// 刷新列表命令
        /// </summary>
        public ICommand RefreshCommand { get; private set; }

        #endregion

        #region 方法

        private void InitCmd()
        {
            StartCommand = new RelayCommand(() =>
            {
                isShow = false;
                var delList = Items.Where(vm => vm.IsSelected).ToList();
                foreach (var item in delList)
                {
                    if (item.VideoState != VideoState.Downloading && item.VideoState != VideoState.Wait)
                    {
                        new StudentWareData().UpdateCwareDownloadState(item.DownId, 0);
                        item.VideoState = VideoState.Wait;
                    }
                    item.IsSelected = false;
                    System.Windows.Forms.Application.DoEvents();
                }
                StartNext();
                Messenger.Default.Send("", TokenManager.RefreshDownloadList);
            });

            PauseCommand = new RelayCommand(() =>
            {
                isShow = false;
                if (!CheckFreeDownload()) return;
                var delList = Items.Where(vm => vm.IsSelected).ToList();
                for (var i = delList.Count - 1; i >= 0; i--)
                {
                    var item = delList[i];
                    if (item.VideoState == VideoState.Downloading || item.VideoState == VideoState.Wait)
                    {
                        PauseDownloadTask(item.DownId);
                        item.VideoState = VideoState.Pause;
                    }
                    item.IsSelected = false;
                    System.Windows.Forms.Application.DoEvents();
                }
                Messenger.Default.Send("", TokenManager.RefreshDownloadList);
            });

            DelCommand = new RelayCommand(() =>
            {
                if (CustomMessageBox.Show("确定取消下载任务？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

                var delList = Items.Where(vm => vm.IsSelected).ToList();
                for (var i = delList.Count - 1; i >= 0; i--)
                {
                    var item = delList[i];
                    CancelDownloadTask(item.DownId);
                    Dispatcher.BeginInvoke(new Action(() => Items.Remove(item)));
                    //if (DownloadFileCanceled != null) DownloadFileCanceled(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
                    System.Windows.Forms.Application.DoEvents();
                }
                Messenger.Default.Send("", TokenManager.RefreshDownloadList);
                Messenger.Default.Send("", TokenManager.RefreshList);
            });

            RedownloadCommand = new RelayCommand<DownloadItemViewModel>(vm =>
            {
                CancelDownloadTask(vm.DownId);
                vm.VideoState = VideoState.Wait;
                vm.DownId = AddToDownloadTask(vm.ViewStudentCwareDown.CwareId, vm.ViewStudentCwareDown.VideoId, vm.ViewStudentCwareDown.DownType, vm.ViewStudentCwareDown.ForImport);
                StartNext();
                Messenger.Default.Send("", TokenManager.RefreshDownloadList);
            });

            ContinueDownloadCommand = new RelayCommand<DownloadItemViewModel>(vm =>
            {
                vm.VideoState = VideoState.Wait;
                new StudentWareData().UpdateCwareDownloadState(vm.DownId, 0);
                StartNext();
                Messenger.Default.Send("", TokenManager.RefreshDownloadList);
            });

            RefreshCommand = new RelayCommand(BindData);
        }

        public void BindData()
        {
            var datas = new StudentWareData().GetCwareDownloadList();
            Items.Clear();
            foreach (var viewStudentCwareDown in datas)
            {
                var vm = new DownloadItemViewModel();
                vm.FromModel(viewStudentCwareDown);
                Items.Add(vm);
            }
        }



        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            //注册下载相关事件
            //RegistEvent();
        }

        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            //取消注册下载相关事件
            //UnRegistEvnet();
        }

        #endregion

        #region 事件

        public event DownloadFileEventHandler DownloadFileStarted;
        public event DownloadFileEventHandler DownloadFileComplete;
        public event DownloadFileEventHandler DownloadFileCanceled;
        public event DownloadFileEventHandler DownloadFileError;
        public event DownloadFileEventHandler DownloadFilePaused;
        public event DownloadFileEventHandler DownloadFileGoWaiting;

        #endregion

        #region 下载事件处理

        private void DownloadProcessing(object sender, DownloadProcessEventArgs e)
        {
            if (_items == null) return;
            var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
            if (item != null)
            {
                var rate = e.Offset / (double)e.FileSize * 100;
                item.DownloadValue = rate;
                item.DownloadValueStr = string.Format("{0}M/{1}M ({2}%)", (e.Offset / 1024.0 / 1024.0).ToString("F1"), (e.FileSize / 1024.0 / 1024.0).ToString("F1"), rate.ToString("F1"));
                if (e.DownSpeed > 1024)
                {
                    item.Speed = ((double)e.DownSpeed / 1024).ToString("0.00") + " MB/s";
                }
                else
                {
                    item.Speed = e.DownSpeed + " KB/s";
                }
                item.VideoState = VideoState.Downloading;
            }
        }

        private void DownloadStarted(object sender, DownloadEventArgs e)
        {
            try
            {
                new StudentWareData().UpdateCwareDownloadState(e.DownId, 1);
                if (_items == null) return;
                var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
                if (item != null)
                {
                    item.VideoState = VideoState.Downloading; // 开始下载
                    if (DownloadFileStarted != null) DownloadFileStarted(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private void DownloadComplate(object sender, DownloadComplateEventArgs e)
        {
            if (_items == null) return;
            var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
            if (item != null)
            {
                item.VideoState = VideoState.Done;
                item.ViewStudentCwareDown.LocalFile = e.LocalFile;
                if (Dispatcher != null)
                {
                    Dispatcher.BeginInvoke(new Action(() => Items.Remove(item)));
                }
                else
                {
                    Items.Remove(item);
                }

                if (DownloadFileComplete != null) DownloadFileComplete(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
            }
        }

        private void DownloadPaused(object sender, DownloadEventArgs e)
        {
            if (_taskCountChangeDownId.Contains(e.DownId))
            {
                _taskCountChangeDownId.Remove(e.DownId);
                if (_items == null) return;
                var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
                if (item != null)
                {
                    item.VideoState = VideoState.Wait;
                    item.Speed = string.Empty;
                    if (DownloadFileGoWaiting != null) DownloadFileGoWaiting(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
                }
            }
            else
            {
                new StudentWareData().UpdateCwareDownloadState(e.DownId, 2);
                if (_items == null) return;
                var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
                if (item != null)
                {
                    if (item.VideoState != VideoState.Pause) item.VideoState = VideoState.Wait;
                    item.Speed = string.Empty;
                    if (DownloadFilePaused != null) DownloadFilePaused(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
                }
            }
        }

        private void DownloadCanceled(object sender, DownloadEventArgs e)
        {
            RemoveDownloadError(e.DownId);
            if (_items == null) return;
            var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
            if (item != null)
            {
                item.Speed = string.Empty;
                item.VideoState = VideoState.Undownload;
                if (DownloadFileCanceled != null) DownloadFileCanceled(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
            }
        }

        private void DownloadCheckFileError(object sender, DownloadEventArgs e)
        {
            int ecnt = SetDownloadError(e.DownId);
            new StudentWareData().UpdateCwareDownloadState(e.DownId, ecnt < 30 ? 0 : 4);
            new StudentWareData().MoveDownloadToLast(e.DownId);
            var downloader = _downLoaderList.FirstOrDefault(x => x.DownId == e.DownId);
            if (downloader != null)
            {
                _downLoaderList.Remove(downloader);
            }
            if (_items == null) return;
            var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
            if (item != null)
            {
                item.Speed = string.Empty;
                item.VideoState = ecnt < 30 ? VideoState.Wait : VideoState.Error;
                if (ecnt >= 30) if (DownloadFileError != null) DownloadFileError(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
            }
            StartNext();
        }

        private void DownloadError(object sender, DownloadEventArgs e)
        {
            int ecnt = AddDownloadError(e.DownId);
            new StudentWareData().UpdateCwareDownloadState(e.DownId, ecnt < 30 ? 0 : 4);
            new StudentWareData().MoveDownloadToLast(e.DownId);
            var downloader = _downLoaderList.FirstOrDefault(x => x.DownId == e.DownId);
            if (downloader != null)
            {
                _downLoaderList.Remove(downloader);
            }
            if (_items == null) return;
            var item = _items.FirstOrDefault(x => x.DownId == e.DownId);
            if (item != null)
            {
                item.Speed = string.Empty;
                item.VideoState = ecnt < 30 ? VideoState.Wait : VideoState.DownloadError;
                if (ecnt >= 30) if (DownloadFileError != null) DownloadFileError(this, new DownloadFileEventArgs(item.ViewStudentCwareDown));
            }
            StartNext();
        }

        #endregion

        #region 下载管理

        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object LockObject = new object();
        /// <summary>
        /// 下载器列表
        /// </summary>
        private readonly IList<IDownloader> _downLoaderList = new List<IDownloader>();

        private readonly Dictionary<long, int> _downloadErrorList = new Dictionary<long, int>();
        /// <summary>
        /// 用来控制提示显示一次
        /// </summary>
        private bool isShow;
        /// <summary>
        /// 是否正在下载
        /// </summary>
        public bool IsDownLoading
        {
            get
            {
                return _downLoaderList.Count > 0;
            }
        }

        /// <summary>
        /// 获取下载类型目录
        /// </summary>
        /// <param name="downType"></param>
        /// <returns></returns>
        private string GetDownTypeDir(int downType)
        {
            switch (downType)
            {
                case 1:
                    return "高清";
                case 2:
                    return "高清音频";
                case 3:
                    return "手机";
                case 4:
                    return "手机音频";
            }
            return string.Empty;
        }

        public long AddToDownVideoTask(int cwareId, string videoId, int downType,string modTime = "",
            Func<bool> confirm = null)
        {
            AddToDownloadTask(cwareId, videoId, downType, 1, modTime,confirm);
            return new StudentWareData().GetCwareDownId(cwareId, videoId);
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public long AddToDownloadTask(int cwareId, string videoId, int downType, int forImport, string modTime="",Func<bool> confirm = null)
        {
            var local = new StudentWareData();
            var vitem = local.GetStudentCWareDetailItem(cwareId, videoId);
            var downUrl = string.Empty;
            switch (downType)
            {
                case 1:
                    downUrl = vitem.VideoHdZipUrl;
                    break;
                case 2:
                    downUrl = vitem.AudioZipUrl;
                    break;
                case 3:
                    downUrl = vitem.VideoZipUrl;
                    break;
                case 4:
                    downUrl = vitem.AudioZipUrl;
                    break;
            }
            if (string.IsNullOrEmpty(downUrl))
            {
                if (forImport == 1 && downType == 1)
                {
                    if (confirm != null && confirm())
                    {
                        downUrl = vitem.VideoZipUrl;
                        downType = 3;
                        if (string.IsNullOrEmpty(downUrl))
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            var downloadItem = local.GetCwareDownloadItem(cwareId, videoId, downType);
            long id = 0;
            if (downloadItem != null && !File.Exists(downloadItem.LocalFile))
            {
                local.CancelDonwload(downloadItem.DownId);
                downloadItem = null;
            }
            if (downloadItem == null)
            {
                var downItem = new StudentCwareDownload
                {
                    CwareId = cwareId,
                    DownState = 0,
                    DownType = downType,
                    DownUrl = downUrl,
                    ForImport = forImport,
                    LocalFile = string.Empty,
                    VideoId = videoId,
                    ModTime=modTime,
                };
                id = local.AddCwareDownload(downItem);
                if (id > 0)
                {
                    var tmpFile = string.Format("{0}\\{1}\\{2}\\{3}{4}", Util.VideoDownSavePath, cwareId, GetDownTypeDir(downType),
                        Path.GetFileName(downUrl), Util.DownloadFileExtension);
                    SystemInfo.TryDeleteFile(tmpFile);
                    var tmpCfgFile = string.Format("{0}\\{1}\\{2}\\{3}{4}", Util.VideoDownSavePath, cwareId, GetDownTypeDir(downType),
                        Path.GetFileName(downUrl), Util.DownloadConfigExtension);
                    SystemInfo.TryDeleteFile(tmpCfgFile);
                }
            }
            if (forImport == 1)
            {
                if (downloadItem != null)
                {
                    if (downloadItem.DownState == 3 && File.Exists(downloadItem.LocalFile))
                    {
                        DoImport(downloadItem.LocalFile);
                        local.UpdateCwareDownloadModTime(downloadItem.DownId, modTime);
                        return id;
                    }
                    local.SetDownloadForImport(downloadItem.DownId);
                }
                var item = new StudentCWareDown
                {
                    CwareId = cwareId,
                    VideoId = videoId,
                    LocalFile = string.Empty,
                    State = 0,
                    Url = string.Empty,
                    Rate = 0.0,
                    ModTime=modTime,
                };
                local.AddCwareDown(item);
            }
            return id;
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        /// <param name="downId"></param>
        /// <returns></returns>
        public void CancelDownloadTask(long downId)
        {
            bool isDownloading = false;
            var downloader = _downLoaderList.FirstOrDefault(x => x.DownId == downId);
            if (downloader != null && !downloader.IsOver)
            {
                downloader.Cancel();
                downloader.IsOver = true;
                _downLoaderList.Remove(downloader);
                isDownloading = true;
            }
            new StudentWareData().CancelDonwload(downId);
            //OnDownloadCanceled(null, new DownloadEventArgs(downId));
            if (isDownloading) StartNext();
        }

        public void CancelDownloadTaskForImport(int cwareId, string videoId)
        {
            var item = Items.FirstOrDefault(x => x.ViewStudentCwareDown.CwareId == cwareId && x.ViewStudentCwareDown.VideoId == videoId && x.ViewStudentCwareDown.ForImport == 1);
            if (item != null)
            {
                CancelDownloadTask(item.DownId);
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        /// <param name="downId"></param>
        /// <returns></returns>
        private void PauseDownloadTask(long downId)
        {
            var downloader = _downLoaderList.FirstOrDefault(x => x.DownId == downId);
            if (downloader != null && !downloader.IsOver)
            {
                downloader.Stop();
                downloader.IsOver = true;
                _downLoaderList.Remove(downloader);
            }
            new StudentWareData().UpdateCwareDownloadState(downId, 2);
            StartNext();
        }

        /// <summary>
        /// 停止所有的下载
        /// </summary>
        public void StopDownloading()
        {
            foreach (var downloader in _downLoaderList)
            {
                downloader.Stop();
                downloader.IsOver = true;
            }
            _downLoaderList.Clear();
        }

        private IDownloader CreateDownloader(ViewStudentCwareDownLoad item)
        {
            if (item.LocalFile.Length == 0)
            {
                string fileName = Path.GetFileName(item.DownUrl);
                item.LocalFile = string.Format("{0}\\{1}\\{2}\\{3}", Util.VideoDownSavePath, item.CwareId, GetDownTypeDir(item.DownType), fileName);
                new StudentWareData().BeginCwareDownload(item.DownId, item.LocalFile);
            }
            //var downloader = new MultiBlockDownloader(item.DownUrl, item.LocalFile, item.DownId);
            //////改成带参数的下载路径 dgh 2016.04.26
            var downloader = new MultiBlockDownloader(item.ParmDownUrl, item.LocalFile, item.DownId);
            downloader.DownloadStarted += DownloadStarted;
            downloader.DownloadProcess += DownloadProcessing;
            downloader.DownloadPaused += DownloadPaused;
            downloader.DownloadError += DownloadError;
            downloader.DownloadComplate += OnDownloadComplate;
            downloader.DownloadCanceled += DownloadCanceled;
            downloader.DownloadCheckFileError += DownloadCheckFileError;
            downloader.InitDownloader();
            return downloader;
        }

        void OnDownloadComplate(object sender, DownloadComplateEventArgs e)
        {
            RemoveDownloadError(e.DownId);
            lock (LockObject)
            {
                new StudentWareData().UpdateCwareDownloadState(e.DownId, 3);
                var item = _downLoaderList.FirstOrDefault(x => x.DownId == e.DownId);
                if (item != null)
                {
                    _downLoaderList.Remove(item);
                }
                var ditem = new StudentWareData().GetCwareDownloadItem(e.DownId);
                if (ditem.ForImport == 1)
                {
                    DoImport(e.LocalFile);
                }
                DownloadComplate(sender, e);
            }
            StartNext();
        }

        void DoImport(string localFile)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var impitem = new ImportItemViewModel
                {
                    FileName = localFile,
                    VideoSavePath = Util.VideoPath,
                };
                App.Loc.Import.Items.Add(impitem);
                App.Loc.Import.ImportNext();
            }));
        }

        int AddDownloadError(long downId)
        {
            if (_downloadErrorList.ContainsKey(downId))
            {
                _downloadErrorList[downId]++;
                return _downloadErrorList[downId];
            }
            _downloadErrorList.Add(downId, 1);
            return 1;
        }

        int SetDownloadError(long downId)
        {
            if (_downloadErrorList.ContainsKey(downId))
            {
                _downloadErrorList[downId] = 30;
                return 30;
            }
            _downloadErrorList.Add(downId, 30);
            return 30;
        }

        void RemoveDownloadError(long downId)
        {
            if (_downloadErrorList.ContainsKey(downId))
            {
                _downloadErrorList.Remove(downId);
            }
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartNext()
        {
            if (!Util.IsOnline) return;
            #region 判断下载文件夹的空间是否足够
            if (!CheckFreeDownload()) return;
            #endregion
          
            if (_downLoaderList.Count >= Util.DownloadTaskCount) return;
            lock (LockObject)
            {
                var downlist = new StudentWareData().GetCwareNeedDownloadList();
                var item = downlist.FirstOrDefault(x => x.DownState < 2 && _downLoaderList.All(y => y.DownId != x.DownId));
                if (item != null)
                {
                    var downloader = CreateDownloader(item);
                    _downLoaderList.Add(downloader);
                    downloader.Start();
                }
                else
                {
                    Messenger.Default.Send(_downLoaderList.Count > 0, TokenManager.DownloadState);
                    return;
                }
            }
            isShow = false;
            StartNext();
        }
        #endregion
        /// <summary>
        /// 判断下载文件夹的空间是否足够 dgh
        /// </summary>
        /// <returns></returns>
        private bool CheckFreeDownload()
        {
            var size = SystemInfo.GetFolderFreeSpaceInMb(Util.VideoDownSavePath);
            if (size < 512)
            {
                StopDownloading();//停止所有下载
                if (!isShow)
                {
                    if (Dispatcher != null) Dispatcher.Invoke(new Action(() =>
                    {
                        CustomMessageBox.Show("您的指定的下载路径空间已不足，无法添加新任务。\r\n请更改下载路径。下载路径的最小可用空间要求512M");
                        Messenger.Default.Send(string.Empty, TokenManager.ShowSetting);
                    }
                      ));
                    else
                    {
                        MessageBox.Show("您的指定的下载路径空间已不足，无法添加新任务。\r\n请更改下载路径。下载路径的最小可用空间要求512M", "提示");
                    }
                }
                isShow = true;
                return false;
            }
            return true;
        }
    }

    public class DownloadFileEventArgs : EventArgs
    {
        public ViewStudentCwareDownLoad DownloadItem { get; private set; }

        public DownloadFileEventArgs(ViewStudentCwareDownLoad item)
        {
            DownloadItem = item;
        }
    }

    public delegate void DownloadFileEventHandler(object sender, DownloadFileEventArgs e);
}
