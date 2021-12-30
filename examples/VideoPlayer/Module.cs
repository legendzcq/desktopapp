using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

using VideoPlayer.Utils;

namespace VideoPlayer
{
    internal class Module
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddToolkit(options => options.CurrentUser = "chenhaibin415");
            ServiceLocator.SetServiceProvider(serviceCollection.BuildServiceProvider());
        }
    }
}
