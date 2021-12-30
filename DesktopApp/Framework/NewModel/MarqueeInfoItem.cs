using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    [DataContract]
    public class MarqueeInfoReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public MarqueeInfoItem Result { get; set; }
    }

    [DataContract]
	public class MarqueeInfoItem
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "msg")]
		public string Message { get; set; }

		[DataMember(Name = "marqueeInfoList")]
		public IEnumerable<MarqueeInfoListItem> MarqueeList { get; set; }
	}

	[DataContract]
	public class MarqueeInfoListItem
	{
		[DataMember(Name = "fontColour")]
		public string FontColor { get; set; }

		[DataMember(Name = "pushContent")]
		public string PushContent { get; set; }
	}
}
