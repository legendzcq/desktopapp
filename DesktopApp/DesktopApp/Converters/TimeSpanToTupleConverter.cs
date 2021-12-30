using System;
using System.Globalization;
using System.Windows.Data;

namespace DesktopApp.Converters
{
    public class TimeSpanToTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Length == 2 && values[0] is TimeSpan position && values[1] is TimeSpan duration
                ? (position, duration)
                : throw new NotSupportedException("Only two items of TimeSpan type are supported");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
