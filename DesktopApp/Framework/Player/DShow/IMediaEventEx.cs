using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaEventEx : IMediaEvent
    {
        #region IMediaEvent Methods

        [PreserveSig]
        new int GetEventHandle([Out] out IntPtr hEvent); // HEVENT

        [PreserveSig]
        new int GetEvent(
            [Out] out EventCode lEventCode,
            [Out] out IntPtr lParam1,
            [Out] out IntPtr lParam2,
            [In] int msTimeout
            );

        [PreserveSig]
        new int WaitForCompletion(
            [In] int msTimeout,
            [Out] out EventCode pEvCode
            );

        [PreserveSig]
        new int CancelDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        new int RestoreDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        new int FreeEventParams(
            [In] EventCode lEvCode,
            [In] IntPtr lParam1,
            [In] IntPtr lParam2
            );

        #endregion

        [PreserveSig]
        int SetNotifyWindow(
            [In] IntPtr hwnd, // HWND *
            [In] int lMsg,
            [In] IntPtr lInstanceData // PVOID
            );

        [PreserveSig]
        int SetNotifyFlags([In] NotifyFlags lNoNotifyFlags);

        [PreserveSig]
        int GetNotifyFlags([Out] out NotifyFlags lplNoNotifyFlags);
    }
}