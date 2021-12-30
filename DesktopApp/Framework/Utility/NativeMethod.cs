using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Framework.Utility
{
    public static class NativeMethod
    {
        [Flags]
        public enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }

        #region Nested type: MINMAXINFO

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        } ;

        #endregion

        #region Nested type: MONITORINFO

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor;

            /// <summary>
            /// </summary>            
            public RECT rcWork;

            /// <summary>
            /// </summary>            
            public int dwFlags;
        }

        #endregion

        #region Nested type: RECT

        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;

            /// <summary> Win32 </summary>
            public int top;

            /// <summary> Win32 </summary>
            public int right;

            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty;

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
            }

            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom +
                       " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right &&
                        rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        #endregion

        /// <summary>
        /// 鼠标的坐标
        /// </summary>
        public struct PointAPI
        {
            public int X;
            public int Y;
        }

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string cls, string win);
        [DllImport("user32")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32")]
        public static extern bool OpenIcon(IntPtr hWnd);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        [DllImport("winInet.dll")]
        internal static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
        [DllImport("sensapi.dll")]
        internal extern static bool IsNetworkAlive(out int connectionDescription);

        /*
         * 在winmm.dll中   
         *第一个参数可以为0，表示首选设备   
         *第二个参数为音量:0xFFFF为最大，0x0000为最小，
         *其中高位（前两位）表示右声道音量，低位（后两位）表示左 声道音量 。
        */
        #region Windows Media API
        [DllImport("winmm.dll")]
        public static extern long waveOutSetVolume(UInt32 deviceID, UInt32 Volume);

        [DllImport("winmm.dll")]
        public static extern long waveOutGetVolume(UInt32 deviceID, out UInt32 Volume);
        #endregion
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public const uint WM_APPCOMMAND = 0x319;
        public const uint APPCOMMAND_VOLUME_UP = 0x0a;
        public const uint APPCOMMAND_VOLUME_DOWN = 0x09;
        public const uint APPCOMMAND_VOLUME_MUTE = 0x08;

        #region 调用user32中获取鼠标位置的函数

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="lpPoint">鼠标的X、Y结构体</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out PointAPI lpPoint);

        #endregion

        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);

        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
        public const uint ES_CONTINUOUS = 0x80000000;

        [DllImport("user32.dll")]
        internal static extern bool SystemParametersInfo(uint uiAction, bool uiParam, ref bool pvParam, uint fWinIni);

        public const uint SPI_GETSCREENSAVEACTIVE = 0x0010;
        public const uint SPI_SETSCREENSAVEACTIVE = 0x0011;
        public const uint SPIF_SENDCHANGE = 0x0002;
        public const uint SPIF_SENDWININICHANGE = SPIF_SENDCHANGE;

        [DllImport("user32.dll")]
        internal static extern int SetParent(IntPtr hwChild, IntPtr hwParent);

        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime")]
        internal extern static int GetDoubleClickTime();

        //private const int winStyle = 0x16cf0000;
        //private const int winStyleEx = 0x00000100;

        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //static extern bool AdjustWindowRectEx(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
        public const int WMGraphNotify = 0x8000 + 13;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
            public POINT(Point pt)
            {
                X = Convert.ToInt32(pt.X);
                Y = Convert.ToInt32(pt.Y);
            }
        };

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("kernel32.dll")]
        internal extern static IntPtr LoadLibrary(String path);
        [DllImport("kernel32.dll")]
        internal extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
        [DllImport("kernel32.dll")]
        internal extern static bool FreeLibrary(IntPtr lib);

        /// <summary>
        /// 获取路径可用空间
        /// </summary>
        [DllImport("kernel32.dll")]
        internal static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct DISPLAY_DEVICE
		{
			public int cb;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;
			public int StateFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceKey;
		}
    }
}
