
namespace Framework.Model
{
    public class ViewStudentPaper
    {
        public int PaperViewId { get; set; }
        public string PaperViewName { get; set; }
        public int AllCnt { get; set; }
        public int DoCnt { get; set; }
        public int FavCnt { get; set; }
        public int WrongCnt { get; set; }
        /// <summary>
        /// 限制提交的次数
        /// </summary>
        public string ContestTimes { get; set; }
        // 试卷公共信息——版本号，用于解决提交的答案在线上不显示的Bug
        public int Version { get; set; }
    }
}
