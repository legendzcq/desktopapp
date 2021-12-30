using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Framework.Remote;
using Framework.Utility;

namespace Framework.Push
{
	public class PushClient
	{
		private const int HeatBeatTime = 30;

		private static readonly Random Rnd = new Random();

		private readonly UdpClient _udp = new UdpClient();

		public event Action<PushMessage> OnPushMessage;

		public string ServerIp { private get; set; }

		public int ServerPort { private get; set; }

		public IEnumerable<string> Course
		{
			set
			{
				_course = value;
				var courseStr = string.Join(",", _course);
				var msg = new HeartBeatPackage
				{
					SsoUid = Util.SsoUid,
					CourseList = courseStr
				};
				_msgData = msg.GetPackageBytes();
			}
		}

		private IPEndPoint _serverep;

		private byte[] _msgData;
		private IEnumerable<string> _course;

		private Thread MainTh;
		private Thread RecieveTh;

		public void StartWork()
		{
			_udp.Client.ReceiveTimeout = (HeatBeatTime + 2) * 1000;

			_serverep = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
			MainTh = new Thread(() =>
			{
				while (true)
				{
					try
					{
						_udp.Send(_msgData, _msgData.Length, _serverep);
						//Trace.WriteLine("Client HearBeat");
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Send:" + ex.Message);
						if (!Util.IsOnline) break;
					}
					Thread.Sleep(HeatBeatTime * 1000 + Rnd.Next(1000));
				}
				// ReSharper disable once FunctionNeverReturns
			}) { IsBackground = true };
			MainTh.Start();
			RecieveTh = new Thread(() =>
			{
				var ep = new IPEndPoint(IPAddress.Any, 0);
				while (true)
				{
					try
					{
						var data = _udp.Receive(ref ep);
						switch (data[0])
						{
							case 1:
								var hr = new HeartBeatReturnPackage();
								hr.ReadFromPackageBytes(data);
								if (hr.SsoUid != Util.SsoUid) Trace.WriteLine(string.Format("Error:{0}-{1}", Util.SsoUid, hr.SsoUid));
								break;
							case 2:
								var ps = new PushedPackage();
								ps.ReadFromPackageBytes(data);
								Trace.WriteLine("Push:" + ps.MessageType + "-" + ps.MessageContent);
								if (OnPushMessage != null)
								{
									if (ps.MessageType == 1)
									{
										var obj = WebProxyClient.JsonDeserialize<PushMessage>(ps.MessageContent, Encoding.UTF8);
										obj.MessageType = ps.MessageType;
										obj.MessageBody = ps.MessageContent;
										OnPushMessage(obj);
									}
									if (ps.MessageType == 2)
									{
										var obj = WebProxyClient.JsonDeserialize<PushLinkMessage>(ps.MessageContent, Encoding.UTF8);
										obj.MessageBody = ps.MessageContent;
										obj.MessageType = ps.MessageType;
										OnPushMessage(obj);
									}
								}
								break;
						}
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Receive:" + Util.GetNowString() + " " + ex.Message);
						if (!Util.IsOnline) break;
					}
				}
				// ReSharper disable once FunctionNeverReturns
			}) { Name="", IsBackground = true };
			RecieveTh.Start();
		}

		public void SendQuery(string datetime)
		{
			var query = new QueryPackage
			{
				SsoUid = Util.SsoUid,
				QueryDateTime = datetime
			};
			var data = query.GetPackageBytes();
			try
			{
				_udp.Send(data, data.Length, _serverep);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public void StopWork()
		{
			RecieveTh.Abort();
			RecieveTh = null;
			MainTh.Abort();
			MainTh = null;
		}
	}
}
