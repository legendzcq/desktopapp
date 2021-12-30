using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

using Framework.Utility;

namespace Framework.Mobile
{
    public class Connector
    {
#if CHINAACC
        private const int MobileUdpPort = 19570;
        private const int MobileFtpPort = 29570;
#endif
#if MED
        private const int MobileUdpPort = 19571;
        private const int MobileFtpPort = 29571;
#endif
#if JIANSHE
        private const int MobileUdpPort = 19572;
        private const int MobileFtpPort = 29572;
#endif
#if LAW
        private const int MobileUdpPort = 19573;
        private const int MobileFtpPort = 29573;
#endif
#if CHINATAT
        private const int MobileUdpPort = 19574;
        private const int MobileFtpPort = 29574;
#endif
#if G12E
        private const int MobileUdpPort = 19575;
        private const int MobileFtpPort = 29575;
#endif
#if ZIKAO
        private const int MobileUdpPort = 19576;
        private const int MobileFtpPort = 29576;
#endif
#if CHENGKAO
        private const int MobileUdpPort = 19577;
        private const int MobileFtpPort = 29577;
#endif
#if KAOYAN
        private const int MobileUdpPort = 19578;
        private const int MobileFtpPort = 29578;
#endif
#if FOR68
        private const int MobileUdpPort = 19579;
        private const int MobileFtpPort = 29579;
#endif
        /// <summary>
        /// 当连接成功时的事件
        /// </summary>
        public event Action OnConnectSuccess;

        /// <summary>
        /// 当连接失败时的事件
        /// </summary>
        public event Action OnConnectFail;

        /// <summary>
        /// 当关闭连接时的事件
        /// </summary>
        public event Action OnDisConnected;

        /// <summary>
        /// 视频在移动设备上已存在
        /// </summary>
        public event Action OnVideoExists;

        /// <summary>
        /// 移动设备无足够空间时的事件
        /// </summary>
        public event Action OnNoSpace;

        public event Action OnTransStart;

        /// <summary>
        /// 显示进度
        /// </summary>
        public event Action<string> OnProgress;

        public event Action OnTrasFinished;

        public event Action OnTrasError;

        /// <summary>
        /// 设备名称更改
        /// </summary>
        public event Action<string> OnDeviceNameChanged;

        /// <summary>
        /// 执行文件传输的回调函数
        /// </summary>
        private Action _doTrasfer;

        /// <summary>
        /// 移动设备的网络端点
        /// </summary>
        private IPEndPoint _clientEndpoint;

        /// <summary>
        /// 移动设备的网络IP
        /// </summary>
        public IPAddress ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端号码
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; set; }

        public bool IsInTrasfer { get; set; }

        /// <summary>
        /// 是否强制退出
        /// </summary>
        private bool _forceExit;

        /// <summary>
        /// UDP
        /// </summary>
        private UdpClient _udp;

        /// <summary>
        /// 设备名称
        /// </summary>
        private string _deviceName;

        /// <summary>
        /// 设备类型
        /// </summary>
        private string _deviceType;

        private int _packageId;

        private readonly FtpClient _ftpClient = new FtpClient();
        private FtpClient _ftpFile;

        private readonly ContentBuilder _content;

        public Connector() => _content = new ContentBuilder();

        /// <summary>
        /// 通过UDP发送内容
        /// </summary>
        private void SendWithUdp(byte command, string content, IPEndPoint ep)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var buffer = new byte[data.Length + 1];
            buffer[0] = command;
            Buffer.BlockCopy(data, 0, buffer, 1, data.Length);
            //_udp.Send(buffer, buffer.Length, ep);
        }

        private void SendObjectWithUdp(byte command, object obj, IPEndPoint ep)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(obj.GetType()).WriteObject(ms, obj);
                var buffer = new byte[ms.Length + 1];
                buffer[0] = command;
                Buffer.BlockCopy(ms.ToArray(), 0, buffer, 1, (int)ms.Length);
                _udp.Send(buffer, buffer.Length, ep);
            }
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="code"></param>
        public void BeginConnect(string code)
        {
            _udp = new UdpClient
            {
                Client =
                {
                    ReceiveTimeout = 2000,
                    SendTimeout = 2000
                },
                Ttl = 30
            };
            SystemInfo.StartBackGroundThread("手机心跳", () =>
            {
                //广播地址
                var boardcastIpArr = SystemInfo.GetNetworkBoardCastIpArr();
                _forceExit = false;
                var content = code + Util.SsoUid;
                var obj = new SendConnectPackage
                {
                    Id = _packageId++,
                    Code = code,
                    Uid = Util.SsoUid,
                    Domain = Remote.Interface.Domain,
                    DownVersion = Util.AppVersion
                };
                foreach (var bip in boardcastIpArr)
                {
                    Trace.WriteLine("BoardCastTo:" + bip);
                    var endpoint = new IPEndPoint(IPAddress.Parse(bip), MobileUdpPort);
                    try
                    {
                        SendWithUdp((byte)'0', content, endpoint);
                        SendObjectWithUdp((byte)'0', obj, endpoint);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
                //尝试接受数据包，是否判断有干扰包的影响？
                try
                {
                    var data = _udp.Receive(ref _clientEndpoint);
                    if (data.Length == 33)
                    {
                        ClientId = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                    }
                    else
                    {
                        ConnectPackage deobj = JsonDeserialize<ConnectPackage>(data, 1, data.Length - 1);
                        ClientId = deobj.DeviceId;
                        _deviceType = deobj.Type;
                    }
                    ClientIpAddress = _clientEndpoint.Address;

                    Trace.WriteLine("ClientId:" + ClientId);
                    //已获取Android_id，连接成功，开始间断性发布广播包。
                    //尝试连接到FTP
                    SendTestFile();
                    _udp.Client.ReceiveTimeout = 10000;
                    BeginEventAndHeartbeat();
                    if (OnConnectSuccess != null) OnConnectSuccess();
                    IsConnected = true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
                if (!IsConnected)
                {
                    _udp.Close();
                    if (OnConnectFail != null)
                    {
                        OnConnectFail();
                    }
                }
            });
        }

        /// <summary>
        /// 开始处理心跳及收到的数据包
        /// </summary>
        public void BeginEventAndHeartbeat()
        {
            IPEndPoint clientEndpoint = null;
            _deviceName = string.Empty;
            SystemInfo.StartBackGroundThread("手机心跳处理线程", () =>
            {
                var errorTime = 0;
                while (!_forceExit)
                {
                    try
                    {
                        var data = _udp.Receive(ref clientEndpoint);
                        if (_clientEndpoint.Address.ToString() != clientEndpoint.Address.ToString() || _clientEndpoint.Port != clientEndpoint.Port)
                        {
                            continue;
                        }
                        errorTime = 0;
                        Trace.WriteLine("Rec:" + data[0]);
                        switch (data[0])
                        {
                            case 49:
                                //心跳包
                                try
                                {
                                    HeartBeatPackage obj = JsonDeserialize<HeartBeatPackage>(data, 1, data.Length - 1);
                                    if (obj != null)
                                    {
                                        if (_deviceName != obj.MobileName)
                                        {
                                            _deviceName = obj.MobileName;
                                            if (OnDeviceNameChanged != null)
                                            {
                                                OnDeviceNameChanged(obj.MobileName);
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    var retStr = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    Trace.WriteLine("DeviceName：" + retStr);
                                    if (_deviceName != retStr)
                                    {
                                        _deviceName = retStr;
                                        if (OnDeviceNameChanged != null)
                                        {
                                            OnDeviceNameChanged(retStr);
                                        }
                                    }
                                }
                                break;
                            case 50:
                                //断开连接
                                _forceExit = true;
                                IsConnected = false;
                                IsInTrasfer = false;
                                _udp.Close();
                                break;
                            case 51:
                                //发送课件名称
                                try
                                {
                                    CwareTransferPackage obj = JsonDeserialize<CwareTransferPackage>(data, 1, data.Length - 1);
                                    if (obj.Status == "NOSPACE")
                                    {
                                        IsInTrasfer = false;
                                        if (OnNoSpace != null)
                                        {
                                            OnNoSpace();
                                        }
                                    }
                                    else if (obj.Status == "DOWNLOADED")
                                    {
                                        IsInTrasfer = false;
                                        if (OnVideoExists != null)
                                        {
                                            OnVideoExists();
                                        }
                                    }
                                    else
                                    {
                                        if (_doTrasfer != null)
                                        {
                                            _doTrasfer();
                                        }
                                    }
                                }
                                catch
                                {
                                    var ret = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    if (ret == "NOSPACE")
                                    {
                                        if (OnNoSpace != null)
                                        {
                                            OnNoSpace();
                                        }
                                    }
                                    else if (ret == "DOWNLOADED")
                                    {
                                        if (OnVideoExists != null)
                                        {
                                            OnVideoExists();
                                        }
                                    }
                                    else
                                    {
                                        if (_doTrasfer != null)
                                        {
                                            _doTrasfer();
                                        }
                                    }
                                }
                                break;
                            case 52:
                                //发送结束
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                        errorTime++;
                        if (errorTime > 3)
                        {
                            _forceExit = true;
                            if (OnDisConnected != null) OnDisConnected();
                        }
                    }
                }
            });
            SystemInfo.StartBackGroundThread("手机心跳线程", () =>
            {
                while (!_forceExit)
                {
                    SendHeartBeat();
                    Thread.Sleep(10000);
                }
            });
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        private void SendHeartBeat()
        {
            var obj = new SendHeartBeatPackage
            {
                Id = _packageId++,
                PcName = Environment.MachineName
            };
            try
            {
                SendWithUdp((byte)'1', Environment.MachineName, _clientEndpoint);
                SendObjectWithUdp((byte)'1', obj, _clientEndpoint);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            _forceExit = true;
            IsConnected = false;
            var obj = new SendDisconnectPackage
            {
                Id = _packageId++,
                Command = "Stop"
            };
            try
            {
                SendWithUdp((byte)'2', "STOP", _clientEndpoint);
                SendObjectWithUdp((byte)'2', obj, _clientEndpoint);
                _udp.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                if (OnDisConnected != null) OnDisConnected();
            }
        }

        /// <summary>
        /// 向FTP发送文件
        /// </summary>
        private void SendTestFile()
        {
            var tcontent = Encoding.UTF8.GetBytes("Test");

            _ftpClient.UploadFileNow(tcontent, GetFtpRoot() + "Test.txt", false);
            _ftpClient.DeleteFile(GetFtpRoot() + "Test.txt");
        }

        /// <summary>
        /// 获取FTP根路径
        /// </summary>
        /// <returns></returns>
        private string GetFtpRoot() => string.Format("ftp://{0}:{1}@{2}:{3}/", Util.SsoUid, ClientId, ClientIpAddress, MobileFtpPort);



        /// <summary>
        /// 向手机发送课件
        /// </summary>
        public void SendCware(int cwareId, string videoId)
        {
            _doTrasfer = () => SystemInfo.StartBackGroundThread("向手机发送课件线程", () =>
            {
                if (IsInTrasfer) return;
                IsInTrasfer = true;
                if (OnTransStart != null) OnTransStart();
                var fileName = Path.GetTempFileName();
                try
                {
                    //先准备数据
                    Model.ViewStudentCwareDown ditem = new Local.StudentWareData().GetCwareDownItem(cwareId, videoId);
                    var kcjyHtml = _content.GetKcjyContent(cwareId, videoId);
                    kcjyHtml = Crypt.EncryptContentForAndroid(kcjyHtml, ClientId);
                    var timepointHtml = _content.GetTimeListString(cwareId, videoId);
                    Trace.WriteLine(fileName);
                    Crypt.DecryptVideoFile(ditem.LocalFile, fileName, out var videoHeader);
                    videoHeader = Crypt.EncryptVideoHeaderForAndroid(videoHeader, ClientId);
                    //开始传输
                    var imgPath = Path.GetDirectoryName(ditem.LocalFile) + "\\" + videoId;
                    if (Directory.Exists(imgPath))
                    {
                        var imgFiles = Directory.GetFiles(imgPath);
                        SendImgFiles(cwareId, videoId, imgFiles);
                    }
                    SendFileContent(cwareId, videoId, "paper.xml", kcjyHtml);
                    SendFileContent(cwareId, videoId, "timepoint.xml", timepointHtml);
                    SendFileContent(cwareId, videoId, "videofile.dat", videoHeader);
                    SendVideoFile(cwareId, videoId, fileName);

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    if (OnTrasError != null) OnTrasError();
                }
                finally
                {

                }
            });
            SendWareInfo(cwareId, videoId);
            //_doTrasfer();
        }

        /// <summary>
        /// 向手机发送课件信息
        /// </summary>
        private void SendWareInfo(int cwareId, string videoId)
        {
            Model.StudentCourseWare citem = new Local.StudentWareData().GetCourseWareItem(cwareId);
            Model.StudentCourseDetail item = new Local.StudentWareData().GetStudentCWareDetailItem(cwareId, videoId);
            var sendjson = string.Format(@"{{""CWareID"":""{0}"",""VideoID"":""{1}"",""CwareName"":""{2}"",""VideoName"":""{3}""}}", item.CWareId, item.VideoId, citem.Name, item.VideoName);
            var obj = new SendCwareInfoPackage
            {
                Id = _packageId++,
                CWareId = cwareId,
                VideoId = videoId,
                CwareName = citem.Name,
                VideoName = item.VideoName
            };
            SendWithUdp((byte)'3', sendjson, _clientEndpoint);
            SendObjectWithUdp((byte)'3', obj, _clientEndpoint);
        }

        /// <summary>
        /// 发送课件完成
        /// </summary>
        private void SendFileFinished(int cwareId, string videoId)
        {
            SendWithUdp((byte)'4', "OK", _clientEndpoint);
            var obj = new SendCwareInfoFinishedPackage
            {
                Id = _packageId++,
                CWareId = cwareId,
                VideoId = videoId
            };
            SendObjectWithUdp((byte)'4', obj, _clientEndpoint);
        }

        /// <summary>
        /// 发送讲义图片
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <param name="imgFiles"></param>
        private void SendImgFiles(int cwareId, string videoId, IEnumerable<string> imgFiles)
        {
            foreach (var imgFile in imgFiles)
            {
                _ftpClient.UploadFileNow(imgFile, GetFtpRoot() + cwareId + "/" + videoId + "/img/" + Path.GetFileName(imgFile), false);
            }
        }

        /// <summary>
        /// 发送视频文件
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <param name="localFileName"></param>
        private void SendVideoFile(int cwareId, string videoId, string localFileName)
        {
            var ftpUrl = GetFtpRoot() + cwareId + "/" + videoId + "/videoFile.mp4";
            _ftpFile = new FtpClient();
            _ftpFile.FileUploading += (s, e) =>
            {
                var rate = ((double)e.BytesTransfered * 100 / e.TotalBytes).ToString("0.0");
                if (_lastRate != rate)
                {
                    if (OnProgress != null)
                    {
                        OnProgress(rate);
                        var obj = new SendCwareInfoProgressPackage
                        {
                            Id = _packageId++,
                            CWareId = cwareId,
                            VideoId = videoId,
                            Progress = rate
                        };
                        SendObjectWithUdp((byte)'5', obj, _clientEndpoint);
                    }
                    _lastRate = rate;
                    Trace.WriteLine(e.BytesTransfered + ":" + e.TotalBytes + ":" + rate);
                }
            };
            _ftpFile.FileDownloadCompleted += (s, e) =>
            {
                _ftpClient.Rename(ftpUrl + ".tmp", "videoFile.mp4");
                SystemInfo.TryDeleteFile(localFileName);
                //发送完成
                SendFileFinished(cwareId, videoId);
                if (OnTrasFinished != null) OnTrasFinished();
                IsInTrasfer = false;
                _doTrasfer = null;
            };
            _ftpFile.UploadFile(localFileName, ftpUrl + ".tmp", true);
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        private void SendFileContent(int cwareId, string videoId, string fileName, string fileContent) => SendFileContent(cwareId, videoId, fileName, Encoding.UTF8.GetBytes(fileContent));

        /// <summary>
        /// 发送文件内容
        /// </summary>
        private void SendFileContent(int cwareId, string videoId, string fileName, byte[] fileContent) => _ftpClient.UploadFileNow(fileContent, GetFtpRoot() + cwareId + "/" + videoId + "/" + fileName, false);

        private string _lastRate = string.Empty;

        private static string JsonSerialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(obj.GetType()).WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static T JsonDeserialize<T>(string s, Encoding encoding)
        {
            using (var ms = new MemoryStream(encoding.GetBytes(s)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        private static T JsonDeserialize<T>(byte[] content)
        {
            using (var ms = new MemoryStream(content))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        private static T JsonDeserialize<T>(byte[] content, int offset, int count)
        {
            using (var ms = new MemoryStream(content, offset, count))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        [DataContract]
        private class SendConnectPackage
        {
            [DataMember]
            public int Id { get; set; }
            [DataMember]
            public string Code { get; set; }
            [DataMember]
            public int Uid { get; set; }
            [DataMember]
            public string Domain { get; set; }
            [DataMember]
            public string DownVersion { get; set; }
        }

        [DataContract]
        private class SendHeartBeatPackage
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public string PcName { get; set; }
        }

        [DataContract]
        private class SendDisconnectPackage
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public string Command { get; set; }
        }

        [DataContract]
        private class SendCwareInfoPackage
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember(Name = "CWareID")]
            public int CWareId { get; set; }

            [DataMember(Name = "VideoID")]
            public string VideoId { get; set; }

            [DataMember]
            public string CwareName { get; set; }

            [DataMember]
            public string VideoName { get; set; }
        }

        [DataContract]
        private class SendCwareInfoFinishedPackage
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember(Name = "CWareID")]
            public int CWareId { get; set; }

            [DataMember(Name = "VideoID")]
            public string VideoId { get; set; }
        }

        [DataContract]
        private class SendCwareInfoProgressPackage
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember(Name = "CWareID")]
            public int CWareId { get; set; }

            [DataMember(Name = "VideoID")]
            public string VideoId { get; set; }

            [DataMember]
            public string Progress { get; set; }
        }

        [DataContract]
        private class ConnectPackage
        {
            [DataMember(Name = "Type")]
            public string Type { get; set; }
            [DataMember(Name = "DeviceID")]
            public string DeviceId { get; set; }
        }

        [DataContract]
        private class HeartBeatPackage
        {
            [DataMember(Name = "MobileName")]
            public string MobileName { get; set; }
        }

        [DataContract]
        private class CwareTransferPackage
        {
            [DataMember(Name = "Status")]
            public string Status { get; set; }
        }
    }
}