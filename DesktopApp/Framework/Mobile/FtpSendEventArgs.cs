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
        /// ���ֽ���
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// �Ѵ����ֽ���
        /// </summary>
        public long BytesTransfered { get; set; }
    }
}