﻿using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From PIN_INFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PinInfo
    {
        [MarshalAs(UnmanagedType.Interface)]
        public IBaseFilter filter;
        public PinDirection dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
    }
}