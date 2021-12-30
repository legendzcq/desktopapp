namespace Framework.Model
{
    public class ViewStudentQuestionResult
    {
        public int PaperViewId { get; set; }
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; }
        public int IsRight { get; set; }
        public bool IsFav { get; set; }
    }
}
