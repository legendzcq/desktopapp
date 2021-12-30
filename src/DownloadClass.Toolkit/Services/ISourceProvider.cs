using System;
using System.IO;
using System.Threading.Tasks;

namespace DownloadClass.Toolkit.Services
{
    public interface ISourceProvider
    {
        Task<Stream> ProvideAsync(Uri uri);

        Task<(bool isSuccessful, Stream? stream)> TryProvideAsync(Uri uri);
    }
}
