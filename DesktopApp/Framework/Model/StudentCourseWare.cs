
namespace Framework.Model
{
	public class StudentCourseWare
	{
		public string CourseId { get; set; }
		public int CWareId { get; set; }
		public string CwId { get; set; }
		public string PathUrl { get; set; }
		public string Name { get; set; }
		public string CWareClassName { get; set; }
		public int CwareClassId { get; set; }
		public int ClassOrder { get; set; }
		public string CYearName { get; set; }
		public string CTeacherName { get; set; }
		public string CWareImg { get; set; }
		public string VideoType { get; set; }
		public string CwareTitle { get; set; }
		/// <summary>
		/// 视频的老方法加密密钥
		/// </summary>
		public string CwareKey { get; set; }
		public int IsOpen { get; set; }
	}
}
