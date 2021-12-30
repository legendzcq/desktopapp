using System.Reflection;
using System.Security;
using System.Windows;

namespace DesktopApp.Utils
{
    internal static class WindowParameters
    {
        private static Thickness? s_paddedBorderThickness;

        private static double? s_ribbonContextualTabGroupHeight;

        /// <summary>
        /// returns the border thickness padding around captioned windows,in pixels. Windows XP/2000:  This value is not supported.
        /// </summary>
        public static Thickness PaddedBorderThickness
        {
            [SecurityCritical]
            get
            {
                if (s_paddedBorderThickness == null)
                {
                    var paddedBorder = NativeMethods.GetSystemMetrics(SM.CXPADDEDBORDER);
                    var dpi = GetDpi();
                    var frameSize = new Size(paddedBorder, paddedBorder);
                    Size frameSizeInDips = DpiHelper.DeviceSizeToLogical(frameSize, dpi / 96.0, dpi / 96.0);
                    s_paddedBorderThickness = new Thickness(frameSizeInDips.Width, frameSizeInDips.Height, frameSizeInDips.Width, frameSizeInDips.Height);
                }

                return s_paddedBorderThickness.Value;
            }
        }

        public static double RibbonContextualTabGroupHeight
        {

            get
            {
                if (s_ribbonContextualTabGroupHeight == null)
                {
                    s_ribbonContextualTabGroupHeight = SystemParameters.WindowNonClientFrameThickness.Top + (1d * GetDpi() / 96.0);
                }

                return s_ribbonContextualTabGroupHeight.Value;
            }
        }

        /// <summary>
        /// Get Dpi
        /// </summary>
        /// <returns>Return 96,144/returns>
        public static double GetDpi()
        {
            PropertyInfo dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            return dpiX;
        }
    }
}
