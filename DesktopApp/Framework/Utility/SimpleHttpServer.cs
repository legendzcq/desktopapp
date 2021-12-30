using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Utility
{
	public class SimpleHttpServer
	{
		private static readonly Dictionary<string, string> ContentType;

		private readonly int _port;
		private HttpListener _listerner;

		static SimpleHttpServer()
		{
			ContentType = new Dictionary<string, string>
            {
                {".htm", "text/html"},
                {".html", "text/html"},
                {".js", "application/javascript"},
                {".css", "text/css"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".bmp", "image/bmp"},
                {".png", "image/png"},
                {".swf", "application/x-shockwave-flash"},
                {".txt", "text/plain"},
                {".xml", "text/xml"}
            };
		}

		public SimpleHttpServer(int port)
		{
			_port = port;
		}

		public void Start()
		{
			SystemInfo.StartBackGroundThread("HTTPServer线程", StartServer);
		}

		private void StartServer()
		{
			try
			{
				using (_listerner = new HttpListener())
				{
					_listerner.Prefixes.Add("http://*:" + _port + "/");
					_listerner.Start();
					Trace.WriteLine("WebServerStarted");
					while (true)
					{
						try
						{
							var context = _listerner.GetContext();
							string url = context.Request.RawUrl.Replace("/", "\\");
							string rawfile = url.Contains("?")
								? url.Substring(0, url.IndexOf("?", StringComparison.Ordinal))
								: url;
							string fileName = SystemInfo.AppDataPath + "web" + rawfile;
							if (fileName.EndsWith("\\")) fileName += "index.htm";
							if (File.Exists(fileName))
							{
								context.Response.StatusCode = 200;
								context.Response.Headers.Add(
									"Cache-Control: no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
								context.Response.Headers.Add("Accept-Ranges: bytes");
								string ext = Path.GetExtension(fileName).ToLower();
								string contenttype = ContentType.ContainsKey(ext)
									? ContentType[ext]
									: "application/octet-stream";
								context.Response.ContentType = contenttype;
								var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
								context.Response.ContentLength64 = fs.Length;
								fs.Position = 0;
								var buffer = new byte[102400];
								int bytesread = fs.Read(buffer, 0, 102400);
								while (bytesread > 0)
								{
									context.Response.OutputStream.Write(buffer, 0, bytesread);
									context.Response.OutputStream.Flush();
									if (bytesread < 102400) break;
									bytesread = fs.Read(buffer, 0, 102400);
								}
								fs.Close();
								context.Response.Close();
							}
							else
							{
								context.Response.StatusCode = 404;
								byte[] buffer = Encoding.Default.GetBytes("<h1>未能找到文件</h1>");
								context.Response.OutputStream.Write(buffer, 0, buffer.Length);
								context.Response.Close();
							}
						}
						catch (Exception ex)
						{
							Trace.WriteLine(ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
	}
}
