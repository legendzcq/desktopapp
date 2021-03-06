using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IVideoWindow
    {
        [PreserveSig]
        int put_Caption([In, MarshalAs(UnmanagedType.BStr)] string caption);

        [PreserveSig]
        int get_Caption([Out, MarshalAs(UnmanagedType.BStr)] out string caption);

        [PreserveSig]
        int put_WindowStyle([In] WindowStyle windowStyle);

        [PreserveSig]
        int get_WindowStyle([Out] out WindowStyle windowStyle);

        [PreserveSig]
        int put_WindowStyleEx([In] WindowStyleEx windowStyleEx);

        [PreserveSig]
        int get_WindowStyleEx([Out] out WindowStyleEx windowStyleEx);

        [PreserveSig]
        int put_AutoShow([In] OABool autoShow);

        [PreserveSig]
        int get_AutoShow([Out] out OABool autoShow);

        [PreserveSig]
        int put_WindowState([In] WindowState windowState);

        [PreserveSig]
        int get_WindowState([Out] out WindowState windowState);

        [PreserveSig]
        int put_BackgroundPalette([In] OABool backgroundPalette);

        [PreserveSig]
        int get_BackgroundPalette([Out] out OABool backgroundPalette);

        [PreserveSig]
        int put_Visible([In] OABool visible);

        [PreserveSig]
        int get_Visible([Out] out OABool visible);

        [PreserveSig]
        int put_Left([In] int left);

        [PreserveSig]
        int get_Left([Out] out int left);

        [PreserveSig]
        int put_Width([In] int width);

        [PreserveSig]
        int get_Width([Out] out int width);

        [PreserveSig]
        int put_Top([In] int top);

        [PreserveSig]
        int get_Top([Out] out int top);

        [PreserveSig]
        int put_Height([In] int height);

        [PreserveSig]
        int get_Height([Out] out int height);

        [PreserveSig]
        int put_Owner([In] IntPtr owner);

        [PreserveSig]
        int get_Owner([Out] out IntPtr owner);

        [PreserveSig]
        int put_MessageDrain([In] IntPtr drain);

        [PreserveSig]
        int get_MessageDrain([Out] out IntPtr drain);

        // Use ColorTranslator to break out RGB
        [PreserveSig]
        int get_BorderColor([Out] out int color);

        // Use ColorTranslator to break out RGB
        [PreserveSig]
        int put_BorderColor([In] int color);

        [PreserveSig]
        int get_FullScreenMode([Out] out OABool fullScreenMode);

        [PreserveSig]
        int put_FullScreenMode([In] OABool fullScreenMode);

        [PreserveSig]
        int SetWindowForeground([In] OABool focus);

        [PreserveSig]
        int NotifyOwnerMessage(
            [In] IntPtr hwnd, // HWND *
            [In] int msg,
            [In] IntPtr wParam, // WPARAM
            [In] IntPtr lParam // LPARAM
            );

        [PreserveSig]
        int SetWindowPosition(
            [In] int left,
            [In] int top,
            [In] int width,
            [In] int height
            );

        [PreserveSig]
        int GetWindowPosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetMinIdealImageSize(
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetMaxIdealImageSize(
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int GetRestorePosition(
            [Out] out int left,
            [Out] out int top,
            [Out] out int width,
            [Out] out int height
            );

        [PreserveSig]
        int HideCursor([In] OABool hideCursor);

        [PreserveSig]
        int IsCursorHidden([Out] out OABool hideCursor);
    }
}