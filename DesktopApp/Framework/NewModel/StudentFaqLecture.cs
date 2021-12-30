using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentFaqLecture
    {
        [DataMember(Name = "answerHours")]
        public string AnswerHours { get; set; }
        [DataMember(Name = "boardID")]
        public int BoardId { get; set; }
        [DataMember(Name = "faqID")]
        public int FaqId { get; set; }
        [DataMember(Name = "topicID")]
        public int TopicId { get; set; }
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }
    }
}
