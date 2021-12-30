
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace DownloadClass.Toolkit.Utils
{
    internal class RegistryUtil
    {
        private const string NavigatingSoundKey = @"HKEY_CURRENT_USER\AppEvents\Schemes\Apps\Explorer\Navigating\.Current";
        private static string? s_navigatingSoundBackup;
        private static readonly ILogger<RegistryUtil> s_logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger<RegistryUtil>>();

        public static void CancelNavigatingSound()
        {
            if (!string.IsNullOrWhiteSpace(s_navigatingSoundBackup))
                return;
            try
            {
                s_navigatingSoundBackup = Registry.GetValue(NavigatingSoundKey, string.Empty, null) as string;
                Registry.SetValue(NavigatingSoundKey, string.Empty, string.Empty);
            }
            catch (Exception exception)
            {
                s_logger.LogWarning(exception, "Cancel navigating sound failed");
            }
        }

        public static void RecoverNavigatingSound()
        {
            if (!string.IsNullOrWhiteSpace(s_navigatingSoundBackup))
            {
                try
                {
                    Registry.SetValue(NavigatingSoundKey, string.Empty, s_navigatingSoundBackup);
                }
                catch (Exception exception)
                {
                    s_logger.LogWarning(exception, "Recover navigating sound failed");
                }

                s_navigatingSoundBackup = null;
            }
        }
    }
}
