using System;

namespace DownloadClass.Toolkit.Events
{
    public class ExceptionEventArgs : EventArgs
    {
        internal ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
