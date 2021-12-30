using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Model
{
    [DataContract]
    public class StudentPaperReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentPaperResult Result { get; set; }
    }

    [DataContract]
    public class StudentPaperResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "centerPaperList")]
        public IEnumerable<StudentPaper> CenterPaperList { get; set; }
    }

    [DataContract]
    public class StudentPaper
    {
        public int CenterId { get; set; }
        /// <summary>
        /// 试卷id
        /// </summary>
        [DataMember(Name = "paperID")]
        public int PaperId { get; set; }

        [DataMember(Name = "paperCatID")]
        public int PaperCatId { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        [DataMember(Name = "courseID")]
        public int CourseId { get; set; }

        [DataMember(Name = "paperYear")]
        public int PaperYear { get; set; }

        [DataMember(Name = "chapter")]
        public string Chapter { get; set; }

        [DataMember(Name = "suitNum")]
        public int SuitNum { get; set; }
        /// <summary>
        /// 试卷名称
        /// </summary>
        [DataMember(Name = "paperName")]
        public string PaperName { get; set; }

        /// <summary>
        /// 试卷总分
        /// </summary>
        [DataMember(Name = " totalScore")]
        public string TotalScore { get; set; }

        /// <summary>
        /// 0否，1是
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember(Name = "creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }
        /// <summary>
        /// 章节ID
        /// </summary>
        [DataMember(Name = "chapterID")]
        public string ChapterId { get; set; }
        #region 新添加的字段 dgh 2018.01.12
        /// <summary>
        /// 做题建议时间
        /// </summary>
        [DataMember(Name = "contestTimeLimit")]
        public string ContestTimeLimit { get; set; }
        /// <summary>
        /// 试卷限制提交次数
        /// </summary>
        [DataMember(Name = "contestTimes")]
        public string ontestTimes { get; set; }

        /// <summary>
        /// 是否竞赛，0否，1是
        /// </summary>
        [DataMember(Name = "isContest")]
        public string IsContest { get; set; }

        [DataMember(Name = "paperOpenStatus")]
        public string PaperOpenStatus { get; set; }

        [DataMember(Name = "paperViewID")]
        public string PaperViewID { get; set; }

        [DataMember(Name = "paperViewName")]
        public string PaperViewName { get; set; }

        [DataMember(Name = "quesNum")]
        public int QuesNum { get; set; }

        [DataMember(Name = "sequence")]
        public int Sequence { get; set; }
        #endregion
    }
}
