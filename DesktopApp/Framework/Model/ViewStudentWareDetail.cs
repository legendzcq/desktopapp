
using System;

namespace Framework.Model
{
	public class ViewStudentWareDetail
	{ /// <summary>
		/// 编号
		/// </summary>
		public int CwareId { get; set; }

		/// <summary>
		/// 章节ID
		/// </summary>
		public int ChapterId { get; set; }

		/// <summary>
		/// 章节名称
		/// </summary>
		public string ChapterName { get; set; }
		/// <summary>
		/// 视频编号
		/// </summary>
		public string VideoId { get; set; }
		/// <summary>
		/// 视频名称
		/// </summary>
		public string VideoName { get; set; }

		public string Title { get; set; }
		/// <summary>
		/// 视频长度
		/// </summary>
		public string VideoLength { get; set; }
		/// <summary>
		/// 状态
		/// -2 没有下载链接,-1 未下载，0,等待中,1,正在下载，2，暂停，3，已完成，4，下载失败
		/// </summary>
		/// <remarks>-2 没有下载链接,-1 未下载，0,等待中,1,正在下载，2，暂停，3，已完成，4，下载失败</remarks>
		public int VideoState { get; set; }
		/// <summary>
		/// 视频路径
		/// </summary>
		public string VideoPath { get; set; }

		//public int LastPosition { get; set; }

		public long DownId { get; set; }

		public int VideoType { get; set; }
		/// <summary>
		/// 课件更新时间
		/// </summary>
		public string ModTime { get; set; }

		public string VideoModTime { get; set; }

		/// <summary>
		/// 课件是否更新 0 否 1是
		/// </summary>
		public bool IsUpdate
		{
			get
			{
				DateTime mTime = DateTime.TryParse(ModTime, out mTime) ? mTime : DateTime.MinValue;
				DateTime vTime = DateTime.TryParse(VideoModTime, out vTime) ? vTime : DateTime.MinValue;
				return mTime > vTime && !string.IsNullOrEmpty(VideoPath);
			}
		}
	}
}
