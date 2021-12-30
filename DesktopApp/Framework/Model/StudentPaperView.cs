using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Model
{
    [DataContract]
    public class StudentPaperViewReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentPaperViewResult Result { get; set; }
    }

    [DataContract]
    public class StudentPaperViewResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "questionTypeList")]
        public IEnumerable<StudentPaperView> QuestionTypeList { get; set; }
    }

    [DataContract]
    public class StudentPaperView
    {
        public int CenterId { get; set; }

        [DataMember(Name = "paperViewID")]
        public int PaperViewId { get; set; }

        [DataMember(Name = "paperViewName")]
        public string PaperViewName { get; set; }

        [DataMember(Name = "paperID")]
        public int PaperId { get; set; }

        [DataMember(Name = "paperParam")]
        public string PaperParam { get; set; }

        [DataMember(Name = "openStatus")]
        public string OpenStatus { get; set; }

        [DataMember(Name = "explainURL")]
        public string ExplainUrl { get; set; }

        [DataMember(Name = "isContest")]
        public int IsContest { get; set; }
        /// <summary>
        /// 做题次数>0有次数限制
        /// </summary>
        [DataMember(Name = "contestTimes")]
        public string ContestTimes { get; set; }

        [DataMember(Name = "contestStartTime")]
        public string ContestStartTime { get; set; }

        [DataMember(Name = "contestEndTime")]
        public string ContestEndTime { get; set; }

        [DataMember(Name = " contestTimeLimit")]
        public string ContestTimeLimit { get; set; }

        [DataMember(Name = "dayiID")]
        public string DayiId { get; set; }

        [DataMember(Name = "doneCount")]
        public string DoneCount { get; set; }

        [DataMember(Name = "avgScore")]
        public string AvgScore { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "creator")]
        public string Creator { get; set; }

        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "paperViewCatID")]
        public string PaperViewCatId { get; set; }

        [DataMember(Name = "modifyStatus")]
        public string ModifyStatus { get; set; }

        [DataMember(Name = "paperType")]
        public string PaperType { get; set; }

        [DataMember(Name = "sequence")]
        public int Sequence { get; set; }

        #region 新加 dgh 2018.01.15
        [DataMember(Name = "paperOpenStatus")]
        public int PaperOpenStatus { get; set; }
        #endregion
    }
}
