using System.Windows.Input;

namespace DownloadClass.Toolkit.Controls
{
    public static class Commands
    {
        public static RoutedUICommand ExitFullScreen { get; } = new RoutedUICommand("退出全屏", nameof(ExitFullScreen), typeof(Commands));

        public static RoutedUICommand TogglePlayPause { get; } = new RoutedUICommand("播放/暂停", nameof(TogglePlayPause), typeof(Commands));

        public static RoutedUICommand GoForward { get; } = new RoutedUICommand("前进", nameof(GoForward), typeof(Commands));

        public static RoutedUICommand Backup { get; } = new RoutedUICommand("后退", nameof(Backup), typeof(Commands));
    }
}
