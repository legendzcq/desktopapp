using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Bdev.Net.Dns;
using Bdev.Net.Dns.Helpers;

namespace Framework.Utility
{
    public static class Network
    {
#if CHINAACC || JIANSHE || MED || LAW || ZIKAO || CNEDU || G12E
        private static readonly IPAddress[] OfficalDnsServers = { IPAddress.Parse("59.151.109.62"), IPAddress.Parse("211.157.0.49") };
#else
        private static readonly IPAddress[] OfficalDnsServers = { };
#endif

        private static readonly IPAddress[] PublicDnsServers = { IPAddress.Parse("1.2.4.8"), IPAddress.Parse("210.2.4.8"), IPAddress.Parse("114.114.114.114"), IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.4.4") };

        public static string[] GetHostIpByPublicDnsServers(string hostName) => GetHostIpByDnsServer(hostName, PublicDnsServers);

        public static string[] GetHostIpByOfficalDnsServer(string hostName) => GetHostIpByDnsServer(hostName, OfficalDnsServers);

        public static string[] GetHostIpByDefaultDnsServer(string hostName) => GetHostIpByDnsServer(hostName, DnsServers.IP4);

        private static string[] GetHostIpByDnsServer(string hostName, IEnumerable<IPAddress> servers)
        {
            var req = new Request();
            var ques = new Question(hostName, DnsType.A, DnsClass.IN);
            req.AddQuestion(ques);
            Response res = null;
            foreach (IPAddress ip in servers)
            {
                try
                {
                    res = Resolver.Lookup(req, ip);
                    break;
                }
                catch
                {
                    res = null;
                }
            }
            if (res == null || res.Answers.Length == 0)
            {
                return new string[] { };
            }
            return res.Answers.Where(x => x.Type == DnsType.A).Select(x => x.Record.ToString()).ToArray();
        }

        /// <summary>
        /// 获取推送服务器IP
        /// </summary>
        /// <returns></returns>
        public static string GetPushServerIp()
        {
            try
            {
                var ips = Dns.GetHostAddresses("udpapi.chinatet.com").Select(x => x.ToString()).ToArray();
                if (ips.Length == 0)
                {
                    return "59.151.113.40";
                }
                return ips[0];
            }
            catch (Exception)
            {
                return "59.151.113.40";
            }
        }

        public static WebProxy GetWebProxy()
        {
            if (Util.ProxyType == 2)
            {
                var proxy = new WebProxy(Util.ProxyAddress, Util.ProxyPort);
                if (!string.IsNullOrEmpty(Util.ProxyUserName))
                {
                    proxy.Credentials = new NetworkCredential(Util.ProxyUserName, Util.ProxyUserPassword);
                }
                return proxy;
            }
            return null;
        }
    }
}
