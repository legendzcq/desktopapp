using System;
using System.Globalization;
using System.Windows.Data;

namespace DownloadClass.Toolkit.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TimeSpan time ? time.TotalSeconds : throw new NotSupportedException("don't support non-TimeSpan type.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double time ? TimeSpan.FromSeconds(time) : throw new NotSupportedException("don't support non-double type.");
        }
    }
}
