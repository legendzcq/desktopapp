using System;

namespace Framework.Mobile
{
    public delegate void FtpErrorEventHandler(object sender, FtpErrorEventArgs e);

    public class FtpErrorEventArgs : EventArgs
    {
        public FtpErrorEventArgs() { }
        public FtpErrorEventArgs(Exception error)
        {
            Error = error;
        }

        /// <summary>
        /// ´íÎóÏûÏ¢
        /// </summary>
        public Exception Error { get; set; }
    }
}