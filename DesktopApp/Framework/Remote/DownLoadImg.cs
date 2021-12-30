using System.IO;

namespace Framework.Remote
{
    internal class DownLoadImg
    {
        internal string DownCwareImage(string url, string savePath)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            var web = new WebProxyClient();
            try
            {
                string saveFileName = Path.GetFileName(url);
                string saveFile = savePath + "\\" + saveFileName;
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                if (File.Exists(saveFile)) return saveFileName;
                byte[] buffer = web.DownloadData(url);
                File.WriteAllBytes(saveFile, buffer);
                return saveFileName;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal byte[] DownCwareImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return new byte[0];
            var web = new WebProxyClient();
            try
            {
                byte[] buffer = web.DownloadData(url);
                return buffer;
            }
            catch
            {
                return new byte[0];
            }
        }
    }
}
