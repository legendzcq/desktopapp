using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentPaperScoresReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "retry")]
        public bool Retry { get; set; }
        [DataMember(Name = "result")]
        public StudentPaperScoresResult Result { get; set; }
    }

    [DataContract]
    public class StudentPaperScoresResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "paperScores")]
        public List<StudentPaperScore> PaperScores { get; set; }
        [DataMember(Name = "finalList")]
        public List<StudentPaperFinalList> FinalList { get; set; }
    }

    [DataContract]
    public class StudentPaperScore
    {
        /// <summary>
        /// 默认为0
        /// </summary>
        [DataMember(Name = "paperScoreID")]
        public string PaperScoreID { get; set; }
        /// <summary>
        /// 试卷得分
        /// </summary>
        [DataMember(Name = "autoScore")]
        public string AutoScore { get; set; }
        /// <summary>
        /// 考试中心id
        /// </summary>
        [DataMember(Name = "centerID")]
        public int CenterID { get; set; }
        /// <summary>
        /// 做题时间
        /// </summary>
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }
        /// <summary>
        /// 试卷得分
        /// </summary>
        [DataMember(Name = "lastScore")]
        public string LastScore { get; set; }
        /// <summary>
        /// 试卷总分
        /// </summary>
        [DataMember(Name = "paperScore")]
        public string PaperScore { get; set; }
        /// <summary>
        /// 对外试卷id
        /// </summary>
        [DataMember(Name = "paperViewID")]
        public int PaperViewID { get; set; }
        /// <summary>
        /// 对外课程id
        /// </summary>
        [DataMember(Name = "siteCourseID")]
        public int SiteCourseID { get; set; }
        /// <summary>
        /// 做题用时
        /// </summary>
        [DataMember(Name = "spendTime")]
        public string SpendTime { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [DataMember(Name = "userID")]
        public int UserID { get; set; }

        // 提交时附带试卷版本信息，用于解决交卷后线上web解析页面不显示学员的答案的问题
        [DataMember(Name = "version")]
        public int Version { get; set; } = 1;

        /// <summary>
        /// 用户答案集合
        /// </summary>
        [DataMember(Name = "answers")]
        public List<StudentPaperScoreAnswer> Answers { get; set; }
    }

    [DataContract]
    public class StudentPaperScoreAnswer
    {
        /// <summary>
        /// 默认为0
        /// </summary>
        public int PaperScoreID { get; set; }
        /// <summary>
        /// 题目id
        /// </summary>
        [DataMember(Name = "questionID")]
        public int QuestionID { get; set; }
        /// <summary>
        /// 用户答案
        /// </summary>
        [DataMember(Name = "userAnswer")]
        public string UserAnswer { get; set; }
        /// <summary>
        /// 用户得分
        /// </summary>
        [DataMember(Name = "userScore")]
        public string UserScore { get; set; }
    }

    [DataContract]
    public class StudentPaperResultFromSave
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        [DataMember(Name = "paperNewScoreID")]
        public int PaperNewScoreID { get; set; }
        [DataMember(Name = "paperOldScoreID")]
        public int PaperOldScoreID { get; set; }
        /// <summary>
        /// 做题时间
        /// </summary>
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }
    }
    
    [DataContract]
    public class StudentPaperFinalList
    {
        [DataMember(Name = "result")]
        public List<StudentPaperResultFromSave> PaperResult { get; set; }
        [DataMember(Name = "guid")]
        public string PaperGuid { get; set; }
        [DataMember(Name = "code")]
        public string Code { get; set; }
    }
}
