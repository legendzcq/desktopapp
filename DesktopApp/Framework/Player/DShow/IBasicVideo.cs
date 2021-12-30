using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868b5-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicVideo
    {
        [PreserveSig]
        int get_AvgTimePerFrame([Out] out double pAvgTimePerFrame);

        [PreserveSig]
        int get_BitRate([Out] out int pBitRate);

        [PreserveSig]
        int get_BitErrorRate([Out] out int pBitRate);

        [PreserveSig]
        int get_VideoWidth([Out] out int pVideoWidth);

        [PreserveSig]
        int get_VideoHeight([Out] out int pVideoHeight);

        [PreserveSig]
        int put_SourceLeft([In] int SourceLeft);

        [PreserveSig]
        int get_SourceLeft([Out] out int pSourceLeft);

        [PreserveSig]
        int put_SourceWidth([In] int SourceWidth);

        [PreserveSig]
        int get_SourceWidth([Out] out int pSourceWidth);

        [PreserveSig]
        int put_SourceTop([In] int SourceTop);

        [PreserveSig]
        int get_SourceTop([Out] out int pSourceTop);

        [PreserveSig]
        int put_SourceHeight([In] int SourceHeight);

        [PreserveSig]
        int get_SourceHeight([Out] out int pSourceHeight);

        [PreserveSig]
        int put_DestinationLeft([In] int DestinationLeft);

        [PreserveSig]
        int get_DestinationLeft([Out] out int pDestinationLeft);

        [PreserveSig]
        int put_DestinationWidth([In] int DestinationWidth);

        [PreserveSig]
        int get_DestinationWidth([Out] out int pDestinationWidth);

        [PreserveSig]
        int put_DestinationTop([In] int DestinationTop);

        [PreserveSig]
        int get_DestinationTop([Out] out int pDestinationTop);

        [PreserveSig]
        int put_DestinationHeight([In] int DestinationHeight);

        [PreserveSig]
        int get_DestinationHeight([Out] out int pDestinationHeight);

        [PreserveSig]
        int SetSourcePosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetSourcePosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int SetDefaultSourcePosition();

        [PreserveSig]
        int SetDestinationPosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetDestinationPosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int SetDefaultDestinationPosition();

        [PreserveSig]
        int GetVideoSize(
            [Out] out int pWidth,
            [Out] out int pHeight
            );

        [PreserveSig]
        int GetVideoPaletteEntries(
            [In] int StartIndex,
            [In] int Entries,
            [Out] out int pRetrieved,
            [Out] out int[] pPalette
            );

        [PreserveSig]
        int GetCurrentImage(
            [In, Out] ref int pBufferSize,
            [Out] IntPtr pDIBImage // int *
            );

        [PreserveSig]
        int IsUsingDefaultSource();

        [PreserveSig]
        int IsUsingDefaultDestination();
    }
}