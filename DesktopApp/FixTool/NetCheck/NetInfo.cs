using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NetCheck
{
	[DataContract]
	public class NetInfo
	{
		[DataMember(Name = "UserCode", Order = 0)]
		public string UserCode { get; set; }
		[DataMember(Name = "DnsRecord", Order = 1)]
		public IEnumerable<string> DnsRecord { get; set; }
		[DataMember(Name = "DnsResolveList", Order = 2)]
		public IEnumerable<DnsResolveItem> DnsResolveList { get; set; }
		[DataMember(Name = "HostState", Order = 3)]
		public IEnumerable<HostStateItem> HostState { get; set; }
		[DataMember(Name = "CdnHostState", Order = 4)]
		public IEnumerable<CdnHostStateItem> CdnHostState { get; set; }

		[IgnoreDataMember]
		public List<IPInfo> CurrentIpInfo { get; set; }

		[DataMember(Name = "Location", Order = 5)]
		public LocationItem Location { get; set; }

#if !NET4
		public string GetJsonString()
		{
			var sb = new StringBuilder();
			sb.Append("{");
			sb.AppendFormat("\"UserCode\":{0}", UserCode == null ? "null" : "\"" + UserCode + "\"");

			var idx = 0;
			sb.Append(",\"DnsRecord\":[");
			foreach (var rec in DnsRecord)
			{
				sb.AppendFormat(idx == 0 ? "\"{0}\"" : ",\"{0}\"", rec);
				idx++;
			}
			sb.Append("]");
			sb.Append(",\"DnsResolveList\":[");
			idx = 0;
			foreach (var item in DnsResolveList)
			{
				sb.AppendFormat(idx == 0 ? "{0}" : ",{0}", item.GetJsonString());
				idx++;
			}
			sb.Append("]");
			sb.Append(",\"HostState\":[");
			idx = 0;
			foreach (var item in HostState)
			{
				sb.AppendFormat(idx == 0 ? "{0}" : ",{0}", item.GetJsonString());
				idx++;
			}
			sb.Append("]");
			sb.Append(",\"CdnHostState\":[");
			idx = 0;
			foreach (var item in CdnHostState)
			{
				sb.AppendFormat(idx == 0 ? "{0}" : ",{0}", item.GetJsonString());
				idx++;
			}
			sb.Append("]");
			sb.Append(",\"Location\":");
			sb.Append(Location.GetJsonString());
			sb.Append("}");
			return sb.ToString();
		}
#endif
		public class DnsResolveItem
		{
			public string DomainName { get; set; }

			public IEnumerable<string> DnsRecord { get; set; }

			public bool HasError { get; set; }
#if !NET4
			public string GetJsonString()
			{
				var sb = new StringBuilder();
				sb.Append("{");
				sb.AppendFormat("\"DomainName\":\"{0}\"", DomainName);
				sb.Append(",\"DnsRecord\":[");
				var idx = 0;
				foreach (var rec in DnsRecord)
				{
					sb.AppendFormat(idx == 0 ? "\"{0}\"" : ",\"{0}\"", rec);
					idx++;
				}
				sb.Append("]}");
				return sb.ToString();
			}
#endif
		}

		public class HostStateItem
		{
			public string HostName { get; set; }

			public bool HostState { get; set; }
#if !NET4
			public string GetJsonString()
			{
				return string.Format("{{\"HostName\":\"{0}\",\"HostState\":{1}}}", HostName, HostState ? "true" : "false");
			}
#endif
		}

		public class CdnHostStateItem
		{
			public string CdnServer { get; set; }

			public IEnumerable<HostStateItem> HostState { get; set; }

#if !NET4
			public string GetJsonString()
			{
				var sb = new StringBuilder();
				sb.Append("{");
				sb.AppendFormat("\"CdnServer\":\"{0}\"", CdnServer);
				sb.Append(",\"HostState\":[");
				var idx = 0;
				foreach (var rec in HostState)
				{
					sb.AppendFormat(idx == 0 ? "{0}" : ",{0}", rec.GetJsonString());
					idx++;
				}
				sb.Append("]}");
				return sb.ToString();
			}
#endif
		}

		public class LocationItem
		{
			public string IpAddress { get; set; }
			public string Province { get; set; }
			public string Idc { get; set; }

#if !NET4
			public string GetJsonString()
			{
				return string.Format("{{\"IpAddress\":\"{0}\",\"Province\":\"{1}\",\"Idc\":\"{2}\"}}", IpAddress, Province, Idc);
			}
#endif
		}
	}
}