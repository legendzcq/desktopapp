using System;
using System.Runtime.InteropServices;

namespace DesktopApp.Interop
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
    }
}
