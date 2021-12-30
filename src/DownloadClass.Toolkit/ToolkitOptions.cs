namespace DownloadClass.Toolkit
{
    public class ToolkitOptions
    {
        public const string Position = "Toolkit";

        /// <summary>
        /// 当前用户的用户名
        /// </summary>
        public string CurrentUser { get; set; } = default!;

        /// <summary>
        /// 设备密钥
        /// </summary>
        public byte[] MachineKey { get; set; } = default!;
    }
}
