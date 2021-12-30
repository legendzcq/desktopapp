using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

using Framework.Utility;

namespace Framework.Remote
{
    internal class WebProxyClient : WebClient
    {
        private static readonly Dictionary<string, string[]> WebHosts = new Dictionary<string, string[]>();
        private static readonly Dictionary<string, string[]> HttpWebHosts = new Dictionary<string, string[]>();

        /// <summary>
        /// Cookie容器
        /// </summary>
        private static readonly CookieContainer CookieContainer = new CookieContainer();

        public WebProxyClient() => TimeOut = 30000;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { private get; set; }

        private bool UseHttpHost { get; set; }

        private bool UseSpecialIp { get; set; }

        /// <summary>
        /// 获取webrequest
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request;
            if (UseSpecialIp)
            {
                Trace.WriteLine("Request Data From Special Ip");
                //通过特定的IP获取数据
                var url = address.Scheme + "://211.157.0.5" + address.PathAndQuery;
                request = (HttpWebRequest)base.GetWebRequest(new Uri(url));
                if (request != null) request.Host = address.Host;
            }
            else if (UseHttpHost)
            {
                //通过HTTP接口上获取域名IP
                string[] ips;
                if (HttpWebHosts.ContainsKey(address.Host))
                {
                    ips = HttpWebHosts[address.Host];
                }
                else
                {
                    ips = GetHttpHostAddress(address.Host);
                    HttpWebHosts.Add(address.Host, ips);
                }
                if (ips.Length > 0)
                {
                    var url = address.Scheme + "://" + ips[0] + address.PathAndQuery;
                    request = (HttpWebRequest)base.GetWebRequest(new Uri(url));
                    if (request != null) request.Host = address.Host;
                }
                else
                {
                    request = (HttpWebRequest)base.GetWebRequest(address);
                }
            }
            else if (Util.DnsType == Util.DnsState.Default)
            {
                request = (HttpWebRequest)base.GetWebRequest(address);
            }
            else
            {
                string[] ips;
                if (WebHosts.ContainsKey(address.Host))
                {
                    ips = WebHosts[address.Host];
                }
                else
                {
                    ips = Util.DnsType == Util.DnsState.OfficalDnsServer
                        ? Network.GetHostIpByOfficalDnsServer(address.Host)
                        : Network.GetHostIpByPublicDnsServers(address.Host);
                    WebHosts.Add(address.Host, ips);
                }
                if (ips.Length > 0)
                {
                    var url = address.Scheme + "://" + ips[0] + address.PathAndQuery;
                    request = (HttpWebRequest)base.GetWebRequest(new Uri(url));
                    if (request != null) request.Host = address.Host;
                }
                else
                {
                    request = (HttpWebRequest)base.GetWebRequest(address);
                }
            }
            if (request == null) return null;
            BuildHttpWebRequest(request);
            //if (address.Host.ToLower() == "portal.cdeledu.com") request.KeepAlive = false;
            return request;
        }

        private void BuildHttpWebRequest(HttpWebRequest request)
        {
            //todo 加个选项吧
            //request.ProtocolVersion = new Version("1.0");
            request.Headers.Add("Accept-Language", "zh-CN");
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 11.0; Windows NT 6.1; Trident/7.0)";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            if (TimeOut > 0)
            {
                request.Timeout = TimeOut;
                request.ReadWriteTimeout = TimeOut;
            }
            request.CookieContainer = CookieContainer;
            //request.KeepAlive = false;
            request.SendChunked = false;
            //如果设置了代理服务器，那么就设置代理
            if (Util.ProxyType != 0) request.Proxy = Network.GetWebProxy();
        }

        /// <summary>
        /// 从Portal上获取域名解析数据
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private string[] GetHttpHostAddress(string host)
        {
            var key = Crypt.Md5(host, "eiiskdui").ToLower();
            var serverList = new[] { "portal.cdeledu.com", "59.151.113.100", "211.157.0.5" };
            const string requestUrl = "http://{0}/interface/getHost.php?host={1}&pkey={2}&_t={3}";

            foreach (var server in serverList)
            {
                var url = string.Format(requestUrl, server, host, key, Util.GetNow().Ticks);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Host = "portal.cdeledu.com";
                BuildHttpWebRequest(req);
                try
                {
                    var res = (HttpWebResponse)req.GetResponse();
                    var buffer = new byte[2048];
                    Stream st = res.GetResponseStream();
                    if (st != null)
                    {
                        var len = st.Read(buffer, 0, 2048);
                        if (len > 0)
                        {
                            var retstr = Encoding.Default.GetString(buffer, 0, len);
                            Model.HttpHost hostaddress = JsonDeserialize<Model.HttpHost>(retstr);
                            if (hostaddress.Code == "0")
                            {
                                return hostaddress.IpList.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            return new string[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static T JsonDeserialize<T>(string s)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(s)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        internal static T JsonDeserialize<T>(string s, Encoding encoding)
        {
            using (var ms = new MemoryStream(encoding.GetBytes(s)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        internal static T JsonDeserialize<T>(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }
        /// <summary>
        /// 转换为JSON对象
        /// </summary>
        /// <returns></returns>
        internal static string ConventToJson<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var json = new DataContractJsonSerializer(obj.GetType());
                json.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        private void AppendPostData(ref NameValueCollection data)
        {
            if (data != null)
            {
#if !ALLCOURSE
                data.Add("fordown", "1");
#endif
                data.Add("downver", Util.AppVersion);
                data.Add("downuid", Util.SsoUid.ToString(CultureInfo.InvariantCulture));
                data.Add("_r", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture));
                //添加随机参数 dgh 2018.01.02
                data.Add("_t", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                data = new NameValueCollection
                {
#if !ALLCOURSE
					{"fordown", "1"},
#endif
					{"downver", Util.AppVersion},
                    {"downuid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                    {"_r", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture)},
                     //添加随机参数 dgh 2018.01.02
                    {"_t", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture)}
                };
            }
        }

        private void BuildHttpForm(string address, NameValueCollection data, string fileName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<form method='post' action='{0}'>\r\n", address);
            foreach (var key in data.AllKeys)
            {
                sb.AppendFormat("<input type='hidden' name='{0}' value='{1}' />\r\n", key, data[key]);
            }
            sb.AppendLine("<input type=submit value='测试' />\r\n</form>");

            File.WriteAllText(fileName, sb.ToString());
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public new byte[] UploadValues(string address, NameValueCollection data)
        {
            AppendPostData(ref data);
            var sb = new StringBuilder();
            foreach (var key in data.AllKeys)
            {
                sb.AppendFormat("&{0}={1}", key, data[key]);
            }
            sb.Replace(' ', '+');
            //var fileName = DateTime.Now.Ticks + ".htm";
            //BuildHttpForm(address, data, fileName);
            //Trace.WriteLine(address.Contains("?")
            //	? string.Format("{0}:{1}:{2}", "获取数据", fileName, address + sb)
            //	: string.Format("{0}:{1}:{2}", "获取数据", fileName, address + "?" + sb.ToString().Substring(1)));
            if (!address.Contains("data.cdeledu.com") /*&& !address.Contains("manage.mobile.cdeledu.com")*/)
            {
                Trace.WriteLine(address.Contains("?")
                    ? string.Format("{0}:{1}", "获取数据", address + sb)
                    : string.Format("{0}:{1}", "获取数据", address + "?" + sb.ToString().Substring(1)));
            }
            try
            {
                ////Trace.WriteLine(address.Contains("?")
                ////    ? string.Format("{0}:{1}", "获取数据", address + sb)
                ////    : string.Format("{0}:{1}", "获取数据", address + "?" + sb.ToString().Substring(1)));
                return base.UploadValues(address, data);
            }
            catch (WebException ex)
            {
                Trace.WriteLine(ex);
                switch (ex.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        UseHttpHost = true;
                        try
                        {
                            return base.UploadValues(address, data);
                        }
                        catch
                        {
                            Trace.WriteLine(ex);
                            UseHttpHost = false;
                            UseSpecialIp = true;
                            return base.UploadValues(address, data);
                        }
                    case WebExceptionStatus.ConnectFailure:
                        UseSpecialIp = true;
                        return base.UploadValues(address, data);
                    default:
                        throw;
                }
            }
            finally
            {
                if (Util.IsOnline)
                {

                }
            }
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public new byte[] UploadValues(Uri address, NameValueCollection data)
        {
            Trace.WriteLine(string.Format("{0}:{1}", "获取数据", address));
            AppendPostData(ref data);
            return base.UploadValues(address, data);
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public new byte[] UploadValues(string address, string method, NameValueCollection data)
        {
            Trace.WriteLine(string.Format("{0}:{1}", "获取数据", address));
            AppendPostData(ref data);
            return base.UploadValues(address, method, data);
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public new byte[] UploadValues(Uri address, string method, NameValueCollection data)
        {
            Trace.WriteLine(string.Format("{0}:{1}", "获取数据", address));
            AppendPostData(ref data);
            return base.UploadValues(address, method, data);
        }

        /// <summary>
        /// 下载数据(get)
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] DownloadData(string address, NameValueCollection data)
        {
            //AppendPostData(ref data);
            var sb = new StringBuilder();
            foreach (var key in data.AllKeys)
            {
                sb.AppendFormat("&{0}={1}", key, data[key]);
            }
            sb.Replace(' ', '+');
            if (!address.Contains("data.cdeledu.com"))
            {
                Trace.WriteLine(address.Contains("?")
                    ? string.Format("{0}:{1}", "获取数据", address + sb)
                    : string.Format("{0}:{1}", "获取数据", address + "?" + sb.ToString().Substring(1)));
            }
            try
            {
                return base.DownloadData(address + "?" + sb.ToString().Substring(1));
            }
            catch (WebException ex)
            {
                Trace.WriteLine(ex);
                switch (ex.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        UseHttpHost = true;
                        try
                        {
                            return base.DownloadData(address + "?" + sb.ToString().Substring(1));
                        }
                        catch
                        {
                            Trace.WriteLine(ex);
                            UseHttpHost = false;
                            UseSpecialIp = true;
                            return base.DownloadData(address + "?" + sb.ToString().Substring(1));
                        }
                    case WebExceptionStatus.ConnectFailure:
                        UseSpecialIp = true;
                        return base.DownloadData(address + "?" + sb.ToString().Substring(1));
                    default:
                        throw;
                }
            }
            finally
            {
                if (Util.IsOnline)
                {

                }
            }
        }


    }
}
