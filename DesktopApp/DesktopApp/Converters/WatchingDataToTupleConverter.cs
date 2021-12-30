using System;
using System.Globalization;
using System.Windows.Data;

namespace DesktopApp.Converters
{
    public class WatchingDataToTupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 5 && values[0] is TimeSpan length && values[1] is TimeSpan position && values[2] is DateTimeOffset beginTime && values[3] is DateTimeOffset endTime && values[4] is double speedRatio)
            {
                return (length, position, beginTime, DateTimeOffset.Now, speedRatio);
            }

            throw new NotSupportedException("Can not support");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
