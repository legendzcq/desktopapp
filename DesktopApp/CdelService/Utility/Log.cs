using System;
using System.Diagnostics;
using System.IO;

namespace CdelService.Utility
{
	public static class Log
	{
		private static readonly string LogFile = AppDomain.CurrentDomain.BaseDirectory + "log.log";
		public static void RecordLog(string logstr)
		{
			string log =  Util.GetNow() + ":" + logstr;
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


	}
}
