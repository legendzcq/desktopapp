namespace Framework.Model
{
	public class StudentCwareDownload
	{
		public long DownId { get; set; }
		public int CwareId { get; set; }
		public string VideoId { get; set; }
		public string DownUrl { get; set; }
		public string LocalFile { get; set; }
		public int DownState { get; set; }
		public int DownType { get; set; }
		public int DownOrder { get; set; }
		public int ForImport { get; set; }
        public string ModTime { get; set; }
	}
}