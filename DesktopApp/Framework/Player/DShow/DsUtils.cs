using System;
using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    static public class DsUtils
    {
        /// <summary>
        /// Returns the PinCategory of the specified pin.  Usually a member of PinCategory.  Not all pins have a category.
        /// </summary>
        /// <param name="pPin"></param>
        /// <returns>Guid indicating pin category or Guid.Empty on no category.  Usually a member of PinCategory</returns>
        public static Guid GetPinCategory(IPin pPin)
        {
            Guid guidRet = Guid.Empty;

            // Memory to hold the returned guid
            int iSize = Marshal.SizeOf(typeof(Guid));
            IntPtr ipOut = Marshal.AllocCoTaskMem(iSize);

            try
            {
                int hr;
                int cbBytes;
                Guid g = PropSetID.Pin;

                // Get an IKsPropertySet from the pin
                IKsPropertySet pKs = pPin as IKsPropertySet;

                if (pKs != null)
                {
                    // Query for the Category
                    hr = pKs.Get(g, (int)AMPropertyPin.Category, IntPtr.Zero, 0, ipOut, iSize, out cbBytes);
                    DsError.ThrowExceptionForHR(hr);

                    // Marshal it to the return variable
                    guidRet = (Guid)Marshal.PtrToStructure(ipOut, typeof(Guid));
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ipOut);
                ipOut = IntPtr.Zero;
            }

            return guidRet;
        }

        /// <summary>
        ///  Free the nested structures and release any
        ///  COM objects within an AMMediaType struct.
        /// </summary>
        public static void FreeAMMediaType(AMMediaType mediaType)
        {
            if (mediaType != null)
            {
                if (mediaType.formatSize != 0)
                {
                    Marshal.FreeCoTaskMem(mediaType.formatPtr);
                    mediaType.formatSize = 0;
                    mediaType.formatPtr = IntPtr.Zero;
                }
                if (mediaType.unkPtr != IntPtr.Zero)
                {
                    Marshal.Release(mediaType.unkPtr);
                    mediaType.unkPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///  Free the nested interfaces within a PinInfo struct.
        /// </summary>
        public static void FreePinInfo(PinInfo pinInfo)
        {
            if (pinInfo.filter != null)
            {
                Marshal.ReleaseComObject(pinInfo.filter);
                pinInfo.filter = null;
            }
        }

    }
}