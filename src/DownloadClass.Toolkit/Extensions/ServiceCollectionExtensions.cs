using System;

using DownloadClass.Toolkit;
using DownloadClass.Toolkit.Services;

using LibVLCSharp;

using Refit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddToolkit(this ServiceCollection @this, Action<ToolkitOptions> configureOptions)
        {
            @this.AddOptions<ToolkitOptions>().Configure(configureOptions);

            @this.AddRefitClient<IPortal>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://mportal.chinaacc.com"));
            @this.AddTransient<IEncryptor, Encryptor>();
            @this.AddTransient<IDecryptor, Decryptor>();
            @this.AddTransient<ISourceProvider, SourceProvider>();

            AddLibVlc(@this);

            ServiceLocator.SetServiceProvider(@this.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true
            }));
            return @this;
        }

        private static void AddLibVlc(ServiceCollection @this)
        {
            Core.Initialize();
            @this.AddSingleton(new LibVLC());
            @this.AddScoped(provider =>
            {
                LibVLC libVlc = provider.GetRequiredService<LibVLC>();
                var mediaPlayer = new MediaPlayer(libVlc);
                return mediaPlayer;
            });
        }
    }
}
