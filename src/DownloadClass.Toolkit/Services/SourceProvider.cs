using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using DownloadClass.Toolkit.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DownloadClass.Toolkit.Services
{
    public class SourceProvider : ISourceProvider
    {
        private readonly ILogger<SourceProvider> _logger;
        private readonly IOptionsSnapshot<ToolkitOptions> _options;
        private readonly IDecryptor _decryptor;
        private readonly IPortal _portal;
        private readonly IEncryptor _encryptor;

        public SourceProvider(ILogger<SourceProvider> logger, IDecryptor decryptor, IPortal portal, IEncryptor encryptor, IOptionsSnapshot<ToolkitOptions> options)
        {
            _logger = logger;
            _decryptor = decryptor;
            _portal = portal;
            _encryptor = encryptor;
            _options = options;
        }

        private async Task<Stream> ProvideByZipAsync(string path)
        {
            ZipArchive zip = ZipFile.OpenRead(path);
            if (!zip.Entries.Any(x => x.Name == "videofile.mp4"))
            {
                _logger.LogWarning("the zip file {path} do not contains videofile.mp4", path);
                throw new ArgumentException($"the zip file {path} do not contains videofile.mp4");
            }

            var zipName = Path.GetFileNameWithoutExtension(path);
            var tempPath = Path.Combine(Path.GetTempPath(), "cdel", zipName);
            zip.ExtractToDirectory(tempPath, true);
            _logger.LogInformation("the zip file {path} has been extracted in {tempPath}", path, tempPath);

            var targetDirectory = Directory.GetDirectories(tempPath).Single(x => zipName.Contains(Path.GetFileName(x)));
            var configXmlPath = Path.Combine(targetDirectory, "config.xml");
            Courseware courseware = _decryptor.DecrypteXml(configXmlPath);

            var hash = await _portal.GetGenKeyAsync().ConfigureAwait(false);
            _logger.LogInformation("get {hash} successful", $"{hash.Substring(0, 6)}...");
            (var primarykey, var aesKey) = _encryptor.EncrypteHash(hash);
            var encryptedhead = await _portal.GetHeadAsync(new GetHeadParams(courseware, _options.Value.CurrentUser!, primarykey, hash)).ConfigureAwait(false);
            _logger.LogInformation("get head successful", $"{encryptedhead.Substring(0, 6)}...");
            var head = _decryptor.DecrypteHead(encryptedhead, aesKey);

            using (FileStream? video = File.Open(Path.Combine(tempPath, "videofile.mp4"), FileMode.Open))
            {
                using var videoWriter = new BinaryWriter(video);
                videoWriter.BaseStream.Position = 0;
                videoWriter.Write(head);
            }
            FileStream? videoBuffer = File.OpenRead(Path.Combine(tempPath, "videofile.mp4"));
            var buffered = new BufferedStream(videoBuffer);
            _logger.LogInformation("get video from {path} successful", path);
            return buffered;
        }
        private Stream ProvideByCdel(string absolutePath) => _decryptor.DecrypteVideo(absolutePath, _options.Value.MachineKey);

        public async Task<Stream> ProvideAsync(Uri uri)
        {
            if (uri.Scheme == Uri.UriSchemeFile)
            {
                var localPath = uri.LocalPath;
                var extension = Path.GetExtension(localPath);
                return extension switch
                {
                    ".zip" => await ProvideByZipAsync(localPath),
                    ".cdel" => ProvideByCdel(localPath),
                    _ => throw new NotSupportedException("Not supported except for zip、cdel."),
                };
            }

            throw new NotSupportedException("Not supported except for file scheme.");
        }

        public async Task<(bool isSuccessful, Stream? stream)> TryProvideAsync(Uri uri)
        {
            try
            {
                Stream stream = await ProvideAsync(uri).ConfigureAwait(false);
                return (true, stream);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "get video from {path} failed.", uri);
                return (false, default);
            }
        }
    }
}