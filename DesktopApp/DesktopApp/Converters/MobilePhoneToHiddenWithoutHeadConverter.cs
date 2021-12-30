using System;
using System.Globalization;
using System.Windows.Data;

namespace DesktopApp.Converters
{
    class MobilePhoneToHiddenWithoutHeadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            string mobilePhone = value.ToString();

            if (mobilePhone.Length == 11)
            {
                result += mobilePhone.Substring(0, 3);
                result += "****";
                result += mobilePhone.Substring(7);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
