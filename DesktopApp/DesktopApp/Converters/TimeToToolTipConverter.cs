using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace DesktopApp.Converters
{
    public class TimeToToolTipConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = (string)value;
            if (!string.IsNullOrEmpty(time))
            {
                // 非空
                time = "上次更新时间：" + time;
            }
            else
            {
                time = "点击更新题库";
            }

            return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /**
     * 控制“更新时间指示图标”是否显示的convertor
     * @author ChW
     * @date 2021-05-18
     */
    public class TimeToVisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = (string)value;
            if (!string.IsNullOrEmpty(time))
            {
                // 非空
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
