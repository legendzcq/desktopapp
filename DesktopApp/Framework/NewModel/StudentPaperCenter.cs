using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentPaperCenterReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentPaperCenterResult Result { get; set; }
    }

    [DataContract]
	public class StudentPaperCenterResult
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "centerList")]
		public IEnumerable<StudentPaperCenter> CenterList { get; set; }
	}

	[DataContract]
	public class StudentPaperCenter
	{
		[DataMember(Name = "createTime")]
		public string CreateTime { get; set; }

		[DataMember(Name = "centerParam")]
		public string CenterParam { get; set; }

		[DataMember(Name = "siteCourseID")]
		public int SiteCourseId { get; set; }

		[DataMember(Name = "sequence")]
		public /*int*/string Sequence { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "centerName")]
		public string CenterName { get; set; }

		[DataMember(Name = "openStatus")]
		public /*int*/string OpenStatus { get; set; }

		[DataMember(Name = "centerYear")]
		public /*int*/string CenterYear { get; set; }

		[DataMember(Name = "centerID")]
		public /*int*/string CenterId { get; set; }

		[DataMember(Name = "centerType")]
		public /*int*/string CenterType { get; set; }

		[DataMember(Name = "creator")]
		public int Creator { get; set; }
	}
    [DataContract]
    public class StudentPaperSubmitCnt
    {
        [DataMember(Name = "paperViewID")]
        public int paperViewID { get; set; }

        [DataMember(Name = "submitTimes")]
        public int SubmitTimes { get; set; }

        [DataMember(Name = "userID")]
        public int UserID { get; set; }
    }
}
