
namespace Framework.Model
{
    public class StudentCWareDown
    {
        public long DownId { get; set; }
        public int CwareId { get; set; }
        public string VideoId { get; set; }
        public string Url { get; set; }
        public string LocalFile { get; set; }
        public int State { get; set; }
        public double Rate { get; set; }
        //课件更新时间
        public string ModTime { get; set; }
    }
}
