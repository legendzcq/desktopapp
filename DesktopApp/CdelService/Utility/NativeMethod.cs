using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CdelService.Utility
{
    public static class NativeMethod
    {
        [DllImport("winInet.dll")]
        internal static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
        [DllImport("sensapi.dll")]
        internal extern static bool IsNetworkAlive(out int connectionDescription);
        [DllImport("kernel32.dll")]
        internal extern static IntPtr LoadLibrary(String path);
        [DllImport("kernel32.dll")]
        internal extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
    }
}
