using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    [DataContract]
    public class TokenReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public TokenResult Result { get; set; }
    }
    [DataContract]
    public class TokenResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "paramValue")]
        public string ParamValue { get; set; }
    }
    [DataContract]
    public class TokenValue
    {
        [DataMember(Name = "token")]
        public string TokenString { get; set; }
        [DataMember(Name = "longtime")]
        public long LongTime { get; set; }
        [DataMember(Name = "timeout")]
        public int Timeout { get; set; }
    }
}
