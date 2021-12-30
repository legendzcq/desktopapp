using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Framework.Utility;

namespace Framework.Download
{
    public class MultiBlockDownloader : IDownloader
    {
        /// <summary>
        /// 任务块大小
        /// </summary>
        public static int MinBlockSize = 1024 * 64;

        /// <summary>
        /// 下载块大小
        /// </summary>
        public static int PackSize = 4096;

        /// <summary>
        /// 镜像服务器列表
        /// </summary>
        private static string[] _serverList;

        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object LockObj = new object();

        /// <summary>
        /// 任务块数量
        /// </summary>
        private int BlockCount { get; set; }

        /// <summary>
        /// 要下载的文件路径
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 最终存放的本地路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 下载编号
        /// </summary>
        public long DownId { get; set; }

        /// <summary>
        /// 是否已下载完成
        /// </summary>
        public bool IsOver { get; set; }

        /// <summary>
        /// 是否采用镜像下载
        /// </summary>
        public bool UseMirrorDown { get; set; }

        /// <summary>
        /// 下载缓存
        /// </summary>
        private DownloadBuffer _downBuffer;

        /// <summary>
        /// 下载临时文件
        /// </summary>
        private string _tempFile;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MultiBlockDownloader(string fileUrl, string filePath, long downId)
        {
            UseMirrorDown = Util.IsUseMirrorDown;
            FileUrl = fileUrl;
            FilePath = filePath;
            DownId = downId;
            BlockCount = Util.DownloadThreadCount;
        }

        public void InitDownloader()
        {
            var path = Path.GetDirectoryName(FilePath);
            var rootPath = Path.GetPathRoot(FilePath);
            if (rootPath != null && !Directory.Exists(rootPath))
            {
                _isError = true;
            }
            else
            {
                if (path != null && !Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch
                    {
                        _isError = true;
                    }
                }
            }
            var fileTrueName = Path.GetFileName(FilePath);
            _tempFile = path + "\\" + fileTrueName + Util.DownloadFileExtension;
            var configFile = path + "\\" + fileTrueName + Util.DownloadConfigExtension;
            if (!File.Exists(_tempFile) && File.Exists(configFile))
            {
                SystemInfo.TryDeleteFile(configFile);
            }
            _downBuffer = new DownloadBuffer(_tempFile, configFile);
        }

        /// <summary>
        /// 是否已强制退出
        /// </summary>
        private bool _isForceExit;

        /// <summary>
        /// 是否已取消下载
        /// </summary>
        private bool _isCanceled;

        /// <summary>
        /// 是否有错误
        /// </summary>
        private bool _isError;

        /// <summary>
        /// 下载进度事件
        /// </summary>
        public event DownloadProcessEventHandler DownloadProcess;

        /// <summary>
        /// 下载进度
        /// </summary>
        private void OnProcess(long fileSize, long offset, long downSpeed)
        {
            DownloadProcessEventHandler handler = DownloadProcess;
            if (handler != null) handler(this, new DownloadProcessEventArgs(DownId, fileSize, offset, downSpeed));
        }

        /// <summary>
        /// 下载开始事件
        /// </summary>
        public event DownloadStartedEventHandler DownloadStarted;

        private void OnStarted()
        {
            Trace.WriteLine("Download: " + DownId + "  " + FileUrl);
            DownloadStartedEventHandler handler = DownloadStarted;
            if (handler != null) handler(this, new DownloadEventArgs(DownId));
        }

        /// <summary>
        /// 下载暂停事件
        /// </summary>
        public event DownloadPausedEventHandler DownloadPaused;

        private void OnPaused()
        {
            DownloadPausedEventHandler handler = DownloadPaused;
            IsOver = true;
            if (handler != null) handler(this, new DownloadEventArgs(DownId));
        }

        /// <summary>
        /// 下载被取消的事件
        /// </summary>
        public event DownloadCanceledEventHandler DownloadCanceled;

        private void OnCanceled()
        {
            DownloadCanceledEventHandler handler = DownloadCanceled;
            IsOver = true;
            if (handler != null) handler(this, new DownloadEventArgs(DownId));
        }

        /// <summary>
        /// 下载完成事件
        /// </summary>
        public event DownloadComplateEventHandler DownloadComplate;

        public event DownloadFileErrorEventHandler DownloadCheckFileError;

        private void OnDownloadCheckFileError()
        {
            DownloadFileErrorEventHandler handler = DownloadCheckFileError;
            if (handler != null) handler(this, new DownloadEventArgs(DownId));
        }

        private void OnDownloadComplate()
        {
            _downBuffer.DeleteConfigFile();

            if (File.Exists(_tempFile)) File.Move(_tempFile, FilePath);
            IsOver = true;
            //todo 对文件做检查，检查完毕后再触发下载完成
            if (File.Exists(FilePath))
            {
                var check = SystemInfo.CheckZipFile(FilePath);
                if (check)
                {
                    DownloadComplateEventHandler handler = DownloadComplate;
                    if (handler != null) handler(this, new DownloadComplateEventArgs(DownId, FilePath));
                }
                else
                {
                    Trace.WriteLine("方法CheckZipFile为false:下载的zip包检测有问题");
                    SystemInfo.TryDeleteFile(FilePath);
                    OnDownloadCheckFileError();
                    Trace.WriteLine("方法：OnDownloadCheckFileError通过");
                }
            }
        }

        /// <summary>
        /// 下载出错事件
        /// </summary>
        public event DownloadErrorEventHandler DownloadError;

        private void OnDownloadError()
        {
            DownloadErrorEventHandler handler = DownloadError;
            //Trace.WriteLine("DownloadError: " + downId);
            IsOver = true;
            if (handler != null) handler(this, new DownloadEventArgs(DownId));
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            if (_isError)
            {
                OnDownloadCheckFileError();
                return;
            }
            var speedByteCount = 0;
            var allByteCount = 0L;
            long fileSize = 0;
            var isComplate = false;
            _isForceExit = false;
            _isCanceled = false;
            _isError = false;
            var taskSpeed = new Task(() =>
            {
                while (!_isForceExit && !isComplate)
                {
                    var skb = speedByteCount / 1024;
                    OnProcess(fileSize, allByteCount, skb);
                    speedByteCount = 0;
                    Thread.Sleep(1000);
                }
            });
            SystemInfo.StartBackGroundThread("多线程下载主线程", () =>
            {
                try
                {
                    OnStarted();
                    HttpWebResponse headRes = null;
                    var isUseIp = false;
                    string currentUrl = string.Empty, currentHost = string.Empty;
                    if (UseMirrorDown)
                    {
                        lock (LockObj)
                        {
                            if (_serverList == null)
                            {
                                //从接口上获取服务器IP
                                _serverList = new Remote.StudentWareRemote().GetDownloadIp();
                            }
                        }
                        if (_serverList != null && _serverList.Length > 0)
                        {
                            var ips = _serverList.OrderBy(x => new object().GetHashCode()).ToArray();
                            var address = new Uri(FileUrl);
                            currentHost = address.Host;
                            var path = address.PathAndQuery;
                            var scheme = address.Scheme.ToLower();
                            var rnd = "?" + Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture);
                            foreach (var ip in ips)
                            {
                                currentUrl = scheme + "://" + ip + path + rnd;
                                var headReq = (HttpWebRequest)WebRequest.Create(currentUrl);
                                headReq.Host = currentHost;
                                SetHeadRequest(headReq);
                                headReq.AddRange(0);
                                //如果设置了代理服务器，那么就设置代理
                                if (Util.ProxyType != 0) headReq.Proxy = Network.GetWebProxy();
                                try
                                {
                                    headRes = (HttpWebResponse)headReq.GetResponse();
                                    Trace.WriteLine("Download From Ip:" + ip);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                                break;
                            }
                            isUseIp = headRes != null;
                        }
                    }
                    if (headRes == null)
                    {
                        var headReq = (HttpWebRequest)WebRequest.Create(FileUrl);
                        SetHeadRequest(headReq);
                        headReq.AddRange(0);
                        //如果设置了代理服务器，那么就设置代理
                        if (Util.ProxyType != 0) headReq.Proxy = Network.GetWebProxy();
                        try
                        {
                            headRes = (HttpWebResponse)headReq.GetResponse();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                            OnDownloadError();
                            return;
                        }
                    }
                    //获取文件头信息

                    //判断网络是否支持断点续传
                    var canMultiDownload = !string.IsNullOrEmpty(headRes.Headers["Content-Range"]);

                    fileSize = headRes.ContentLength;
                    _downBuffer.FileSize = fileSize;
                    DateTime lastModifyTime = headRes.LastModified;
                    if (lastModifyTime < new DateTime(2000, 1, 1)) lastModifyTime = new DateTime(2000, 1, 1);
                    _downBuffer.LastModified = lastModifyTime;
                    if (fileSize <= 0) return;
                    Trace.WriteLine(string.Format("文件大小：{0} ,最后修改时间：{1}", fileSize, lastModifyTime));
                    headRes.Close();
                    //加入对已存在zip包文件的判断，如果在同样的目录下存在，那么就直接Copy文件直接结束了。
                    try
                    {
                        if (File.Exists(FilePath))
                        {
                            var finfo = new FileInfo(FilePath);
                            var flen = finfo.Length;
                            DateTime lasttime = File.GetLastWriteTime(FilePath);
                            if (lastModifyTime == lasttime && flen == fileSize)
                            {
                                OnDownloadComplate();
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                    var taskArr = new Task[0];

                    List<Tuple<long, long>> lst;
                    if (canMultiDownload)
                    {
                        lst = _downBuffer.ReadNeededBlocks(ref allByteCount);
                    }
                    else
                    {
                        //如果不支持多线程下载
                        lst = new List<Tuple<long, long>>();
                        BlockCount = 1;
                    }

                    if (lst.Count == 1 && lst[0].Item1 == fileSize)
                    {
                        //已经下载完毕
                    }
                    else if (lst.Count == 0)
                    {
                        //尚未下载过
                        taskSpeed.Start();
                        using (FileStream fs = File.Create(_tempFile))
                        {
                            fs.Position = fileSize - 1;
                            fs.WriteByte(0);
                        }
                        var blockSize = (fileSize / MinBlockSize) / BlockCount;
                        taskArr = new Task[BlockCount];
                        for (var i = 0; i < BlockCount; i++)
                        {
                            var part = i;
                            var partOffset = part * blockSize * MinBlockSize;
                            var partOver = part == BlockCount - 1 ? fileSize : (part + 1) * blockSize * MinBlockSize;
                            var task = new Task(() => DownloadBlock(partOffset, partOver, isUseIp, currentHost, currentUrl, ref allByteCount, ref speedByteCount));
                            taskArr[i] = task;
                            task.Start();
                        }
                    }
                    else
                    {
                        //已经下载，但是下载未完成
                        taskSpeed.Start();
                        taskArr = new Task[lst.Count];
                        for (var i = 0; i < lst.Count; i++)
                        {
                            Tuple<long, long> item = lst[i];
                            var partOffset = item.Item1;
                            var partOver = item.Item2;
                            var task = new Task(() => DownloadBlock(partOffset, partOver, isUseIp, currentHost, currentUrl, ref allByteCount, ref speedByteCount));
                            taskArr[i] = task;
                            task.Start();
                        }
                    }
                    if (taskArr.Length > 0)
                    {
                        Task.WaitAll(taskArr);
                        if (!_isCanceled)
                        {
                            _downBuffer.WriteAllBufferToFile();
                        }
                    }
                    isComplate = true;
                    if (_isForceExit)
                    {
                        if (_isError)
                        {
                            OnDownloadError();
                            return;
                        }
                        if (_isCanceled)
                        {
                            OnCanceled();
                            return;
                        }
                        OnPaused();
                    }
                    else
                    {
                        Trace.WriteLine("下载完成");
                        OnDownloadComplate();
                        Trace.WriteLine("方法OnDownloadComplate通过");
                    }
                }
                catch (Exception ex)
                {
                    _downBuffer.WriteAllBufferToFile();
                    isComplate = true;
                    OnDownloadError();
                    Trace.WriteLine(ex);
                }
            });
        }

        /// <summary>
        /// 设置为HEAD请求
        /// </summary>
        /// <param name="headReq"></param>
        private static void SetHeadRequest(HttpWebRequest headReq)
        {
            headReq.Method = "HEAD";
            //headReq.Timeout = 10000;
            headReq.UserAgent = "Mozilla/5.0 (compatible; MSIE 11.0; Windows NT 6.1; Trident/7.0)";
            headReq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        /// <summary>
        /// 下载某一个块
        /// </summary>
        private void DownloadBlock(long partOffset, long partOverOffset, bool isUseIp, string currentHost, string currentUrl, ref long allByteCount, ref int speedByteCount)
        {
            //如果最后一块的起始位置和结束位置一致，会导致WebServer抛出416错误，因此要对这个做处理。
            if (partOffset >= partOverOffset) return;
            var trycnt = 0;
            while (trycnt < 30)
            {
                try
                {
                    HttpWebRequest webReq;
                    if (isUseIp)
                    {
                        webReq = (HttpWebRequest)WebRequest.Create(currentUrl);
                        webReq.Host = currentHost;
                    }
                    else
                    {
                        webReq = (HttpWebRequest)WebRequest.Create(FileUrl);
                    }
                    webReq.Method = "GET";
                    webReq.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    if (partOffset > 0) webReq.AddRange(partOffset);
                    //如果设置了代理服务器，那么就设置代理
                    if (Util.ProxyType != 0) webReq.Proxy = Network.GetWebProxy();
                    var webRes = (HttpWebResponse)webReq.GetResponse();
                    var rangeStr = webRes.Headers["Content-Range"];
                    if (partOffset > 0 && string.IsNullOrEmpty(rangeStr))
                    {
                        throw new RangeException();
                    }
                    Stream webStream = webRes.GetResponseStream();
                    if (webStream != null)
                    {
                        //webStream.ReadTimeout = 30000;
                        var buffer = new byte[PackSize];
                        int byr;
                        while (!_isForceExit && partOffset < partOverOffset && ((byr = webStream.Read(buffer, 0, PackSize)) > 0))
                        {
                            var block = new DownloadBlock();
                            block.WriteBlock(buffer, byr, partOffset);
                            _downBuffer.AddBufferBlock(block);
                            partOffset += byr;
                            allByteCount += byr;
                            speedByteCount += byr;
                        }
                        webStream.Close();
                        break;
                    }
                }
                catch (RangeException)
                {
                    trycnt = 30;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.NameResolutionFailure) trycnt = 30;
                    Trace.WriteLine(ex);
                }
                catch (Exception ex)
                {
                    Log.RecordLog(ex.ToString());
                    trycnt++;
                }
            }
            if (trycnt >= 30)
            {
                _isError = true;
                Stop();
            }
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void Stop() => _isForceExit = true;

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            _isCanceled = true;
            _isForceExit = true;
        }

        private class RangeException : Exception
        { }
    }
}