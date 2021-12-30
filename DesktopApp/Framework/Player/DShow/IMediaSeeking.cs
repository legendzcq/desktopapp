using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("36b73880-c2c8-11cf-8b46-00805f6cef60"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSeeking
    {
        [PreserveSig]
        int GetCapabilities([Out] out AMSeekingSeekingCapabilities pCapabilities);

        [PreserveSig]
        int CheckCapabilities([In, Out] ref AMSeekingSeekingCapabilities pCapabilities);

        [PreserveSig]
        int IsFormatSupported([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int QueryPreferredFormat([Out] out Guid pFormat);

        [PreserveSig]
        int GetTimeFormat([Out] out Guid pFormat);

        [PreserveSig]
        int IsUsingTimeFormat([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int SetTimeFormat([In, MarshalAs(UnmanagedType.LPStruct)] Guid pFormat);

        [PreserveSig]
        int GetDuration([Out] out long pDuration);

        [PreserveSig]
        int GetStopPosition([Out] out long pStop);

        [PreserveSig]
        int GetCurrentPosition([Out] out long pCurrent);

        [PreserveSig]
        int ConvertTimeFormat(
            [Out] out long pTarget,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pTargetFormat,
            [In] long Source,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pSourceFormat
            );

        [PreserveSig]
        int SetPositions(
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pCurrent,
            [In] AMSeekingSeekingFlags dwCurrentFlags,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsLong pStop,
            [In] AMSeekingSeekingFlags dwStopFlags
            );

        [PreserveSig]
        int GetPositions(
            [Out] out long pCurrent,
            [Out] out long pStop
            );

        [PreserveSig]
        int GetAvailable(
            [Out] out long pEarliest,
            [Out] out long pLatest
            );

        [PreserveSig]
        int SetRate([In] double dRate);

        [PreserveSig]
        int GetRate([Out] out double pdRate);

        [PreserveSig]
        int GetPreroll([Out] out long pllPreroll);
    }
}