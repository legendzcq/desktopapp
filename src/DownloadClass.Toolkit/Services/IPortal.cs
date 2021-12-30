using System.Threading.Tasks;

using DownloadClass.Toolkit.Models;

using Refit;

namespace DownloadClass.Toolkit.Services
{
    public interface IPortal
    {
        [Post("/video/header/genkey")]
        Task<string> GetGenKeyAsync();

        [Post("/video/header/gethead")]
        Task<string> GetHeadAsync([Body(BodySerializationMethod.UrlEncoded)] GetHeadParams @params);
    }
}
