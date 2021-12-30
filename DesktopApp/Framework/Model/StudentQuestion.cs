using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Model
{
    [DataContract]
    public class StudentQuestionReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentQuestionResult Result { get; set; }
    }

    [DataContract]
    public class StudentQuestionResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "commonInfo")]
        public PaperCommonInfo CommonInfo { get; set; }

        [DataMember(Name = "questionList")]
        public IEnumerable<StudentQuestion> QuestionList { get; set; }
    }

    // 公用试卷信息
    [DataContract]
    public class PaperCommonInfo
    {
        // 暂不使用
        [DataMember(Name = "paperBizKey")]
        public string PaperBizKey { get; set; }

        // 暂不使用
        [DataMember(Name = "statVer")]
        public int StatVer { get; set; }

        [DataMember(Name = "version")]
        public int Version { get; set; } = 1;
    }

    [DataContract]
    public class StudentQuestion
    {
        public int PaperViewId { get; set; }

        [DataMember(Name = "questionID")]
        public int QuestionId { get; set; }

        [DataMember(Name = "parentID")]
        public int ParentId { get; set; }

        [DataMember(Name = "quesTypeID")]
        public int QuesTypeId { get; set; }

        [DataMember(Name = "quesViewType")]
        public int QuesViewType { get; set; }

        /// <summary>
        /// 问题内容
        /// </summary>
        [DataMember(Name = "content")]
        public string Content { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [DataMember(Name = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// 解析
        /// </summary>
        [DataMember(Name = "analysis")]
        public string Analysis { get; set; }

        [DataMember(Name = "limitMinute")]
        public int LimitMinute { get; set; }

        [DataMember(Name = "score")]
        public double Score { get; set; }

        [DataMember(Name = "splitScore")]
        public double SplitScore { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "lecture")]
        public string Lecture { get; set; }

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

        [DataMember(Name = "modifyStatus")]
        public int ModifyStatus { get; set; }

        public string WrongRate { get; set; }

        [DataMember(Name = "sequence")]
        public int Sequence { get; set; }

        #region 新加
        [DataMember(Name = "optNum")]
        public int? OptNum { get; set; }

        [DataMember(Name = "qppsequence")]
        public int? QppSequence { get; set; }

        [DataMember(Name = "rowNum")]
        public int? RowNum { get; set; }
        #endregion
    }
}
