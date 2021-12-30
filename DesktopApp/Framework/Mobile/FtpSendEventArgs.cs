using System;

namespace Framework.Mobile
{
    public delegate void FtpSendEventHandler(object sender, FtpSendEventArgs e);

    public class FtpSendEventArgs : EventArgs
    {
        public FtpSendEventArgs()
        {
            TotalBytes = 0;
            BytesTransfered = 0;
        }
        public FtpSendEventArgs(long lTotalBytes, long lBytesTransfered)
        {
            TotalBytes = lTotalBytes;
            BytesTransfered = lBytesTransfered;
        }

        /// <summary>
        /// 总字节数
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// 已传输字节数
        /// </summary>
        public long BytesTransfered { get; set; }
    }
}