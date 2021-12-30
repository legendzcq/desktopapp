using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Framework.Local;
using Framework.Model;
using Framework.Remote;
using Framework.Utility;

namespace Framework.Import
{
	internal static class Helper
	{
		static readonly Regex Re = new Regex(@"<div\sonMouseOver=""""><a.*?id=""(.*?)"".*?>(.*?)</a></div>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		private static readonly Regex ReNew =
			new Regex(@"<div\sonMouseOver=""""><a.*?id=""([^""]*)""[^>]*title=""[^""]*\(([^\)]*)\)""[^>]*>(.*?)</a></div>",
				RegexOptions.Singleline | RegexOptions.IgnoreCase);
		static readonly Regex ReImg = new Regex(@"<img(.*?)src=""(.*?)""(.*?)>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

		/// <summary>
		/// 获取基本配置信息
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ZipConfig GetConfigFromXml(string xml)
		{
			//处理章节中的 & 连字符导致的无法解析xml的问题
			//xml = xml.Replace("&", "&amp;");
			var doc = XDocument.Parse(xml);
			var res = doc.Element("res");
			if (res == null) return null;
			var item = new ZipConfig
			{
				CwareId = res.GetString("CwareID"),
				CwareName = res.GetString("CwareName"),
				CwId = res.GetInt("cwid"),
				PathUrl = res.GetString("pathurl"),
				PlayType = res.GetInt("PlayType"),
				VideoId = res.GetString("VideoID"),
				VideoName = res.GetString("VideoName"),
				VideoUrl = res.GetString("VideoUrl"),
				Ts = res.GetLong("ts"),
				VideoType = res.GetInt("VideoType", -1),
				HMd5 = res.GetString("hmd5", string.Empty)
			};
			return item;
		}

		/// <summary>
		/// 从导入的文件中的xml文件读出讲义及时间点
		/// </summary>
		/// <param name="config"></param>
		/// <param name="timeNodeString"></param>
		/// <param name="kcjyHtml"></param>
		/// <param name="imgPath"></param>
		public static void GetKcjyAndTimeNodeFromXml(ZipConfig config, string timeNodeString, string kcjyHtml, string imgPath)
		{
			if (timeNodeString == null) throw new ArgumentNullException("timeNodeString");
			if (kcjyHtml == null) throw new ArgumentNullException("kcjyHtml");
			var local = new StudentWareData();
			if (ReNew.IsMatch(kcjyHtml))
			{
				//读新讲义
				var mc = ReNew.Matches(kcjyHtml);
				var kcjyList = (from Match m in mc
								let nodeid = m.Groups[1].Value
								let timestring = m.Groups[2].Value
								select new StudentCwareKcjy
								{
									CWareId = config.CwId,
									VideoId = config.VideoId,
									NodeId = nodeid,
									NodeText = DealImg(m.Groups[3].Value, imgPath, config.CwId, config.VideoId),
									TimeStart = timestring,
									VideoTime = GetTimeSecondFromString(timestring)
								}).ToList();
				local.AddCourseKcjyList(kcjyList);
				return;
			}
			try
			{
				//读旧讲义
				var doc = XDocument.Parse(timeNodeString);
				var ret = doc.Element("ret");
				if (ret == null) return;
				List<StudentTimeNode> timeList = ret.Elements("timeNode").Select(element => new StudentTimeNode
				{
					CWareId = config.CwId,
					VideoId = config.VideoId,
					FlashUrl = element.GetString("FlashUrl").Replace(@"""", string.Empty),
					NodeId = element.GetString("id").Replace(@"""", string.Empty),
					Timestart = element.GetString("timestart").Replace(@"""", string.Empty),
					TimeEnd = element.GetString("timeEnd").Replace(@"""", string.Empty)
				}).ToList();

				local.AddCourseTimeNodeList(timeList);
				var mc = Re.Matches(kcjyHtml);
				var kcjyList = (from Match m in mc
								let nodeid = m.Groups[1].Value
								let titem = timeList.FirstOrDefault(x => x.NodeId == nodeid)
								select new StudentCwareKcjy
								{
									CWareId = config.CwId,
									VideoId = config.VideoId,
									NodeId = nodeid,
									NodeText = DealImg(m.Groups[2].Value, imgPath, config.CwId, config.VideoId),
									TimeStart = titem == null ? string.Empty : titem.Timestart,
									VideoTime = titem == null ? 0 : GetTimeSecondFromString(titem.Timestart)
								}).ToList();
				local.AddCourseKcjyList(kcjyList);
			}
			catch (Exception ex)
			{
				Log.RecordLog(ex.ToString());
			}
		}

		/// <summary>
		/// 获取时间串指定的秒数
		/// </summary>
		/// <returns></returns>
		private static int GetTimeSecondFromString(string time)
		{
			var dt = DateTime.Parse(time);
			return dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
		}

		/// <summary>
		/// 处理讲义中的图片
		/// </summary>
		/// <returns></returns>
		private static string DealImg(string content, string imgPath, int cwareId, string videoId)
		{
			try
			{
				var mc = ReImg.Matches(content);
				foreach (Match m in mc)
				{
					try
					{
						string imgp = m.Groups[0].Value;
						string imgf = m.Groups[2].Value.Trim();

						imgf = imgf.ToLower().StartsWith("http://") ? DealRemoteImage(imgf, cwareId, videoId) : DealLocalImage(imgPath + imgf, cwareId, videoId);
						string imgo = "<img" + m.Groups[1].Value + @"src=""" + imgf + @"""" + m.Groups[3].Value + ">";
						content = content.Replace(imgp, imgo);
					}
					catch (Exception ex)
					{
						Log.RecordLog(ex.ToString());
					}
				}
				return content;
			}
			catch
			{
				return content;
			}
		}

		/// <summary>
		/// 处理讲义中的远程图片
		/// </summary>
		/// <param name="url"></param>
		/// <param name="cwareId"></param>
		/// <param name="videoId"></param>
		/// <returns></returns>
		private static string DealRemoteImage(string url, int cwareId, string videoId)
		{
			var web = new DownLoadImg();
			byte[] by = web.DownCwareImage(url);
			string fileName = Path.GetFileName(url);
			string localPath = Util.VideoPath + "\\" + cwareId + "\\" + videoId;
			if (!Directory.Exists(localPath))
			{
				Directory.CreateDirectory(localPath);
			}
			localPath += "\\" + fileName;
			File.WriteAllBytes(localPath, by);
			return localPath;
		}

		/// <summary>
		/// 处理讲义中的本地图片
		/// </summary>
		/// <param name="imgFile"></param>
		/// <param name="cwareId"></param>
		/// <param name="videoId"></param>
		/// <returns></returns>
		private static string DealLocalImage(string imgFile, int cwareId, string videoId)
		{
			if (!File.Exists(imgFile)) return imgFile;
			string fileName = Path.GetFileName(imgFile);
			string localPath = Util.VideoPath + "\\" + cwareId + "\\" + videoId;
			if (!Directory.Exists(localPath))
			{
				Directory.CreateDirectory(localPath);
			}
			localPath += "\\" + fileName;
			File.Copy(imgFile, localPath, true);
			return localPath;
		}
	}
}
