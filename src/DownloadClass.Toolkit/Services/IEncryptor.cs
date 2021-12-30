namespace DownloadClass.Toolkit.Services
{
    public interface IEncryptor
    {
        (byte[] primaryKey, byte[] aesKey) EncrypteHash(string hash);
    }
}
