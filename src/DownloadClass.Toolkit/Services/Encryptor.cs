using System;
using System.Linq;
using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

namespace DownloadClass.Toolkit.Services
{
    public class Encryptor : IEncryptor
    {
        private readonly ILogger<Encryptor> _logger;

        public Encryptor(ILogger<Encryptor> logger) => _logger = logger;

        public (byte[] primaryKey, byte[] aesKey) EncrypteHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException($"'{nameof(hash)}' cannot be null or whitespace.", nameof(hash));

            var primaryKey = new byte[64];
            var rngProvider = new RNGCryptoServiceProvider();
            rngProvider.GetBytes(primaryKey);
            _logger.LogInformation("{primaryKey} has been generated.", Convert.ToBase64String(primaryKey));

            var converted = Enumerable.Range(0, hash.Length).Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
                .ToArray();
            using var md5 = new MD5Algorithm();
            var aeskey = md5.ComputeHash(converted.Concat(primaryKey).ToArray());
            _logger.LogInformation("{aesKey} has been computed", Convert.ToBase64String(aeskey));
            return (primaryKey, aeskey);
        }
    }
}
