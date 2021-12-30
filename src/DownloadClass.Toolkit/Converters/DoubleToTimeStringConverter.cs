using System;
using System.Globalization;
using System.Windows.Data;

namespace DownloadClass.Toolkit.Converters
{
    public class DoubleToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double time
                ? TimeSpan.FromSeconds(time).ToString(@"mm\:hh\:ss")
                : throw new NotSupportedException("don't support non-double type.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
