using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868b6-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaEvent
    {
        [PreserveSig]
        int GetEventHandle([Out] out IntPtr hEvent); // HEVENT

        [PreserveSig]
        int GetEvent(
            [Out] out EventCode lEventCode,
            [Out] out IntPtr lParam1,
            [Out] out IntPtr lParam2,
            [In] int msTimeout
            );

        [PreserveSig]
        int WaitForCompletion(
            [In] int msTimeout,
            [Out] out EventCode pEvCode
            );

        [PreserveSig]
        int CancelDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        int RestoreDefaultHandling([In] EventCode lEvCode);

        [PreserveSig]
        int FreeEventParams(
            [In] EventCode lEvCode,
            [In] IntPtr lParam1,
            [In] IntPtr lParam2
            );
    }
}