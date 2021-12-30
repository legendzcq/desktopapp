namespace Framework.Model
{
	public class ViewStudentCwareDownLoad
	{
		public long DownId { get; set; }
		public int CwareId { get; set; }
		public string VideoId { get; set; }
		public string VideoName { get; set; }
		public string DownUrl { get; set; }
		public string LocalFile { get; set; }
		public int DownState { get; set; }
		public int DownType { get; set; }
		public int DownOrder { get; set; }
		public int ForImport { get; set; }
        public string ModeTime { get; set; }
        /// <summary>
        /// 带参数的下载路径
        /// </summary>
        public string ParmDownUrl { get; set; }
	}
}