namespace DesktopApp.Infrastructure
{
    public static class TokenManager
    {
        /// <summary>
        /// 下载完成消息，用于刷新列表
        /// </summary>
        public static string RefreshList => "RefreshList";

        /// <summary>
        /// 下载开始消息，用于通知主界面显示下载选项卡
        /// </summary>
        public static string DownStart => "DownStart";

        public static string ShowSetting => "ShowSetting";

        public static string DownloadComplete => "DownloadComplete";

        public static string RefreshDownloadList => "RefreshDownloadList";

        public static string CloseCustomWindow => "CloseCustomWindow";

        public static string ImportState => "ImportState";

        public static string DownloadState => "DownloadState";
    }
}
