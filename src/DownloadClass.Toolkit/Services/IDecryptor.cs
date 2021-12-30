
using System.IO;

using DownloadClass.Toolkit.Models;

namespace DownloadClass.Toolkit.Services
{
    public interface IDecryptor
    {
        Courseware DecrypteXml(string path);

        bool TryDecrypteXml(string path, out Courseware? courseware);

        byte[] DecrypteHead(string encrypted, byte[] aesKey);

        bool TryDecrypteHead(string encrypted, byte[] aesKey, out byte[]? head);

        Stream DecrypteVideo(string cdelPath, byte[] aesKey);
    }
}