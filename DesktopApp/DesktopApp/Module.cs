using System.Runtime.CompilerServices;
using System.Timers;
using DesktopApp.Utils;

using Framework.Utility;

using Microsoft.Extensions.DependencyInjection;

namespace DesktopApp
{
    internal class Module
    {

        [ModuleInitializer]
        public static void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddToolkit(options =>
            {
                options.CurrentUser = Util.UserName;
                options.MachineKey = Crypt.MachineKey;
            });

            Timer timer = new Timer(30000);
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Elapsed += delegate
            {
                // 调度到主线程触发更新，直接在当前线程更新会报错
                App.CurrentMainWindow.AutoUpdaterStart();
                //Updater.Start(true);
            };
            timer.Start();
        }
    }
}