using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Framework.Utility;

namespace Framework.Mobile
{
    public class FtpClient : IDisposable
    {
        #region 声明事件
        /// <summary>
        /// 正在下载文件
        /// </summary>
        public event FtpSendEventHandler FileDownloading;
        private delegate void OnFileDownloadingDelegate(FtpConnect ftpConnect, long lTotalBytes, long lBytesTransfered);
        /// <summary>
        /// 正在下载文件封装模式
        /// </summary>
        private void OnFileDownloading(FtpConnect ftpConnect, long lTotalBytes, long lBytesTransfered)
        {
            if (FileDownloading != null)
            {
                if (lBytesTransfered > lTotalBytes)
                    lBytesTransfered = lTotalBytes;
                var aSynch = FileDownloading.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileDownloadingDelegate(OnFileDownloading), new object[] { ftpConnect, lTotalBytes, lBytesTransfered });
                else
                    FileDownloading(ftpConnect, new FtpSendEventArgs(lTotalBytes, lBytesTransfered));
            }
        }
        /// <summary>
        /// 文件下载完成
        /// </summary>
        public event EventHandler FileDownloadCompleted;
        private delegate void OnFileDownloadCompletedDelegate(FtpConnect ftpConnect);
        /// <summary>
        /// 文件下载完成封装模式
        /// </summary>
        private void OnFileDownloadCompleted(FtpConnect ftpConnect)
        {
            if (FileDownloadCompleted != null)
            {
                var aSynch = FileDownloadCompleted.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileDownloadCompletedDelegate(OnFileDownloadCompleted), new object[] { ftpConnect });
                else
                    FileDownloadCompleted(ftpConnect, new EventArgs());
            }
        }
        /// <summary>
        /// 取消正在下载的文件
        /// </summary>
        public event EventHandler FileDownloadCanceled;
        private delegate void OnFileDownloadCanceledDelegate(FtpConnect ftpConnect);
        /// <summary>
        /// 取消正在下载的文件封装模式
        /// </summary>
        private void OnFileDownloadCanceled(FtpConnect ftpConnect)
        {
            if (FileDownloadCanceled != null)
            {
                var aSynch = FileDownloadCanceled.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileDownloadCanceledDelegate(OnFileDownloadCanceled), new object[] { ftpConnect });
                else
                    FileDownloadCanceled(ftpConnect, new EventArgs());
            }
        }
        /// <summary>
        /// 正在上传文件
        /// </summary>
        public event FtpSendEventHandler FileUploading;
        private delegate void OnFileUploadingDelegate(FtpConnect ftpConnect, long lTotalBytes, long lBytesTransfered);

        /// <summary>
        /// 正在下载事件封装模式
        /// </summary>
        /// <param name="ftpConnect"></param>
        /// <param name="lTotalBytes"></param>
        /// <param name="lBytesTransfered"></param>
        private void OnFileUploading(FtpConnect ftpConnect, long lTotalBytes, long lBytesTransfered)
        {
            if (FileUploading != null)
            {
                if (lBytesTransfered > lTotalBytes)
                    lBytesTransfered = lTotalBytes;
                var aSynch = FileUploading.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileUploadingDelegate(OnFileUploading), new object[] { ftpConnect, lTotalBytes, lBytesTransfered });
                else
                    FileUploading(ftpConnect, new FtpSendEventArgs(lTotalBytes, lBytesTransfered));
            }
        }
        /// <summary>
        /// 文件上传完成
        /// </summary>
        public event EventHandler FileUploadCompleted;
        private delegate void OnFileUploadCompletedDelegate(FtpConnect ftpConnect);
        private void OnFileUploadCompleted(FtpConnect ftpConnect)
        {
            if (FileUploadCompleted != null)
            {
                var aSynch = FileUploadCompleted.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileUploadCompletedDelegate(OnFileUploadCompleted), new object[] { ftpConnect });
                else
                    FileUploadCompleted(ftpConnect, new EventArgs());
            }
        }
        /// <summary>
        /// 取消了上传文件
        /// </summary>
        public event EventHandler FileUploadCanceled;
        private delegate void OnFileUploadCanceledDelegate(FtpConnect ftpConnect);
        private void OnFileUploadCanceled(FtpConnect ftpConnect)
        {
            if (FileUploadCanceled != null)
            {
                var aSynch = FileUploadCanceled.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFileUploadCanceledDelegate(OnFileUploadCanceled), new object[] { ftpConnect });
                else
                    FileUploadCanceled(ftpConnect, new EventArgs());
            }
        }
        /// <summary>
        /// 传输过程发生错误事件
        /// </summary>
        public event FtpErrorEventHandler FtpError;
        private delegate void OnFtpErrorDelegate(FtpConnect ftpConnect, Exception error);
        public void OnFtpError(FtpConnect ftpConnect, Exception error)
        {
            if (FtpError != null)
            {
                var aSynch = FtpError.Target as ISynchronizeInvoke;
                if (aSynch != null && aSynch.InvokeRequired)
                    aSynch.Invoke(new OnFtpErrorDelegate(OnFtpError), new object[] { ftpConnect, error });
                else
                    FtpError(ftpConnect, new FtpErrorEventArgs(error));
            }
        }
        #endregion
        #region FTPUrl结构
        public class FtpUrl
        {
            private string _mUrl = String.Empty;
            private string _mRemoteHost = String.Empty;
            private IPAddress _mRemoteHostIp = IPAddress.None;
            private int _mRemotePort = 21;
            private string _mUserName = String.Empty;
            private string _mPassword = String.Empty;
            private string _mSubUrl = String.Empty;
            public FtpUrl()
            { }
            public FtpUrl(string url)
            {
                Url = url;
            }
            /// <summary>
            /// FTP的全地址
            /// </summary>
            public string Url
            {
                get { return _mUrl; }
                set
                {
                    if (value.IndexOf("@", StringComparison.Ordinal) < 0)
                        throw (new Exception("FTP地址路径不合法！格式应为ftp://用户名:密码@地址[:端口]/"));
                    string strSubUrl = "";
                    string strRemoteHostAndPort = value.Substring(value.IndexOf("@", StringComparison.Ordinal) + 1);
                    if (strRemoteHostAndPort.IndexOf("/", StringComparison.Ordinal) > 0)
                    {
                        strSubUrl = strRemoteHostAndPort.Substring(strRemoteHostAndPort.IndexOf("/", StringComparison.Ordinal));
                        strRemoteHostAndPort = strRemoteHostAndPort.Substring(0, strRemoteHostAndPort.IndexOf("/", StringComparison.Ordinal));
                    }
                    string strRemoteHost = strRemoteHostAndPort;
                    int iRemotePort = 21;
                    if (strRemoteHostAndPort.IndexOf(":", StringComparison.Ordinal) > 0)
                    {
                        strRemoteHost = strRemoteHostAndPort.Substring(0, strRemoteHostAndPort.IndexOf(":", StringComparison.Ordinal));
                        string strRemotePort = strRemoteHostAndPort.Substring(strRemoteHostAndPort.IndexOf(":", StringComparison.Ordinal) + 1);
                        if (!int.TryParse(strRemotePort, out iRemotePort))
                            iRemotePort = 21;
                    }
                    string strUserNameAndPassword = value.Substring(0, value.IndexOf("@", StringComparison.Ordinal)).Trim();
                    if (strUserNameAndPassword.ToLower().StartsWith("ftp://"))
                        strUserNameAndPassword = strUserNameAndPassword.Substring(6).Trim();
                    if (strUserNameAndPassword == string.Empty || strUserNameAndPassword.IndexOf(":", StringComparison.Ordinal) < 0)
                        throw (new Exception("FTP地址路径不合法！格式应为ftp://用户名:密码@地址[:端口]/"));
                    string strUserName = strUserNameAndPassword.Substring(0, strUserNameAndPassword.IndexOf(":", StringComparison.Ordinal));
                    string strPassword = strUserNameAndPassword.Substring(strUserNameAndPassword.IndexOf(":", StringComparison.Ordinal) + 1);

                    IPAddress[] ips = Dns.GetHostAddresses(strRemoteHost);
                    if (ips.Length == 0)
                        throw (new Exception("FTP地址路径中主机地址无效！"));
                    strSubUrl = strSubUrl.Replace("//", "/");
                    _mRemoteHostIp = ips[0];
                    _mRemoteHost = strRemoteHost;
                    _mRemotePort = iRemotePort;
                    _mUserName = strUserName;
                    _mPassword = strPassword;
                    _mSubUrl = strSubUrl;
                    _mUrl = value;
                }
            }
            /// <summary>
            /// 主机地址
            /// </summary>
            public string RemoteHost
            {
                get { return _mRemoteHost; }
            }
            /// <summary>
            /// 主机IP
            /// </summary>
            public IPAddress RemoteHostIp
            {
                get { return _mRemoteHostIp; }
            }
            /// <summary>
            /// 主机端口
            /// </summary>
            public int RemotePort
            {
                get { return _mRemotePort; }
            }
            public string UserName
            {
                get { return _mUserName; }
            }
            public string Password
            {
                get { return _mPassword; }
            }
            public string SubUrl
            {
                get { return _mSubUrl; }
            }
        }
        #endregion
        #region 传输模式
        /// <summary>
        /// 传输模式:二进制类型、ASCII类型
        /// </summary>
        public enum TransferType
        {
            /// <summary>
            /// Binary
            /// </summary>
            Binary,
            /// <summary>
            /// ASCII
            /// </summary>
            Ascii
        };
        #endregion
        #region 存贮FTP的连接结构类
        public class FtpConnect
        {
            #region 私有字段
            /// <summary>
            /// 数据传送套接字列表
            /// </summary>
            private readonly List<Socket> _mDataSocketList;
            private readonly string _mId;
            /// <summary>
            /// 唯一ID
            /// </summary>
            public string Id
            {
                get { return _mId; }
            }

            /// <summary>
            /// 扩展标记
            /// </summary>
            public object Tag { get; set; }

            private bool _mDataTransmitting;
            /// <summary>
            /// 数据正在传输 标记
            /// </summary>
            public bool DataTransmitting
            {
                get { return _mDataTransmitting; }
                set { _mDataTransmitting = value; }
            }
            private Socket _mSocketControl;
            /// <summary>
            /// FTPUrl
            /// </summary>
            private FtpUrl _mFtpUrl;
            /// <summary>
            /// 是否已经连接
            /// </summary>
            private bool _mIsConnected;
            private Encoding _mEncodeType = Encoding.Default;
            /// <summary>
            /// 编码方式
            /// </summary>
            public Encoding EncodeType
            {
                get { return _mEncodeType; }
                set { _mEncodeType = value; }
            }

            /// <summary>
            /// 接收和发送数据的缓冲区
            /// </summary>
            private const int BlockSize = 4096;

            /// <summary>
            /// 缓冲区大小
            /// </summary>
            private Byte[] _mBuffer;
            public Byte[] Buffer
            {
                get { return _mBuffer; }
                set { _mBuffer = value; }
            }
            private string _mMessage;
            /// <summary>
            /// 当前的消息
            /// </summary>
            public string Message
            {
                get { return _mMessage; }
                set { _mMessage = value; }
            }
            private string _mReplyString;
            /// <summary>
            /// 应答字符串
            /// </summary>
            public string ReplyString
            {
                get { return _mReplyString; }
                set { _mReplyString = value; }
            }
            private int _mReplyCode;
            /// <summary>
            /// 应答代码
            /// </summary>
            public int ReplyCode
            {
                get { return _mReplyCode; }
                set { _mReplyCode = value; }
            }
            /// <summary>
            /// 传输模式
            /// </summary>
            private TransferType _trType;
            #endregion
            public FtpConnect()
            {
                _mId = Guid.NewGuid().ToString();
                _mDataSocketList = new List<Socket>();
                _mBuffer = new Byte[BlockSize];
            }
            public FtpConnect(FtpUrl ftpUrl)
            {
                _mId = Guid.NewGuid().ToString();
                _mDataSocketList = new List<Socket>();
                _mBuffer = new Byte[BlockSize];
                FtpUrl = ftpUrl;
            }
            public FtpConnect(FtpUrl ftpUrl, string ftpId)
            {
                if (String.IsNullOrEmpty(ftpId))
                    ftpId = Guid.NewGuid().ToString();
                _mId = ftpId;
                _mDataSocketList = new List<Socket>();
                _mBuffer = new Byte[BlockSize];
                FtpUrl = ftpUrl;
            }
            #region 公共字段
            /// <summary>
            /// 套接字连接
            /// </summary>
            public Socket SocketControl
            {
                get { return _mSocketControl; }
                set { _mSocketControl = value; }
            }
            /// <summary>
            /// 对应的URL
            /// </summary>
            public FtpUrl FtpUrl
            {
                get { return _mFtpUrl; }
                set { _mFtpUrl = value; }
            }
            /// <summary>
            /// 是否已经连接
            /// </summary>
            public bool IsConnected
            {
                get { return _mIsConnected; }
                set { _mIsConnected = value; }
            }
            #endregion
            #region 公共方法
            #region 取消传送数据
            public void CancelDataTransmit()
            {
                _mDataTransmitting = false;
            }
            #endregion
            #region 发送命令
            /// <summary>
            /// 发送命令并获取应答码和最后一行应答字符串
            /// </summary>
            /// <param name="strCommand">命令</param>
            public void SendCommand(string strCommand)
            {
                if (_mSocketControl == null)
                    throw (new Exception("请先连接服务器再进行操作！"));
                Trace.WriteLine(strCommand);
                Byte[] cmdBytes = _mEncodeType.GetBytes((strCommand + "\r\n").ToCharArray());
                _mSocketControl.Send(cmdBytes, cmdBytes.Length, 0);
                ReadReply();
            }
            #endregion
            #region 读取最后一行的消息
            /// <summary>
            /// 读取Socket返回的所有字符串
            /// </summary>
            /// <returns>包含应答码的字符串行</returns>
            private string ReadLine()
            {
                if (_mSocketControl == null)
                    throw (new Exception("请先连接服务器再进行操作！"));
                while (true)
                {
                    int iBytes = _mSocketControl.Receive(_mBuffer, _mBuffer.Length, 0);
                    _mMessage += _mEncodeType.GetString(_mBuffer, 0, iBytes);
                    if (iBytes < _mBuffer.Length)
                    {
                        break;
                    }
                }
                char[] seperator = { '\n' };
                string[] mess = _mMessage.Split(seperator);
                _mMessage = _mMessage.Length > 2 ? mess[mess.Length - 2] : mess[0];
                if (!_mMessage.Substring(3, 1).Equals(" "))//返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)
                {
                    return ReadLine();
                }
                return _mMessage;
            }
            #endregion
            #region 读取应答代码
            /// <summary>
            /// 将一行应答字符串记录在strReply和strMsg
            /// 应答码记录在iReplyCode
            /// </summary>
            public void ReadReply()
            {
                _mMessage = "";
                _mReplyString = ReadLine();
                Trace.WriteLine(_mReplyString);
                _mReplyCode = Int32.Parse(_mReplyString.Substring(0, 3));
            }
            #endregion
            #region 断开连接
            /// <summary>
            /// 关闭连接
            /// </summary>
            public void DisConnect()
            {
                _mDataTransmitting = false;
                while (_mDataSocketList.Count > 0)
                {
                    Socket socket = _mDataSocketList[0];
                    if (socket != null && socket.Connected)
                        socket.Close();
                    _mDataSocketList.RemoveAt(0);
                }
                if (_mIsConnected && _mSocketControl != null)
                    SendCommand("QUIT");
                CloseSocketConnect();
                _mBuffer = null;
            }
            /// <summary>
            /// 关闭socket连接(用于登录以前)
            /// </summary>
            private void CloseSocketConnect()
            {
                if (_mSocketControl != null && _mSocketControl.Connected)
                {
                    _mSocketControl.Close();
                    _mSocketControl = null;
                }
                _mIsConnected = false;
            }
            #endregion
            #region 连接服务器
            public void Connect()
            {
                DisConnect();//先断开现有连接
                _mBuffer = new byte[BlockSize];
                _mSocketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = 10000
                };
                var ep = new IPEndPoint(_mFtpUrl.RemoteHostIp, _mFtpUrl.RemotePort);
                try
                {
                    _mSocketControl.Connect(ep);
                }
                catch (Exception)
                {
                    throw new IOException(String.Format("无法连接到远程服务器{0}！", _mFtpUrl.RemoteHost));
                }
                // 获取应答码
                ReadReply();
                if (_mReplyCode != 220)
                {
                    DisConnect();
                    throw new IOException(_mReplyString.Substring(4));
                }
                // 登陆
                SendCommand("USER " + _mFtpUrl.UserName);
                if (!(_mReplyCode == 331 || _mReplyCode == 230))
                {
                    CloseSocketConnect();//关闭连接
                    throw new IOException(_mReplyString.Substring(4));
                }
                if (_mReplyCode != 230)
                {
                    SendCommand("PASS " + _mFtpUrl.Password);
                    if (!(_mReplyCode == 230 || _mReplyCode == 202))
                    {
                        CloseSocketConnect();//关闭连接
                        throw new IOException(_mReplyString.Substring(4));
                    }
                }
                _mIsConnected = true;
            }
            #endregion
            #region 改变目录

            /// <summary>
            /// 改变目录
            /// </summary>
            public void ChangeDir(string dirName)
            {
                if (!_mIsConnected)
                    throw (new Exception("请先连接服务器然后再进行CWD操作！"));
                if (dirName.Equals(".") || dirName.Equals(""))
                    return;
                SendCommand("CWD " + dirName);
                if (_mReplyCode != 250)
                    throw new IOException(_mReplyString.Substring(4));
            }
            #endregion
            #region 传输模式
            /// <summary>
            /// 设置传输模式
            /// </summary>
            /// <param name="ttType">传输模式</param>
            public void SetTransferType(TransferType ttType)
            {
                SendCommand(ttType == TransferType.Binary ? "TYPE I" : "TYPE A");
                if (_mReplyCode != 200)
                {
                    throw new IOException(_mReplyString.Substring(4));
                }
                _trType = ttType;
            }
            /// <summary>
            /// 获得传输模式
            /// </summary>
            /// <returns>传输模式</returns>
            public TransferType GetTransferType()
            {
                return _trType;
            }
            #endregion
            #region 建立进行数据连接的socket
            /// <summary>
            /// 建立进行数据连接的socket
            /// </summary>
            /// <returns>数据连接socket</returns>
            public Socket CreateDataSocket()
            {
                SendCommand("PASV");
                if (_mReplyCode != 227)
                    throw new IOException(_mReplyString.Substring(4));
                int index1 = _mReplyString.IndexOf('(');
                int index2 = _mReplyString.IndexOf(')');
                string ipData = _mReplyString.Substring(index1 + 1, index2 - index1 - 1);
                var parts = new int[6];
                int len = ipData.Length;
                int partCount = 0;
                string buf = "";
                for (int i = 0; i < len && partCount <= 6; i++)
                {
                    char ch = Char.Parse(ipData.Substring(i, 1));
                    if (Char.IsDigit(ch))
                        buf += ch;
                    else if (ch != ',')
                    {
                        throw new IOException("Malformed PASV Reply: " + _mReplyString);
                    }
                    if (ch == ',' || i + 1 == len)
                    {
                        try
                        {
                            parts[partCount++] = Int32.Parse(buf);
                            buf = "";
                        }
                        catch (Exception)
                        {
                            throw new IOException("Malformed PASV Reply: " + _mReplyString);
                        }
                    }
                }
                string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
                int port = (parts[4] << 8) + parts[5];
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                try
                {
                    socket.Connect(ep);
                }
                catch (Exception)
                {
                    throw new IOException(String.Format("无法连接到远程服务器{0}:{1}！", ipAddress, port));
                }
                _mDataSocketList.Add(socket);
                return socket;
            }
            #endregion
            #endregion
        }
        #endregion
        #region 变量区
        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        private readonly List<FtpConnect> _mFtpConnectList;

        /// <summary>
        /// 标记
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region 实例化
        public FtpClient()
        {
            _mFtpConnectList = new List<FtpConnect>();
        }
        #endregion
        #region Dispose

        public void Dispose()
        {
            while (_mFtpConnectList.Count > 0)
            {
                FtpConnect ftpConnect = _mFtpConnectList[0];
                ftpConnect.DisConnect();
                _mFtpConnectList.Remove(ftpConnect);
            }
        }
        #endregion
        #region 得到文件大小

        /// <summary>
        /// 得到文件大小
        /// </summary>
        /// <param name="fileUrl">目标文件，包含用户名与密码。如：ftp://username:password@127.0.0.1/1.txt</param>
        /// <returns></returns>
        public long GetFileSize(string fileUrl)
        {
            FtpConnect ftpConnect = null;
            try
            {
                var ftpUrl = new FtpUrl(fileUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                long lFileSize;
                if (ftpConnect.ReplyCode == 213)
                    lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                else
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                return lFileSize;
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 得到文件大小

        /// <summary>
        /// 得到文件大小
        /// </summary>
        /// <param name="fileUrl">目标文件，包含用户名与密码。如：ftp://username:password@127.0.0.1/1.txt</param>
        /// <returns></returns>
        public DateTime GetDateTimestamp(string fileUrl)
        {
            FtpConnect ftpConnect = null;
            try
            {
                var ftpUrl = new FtpUrl(fileUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("MDTM " + Path.GetFileName(ftpUrl.SubUrl));
                if (ftpConnect.ReplyCode == 213)
                {
                    string strDateTime = ftpConnect.ReplyString.Substring(4);
                    int iYear = 0, iMonth = 0, iDay = 0, iHour = 0, iMinute = 0, iSecond = 0;
                    if (strDateTime.Length >= 4)
                        int.TryParse(strDateTime.Substring(0, 4), out iYear);
                    if (strDateTime.Length >= 6)
                        int.TryParse(strDateTime.Substring(4, 2), out iMonth);
                    if (strDateTime.Length >= 8)
                        int.TryParse(strDateTime.Substring(6, 2), out iDay);
                    if (strDateTime.Length >= 10)
                        int.TryParse(strDateTime.Substring(8, 2), out iHour);
                    if (strDateTime.Length >= 12)
                        int.TryParse(strDateTime.Substring(10, 2), out iMinute);
                    if (strDateTime.Length >= 14)
                        int.TryParse(strDateTime.Substring(12, 2), out iSecond);
                    return new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond).ToLocalTime();
                }
                else
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 删除指定的文件
        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="fileUrl">待删除文件名</param>
        public void DeleteFile(string fileUrl)
        {
            FtpConnect ftpConnect = null;
            try
            {
                var ftpUrl = new FtpUrl(fileUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("DELE " + Path.GetFileName(ftpUrl.SubUrl));
                if (ftpConnect.ReplyCode != 250)
                    throw (new Exception(ftpConnect.ReplyString.Substring(4)));
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 重命名

        /// <summary>
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// </summary>
        public void Rename(string originalUrl, string newName)
        {
            FtpConnect ftpConnect = null;
            try
            {
                var ftpUrl = new FtpUrl(originalUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("RNFR " + Path.GetFileName(ftpUrl.SubUrl));
                if (ftpConnect.ReplyCode != 350)
                    throw (new Exception(ftpConnect.ReplyString.Substring(4)));
                ftpConnect.SendCommand("RNTO " + newName);
                if (ftpConnect.ReplyCode != 250)
                    throw (new Exception(ftpConnect.ReplyString.Substring(4)));
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 创建文件夹
        public void MakeDirectory(string dirUrl)
        {
            FtpConnect ftpConnect = null;
            int iCharIndex = 0;
            try
            {
                var ftpUrl = new FtpUrl(dirUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirUrl = ftpUrl.SubUrl;
                while (strDirUrl.Trim() != "")
                {
                    iCharIndex = strDirUrl.IndexOf("/", iCharIndex, StringComparison.Ordinal) + 1;
                    string strDirUrlTemp = iCharIndex == 0 ? strDirUrl : strDirUrl.Substring(0, iCharIndex);
                    if (strDirUrlTemp == "")
                        continue;
                    ftpConnect.SendCommand("MKD " + strDirUrlTemp);
                    if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                    if (strDirUrlTemp == strDirUrl)
                        break;
                }
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 删除目录
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirUrl">目录名</param>
        public void RemoveDirectory(string dirUrl)
        {
            FtpConnect ftpConnect = null;
            try
            {
                var ftpUrl = new FtpUrl(dirUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirPath = ftpUrl.SubUrl;
                string strDirName = ftpUrl.SubUrl;
                if (strDirPath.IndexOf("/", StringComparison.Ordinal) >= 0)
                {
                    strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/", StringComparison.Ordinal));
                    strDirName = strDirPath.Substring(strDirPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                }
                ftpConnect.ChangeDir(strDirPath);
                ftpConnect.SendCommand("RMD " + strDirName);
                if (ftpConnect.ReplyCode != 250)
                    throw (new Exception(ftpConnect.ReplyString.Substring(4)));
            }
            finally
            {
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #endregion
        #region 得到简单的文件列表
        public string[] ListDirectoryFile(string listUrl)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            try
            {
                var ftpUrl = new FtpUrl(listUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirPath = ftpUrl.SubUrl;
                if (strDirPath.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirPath);
                socketData = ftpConnect.CreateDataSocket();
                ftpConnect.SendCommand("NLST");
                if (ftpConnect.ReplyCode != 150 && ftpConnect.ReplyCode != 125)
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                //获得结果
                ftpConnect.Message = "";
                while (true)
                {
                    int iBytes = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
                    ftpConnect.Message += ftpConnect.EncodeType.GetString(ftpConnect.Buffer, 0, iBytes);
                    if (iBytes < ftpConnect.Buffer.Length)
                        break;
                }
                ftpConnect.Message = ftpConnect.Message.Replace('\r', ' ');
                char[] seperator = { '\n' };
                string[] strsFileList = ftpConnect.Message.Split(seperator);
                socketData.Close();//数据socket关闭时也会有返回码
                socketData = null;
                if (ftpConnect.ReplyCode != 226)
                {
                    ftpConnect.ReadReply();
                    if (ftpConnect.ReplyCode != 226)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                }
                return strsFileList;
            }
            finally
            {
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        //获取当前目录下的目录
        public List<string> ListDirectory(string listUrl)
        {
            var strsFileList = new List<string>();
            List<FileStruct> files = ListDirectoryDetails(listUrl);
            foreach (FileStruct fs in files)
            {
                if (fs.IsDirectory)
                {
                    strsFileList.Add(fs.Name);
                }
            }
            return strsFileList;
        }
        #endregion
        #region 得到详细的文件列表
        public List<FileStruct> ListDirectoryDetails(string listUrl)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            try
            {
                var ftpUrl = new FtpUrl(listUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                string strDirPath = ftpUrl.SubUrl;
                if (strDirPath.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirPath = strDirPath.Substring(0, strDirPath.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirPath);
                socketData = ftpConnect.CreateDataSocket();
                ftpConnect.SendCommand("LIST");
                if (ftpConnect.ReplyCode != 150 && ftpConnect.ReplyCode != 125)
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                //获得结果
                ftpConnect.Message = "";
                while (true)
                {
                    int iBytes = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
                    ftpConnect.Message += ftpConnect.EncodeType.GetString(ftpConnect.Buffer, 0, iBytes);
                    if (iBytes < ftpConnect.Buffer.Length)
                        break;
                }
                if (ftpConnect.Message.StartsWith("t"))
                    ftpConnect.Message = ftpConnect.Message.Substring(ftpConnect.Message.IndexOf("\r\n", StringComparison.Ordinal) + 2);
                return GetList(ftpConnect.Message);
            }
            finally
            {
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                    ftpConnect.DisConnect();
            }
        }
        #region 用于得到文件列表的方法
        /// <summary>
        /// 文件列表类型枚举
        /// </summary>
        private enum FileListStyle
        {
            /// <summary>
            /// Unix系统类型
            /// </summary>
            UnixStyle,
            /// <summary>
            /// Windows系统类型
            /// </summary>
            WindowsStyle,
            /// <summary>
            /// 未知类型
            /// </summary>
            Unknown
        }
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件
        /// </summary>
        /// <param name="listUrl">查看目标目录</param>
        /// <returns>返回文件信息列表</returns>
        public List<FileStruct> ListFiles(string listUrl)
        {
            List<FileStruct> listAll = ListDirectoryDetails(listUrl);
            var listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile;
        }
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有的目录
        /// </summary>
        /// <param name="listUrl">查看目标目录</param>
        /// <returns>返回目录信息列表</returns>
        public List<FileStruct> ListDirectories(string listUrl)
        {
            List<FileStruct> listAll = ListDirectoryDetails(listUrl);
            var listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory;
        }
        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private List<FileStruct> GetList(string datastring)
        {
            var myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            FileListStyle directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (directoryListStyle != FileListStyle.Unknown && s != "")
                {
                    var f = new FileStruct { Name = ".." };
                    switch (directoryListStyle)
                    {
                        case FileListStyle.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break;
                        case FileListStyle.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s);
                            break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray;
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="record">文件信息</param>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string record)
        {
            var f = new FileStruct();
            string processstr = record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDtfi = new CultureInfo("en-US", false).DateTimeFormat;
            myDtfi.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDtfi);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                //string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                //processstr = strs[1];
                processstr = processstr.Substring(processstr.IndexOf(' ') + 1);
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }
        /// <summary>
        /// 根据文件列表记录猜想文件列表类型
        /// </summary>
        /// <param name="recordList"></param>
        /// <returns></returns>
        private FileListStyle GuessFileListStyle(IEnumerable<string> recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                if (s.Length > 8 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }
        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string record)
        {
            var f = new FileStruct();
            string processstr = record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.FileSize = Convert.ToInt32(_cutSubstringFromStringWithTrim(ref processstr, ' ', 0));
            //_cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":", StringComparison.Ordinal) >= 0)  //time
            {
				processstr = processstr.Replace(yearOrTime, Util.GetNow().Year.ToString(CultureInfo.InvariantCulture));
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            return f;
        }
        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion
        #endregion
        #region 上传文件

        /// <summary>
        /// 直接上传文件
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="uploadUrl">上传的目标全路径与文件名</param>
        /// <param name="isContinueUpload">是否断点续传</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFileNow(byte[] fileBytes, string uploadUrl, bool isContinueUpload)
        {
            Trace.WriteLine(uploadUrl);
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            int iCharIndex = 0;
            long lOffset = 0;
            try
            {
                var ftpUrl = new FtpUrl(uploadUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                #region 判断并创建文件夹

                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                while (strDirUrl.Trim() != "")
                {
                    iCharIndex = strDirUrl.IndexOf("/", iCharIndex, StringComparison.Ordinal) + 1;
                    string strDirUrlTemp = iCharIndex == 0 ? strDirUrl : strDirUrl.Substring(0, iCharIndex);
                    if (strDirUrlTemp == "")
                        continue;
                    ftpConnect.SendCommand("MKD " + strDirUrlTemp);
                    if (ftpConnect.ReplyCode != 250 && ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                    if (strDirUrlTemp == strDirUrl)
                        break;
                }
                #endregion
                ftpConnect.ChangeDir(strDirUrl);
                ftpConnect.SendCommand("TYPE I");
                #region 得到服务器上的文件大小
                if (isContinueUpload)
                {
                    string strDirName = ftpUrl.SubUrl;
                    if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                        strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                    ftpConnect.ChangeDir(strDirName);
                    ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                    if (ftpConnect.ReplyCode == 213)
                        lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                }
                #endregion
                #region 开始上传
                long lTotalReaded = lOffset;
                socketData = ftpConnect.CreateDataSocket();
                if (lOffset > 0)
                    ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
                else
                    ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
                if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)
                    {
                        OnFileUploadCanceled(ftpConnect);
                        break;
                    }
                    OnFileUploading(ftpConnect, fileBytes.Length, lTotalReaded);
                    //开始上传资料
                    var bytesRead = (int)((fileBytes.Length > lTotalReaded + ftpConnect.Buffer.Length) ? ftpConnect.Buffer.Length : (fileBytes.Length - lTotalReaded));
                    if (bytesRead == 0)
                        break;
                    Array.Copy(fileBytes, lTotalReaded, ftpConnect.Buffer, 0, bytesRead);
                    socketData.Send(ftpConnect.Buffer, bytesRead, 0);
                    lTotalReaded += bytesRead;
                }
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileUploadCompleted(ftpConnect);
                }
                #endregion
            }
            finally
            {
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
            return true;
        }
        /// <summary>
        /// 直接上传文件
        /// </summary>
        /// <param name="filePath">上传文件的全路径</param>
        /// <param name="uploadUrl">上传的目标全路径与文件名</param>
        /// <param name="isContinueUpload">是否断点续传</param>
        /// <returns>上传是否成功</returns>
        public bool UploadFileNow(string filePath, string uploadUrl, bool isContinueUpload)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            FileStream fileStream = null;
            int iCharIndex = 0;
            long lOffset = 0;
            try
            {
                var ftpUrl = new FtpUrl(uploadUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                #region 判断并创建文件夹
                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                while (strDirUrl.Trim() != "")
                {
                    iCharIndex = strDirUrl.IndexOf("/", iCharIndex, StringComparison.Ordinal) + 1;
                    string strDirUrlTemp = iCharIndex == 0 ? strDirUrl : strDirUrl.Substring(0, iCharIndex);
                    if (strDirUrlTemp == "")
                        continue;
                    ftpConnect.SendCommand("MKD " + strDirUrlTemp);
                    if (ftpConnect.ReplyCode != 250 && ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                    if (strDirUrlTemp == strDirUrl)
                        break;
                }
                #endregion
                ftpConnect.ChangeDir(strDirUrl);
                ftpConnect.SendCommand("TYPE I");
                #region 得到服务器上的文件大小
                if (isContinueUpload)
                {
                    string strDirName = ftpUrl.SubUrl;
                    if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                        strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                    ftpConnect.ChangeDir(strDirName);
                    ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                    if (ftpConnect.ReplyCode == 213)
                        lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                }
                #endregion
                #region 开始上传
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                long lTotalReaded = lOffset;
                fileStream.Seek(lOffset, SeekOrigin.Begin);
                socketData = ftpConnect.CreateDataSocket();
                if (lOffset > 0)
                    ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
                else
                    ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
                if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)
                    {
                        OnFileUploadCanceled(ftpConnect);
                        break;
                    }
                    OnFileUploading(ftpConnect, fileStream.Length, lTotalReaded);
                    //开始上传资料
                    int bytesRead = fileStream.Read(ftpConnect.Buffer, 0, ftpConnect.Buffer.Length);
                    if (bytesRead == 0)
                        break;
                    socketData.Send(ftpConnect.Buffer, bytesRead, 0);
                    lTotalReaded += bytesRead;
                }
                fileStream.Close();
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileUploadCompleted(ftpConnect);
                }
                #endregion
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
            return true;
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="uploadUrl"></param>
        public string UploadFile(string filePath, string uploadUrl)
        {
            return UploadFile(filePath, uploadUrl, false);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">上传文件的全路径</param>
        /// <param name="uploadUrl">上传的目标全路径 包含了用户名用户密码与文件名</param>
        /// <param name="isContinueUpload">是否断点续传</param>
        /// <returns>返回控制上传下载的ID</returns>
        public string UploadFile(string filePath, string uploadUrl, bool isContinueUpload)
        {
            String strFtpId = Guid.NewGuid().ToString();
            IList<object> objList = new List<object> { filePath, uploadUrl, isContinueUpload, strFtpId };
            var threadUpload = new Thread(ThreadUploadFile) { Name = "FTP上传" };
            threadUpload.Start(objList);  //开始采用线程方式下载
            return strFtpId;
        }
        /// <summary>
        /// 线程接收上传
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadUploadFile(object obj)
        {
            var objList = obj as IList<object>;
            if (objList != null && objList.Count == 4)
            {
                var filePath = objList[0] as string;
                var uploadUrl = objList[1] as string;
                var isContinueUpload = (bool)objList[2];
                var strFtpId = objList[3] as string;
                try
                {
                    ThreadUploadFile(filePath, uploadUrl, isContinueUpload, strFtpId);
                }
                catch
                {
                    Console.WriteLine(@"error");
                }
            }
        }

        /// <summary>
        /// 线程上传文件
        /// </summary>
        /// <param name="filePath">上传文件的全路径</param>
        /// <param name="uploadUrl">上传的目标全路径与文件名</param>
        /// <param name="isContinueUpload">是否断点续传</param>
        /// <param name="strFtpId"></param>
        /// <returns>上传是否成功</returns>
        private void ThreadUploadFile(string filePath, string uploadUrl, bool isContinueUpload, string strFtpId)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            FileStream fileStream = null;
            int iCharIndex = 0;
            long lOffset = 0;
            try
            {
                var ftpUrl = new FtpUrl(uploadUrl);
                ftpConnect = new FtpConnect(ftpUrl, strFtpId);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                #region 判断并创建文件夹
                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                while (strDirUrl.Trim() != "")
                {
                    iCharIndex = strDirUrl.IndexOf("/", iCharIndex, StringComparison.Ordinal) + 1;
                    string strDirUrlTemp = iCharIndex == 0 ? strDirUrl : strDirUrl.Substring(0, iCharIndex);
                    if (strDirUrlTemp == "")
                        continue;
                    ftpConnect.SendCommand("MKD " + strDirUrlTemp);
                    if (ftpConnect.ReplyCode != 257 && ftpConnect.ReplyCode != 550)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                    if (strDirUrlTemp == strDirUrl)
                        break;
                }
                #endregion
                ftpConnect.ChangeDir(strDirUrl);
                ftpConnect.SendCommand("TYPE I");
                #region 得到服务器上的文件大小
                if (isContinueUpload)
                {
                    string strDirName = ftpUrl.SubUrl;
                    if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                        strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                    ftpConnect.ChangeDir(strDirName);
                    ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                    if (ftpConnect.ReplyCode == 213)
                        lOffset = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                }
                #endregion
                #region 开始上传
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                long lTotalReaded = lOffset;
                fileStream.Seek(lOffset, SeekOrigin.Begin);
                socketData = ftpConnect.CreateDataSocket();
                if (lOffset > 0)
                    ftpConnect.SendCommand("APPE " + Path.GetFileName(uploadUrl));
                else
                    ftpConnect.SendCommand("STOR " + Path.GetFileName(uploadUrl));
                if (!(ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 150))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)
                    {
                        OnFileUploadCanceled(ftpConnect);
                        break;
                    }
                    OnFileUploading(ftpConnect, fileStream.Length, lTotalReaded);
                    //开始上传资料
                    int bytesRead = fileStream.Read(ftpConnect.Buffer, 0, ftpConnect.Buffer.Length);
                    if (bytesRead == 0)
                        break;
                    socketData.Send(ftpConnect.Buffer, bytesRead, 0);
                    lTotalReaded += bytesRead;
                }
                fileStream.Close();
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileUploadCompleted(ftpConnect);
                }
                #endregion
            }
            catch (Exception ex)
            {
                OnFtpError(ftpConnect, ex);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
        }
        /// <summary>
        /// 取消正在上传的文件
        /// </summary>
        /// <returns></returns>
        public void CancelUploadFile(FtpConnect ftpConnect)
        {
            if (ftpConnect != null)
                ftpConnect.DataTransmitting = false;
        }
        /// <summary>
        /// 取消正在上传的文件
        /// </summary>
        /// <param name="strId"></param>
        public void CancelUploadFile(string strId)
        {
            foreach (FtpConnect ftp in _mFtpConnectList)
            {
                if (ftp != null && ftp.Id == strId)
                {
                    ftp.DataTransmitting = false;
                    break;
                }
            }
        }
        #endregion
        #region 下载文件
        /// <summary>
        /// 直接下载文件
        /// </summary>
        /// <param name="downloadUrl">要下载文件的路径</param>
        /// <param name="fileBytes">存贮的内容</param>
        /// <returns>下载是否成功</returns>
        public bool DownloadFileNow(string downloadUrl, out byte[] fileBytes)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            fileBytes = new byte[] { };
            long lTotalReaded = 0;
            try
            {
                var ftpUrl = new FtpUrl(downloadUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirUrl);
                #region 得到服务器上的文件大小
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                long lFileSize;
                if (ftpConnect.ReplyCode == 213)
                    lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                else
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #endregion

                socketData = ftpConnect.CreateDataSocket();
                fileBytes = new byte[lFileSize];
                ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
                if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #region 开始下载
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
                    {
                        OnFileDownloadCanceled(ftpConnect);
                        break;
                    }
                    OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
                    //开始将文件流写入本地
                    int bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
                    if (bytesRead <= 0)
                        break;
                    Array.Copy(ftpConnect.Buffer, 0, fileBytes, lTotalReaded, bytesRead);
                    lTotalReaded += bytesRead;
                }
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileDownloadCompleted(ftpConnect);
                }
                #endregion
            }
            finally
            {
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
            return true;
        }
        /// <summary>
        /// 直接下载文件
        /// </summary>
        /// <param name="downloadUrl">要下载文件的路径</param>
        /// <param name="targetFile">目标存在全路径</param>
        /// <param name="isContinueDownload">是否断点续传</param>
        /// <returns>下载是否成功</returns>
        public bool DownloadFileNow(string downloadUrl, string targetFile, bool isContinueDownload)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            FileStream fileStream = null;
            long lTotalReaded = 0;
            try
            {
                var ftpUrl = new FtpUrl(downloadUrl);
                ftpConnect = new FtpConnect(ftpUrl);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirUrl);
                #region 得到服务器上的文件大小
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                long lFileSize;
                if (ftpConnect.ReplyCode == 213)
                    lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                else
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #endregion

                socketData = ftpConnect.CreateDataSocket();
                //断点续传长度的偏移量
                if (File.Exists(targetFile) && isContinueDownload)
                {
                    var fiInfo = new FileInfo(targetFile);
                    lTotalReaded = fiInfo.Length;
                    ftpConnect.SendCommand("REST " + fiInfo.Length);
                    if (ftpConnect.ReplyCode != 350)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                }
                ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
                if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #region 开始下载
                string strTargetPath = targetFile;
                strTargetPath = strTargetPath.Substring(0, strTargetPath.LastIndexOf("\\", StringComparison.Ordinal));//这里本来是//
                if (!Directory.Exists(strTargetPath)) //判断目标路径是否存在，如果不存在就创建
                    Directory.CreateDirectory(strTargetPath);
                if (File.Exists(targetFile) && isContinueDownload)  //目标文件已经是全路径了 断点续传
                    fileStream = new FileStream(targetFile, FileMode.Append, FileAccess.Write);
                else
                    fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write);
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
                    {
                        OnFileDownloadCanceled(ftpConnect);
                        break;
                    }
                    OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
                    //开始将文件流写入本地
                    int bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
                    if (bytesRead <= 0)
                        break;
                    fileStream.Write(ftpConnect.Buffer, 0, bytesRead);
                    lTotalReaded += bytesRead;
                }
                fileStream.Close();
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileDownloadCompleted(ftpConnect);
                }
                #endregion
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
            return true;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downloadUrl"></param>
        /// <param name="targetFile"></param>
        public string DownloadFile(string downloadUrl, string targetFile)
        {
            return DownloadFile(downloadUrl, targetFile, false);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downloadUrl">要下载文件的路径 包含了登录名与登录密码</param>
        /// <param name="targetFile"></param>
        /// <param name="isContinueDownload">是否断点续传</param>
        /// <returns>返回下载控制ID</returns>
        public string DownloadFile(string downloadUrl, string targetFile, bool isContinueDownload)
        {
            String strFtpId = Guid.NewGuid().ToString();
            IList<object> objList = new List<object> { downloadUrl, targetFile, isContinueDownload, strFtpId };
            var threadDownload = new Thread(ThreadDownloadFile);
            threadDownload.Start(objList);  //开始采用线程方式下载
            return strFtpId;
        }
        /// <summary>
        /// 线程接收下载
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadDownloadFile(object obj)
        {
            var objList = obj as IList<object>;
            if (objList != null && objList.Count == 4)
            {
                var downloadUrl = objList[0] as string;
                var targetFile = objList[1] as string;
                var isContinueDownload = (bool)objList[2];
                var strFtpId = objList[3] as String;
                ThreadDownloadFile(downloadUrl, targetFile, isContinueDownload, strFtpId);
            }
        }

        /// <summary>
        /// 线程下载文件
        /// </summary>
        /// <param name="downloadUrl">要下载文件的路径</param>
        /// <param name="targetFile">目标存在全路径</param>
        /// <param name="isContinueDownload">是否断点续传</param>
        /// <param name="strFtpId"></param>
        /// <returns>下载是否成功</returns>
        private void ThreadDownloadFile(string downloadUrl, string targetFile, bool isContinueDownload, string strFtpId)
        {
            FtpConnect ftpConnect = null;
            Socket socketData = null;
            FileStream fileStream = null;
            long lTotalReaded = 0;
            try
            {
                var ftpUrl = new FtpUrl(downloadUrl);
                ftpConnect = new FtpConnect(ftpUrl, strFtpId);
                ftpConnect.Connect();
                _mFtpConnectList.Add(ftpConnect);//添加到控制器中
                string strDirUrl = ftpUrl.SubUrl;
                if (strDirUrl.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirUrl = strDirUrl.Substring(0, strDirUrl.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirUrl);
                #region 得到服务器上的文件大小
                string strDirName = ftpUrl.SubUrl;
                if (strDirName.IndexOf("/", StringComparison.Ordinal) >= 0)
                    strDirName = strDirName.Substring(0, strDirName.LastIndexOf("/", StringComparison.Ordinal));
                ftpConnect.ChangeDir(strDirName);
                ftpConnect.SendCommand("SIZE " + Path.GetFileName(ftpUrl.SubUrl));
                long lFileSize;
                if (ftpConnect.ReplyCode == 213)
                    lFileSize = Int64.Parse(ftpConnect.ReplyString.Substring(4));
                else
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #endregion

                socketData = ftpConnect.CreateDataSocket();
                //断点续传长度的偏移量
                if (File.Exists(targetFile) && isContinueDownload)
                {
                    var fiInfo = new FileInfo(targetFile);
                    lTotalReaded = fiInfo.Length;
                    ftpConnect.SendCommand("REST " + fiInfo.Length);
                    if (ftpConnect.ReplyCode != 350)
                        throw new IOException(ftpConnect.ReplyString.Substring(4));
                }
                ftpConnect.SendCommand("RETR " + Path.GetFileName(ftpUrl.SubUrl));
                if (!(ftpConnect.ReplyCode == 150 || ftpConnect.ReplyCode == 125 || ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    throw new IOException(ftpConnect.ReplyString.Substring(4));
                #region 开始下载
                string strTargetPath = targetFile;
                strTargetPath = strTargetPath.Substring(0, strTargetPath.LastIndexOf("//", StringComparison.Ordinal));
                if (!Directory.Exists(strTargetPath)) //判断目标路径是否存在，如果不存在就创建
                    Directory.CreateDirectory(strTargetPath);
                if (File.Exists(targetFile) && isContinueDownload)  //目标文件已经是全路径了 断点续传
                    fileStream = new FileStream(targetFile, FileMode.Append, FileAccess.Write);
                else
                    fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write);
                ftpConnect.DataTransmitting = true;
                while (true)
                {
                    if (!ftpConnect.DataTransmitting)    //判断取消是否取消了下载
                    {
                        OnFileDownloadCanceled(ftpConnect);
                        break;
                    }
                    OnFileDownloading(ftpConnect, lFileSize, lTotalReaded);
                    //开始将文件流写入本地
                    int bytesRead = socketData.Receive(ftpConnect.Buffer, ftpConnect.Buffer.Length, 0);
                    if (bytesRead <= 0)
                        break;
                    fileStream.Write(ftpConnect.Buffer, 0, bytesRead);
                    lTotalReaded += bytesRead;
                }
                fileStream.Close();
                if (socketData.Connected)
                    socketData.Close();
                if (ftpConnect.DataTransmitting)
                {
                    if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                    {
                        ftpConnect.ReadReply();
                        if (!(ftpConnect.ReplyCode == 226 || ftpConnect.ReplyCode == 250))
                            throw new IOException(ftpConnect.ReplyString.Substring(4));
                    }
                    OnFileDownloadCompleted(ftpConnect);
                }
                #endregion
            }
            catch (Exception ex)
            {
                OnFtpError(ftpConnect, ex);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (socketData != null && socketData.Connected)
                    socketData.Close();
                if (ftpConnect != null)
                {
                    ftpConnect.DisConnect();
                    _mFtpConnectList.Remove(ftpConnect);
                }
            }
        }
        /// <summary>
        /// 取消正在下载的文件
        /// </summary>
        /// <returns></returns>
        public void CancelDownloadFile(FtpConnect ftpConnect)
        {
            if (ftpConnect != null)
                ftpConnect.DataTransmitting = false;
        }
        /// <summary>
        /// 取消正在下载的文件
        /// </summary>
        /// <param name="strId"></param>
        public void CancelDownloadFile(string strId)
        {
            foreach (FtpConnect ftp in _mFtpConnectList)
            {
                if (ftp != null && ftp.Id == strId)
                {
                    ftp.DataTransmitting = false;
                    break;
                }
            }
        }
        #endregion
        #region 根据指定的ID查找FTPConnect
        /// <summary>
        /// 根据指定的ID查找FTPConnect
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FtpConnect FindFtpConnectById(string id)
        {
            foreach (FtpConnect ftpConnect in _mFtpConnectList)
            {
                if (ftpConnect != null && ftpConnect.Id == id)
                    return ftpConnect;
            }
            return null;
        }
        #endregion

        #region 传输类型
        public enum FtpType
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None,
            /// <summary>
            /// 上传
            /// </summary>
            Upload,
            /// <summary>
            /// 下载
            /// </summary>
            Download
        }
        #endregion
    }
}