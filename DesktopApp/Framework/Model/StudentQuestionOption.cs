
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Framework.Model
{
    [DataContract]
    public class StudentQuestionOptionReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentQuestionOptionResult Result { get; set; }
    }

    [DataContract]
    public class StudentQuestionOptionResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "msg")]
        public string Msg { get; set; }

        [DataMember(Name = "questionOptionList")]
        public IEnumerable<StudentQuestionOption> QuestionOptionList { get; set; }
    }

    [DataContract]
    public class StudentQuestionOption
    {
        [DataMember(Name = "questionID")]
        public int QuestionId { get; set; }
        /// <summary>
        /// 选项文本值
        /// </summary>
        [DataMember(Name = "quesOption")]
        public string QuesOption { get; set; }

        [DataMember(Name = "quesValue")]
        public string QuesValue { get; set; }

        [DataMember(Name = "sequence")]
        public int Sequence { get; set; }
    }
}
