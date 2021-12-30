using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CdelService.Utility
{
	public static class Network
	{
		public static WebProxy GetWebProxy()
		{
			if (Util.ProxyType == 2)
			{
				var proxy = new WebProxy(Util.ProxyAddress, Util.ProxyPort);
				if (!string.IsNullOrEmpty(Util.ProxyUserName))
				{
					proxy.Credentials = new NetworkCredential(Util.ProxyUserName, Util.ProxyUserPassword);
				}
				return proxy;
			}
			return null;
		}
	}
}
