using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;

using DownloadClass.Toolkit.Models;

using Microsoft.Extensions.Logging;

namespace DownloadClass.Toolkit.Services
{
    public class Decryptor : IDecryptor
    {
        private readonly ILogger<Decryptor> _logger;

        public Decryptor(ILogger<Decryptor> logger) => _logger = logger;

        public Courseware DecrypteXml(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));

            using FileStream fileStream = File.OpenRead(path);
            using var fromBase64 = new CryptoStream(fileStream, new FromBase64Transform(), CryptoStreamMode.Read);

            using var desProvider = new DESCryptoServiceProvider()
            {
                Key = new byte[] { 0x45, 0x34, 0x48, 0x44, 0x39, 0x68, 0x34, 0x44 },
                IV = new byte[] { 0x66, 0x59, 0x66, 0x68, 0x48, 0x65, 0x44, 0x6D },
                Padding = PaddingMode.None,
                Mode = CipherMode.CBC
            };
            using var fromDes = new CryptoStream(fromBase64, desProvider.CreateDecryptor(), CryptoStreamMode.Read);

            using var reader = XmlReader.Create(fromDes);
            var serializer = new XmlSerializer(typeof(Courseware));
            Courseware courseware = (serializer.Deserialize(reader) as Courseware)!;

            _logger.LogInformation("Decrypted {path} xml successfully.", path);
            return courseware;
        }

        public bool TryDecrypteXml(string path, out Courseware? courseware)
        {
            try
            {
                courseware = DecrypteXml(path);
                return true;
            }
            catch (Exception exception)
            {
                courseware = default;
                _logger.LogWarning(exception, "decrypting {path} xml failed.", path);
                return false;
            }
        }

        public byte[] DecrypteHead(string encrypted, byte[] aesKey)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
                throw new ArgumentException($"'{nameof(encrypted)}' cannot be null or whitespace.", nameof(encrypted));
            if (encrypted.Contains("error", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"the encrypted head {encrypted} is not valid");

            var index = encrypted.IndexOf('|');
            if (index >= 0)
            {
                encrypted = encrypted[(index + 1)..];
            }

            var converted = Enumerable.Range(0, encrypted.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(encrypted.Substring(x, 2), 16))
                .ToArray();

            using var aesProvider = new AesCryptoServiceProvider()
            {
                Key = aesKey,
                Padding = PaddingMode.None,
                IV = converted[..16]
            };
            using var encryptedStream = new MemoryStream(converted[16..]);
            using var decryptedStream = new CryptoStream(encryptedStream, aesProvider.CreateDecryptor(), CryptoStreamMode.Read);
            var decrypted = new byte[converted.Length - 16];
            decryptedStream.Read(decrypted);
            _logger.LogInformation("decrypted {head} successful", $"{encrypted.Substring(0, 6)}...");
            return decrypted;
        }

        public bool TryDecrypteHead(string encrypted, byte[] key, out byte[]? head)
        {
            try
            {
                head = DecrypteHead(encrypted, key);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "decrypted {head} failed", $"{encrypted.Substring(0, 6)}...");
                head = default;
                return false;
            }
        }

        public Stream DecrypteVideo(string cdelPath, byte[] aesKey)
        {
            var extension = Path.GetExtension(cdelPath);
            if (extension != ".cdel")
                throw new NotSupportedException("only support .cdel file");

            using FileStream? cdelStream = File.Open(cdelPath, FileMode.Open);
            cdelStream.Seek(4, SeekOrigin.Current);
            Span<byte> encryptedLengthRaw = stackalloc byte[4];
            cdelStream.Read(encryptedLengthRaw);
            var encryptedLength = (BitConverter.ToInt32(encryptedLengthRaw) >> 16) * 1024;
            Span<byte> iv = stackalloc byte[16];
            cdelStream.Read(iv);

            cdelStream.Seek(32, SeekOrigin.Begin);
            var encrypted = new byte[encryptedLength];
            cdelStream.Read(encrypted);
            using var aesProvider = new AesCryptoServiceProvider()
            {
                Padding = PaddingMode.None,
                KeySize = 128,
                Key = aesKey,
                IV = iv.ToArray(),
            };
            var decrypted = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV).TransformFinalBlock(encrypted, 0, encryptedLength);
            var buffer = new TempFileStream(Path.GetTempFileName(), FileMode.Create);
            buffer.Write(decrypted);
            cdelStream.CopyTo(buffer);
            buffer.Seek(0, SeekOrigin.Begin);
            return buffer;
        }
    }
}