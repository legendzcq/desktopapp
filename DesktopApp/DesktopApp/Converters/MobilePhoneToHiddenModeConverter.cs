using System;
using System.Globalization;
using System.Windows.Data;

namespace DesktopApp.Converters
{
    class MobilePhoneToHiddenModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "已绑定手机";
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
