using System;
using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    static public class DsFindPin
    {
        /// <summary>
        /// Scans a filter's pins looking for a pin in the specified direction
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vDir">The direction to find</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin in the specified direction)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByDirection(IBaseFilter vSource, PinDirection vDir, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            PinDirection ppindir;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Read the direction
                    hr = pPins[0].QueryDirection(out ppindir);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right direction?
                    if (ppindir == vDir)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified name
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vPinName">The pin name to find</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByName(IBaseFilter vSource, string vPinName)
        {
            int hr;
            IEnumPins ppEnum;
            PinInfo ppinfo;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Read the info
                    hr = pPins[0].QueryPinInfo(out ppinfo);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right name?
                    if (ppinfo.name == vPinName)
                    {
                        DsUtils.FreePinInfo(ppinfo);
                        pRet = pPins[0];
                        break;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                    DsUtils.FreePinInfo(ppinfo);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scan's a filter's pins looking for a pin with the specified category
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="PinCategory"></param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin of the specified category)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByCategory(IBaseFilter vSource, Guid PinCategory, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Is it the right category?
                    if (DsUtils.GetPinCategory(pPins[0]) == PinCategory)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }
        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified connection status
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vStat">The status to find (connected/unconnected)</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin with the specified status)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByConnectionStatus(IBaseFilter vSource, PinConnectedStatus vStat, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin pOutPin;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Read the connected status
                    hr = pPins[0].ConnectedTo(out pOutPin);

                    // Check for VFW_E_NOT_CONNECTED.  Anything else is bad.
                    if (hr != DsResults.E_NotConnected)
                    {
                        DsError.ThrowExceptionForHR(hr);

                        // The ConnectedTo call succeeded, release the interface
                        Marshal.ReleaseComObject(pOutPin);
                    }

                    // Is it the right status?
                    if (
                        (hr == 0 && vStat == PinConnectedStatus.Connected) ||
                        (hr == DsResults.E_NotConnected && vStat == PinConnectedStatus.Unconnected)
                        )
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }
    }
}