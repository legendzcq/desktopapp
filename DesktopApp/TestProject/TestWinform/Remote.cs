using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace TestWinform
{
	public class Remote
	{
		private const string NewCourseSalt = "fJ3UjIFyTu";

		internal const string GetStudentCourseJsonUrl = "http://member.chinaacc.com/mobile/classroom/course/getUserCourse.shtm";

		/// <summary>
		/// MD5
		/// </summary>
		/// <param name="contents"></param>
		/// <returns></returns>
		internal static string Md5(params string[] contents)
		{
			string content = string.Join(string.Empty, contents);
			byte[] buffer = Encoding.UTF8.GetBytes(content);
			using (var md5 = new MD5CryptoServiceProvider())
			{
				return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "");
			}
		}

		public static string GetNowString()
		{
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public byte[] GetStudentCourseNew(string sid)
		{
			var time = GetNowString();
			string pkey = Md5(sid, time, NewCourseSalt);
			var values = new NameValueCollection
            {
                {"sid", sid},
                {"pkey", pkey.ToLower()},
                {"time", time},
            };
			var web = new WebProxyClient();
			try
			{
				var buffer = web.UploadValues(GetStudentCourseJsonUrl, values);
				return buffer;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				return null;
			}
		}
	}

	[DataContract]
	public class StudentCourse
	{
		[DataMember(Name = "uid")]
		public string Uid { get; set; }

		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "myCourseInfo")]
		public IEnumerable<CourseItem> MyCourseInfo { get; set; }

		[DataContract]
		public class CourseItem
		{
			[DataMember(Name = "boardID")]
			public string BoardId { get; set; }

			[DataMember(Name = "uid")]
			public string Uid { get; set; }

			[DataMember(Name = "title")]
			public string Title { get; set; }

			[DataMember(Name = "eduSubjectID")]
			public int EduSubjectId { get; set; }

			[DataMember(Name = "courseID")]
			public string CourseId { get; set; }

			[DataMember(Name = "disporder")]
			public int Disporder { get; set; }

			[DataMember(Name = "mobileTitle")]
			public string MobileTitle { get; set; }

			[DataMember(Name = "dateEnd")]
			public string DateEnd { get; set; }

			[DataMember(Name = "downLoad")]
			public string DownLoad { get; set; }

			[DataMember(Name = "courseEduID")]
			public int CourseEduId { get; set; }
		}
	}
}
