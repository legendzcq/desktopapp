using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace CdelService.Utility
{
	public static class Util
	{

		#region 字段
        private static bool _isOnline;
        private static bool _lastOnlineState;
        private static DnsState _dnsType;
        private static string _proxyAddress;
        private static int _proxyPort = -1;
        private static string _proxyUserName;
        private static string _proxyUserPassword;
        private static int _proxyType = -1;
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
        /// <summary>
        /// 是否联网在线
        /// </summary>
        public static bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                _isOnline = value;
                //if (_lastOnlineState != value)
                //{
                    Log.RecordLog("在线状态 --> " + value);
                    _lastOnlineState = value;
                    //if (OnlineStateChanged != null) OnlineStateChanged(null, EventArgs.Empty);
                //}
            }
        }
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
                    DnsType = obj == null ? DnsState.Default : (DnsState)Int32.Parse(obj.ToString());
                }
                return _dnsType;
            }
            set
            {
                _dnsType = value;
                SystemInfo.SaveSetting("DnsType", (int)value);
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
        /// 设备序列号
        /// </summary>
        public static string DeviceId
        {
            get
            {
                return Crypt.GetMachineKeyString();
            }
        }
        /// <summary>
        /// 获取当前时间，主要是远程接口用
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow()
        {
            return DateTime.Now;
        }
        /// <summary>
        /// 获取当前时间的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNowString()
        {
            return GetNow().ToString("yyyy-MM-dd HH:mm:ss");
        }
	}
}
