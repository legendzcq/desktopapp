using System.Runtime.InteropServices;
using System.Security;

namespace Framework.Player.DShow
{
    [ComImport, SuppressUnmanagedCodeSecurity,
     Guid("56a868b3-0ad4-11ce-b03a-0020af0ba770"),
     InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IBasicAudio
    {
        [PreserveSig]
        int put_Volume([In] int lVolume);

        [PreserveSig]
        int get_Volume([Out] out int plVolume);

        [PreserveSig]
        int put_Balance([In] int lBalance);

        [PreserveSig]
        int get_Balance([Out] out int plBalance);
    }
}