namespace Framework.Model
{
    public class ViewStudentCenter
    {
        public int SiteCourseId { get; set; }
        public string SiteCourseName { get; set; }
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public int AllCnt { get; set; }
        public int FavCnt { get; set; }
        public int WrongCnt { get; set; }
        public int DoCnt { get; set; }
        public string Rate
        {
            get
            {
                if (AllCnt == 0) return "0.0%";
                var ss = ((double)DoCnt - WrongCnt) / AllCnt;
                return ss.ToString("0.0%");
            }
        }

        public string UpdateTime { get; set; }
    }
}
