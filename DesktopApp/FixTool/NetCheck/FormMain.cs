using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NetCheck.Dns;
using System.Text.RegularExpressions;
using System.IO;

#if NET4
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Json;
#endif

namespace NetCheck
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
			ServicePointManager.DefaultConnectionLimit = 50;
			ServicePointManager.Expect100Continue = false;
		}

		private static readonly string[] DnsHostList =
		{
			"portal.cdeledu.com",
			"www.{0}",
			"member.{0}",
			"elearning.{0}"
		};

		private static readonly string[] HeadHostList =
		{
			"http://portal.cdeledu.com",
			"https://portal.cdeledu.com",
			"http://www.{0}",
			"http://member.{0}", 
			"http://elearning.{0}"
		};

		private static readonly Dictionary<string, string> WangxiaoList = new Dictionary<string, string>
		{
			{ "中华会计网校", "chinaacc.com" } ,
			{ "医学教育网", "med66.com" } ,
			{ "建设工程教育网", "jianshe99.com" } ,
			{ "法律教育网", "chinalawedu.com" } ,
			{ "职业培训教育网", "chinatat.com" } ,
			{ "中小学教育网", "g12e.com" } ,
			{ "自考365", "zikao365.com" } ,
			{ "成考365", "chengkao365.com" } ,
			{ "考研教育网", "cnedu.cn" } ,
			{ "外语教育网", "for68.com" } 
		};

		private static readonly string[] CdnHostIp =
		{
			"211.157.0.5",
			"219.153.76.76",
			"183.203.15.75"
		};

		private readonly NetInfo _netInfoItem = new NetInfo();

		private string _currentDomain;

		private EventLog _eventLog = new EventLog() { Source = Application.ProductName };

		private void FormMain_Load(object sender, EventArgs e)
		{
			foreach (KeyValuePair<string, string> kv in WangxiaoList)
			{
				CbList.Items.Add(kv.Key);
			}
			CbList.SelectedIndex = 0;
		}

		private void CbList_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentDomain = WangxiaoList[CbList.Text];
		}

		private void BtnStart_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TbCode.Text))
			{
				MessageBox.Show(@"请输入学员代码", @"提示");
				TbCode.Focus();
				return;
			}
			_netInfoItem.UserCode = TbCode.Text.Trim();
			BtnStart.Enabled = false;
			CbList.Enabled = false;
			TbCode.Enabled = false;
			tbMessage.Text = string.Empty;
			var th = new Thread(CheckNetwork) { IsBackground = true };
			th.Start();
		}

		private void CheckNetwork()
		{
			Thread.Sleep(1000);
			var dnsServers = DnsProvider.SystemDnsServers();
			Write("系统默认DNS: ");
			var srvList = new List<string>();
			for (int i = 0; i < dnsServers.Count; i++)
			{
				if (dnsServers[i].AddressFamily == AddressFamily.InterNetwork)
				{
					if (i > 0) Write(",");
					Write(dnsServers[i].ToString());
					srvList.Add(dnsServers[i].ToString());
				}
			}
			_netInfoItem.DnsRecord = srvList;
			WriteLine();
			var ip1 = GetIpInfoFromIp138();
			WriteLine("IP138结果:{0} {1} {2}", ip1.IPAddress, ip1.Location, ip1.Idc);
			var ip2 = GetIpInfoFromIPcn();
			WriteLine("IP.cn结果:{0} {1} {2}", ip2.IPAddress, ip2.Location, ip2.Idc);
			var ip3 = GetIpInfoFromQQ();
			WriteLine("IP.qq结果:{0} {1} {2}", ip3.IPAddress, ip3.Location, ip3.Idc);
			var ip4 = GetIpInfoFromChinaz();
			WriteLine("Chinaz结果:{0} {1} {2}", ip4.IPAddress, ip4.Location, ip4.Idc);
			_netInfoItem.CurrentIpInfo = new List<IPInfo> { ip1, ip2, ip3, ip4 };

			_netInfoItem.Location = GetIpLocation(ip1.IPAddress ?? ip2.IPAddress ?? ip3.IPAddress ?? ip4.IPAddress);
			//_netInfoItem.Location = GetIpLocation("218.140.136.10");
			WriteLine();
#if !NET4
			var dnsList = new List<NetInfo.DnsResolveItem>();
			foreach (var host in DnsHostList)
			{
				dnsList.Add(CheckHostDns(string.Format(host, _currentDomain), dnsServers));
			}
			_netInfoItem.DnsResolveList = dnsList;
#else
			_netInfoItem.DnsResolveList = DnsHostList.Select(host => CheckHostDns(string.Format(host, _currentDomain), dnsServers)).ToList();
#endif
			WriteLine();
#if !NET4
			var stateList = new List<NetInfo.HostStateItem>();
			foreach (var host in HeadHostList)
			{
				stateList.Add(HeadHost(string.Format(host, _currentDomain)));
			}
			_netInfoItem.HostState = stateList;
#else
			_netInfoItem.HostState = HeadHostList.Select(host => HeadHost(string.Format(host, _currentDomain))).ToList();
#endif
			WriteLine();
#if !NET4
			var cdnStateList = new List<NetInfo.CdnHostStateItem>();
			foreach (var ip in CdnHostIp)
			{
				var item = new NetInfo.CdnHostStateItem { CdnServer = ip };
				var cdnhostState = new List<NetInfo.HostStateItem>();
				foreach (var host in HeadHostList)
				{
					cdnhostState.Add(HeadHost(string.Format(host, _currentDomain), ip));
				}
				item.HostState = cdnhostState;
				cdnStateList.Add(item);
			}
			_netInfoItem.CdnHostState = cdnStateList;
#else
			_netInfoItem.CdnHostState = CdnHostIp.Select(ip => new NetInfo.CdnHostStateItem
			{
				CdnServer = ip,
				HostState = HeadHostList.Select(host => HeadHost(string.Format(host, _currentDomain), ip)).ToList()
			}).ToList();
#endif
			WriteLine();

#if !NET4
			SaveNetInfo(TbCode.Text.Trim(), _netInfoItem.GetJsonString());
#else
			SaveNetInfo(TbCode.Text.Trim(), GetJsonString(_netInfoItem));
#endif

			if (!CheckSameIp() || !DealHost())
			{
				Invoke(new MethodInvoker(() => MessageBox.Show(@"您的网络情况复杂，请联系客服人员处理", @"提示")));
			}

			Invoke(new MethodInvoker(() =>
			{
				BtnStart.Enabled = true;
				CbList.Enabled = true;
				TbCode.Enabled = true;
			}));
			WriteLine("完成");
		}

#if NET4
		private string GetJsonString<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var ser = new DataContractJsonSerializer(typeof(T));
				ser.WriteObject(ms, obj);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}
#endif

		private bool CheckSameIp()
		{
			var ip = string.Empty;
			foreach (var item in _netInfoItem.CurrentIpInfo)
			{
				if (!string.IsNullOrEmpty(item.IPAddress))
				{
					if (string.IsNullOrEmpty(ip))
					{
						ip = item.IPAddress;
					}
					else
					{
						if (ip != item.IPAddress)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool DealHost()
		{
			if (_netInfoItem.Location == null || string.IsNullOrEmpty(_netInfoItem.Location.Idc)) return false;
			var cdnIp = _netInfoItem.Location.Idc == "联通" ? "211.157.0.5" : string.Empty;
			if (cdnIp == string.Empty) cdnIp = _netInfoItem.Location.Idc == "电信" ? "219.153.76.76" : string.Empty;
			if (cdnIp == string.Empty) cdnIp = _netInfoItem.Location.Idc == "移动" ? "183.203.15.75" : string.Empty;
#if NET4
			var errorList = new List<string>();
			errorList.AddRange(_netInfoItem.DnsResolveList.Where(x => x.HasError).Select(x => x.DomainName));

			foreach (var domain in DnsHostList.Select(item => string.Format(item, _currentDomain)))
			{
				errorList.AddRange(from hitem in _netInfoItem.HostState where hitem.HostName.StartsWith("http://") && hitem.HostName.Contains(domain) where !hitem.HostState select domain);
			}
			if (errorList.Count == 0) return true;
			if (_netInfoItem.CdnHostState
				.Where(cdn => cdn.CdnServer == cdnIp).
				Any(cdn => errorList.
					Any(domain => cdn.HostState.
						Any(hitem => hitem.HostName.StartsWith("http://") &&
							hitem.HostName.Contains(domain) && !hitem.HostState))))
			{
				return false;
			}
#else
			var errorList = new List<string>();
			foreach (var ditem in _netInfoItem.DnsResolveList)
			{
				if (ditem.HasError) errorList.Add(ditem.DomainName);
			}

			foreach (var item in DnsHostList)
			{
				var domain = string.Format(item, _currentDomain);
				foreach (var hitem in _netInfoItem.HostState)
				{
					if (hitem.HostName.StartsWith("http://") && hitem.HostName.Contains(domain))
					{
						if (!hitem.HostState)
						{
							errorList.Add(domain);
						}
					}
				}
			}


			foreach (var cdn in _netInfoItem.CdnHostState)
			{
				if (cdn.CdnServer == cdnIp)
				{
					foreach (var domain in errorList)
					{
						foreach (var hitem in cdn.HostState)
						{
							if (hitem.HostName.StartsWith("http://") && hitem.HostName.Contains(domain))
							{
								if (!hitem.HostState)
								{
									return false;
								}
							}
						}
					}
				}
			}
#endif
			if (errorList.Count == 0) return true;
			if (cdnIp == string.Empty) return false;

			SetHost(cdnIp, errorList);
			return true;
		}

		private void Write(string text)
		{
			Invoke(new MethodInvoker(() =>
			{
				tbMessage.AppendText(text);
				Application.DoEvents();
			}));
		}

		private void WriteLine()
		{
			Invoke(new MethodInvoker(() =>
			{
				tbMessage.AppendText(Environment.NewLine);
				Application.DoEvents();
			}));
		}

		private void WriteLine(string text)
		{
			Invoke(new MethodInvoker(() =>
			{
				tbMessage.AppendText(text + Environment.NewLine);
				Application.DoEvents();
			}));
		}

		private void WriteLine(string format, params object[] @params)
		{
			Invoke(new MethodInvoker(() =>
			{
				tbMessage.AppendText(string.Format(format, @params) + Environment.NewLine);
				Application.DoEvents();
			}));
		}

		private NetInfo.DnsResolveItem CheckHostDns(string domainName, IEnumerable<IPAddress> servers)
		{
			var dnsItem = new NetInfo.DnsResolveItem { DomainName = domainName };
			Write("正在检查DNS解析" + domainName + ": ");
			var list = GetHostIpByDnsServer(domainName, servers);
			dnsItem.DnsRecord = list;
#if NET4
			Write(string.Join(",", list));
			dnsItem.HasError = !list.Any(CheckDnsIp);
#else
			for (int i = 0; i < list.Length; i++)
			{
				if (i > 0) Write(",");
				Write(list[i]);
			}
			foreach (var item in list)
			{
				if (!CheckDnsIp(item))
				{
					dnsItem.HasError = true;
					break;
				}
			}
#endif
			WriteLine();
			return dnsItem;
		}

		private NetInfo.HostStateItem HeadHost(string url, string hostip = null)
		{
			var item = new NetInfo.HostStateItem { HostName = url };
			if (hostip == null)
			{
				WriteLine("检查服务器访问: {0}", url);
			}
			else
			{
				WriteLine("检查服务器访问 ByIP {0}: {1}", hostip, url);
			}
			HttpWebRequest request;

			if (hostip != null)
			{
				var ui = new Uri(url);
#if NET4
				request = (HttpWebRequest)WebRequest.Create(ui.Scheme + "://" + hostip);
				request.Host = ui.Host;
#else
				//2.0下采取设置代理的方式
				request = (HttpWebRequest)WebRequest.Create(url);
				request.Proxy = new WebProxy(hostip, ui.Scheme.ToLower() == "https" ? 443 : 80);
#endif
			}
			else
			{
				request = (HttpWebRequest)WebRequest.Create(url);
			}
			request.Method = "HEAD";
			request.Headers.Add("Accept-Language", "zh-CN");
			request.UserAgent = "Mozilla/5.0 (compatible; MSIE 11.0; Windows NT 6.1; Trident/7.0)";
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			request.SendChunked = false;

			try
			{
				var res = (HttpWebResponse)request.GetResponse();
				WriteLine("HTTP/{0} {1} {2}", res.ProtocolVersion, (int)res.StatusCode, res.StatusCode);
				//foreach (string head in res.Headers.Keys)
				//{
				//	WriteLine(head + ": " + res.Headers.Get(head));
				//}
				item.HostState = true;
			}
			catch (WebException ex)
			{
				var res = (HttpWebResponse)ex.Response;
				if (res != null)
				{
					WriteLine("HTTP/{0} {1} {2}", res.ProtocolVersion, (int)res.StatusCode, res.StatusCode);
					//foreach (string head in res.Headers.Keys)
					//{
					//	WriteLine(head + ": " + res.Headers.Get(head));
					//}
					item.HostState = true;
				}
				else
				{
					WriteLine(ex.Message);
				}
			}
			catch (Exception ex)
			{
				WriteLine(ex.Message);
			}
			WriteLine();
			return item;
		}

		private string[] GetHostIpByDnsServer(string hostName, IEnumerable<IPAddress> servers)
		{
			var req = new Request();
			var ques = new Question(hostName, DnsQType.A, DnsClass.IN);
			req.AddQuestion(ques);
			Response res = null;
			foreach (var ip in servers)
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
#if NET4
			return res.Answers.Where(x => x.Type == DnsType.A).Select(x => x.Record.ToString()).ToArray();
#else
			var answerlist = new List<string>();
			//
			foreach (var ans in res.Answers)
			{
				if (ans.Type == DnsType.A)
				{
					answerlist.Add(ans.Record.ToString());
				}
			}
			return answerlist.ToArray();
#endif
		}

		private IPInfo GetIpInfoFromIp138()
		{
			try
			{
				var web = new WebClient();
				var data = web.DownloadData("http://1111.ip138.com/ic.asp");
				var str = Encoding.Default.GetString(data);
				var re = new Regex(@"您的IP是：\[([^\]]*)\]\s来自：(.*)\s([^\<]*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var mc = re.Matches(str);
				if (mc.Count > 0)
				{
					return new IPInfo
					{
						IPAddress = mc[0].Groups[1].Value,
						Location = mc[0].Groups[2].Value,
						Idc = mc[0].Groups[3].Value
					};
				}
			}
			catch
			{
				;
			}
			return new IPInfo();
		}

		private IPInfo GetIpInfoFromIPcn()
		{
			try
			{
				var web = new WebClient();
				var data = web.DownloadData("http://ip.cn");
				var str = Encoding.UTF8.GetString(data);
				var re = new Regex(@"<p>当前 IP：<code>(.*?)</code>.*?来自：(.*?)\s(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var mc = re.Matches(str);
				if (mc.Count > 0)
				{
					return new IPInfo
					{
						IPAddress = mc[0].Groups[1].Value,
						Location = mc[0].Groups[2].Value,
						Idc = mc[0].Groups[3].Value
					};
				}
			}
			catch
			{
				;
			}
			return new IPInfo();
		}

		private IPInfo GetIpInfoFromQQ()
		{
			try
			{
				var web = new WebClient();
				var data = web.DownloadData("http://ip.qq.com");
				var str = Encoding.Default.GetString(data);
				var re = new Regex(@"<span class=""red"">([^\<]*)</span>.*?<p>该IP所在地为：<span>(.*?)&nbsp;(.*?)</span></p>",
					RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var mc = re.Matches(str);
				if (mc.Count > 0)
				{
					return new IPInfo
					{
						IPAddress = mc[0].Groups[1].Value,
						Location = mc[0].Groups[2].Value,
						Idc = mc[0].Groups[3].Value
					};
				}
			}
			catch
			{
				;
			}
			return new IPInfo();
		}

		private IPInfo GetIpInfoFromChinaz()
		{
			try
			{
				var web = new WebClient();
				var data = web.DownloadData("http://ip.chinaz.com");
				var str = Encoding.UTF8.GetString(data);
				var re = new Regex(@"<strong class=""red"">([^\<]*)</strong>[^\<]*<strong>([^\<]*)\s([^\<]*)</strong>",
					RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var mc = re.Matches(str);
				if (mc.Count > 0)
				{
					return new IPInfo
					{
						IPAddress = mc[0].Groups[1].Value,
						Location = mc[0].Groups[2].Value,
						Idc = mc[0].Groups[3].Value
					};
				}
			}
			catch
			{
				;
			}
			return new IPInfo();
		}

		private bool CheckDnsIp(string ip)
		{
			ip = ip.Trim();
			return ip.StartsWith("183.57.78.") || ip.StartsWith("223.95.76.") || ip.StartsWith("59.151.113.") || ip.StartsWith("211.57.0.");
		}

		private NetInfo.LocationItem GetIpLocation(string ip)
		{
			var li = new NetInfo.LocationItem { IpAddress = ip };
			try
			{
				var re = new Regex(@"""provincial"":""([^""]*)"".*""carrieroperator"":""([^""]*)""");

				var url = "http://util.chnedu.com/AreaInfoServerlet?ip={0}&time={1}&key={2}&code=gbk";
				var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				var key = Md5("fJ3UjIFyTu", time, ip).ToLower();
				url = string.Format(url, ip, time, key);
				var web = new WebClient();
				var data = web.DownloadData(url);
				var str = Encoding.Default.GetString(data);
				var mc = re.Matches(str);
				if (mc.Count > 0)
				{
					li.Province = mc[0].Groups[1].Value.Trim();
					li.Idc = mc[0].Groups[2].Value.Trim();
				}
			}
			catch
			{
				;
			}
			return li;
		}

		private string Md5(params string[] contents)
		{
			string content = string.Join(string.Empty, contents);
			byte[] buffer = Encoding.UTF8.GetBytes(content);
			using (var md5 = new MD5CryptoServiceProvider())
			{
				return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "");
			}
		}

		private void SetHost(string ip, IEnumerable<string> domainList = null)
		{
			var regIp = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$");
			var hostFile = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";
			var list = new List<string>();
			if (File.Exists(hostFile))
			{
				var lines = File.ReadAllLines(hostFile, Encoding.Default);
				foreach (string line in lines)
				{
					var str = line.Trim().ToLower();
					Trace.WriteLine(str);
					if (string.IsNullOrEmpty(str))
					{
						list.Add(string.Empty);
						continue;
					}
					if (str.StartsWith("#"))
					{
						list.Add(line);
						continue;
					}
#if NET4
					str = domainList != null ?
						domainList.Aggregate(str, (current, host) => current.Replace(host, string.Empty)) :
						DnsHostList.Aggregate(str, (current, host) => current.Replace(string.Format(host, _currentDomain), string.Empty));
#else
					if (domainList != null)
					{
						foreach (var host in domainList)
						{
							str = str.Replace(host, string.Empty);
						}
					}
					else
					{
						foreach (var host in DnsHostList)
						{
							str = str.Replace(string.Format(host, _currentDomain), string.Empty);
						}
					}
#endif
					str = str.Trim();
					if (regIp.IsMatch(str))
					{
						continue;
					}
					list.Add(str);
				}
			}
			if (!string.IsNullOrEmpty(ip))
			{
				var newline = ip + " ";
#if NET4
				newline = domainList != null ?
					domainList.Aggregate(newline, (current, host) => current + (" " + host)) :
					DnsHostList.Aggregate(newline, (current, host) => current + (" " + string.Format(host, _currentDomain)));
#else
				if (domainList != null)
				{
					foreach (var host in domainList)
					{
						newline += " " + host;
					}
				}
				else
				{
					foreach (var host in DnsHostList)
					{
						newline += " " + string.Format(host, _currentDomain);
					}
				}
#endif
				list.Add(newline);
#if NET4
				File.WriteAllLines(hostFile, list, Encoding.Default);
#else
				var sb = new StringBuilder();
				foreach (var line in list)
				{
					sb.AppendLine(line);
				}
				File.WriteAllText(hostFile, sb.ToString(), Encoding.Default);
#endif
			}
		}

		private void SaveNetInfo(string userName, string netInfo)
		{
			var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			var pkey = Md5(userName, Application.ProductVersion, time, "eiiskdui");
			var values = new NameValueCollection
			{
				{"pkey", pkey.ToLower()},
				{"time", time},
				{"version", Application.ProductVersion},
				{"userName", userName},
				{"content", netInfo}
			};
			try
			{
				var web = new WebClient();
				var data = web.UploadValues("http://manage.mobile.cdeledu.com/analysisApi/network/networkInsert.shtm", values);
				var str = Encoding.UTF8.GetString(data);
				//WriteLine(str);
			}
			catch (Exception ex)
			{

			}
		}

		private void BtnSetUC_Click(object sender, EventArgs e)
		{
			SetHost("211.157.0.5");
			//_eventLog.WriteEntry("设置为联通Host", EventLogEntryType.Information);
		}

		private void BtnSetCT_Click(object sender, EventArgs e)
		{
			SetHost("219.153.76.76");
			//_eventLog.WriteEntry("设置为电信Host", EventLogEntryType.Error);
		}

		private void BtnSetCM_Click(object sender, EventArgs e)
		{
			SetHost("183.203.15.75");
			//_eventLog.WriteEntry("设置为移动Host", EventLogEntryType.Warning);
		}

		private void BtnSetNone_Click(object sender, EventArgs e)
		{
			SetHost(null);
			//_eventLog.WriteEntry("清空Host", EventLogEntryType.Information);
		}
	}
}
