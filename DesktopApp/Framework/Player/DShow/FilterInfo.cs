using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From FILTER_INFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;
        [MarshalAs(UnmanagedType.Interface)]
        public IFilterGraph pGraph;
    }
}