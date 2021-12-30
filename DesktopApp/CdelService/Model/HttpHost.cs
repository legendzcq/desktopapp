using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CdelService.Model
{
    [DataContract]
    internal class HttpHost
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "iplist")]
        public IpArr IpList { get; set; }

        [DataContract]
        public class IpArr
        {
            [DataMember(Name = "ip1")]
            public string Ip1 { get; set; }
            [DataMember(Name = "ip2")]
            public string Ip2 { get; set; }
            [DataMember(Name = "ip3")]
            public string Ip3 { get; set; }
            [DataMember(Name = "ip4")]
            public string Ip4 { get; set; }
            [DataMember(Name = "ip5")]
            public string Ip5 { get; set; }
            [DataMember(Name = "ip6")]
            public string Ip6 { get; set; }

            public string[] ToArray()
            {
                var list = new List<string>();
                if (!string.IsNullOrWhiteSpace(Ip1)) list.Add(Ip1);
                if (!string.IsNullOrWhiteSpace(Ip2)) list.Add(Ip2);
                if (!string.IsNullOrWhiteSpace(Ip3)) list.Add(Ip3);
                if (!string.IsNullOrWhiteSpace(Ip4)) list.Add(Ip4);
                if (!string.IsNullOrWhiteSpace(Ip5)) list.Add(Ip5);
                if (!string.IsNullOrWhiteSpace(Ip6)) list.Add(Ip6);
                return list.ToArray();
            }
        }
    }
}