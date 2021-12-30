using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868b2-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaPosition
    {
        [PreserveSig]
        int get_Duration([Out] out double pLength);

        [PreserveSig]
        int put_CurrentPosition([In] double llTime);

        [PreserveSig]
        int get_CurrentPosition([Out] out double pllTime);

        [PreserveSig]
        int get_StopTime([Out] out double pllTime);

        [PreserveSig]
        int put_StopTime([In] double llTime);

        [PreserveSig]
        int get_PrerollTime([Out] out double pllTime);

        [PreserveSig]
        int put_PrerollTime([In] double llTime);

        [PreserveSig]
        int put_Rate([In] double dRate);

        [PreserveSig]
        int get_Rate([Out] out double pdRate);

        [PreserveSig]
        int CanSeekForward([Out] out OABool pCanSeekForward);

        [PreserveSig]
        int CanSeekBackward([Out] out OABool pCanSeekBackward);
    }
}