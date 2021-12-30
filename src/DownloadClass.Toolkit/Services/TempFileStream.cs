using System.IO;

namespace DownloadClass.Toolkit.Services
{
    internal class TempFileStream : FileStream
    {
        private bool _disposed;
        private readonly string _tempFilePath;

        public TempFileStream(string path, FileMode mode) : base(path, mode) => _tempFilePath = path;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            base.Dispose(disposing);
            File.Delete(_tempFilePath);

            _disposed = true;
        }
    }
}
