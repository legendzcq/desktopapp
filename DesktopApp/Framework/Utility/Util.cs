using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;

namespace Framework.Utility
{
    public static class Util
    {

#if CHINAACC
        public const string AppName = "会计下载课堂";
        public const string AppNameForLogin = "AccDownClass10";
        public const int HttpPort = 18951;
        public const int siteID = 1;
#endif
#if MED
        public const string AppName = "医学下载课堂";
		public const string AppNameForLogin = "MedDownClass10";
        public const int HttpPort = 18952;
        public const int siteID = 5;
#endif
#if JIANSHE
        public const string AppName = "建筑下载课堂";
		public const string AppNameForLogin = "JiansheDownClass10";
        public const int HttpPort = 18953;
        public const int siteID = 4;
#endif
#if LAW
        public const string AppName = "法律下载课堂";
		public const string AppNameForLogin = "LawDownClass10";
        public const int HttpPort = 18954;
        public const int siteID = 2;
#endif
#if CHINATAT
        public const string AppName = "职教下载课堂";
		public const string AppNameForLogin = "ChinatatDownClass10";
        public const int HttpPort = 18955;
        public const int siteID = 9;
#endif
#if G12E
        public const string AppName = "中小学下载课堂";
		public const string AppNameForLogin = "G12eDownClass10";
        public const int HttpPort = 18956;
        public const int siteID = 6;
#endif
#if ZIKAO
        public const string AppName = "自考下载课堂";
		public const string AppNameForLogin = "ZikaoDownClass10";
        public const int HttpPort = 18957;
        public const int siteID = 3;
#endif
#if CHENGKAO
        public const string AppName = "成考下载课堂";
		public const string AppNameForLogin = "ChengkaoDownClass10";
        public const int HttpPort = 18958;
#endif
#if KAOYAN
        public const string AppName = "考研下载课堂";
		public const string AppNameForLogin = "KaoyanDownClass10";
        public const int HttpPort = 18959;
        public const int siteID = 8;
#endif
#if FOR68
        public const string AppName = "外语下载课堂";
		public const string AppNameForLogin = "For68DownClass10";
        public const int HttpPort = 18960;
        public const int siteID = 7;
#endif

#if CK100
        public const string AppName = "财考下载课堂";
		public const string AppNameForLogin = "Ck100DownClass10";
        public const int HttpPort = 18961;
        public const int siteID = 1;
#endif

        #region 字段
        public const string FormatExtension = ".cdel";
        public const string DownloadFileExtension = ".cdwn";
        public const string DownloadConfigExtension = ".cdwn.cfg";
        private static string _videoPath;
        private static bool _isOnline;
        private static bool _lastOnlineState;
        private static bool? _disableOnlineCheck;
        private static bool? _isUseMirrorDown;
        private static bool? _disableCookie;
        private static DnsState _dnsType;
        private static string _baseFont;
        private static string _jiangyiSavePath;
        private static string _videoDownSavePath;
        private static int _downType = -1;
        private static int _lastOffLineTime = -1;
        private static int _downloadTaskCount = -1;
        private static int _downloadThreadCount = -1;
        private static readonly bool? _isUseProxy;
        private static string _proxyAddress;
        private static int _proxyPort = -1;
        private static string _proxyUserName;
        private static string _proxyUserPassword;
        private static int _proxyType = -1;
        private static bool? _isUseffDshow;
        private static string _questionBackColor;
        private static bool? _isNotUseSpeed;
        private static bool? _isUsevmr9;
        private static bool? _isCheckFile;

        private static int _questionFontSize = -1;
        private static int _kcjyFontSize = -1;

        private static int _audioType = -1;

        private static bool? _isShowAdv;
        private static bool? _isAutoShowPoint;
        private static bool? _isAutoPlay;

        public static event EventHandler OnlineStateChanged;
        public static event EventHandler FontChanged;
        public static event EventHandler DownloadTaskCountChanged;

        #endregion

        /// <summary>
        /// DNS解析类型
        /// </summary>
        public enum DnsState
        {
            /// <summary>
            /// 未设置
            /// </summary>
            Noset = 0,
            /// <summary>
            /// 默认DNS
            /// </summary>
            Default,
            /// <summary>
            /// 官方DNS
            /// </summary>
            OfficalDnsServer,
            /// <summary>
            /// 公开的DNS
            /// </summary>
            PublicDnsServer
        }

        #region 基础设置

        /// <summary>
        /// 视频保存路径
        /// </summary>
        public static string VideoPath
        {
            get
            {
                if (_videoPath == null || string.IsNullOrEmpty(_videoPath) || !Directory.Exists(_videoPath))
                {
                    var obj = SystemInfo.GetSetting("VideoPath");
                    _videoPath = obj as string;
                    if (_videoPath == null || string.IsNullOrEmpty(_videoPath) || !Directory.Exists(_videoPath))
                    {
                        var drive = SystemInfo.GetMaxDrive();
                        VideoPath = drive + "CdelVideo";
                    }
                }
                return _videoPath;
            }
            set
            {
                _videoPath = value;
                SystemInfo.SaveSetting("VideoPath", value);
            }
        }

        /// <summary>
        /// 下载保存路径
        /// </summary>
        public static string VideoDownSavePath
        {
            get
            {
                if (_videoDownSavePath == null || string.IsNullOrEmpty(_videoDownSavePath) || !Directory.Exists(_videoDownSavePath))
                {
                    var obj = SystemInfo.GetSetting("VideoDownSavePath");
                    _videoDownSavePath = obj as string;
                    if (_videoDownSavePath == null || string.IsNullOrEmpty(_videoDownSavePath) || !Directory.Exists(_videoDownSavePath))
                    {
                        var drive = SystemInfo.GetMaxDrive();
                        VideoDownSavePath = drive + "CdelVideoDown";
                    }
                }
                return _videoDownSavePath;
            }
            set
            {
                _videoDownSavePath = value;
                SystemInfo.SaveSetting("VideoDownSavePath", value);
            }
        }

        /// <summary>
        /// 讲义保存路径
        /// </summary>
        public static string JiangyiSavePath
        {
            get
            {
                if (string.IsNullOrEmpty(_jiangyiSavePath))
                {
                    var obj = SystemInfo.GetSetting("JiangyiSavePath");
                    _jiangyiSavePath = obj as string;
                    if (string.IsNullOrEmpty(_jiangyiSavePath))
                    {
                        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        JiangyiSavePath = documentPath + "\\课程讲义";
                    }
                }
                return _jiangyiSavePath;
            }
            set
            {
                if (value != _jiangyiSavePath)
                {
                    _jiangyiSavePath = value;
                    SystemInfo.SaveSetting("JiangyiSavePath", value);
                }
            }
        }

        /// <summary>
        /// 是否联网在线
        /// </summary>
        public static bool IsOnline
        {
            get => _isOnline;
            set
            {
                _isOnline = value;
                if (_lastOnlineState != value)
                {
                    Trace.WriteLine("在线状态 --> " + value);
                    _lastOnlineState = value;
                    if (OnlineStateChanged != null) OnlineStateChanged(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 是否禁用在线检查
        /// </summary>
        public static bool DisableOnlineCheck
        {
            get
            {
                if (!_disableOnlineCheck.HasValue)
                {
                    var obj = SystemInfo.GetSetting("DisableOnlineCheck");
                    _disableOnlineCheck = obj != null && (obj.ToString() == "1");
                }
                return _disableOnlineCheck.Value;
            }
            set
            {
                _disableOnlineCheck = value;
                SystemInfo.SaveSetting("DisableOnlineCheck", value ? "1" : "0");
            }
        }

        /// <summary>
        /// 是否启用镜像下载
        /// </summary>
        public static bool IsUseMirrorDown
        {
            get
            {
                if (!_isUseMirrorDown.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsUseMirrorDown");
                    _isUseMirrorDown = obj != null && (obj.ToString() == "1");
                }
                return _isUseMirrorDown.Value;
            }
            set
            {
                _isUseMirrorDown = value;
                SystemInfo.SaveSetting("IsUseMirrorDown", value ? "1" : "0");
            }
        }

        /// <summary>
        /// DNS类型
        /// </summary>
        public static DnsState DnsType
        {
            get
            {
                if (_dnsType == DnsState.Noset)
                {
                    var obj = SystemInfo.GetSetting("DnsType");
                    DnsType = obj == null ? DnsState.Default : (DnsState)int.Parse(obj.ToString());
                }
                return _dnsType;
            }
            set
            {
                _dnsType = value;
                SystemInfo.SaveSetting("DnsType", (int)value);
            }
        }

        /// <summary>
        /// 字体名称
        /// </summary>
        public static string BaseFont
        {
            get
            {
                if (string.IsNullOrEmpty(_baseFont))
                {
                    var obj = SystemInfo.GetSetting("BaseFont");
                    _baseFont = obj as string;
                    if (string.IsNullOrEmpty(_baseFont))
                    {
                        BaseFont = Environment.OSVersion.Version.Major < 6 ? "宋体" : "Microsoft YaHei";
                    }
                }
                return _baseFont;
            }
            set
            {
                if (value != _baseFont)
                {
                    _baseFont = value;
                    SystemInfo.SaveSetting("BaseFont", value);
                    if (FontChanged != null)
                    {
                        FontChanged(new object(), EventArgs.Empty);
                    }
                }
            }
        }
        /// <summary>
        /// 做题界面背景色设置
        /// </summary>
        public static string QuestionBackColor
        {
            get
            {
                if (string.IsNullOrEmpty(_questionBackColor))
                {
                    var obj = SystemInfo.GetSetting("QuestionBackColor");
                    _questionBackColor = obj as string;
                    if (string.IsNullOrEmpty(_questionBackColor))
                    {
                        QuestionBackColor = "#fff";
                    }
                }
                return _questionBackColor;
            }
            set
            {
                if (value != _questionBackColor)
                {
                    _questionBackColor = value;
                    SystemInfo.SaveSetting("QuestionBackColor", value);
                }
            }
        }
        /// <summary>
        /// 下载任务总数
        /// </summary>
        public static int DownloadTaskCount
        {
            get
            {
                if (_downloadTaskCount == -1)
                {
                    var obj = SystemInfo.GetSetting("DownloadTaskCount");
                    _downloadTaskCount = obj == null ? 0 : int.Parse(obj.ToString());
                    if (_downloadTaskCount <= 0)
                    {
                        DownloadTaskCount = 3;
                    }
                    if (_downloadTaskCount >= 5)
                    {
                        DownloadTaskCount = 5;
                    }
                }
                return _downloadTaskCount;
            }
            set
            {
                if (value != _downloadTaskCount)
                {
                    _downloadTaskCount = value;
                    SystemInfo.SaveSetting("DownloadTaskCount", value);
                    if (DownloadTaskCountChanged != null)
                    {
                        DownloadTaskCountChanged(new object(), EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 下载线程数
        /// </summary>
        public static int DownloadThreadCount
        {
            get
            {
                if (_downloadThreadCount == -1)
                {
                    var obj = SystemInfo.GetSetting("DownloadThreadCount");
                    _downloadThreadCount = obj == null ? 0 : int.Parse(obj.ToString());
                    if (_downloadThreadCount <= 0)
                    {
                        DownloadThreadCount = 2;
                    }
                    if (_downloadThreadCount > 5)
                    {
                        DownloadThreadCount = 5;
                    }
                }
                return _downloadThreadCount;
            }
            set
            {
                if (_downloadThreadCount != value)
                {
                    _downloadThreadCount = value;
                    SystemInfo.SaveSetting("DownloadThreadCount", value);
                }
            }
        }

        /// <summary>
        /// 是否禁用Coopie
        /// </summary>
        public static bool DisableCookie
        {
            get
            {
                if (!_disableCookie.HasValue)
                {
                    var obj = SystemInfo.GetSetting("DisableCookie");
                    _disableCookie = obj != null && (obj.ToString() == "1");
                }
                return _disableCookie.Value;
            }
            set
            {
                _disableCookie = value;
                SystemInfo.SaveSetting("DisableCookie", value ? "1" : "0");
            }
        }

        /// <summary>
        /// 下载类型,0，未定义，1，高清，2，标清，3，音频
        /// </summary>
        public static int DownType
        {
            get
            {
                if (_downType == -1)
                {
                    var obj = SystemInfo.GetSetting("VideoType");
                    _downType = obj == null ? 0 : int.Parse(obj.ToString());
                    if (_downType != 1 && _downType != 2) _downType = 0;
                }
                return _downType;
            }
            set
            {
                _downType = value;
                SystemInfo.SaveSetting("VideoType", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// 设备序列号
        /// </summary>
        public static string DeviceId => Crypt.GetMachineKeyString();

        /// <summary>
        /// 剩余的离线时间
        /// </summary>
        public static int LastOffLineTime
        {
            get
            {
                if (_lastOffLineTime == -1)
                {
                    var obj = SystemInfo.GetSetting("LastOffLineTime");
                    if (obj == null)
                    {
                        LastOffLineTime = 14 * 3600;
                    }
                    else
                    {
                        _lastOffLineTime = int.Parse(Crypt.Rc4DecryptString(obj.ToString().Trim()));
                    }
                }
                return _lastOffLineTime;
            }
            set
            {
                _lastOffLineTime = value;
                var str = new string(' ', 10) + value + new string('\t', 10);
                SystemInfo.SaveSetting("LastOffLineTime", Crypt.Rc4EncryptString(str));
            }
        }

        public static bool IsUseffDshow
        {
            get
            {
                if (!_isUseffDshow.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsUseffDshow");
                    _isUseffDshow = obj != null && (obj.ToString() == "1");
                }
                return _isUseffDshow.Value;
            }
            set
            {
                _isUseffDshow = value;
                SystemInfo.SaveSetting("IsUseffDshow", value ? "1" : "0");
            }
        }
        /// <summary>
        /// 是否禁用播放中声音加速 true：启用 false：不启用
        /// </summary>
        public static bool IsNotUseSpeed
        {
            get
            {
                if (!_isNotUseSpeed.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsNotUseSpeed");
                    _isNotUseSpeed = obj != null && (obj.ToString() == "1");
                }
                return _isNotUseSpeed.Value;
            }
            set
            {
                _isNotUseSpeed = value;
                SystemInfo.SaveSetting("IsNotUseSpeed", value ? "1" : "0");
            }
        }
        /// <summary>
        /// 是否采用vmr9作为视频输出 true：启用 false：不启用
        /// </summary>
        public static bool IsUsevmr9
        {
            get
            {
                if (!_isUsevmr9.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsUsevmr9");
                    _isUsevmr9 = obj != null && (obj.ToString() == "1");
                }
                return _isUsevmr9.Value;
            }
            set
            {
                _isUsevmr9 = value;
                SystemInfo.SaveSetting("IsUsevmr9", value ? "1" : "0");
            }
        }
        /// <summary>
        /// 是否禁用文件检测 true：启用 false：不启用
        /// </summary>
        public static bool IsCheckFile
        {
            get
            {
                if (!_isCheckFile.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsCheckFile");
                    _isCheckFile = obj != null && (obj.ToString() == "1");
                }
                return _isCheckFile.Value;
            }
            set
            {
                _isCheckFile = value;
                SystemInfo.SaveSetting("IsCheckFile", value ? "1" : "0");
            }
        }
        /// <summary>
        /// 题库字体设置大小 dgh 2017.04.25
        /// </summary>
        public static int QuestionFontSize
        {
            get
            {
                if (_questionFontSize == -1)
                {
                    var obj = SystemInfo.GetSetting("QuestionFontSize");
                    _questionFontSize = obj == null ? 18 : int.Parse(obj.ToString());
                }
                return _questionFontSize;
            }
            set
            {
                _questionFontSize = value;
                SystemInfo.SaveSetting("QuestionFontSize", value.ToString(CultureInfo.InvariantCulture));
            }
        }
        /// <summary>
        /// 讲义字体大小设置 dgh 2017.04.25
        /// </summary>
        public static int KcjyFontSize
        {
            get
            {
                if (_kcjyFontSize == -1)
                {
                    var obj = SystemInfo.GetSetting("KcjyFontSize");
                    _kcjyFontSize = obj == null ? 14 : int.Parse(obj.ToString());
                }
                return _kcjyFontSize;
            }
            set
            {
                _kcjyFontSize = value;
                SystemInfo.SaveSetting("KcjyFontSize", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// 音频输出类型,0：默认(Default DirectSound Device)，1：Default WaveOut Device
        /// </summary>
        public static int AudioType
        {
            get
            {
                if (_audioType == -1)
                {
                    var obj = SystemInfo.GetSetting("AudioType");
                    _audioType = obj == null ? 0 : int.Parse(obj.ToString());
                    if (_audioType != 1) _audioType = 0;
                }
                return _audioType;
            }
            set
            {
                _audioType = value;
                SystemInfo.SaveSetting("AudioType", value.ToString(CultureInfo.InvariantCulture));
            }
        }
        /// <summary>
        /// 是否显示广告信息 1:显示  0:不显示
        /// </summary>
        public static bool IsShowAdv
        {
            get
            {
                if (!_isShowAdv.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsShowAdv");
                    _isShowAdv = obj == null || (obj.ToString() == "1");
                }
                return _isShowAdv.Value;
            }
            set
            {
                _isShowAdv = value;
                SystemInfo.SaveSetting("IsShowAdv", value ? "1" : "0");
            }
        }

        /// <summary>
        /// 是否自动显示知识点  1:显示  0:不显示
        /// </summary>
        public static bool IsAutoShowPoint
        {
            get
            {
                if (!_isAutoShowPoint.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsAutoShowPoint");
                    _isAutoShowPoint = obj == null || (obj.ToString() == "1");
                }
                return _isAutoShowPoint.Value;
            }
            set
            {
                _isAutoShowPoint = value;
                SystemInfo.SaveSetting("IsAutoShowPoint", value ? "1" : "0");
            }
        }

        /// <summary>
        /// 是否自动播放下一节课程 1:显示  0:不显示
        /// </summary>
        public static bool IsAutoPlay
        {
            get
            {
                if (!_isAutoPlay.HasValue)
                {
                    var obj = SystemInfo.GetSetting("IsAutoPlay");
                    _isAutoPlay = obj == null || (obj.ToString() == "1");
                }
                return _isAutoPlay.Value;
            }
            set
            {
                _isAutoPlay = value;
                SystemInfo.SaveSetting("IsAutoPlay", value ? "1" : "0");
            }
        }



        #endregion

        #region 基础信息
        /// <summary>
        /// 当前登录用户名
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// 当前用户密码
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// SSOUID
        /// </summary>
        public static int SsoUid { get; set; }

        /// <summary>
        /// 当前用户唯一识别码
        /// </summary>
        public static string SessionId { get; set; }
        #endregion

        #region 代理服务器设置

        //代理服务器类型
        public static int ProxyType
        {
            get
            {
                if (_proxyType < 0)
                {
                    var obj = SystemInfo.GetSetting("ProxyType");
                    ProxyType = obj == null ? 0 : int.Parse(obj.ToString());
                }
                return _proxyType;
            }
            set
            {
                _proxyType = value;
                SystemInfo.SaveSetting("ProxyType", value);
            }
        }

        /// <summary>
        /// 代理服务器地址
        /// </summary>
        public static string ProxyAddress
        {
            get
            {
                if (_proxyAddress == null)
                {
                    var obj = SystemInfo.GetSetting("ProxyAddress") as string;
                    ProxyAddress = obj ?? string.Empty;
                }
                return _proxyAddress;
            }
            set
            {
                _proxyAddress = value;
                SystemInfo.SaveSetting("ProxyAddress", value);
            }
        }

        /// <summary>
        /// 代理服务器端口
        /// </summary>
        public static int ProxyPort
        {
            get
            {
                if (_proxyPort < 0)
                {
                    var obj = SystemInfo.GetSetting("ProxyPort");
                    ProxyPort = obj == null ? 0 : int.Parse(obj.ToString());
                }
                return _proxyPort;
            }
            set
            {
                _proxyPort = value;
                SystemInfo.SaveSetting("ProxyPort", value);
            }
        }

        /// <summary>
        /// 代理服务器用户名
        /// </summary>
        public static string ProxyUserName
        {
            get
            {
                if (_proxyUserName == null)
                {
                    var obj = SystemInfo.GetSetting("ProxyUserName") as string;
                    ProxyUserName = obj ?? string.Empty;
                }
                return _proxyUserName;
            }
            set
            {
                _proxyUserName = value;
                SystemInfo.SaveSetting("ProxyUserName", value);
            }
        }

        /// <summary>
        /// 代理服务器密码
        /// </summary>
        public static string ProxyUserPassword
        {
            get
            {
                if (_proxyUserPassword == null)
                {
                    var obj = SystemInfo.GetSetting("ProxyUserPassword") as string;
                    ProxyUserPassword = obj ?? string.Empty;
                }
                return _proxyUserPassword;
            }
            set
            {
                _proxyUserPassword = value;
                SystemInfo.SaveSetting("ProxyUserPassword", value);
            }
        }
        #endregion

        static Util()
        {
            //设置HTTP最大连接数量
            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// 获取当前时间，主要是远程接口用
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow() => DateTime.Now;

        /// <summary>
        /// 获取当前时间的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNowString() => GetNow().ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 获取当前系统时间的时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetNowTimeStamp() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        /// <summary>
        /// 获取13位的时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetNowTimeStamp13() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        /// <summary>
        /// Token值
        /// </summary>
        public static string TokenString { get; set; }

        /// <summary>
        /// tokenTime
        /// </summary>
        public static long TokenLongTime { get; set; }
        /// <summary>
        /// 本次请求令牌码失效时长
        /// </summary>
        public static int Timeout { get; set; }

        /// <summary>
        /// base64加密后的字符串
        /// </summary>
        public static string ParamValue { get; set; }

        /// <summary>
        /// 获取应用程序版本
        /// </summary>
        public static string AppVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
    }
}
