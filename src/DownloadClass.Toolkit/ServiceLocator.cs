using System;

namespace DownloadClass.Toolkit
{
    internal class ServiceLocator
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;

        public static void SetServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    }
}
