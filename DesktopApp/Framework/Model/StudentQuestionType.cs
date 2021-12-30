using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Model
{
    [DataContract]
    public class StudentQuestionType
    {
        /// <summary>
        /// 题型
        /// </summary>
        [DataMember(Name = "quesViewType")]
        public int QuesViewType { get; set; }

        /// <summary>
        /// 题型名称
        /// </summary>
        [DataMember(Name = "viewTypeName")]
        public string ViewTypeName { get; set; }

        /// <summary>
        /// 基础题型id
        /// </summary>
        [DataMember(Name = "quesTypeID")]
        public int QuesTypeId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// 0否，1是
        /// </summary>
        [DataMember(Name = "status")]
        public int Status { get; set; }

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
        /// 试卷题型名称
        /// </summary>
        [DataMember(Name = "paperTypeName")]
        public string PaperTypeName { get; set; }
    }
    [DataContract]
    public class StudentQuestionTypeReturnItem
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "questionTypeList")]
        public IEnumerable<StudentQuestionType> QuestionTypeList { get; set; }
    }
}
