using System.Collections.Generic;

namespace Framework.Model
{
    public class ViewStudentQuestion
    {
        public ViewStudentQuestion()
        {
            OptionList = new List<StudentQuestionOption>();
        }
        public int MainId { get; set; }
        public int SubId { get; set; }
        internal int SubCnt { get; set; }

        public string PartName { get; set; }
        public int PartSequence { get; set; }

        public int Sequence { get; set; }

        public string Num { get; set; }
        public string QuestionTypeName { get; set; }
        public int QuestionId { get; set; }
        public int ParentId { get; set; }
        public int QuesTypeId { get; set; }
        public int QuesViewType { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public string Analysis { get; set; }
        public double Score { get; set; }
        public string UserAnswer { get; set; }
        public double UserScore { get; set; }
        public bool IsDone { get; set; }
        public bool IsFav { get; set; }
        public bool IsWrong { get; set; }
        public ViewStudentQuestion Parent { get; set; }
        public List<StudentQuestionOption> OptionList { get; set; }
    }
}
