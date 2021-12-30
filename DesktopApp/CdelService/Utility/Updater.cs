using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace CdelService.Utility
{
	//todo : NOUAC 的情况下，升级需要变更模式，因此，这里必须重新写。
	public class Updater
	{
		private static string _softVersion;

		public static string SoftVersion
		{
			get
			{
				if (string.IsNullOrEmpty(_softVersion))
				{
					var loxml = new XmlDocument();
					try
					{
						string filecontent = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"version.xml");
						loxml.LoadXml(filecontent);
					}
					catch
					{
						loxml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?><root><ver>1.0.0.0</ver></root>");
					}
					var lovernode = loxml.SelectSingleNode("root/ver");
					_softVersion = lovernode != null ? lovernode.InnerText : "1.0.0.0";
				}
				return _softVersion;
			}
		}
	}
}
