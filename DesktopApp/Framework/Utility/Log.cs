using System;
using System.Diagnostics;
using System.IO;

namespace Framework.Utility
{
	public static class Log
	{
		private static readonly string LogFile = SystemInfo.AppDataPath + "log.log";

		public static void RecordLog(string logstr)
		{
			string log = "=======" + Util.GetNow() + "=======\r\n" + logstr;
			try
			{
				Trace.WriteLine(log);
				File.AppendAllText(LogFile, log + "\r\n");
			}
			catch
			{
				;
			}
		}

		internal static readonly string DataFile = SystemInfo.AppDataPath + "data.log";

		public static void RecordData(string type, params object[] dataStr)
		{
#if !CK100 && BIGDATA
			var arr = new object[] { "-", "-", "-", "-" };
			var len = dataStr.Length > 4 ? 4 : dataStr.Length;
			if (len > 0) Array.Copy(dataStr, 0, arr, 0, len);
			var str = string.Format("{{\"isonline\":\"{0}\",\"time\":\"{1}\",\"uid\":\"{2}\",\"action\":\"{3}\",\"param1\":\"{4}\",\"param2\":\"{5}\",\"param3\":\"{6}\",\"param4\":\"{7}\"}}\r\n", Util.IsOnline, Util.GetNow().ToString("yyyy-MM-dd HH:mm:ss"), Util.SsoUid, type, arr[0], arr[1], arr[2], arr[3]);
			try
			{
				//Trace.WriteLine(str);
				File.AppendAllText(DataFile, str);
			}
			catch (Exception)
			{
				;
			}
#endif
		}
	}
}
