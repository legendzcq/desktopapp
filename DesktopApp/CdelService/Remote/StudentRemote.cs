using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using CdelService.Utility;
using CdelService.Model;
using CdelService.NewModel;

namespace CdelService.Remote
{
	public class StudentRemote : RemoteBase
	{
		/// <summary>
		/// 上传收集的用户操作记录
		/// </summary>
		public void UploadBigDataLog()
		{
			try
			{
				var dataFile = Common._path + "data.log";
				Log.RecordLog("CheckFile " + dataFile);
				if (File.Exists(dataFile))
				{
					Log.RecordLog("GetContent " + dataFile);
					var content = File.ReadAllText(dataFile);
					if (content.Length == 0)
					{
						return;
					}
					content = content.Trim();
					content = "[" + content.Replace("\r\n", ",") + "]";
					var time = Util.GetNowString();
					var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), time, "cdelofflineClint").ToLower();
					var values = new NameValueCollection
					{
						{"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
						{"siteID", Common.SiteId.ToString(CultureInfo.InvariantCulture)},
                        {"platformSource", Common.PlatformSource},
						{"deviceId", Util.DeviceId},
						{"file", content},
						{"submitTime", time},
						{"pKey", key}
					};
					var web = new WebProxyClient();
					var data = web.UploadValues(Common.UploadBigDataLogUrl, values);
					var str = Encoding.UTF8.GetString(data);
					Log.RecordLog(str);
					File.WriteAllText(dataFile, string.Empty);
				}
			}
			catch (Exception ex)
			{
				Log.RecordLog(ex.ToString());
			}
		}

		/// <summary>
		/// 检查用户是否冻结
		/// </summary>
		/// <returns></returns>
		public ReturnItem CheckUserFrozen()
		{
			var re = new ReturnItem() { State = true };
			var time = Util.GetNowString();
			var pkey = Crypt.Md5(Util.SsoUid.ToString(), Common.PlatformSource, Updater.SoftVersion, time, Common.MemberSalt);
			var values = new NameValueCollection
			{
				{"pkey", pkey.ToLower()},
				{"time", time},
				{"uid", Util.SsoUid.ToString()},
                {"platformSource", Common.PlatformSource},
				{"version",Updater.SoftVersion}
			};
			var web = new WebProxyClient();
			try
			{
				var data = web.UploadValues(Common.CheckUserFrozenUrl, values);
				var str = Encoding.UTF8.GetString(data);
				Log.RecordLog(str);
				var obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
				re.State = obj.Code != 1;
				re.Code = obj.Code.ToString();
				re.Message = obj.Message;
			}
			catch (Exception ex)
			{
				Log.RecordLog(ex.ToString());
			}
			return re;
		}
	}

}
