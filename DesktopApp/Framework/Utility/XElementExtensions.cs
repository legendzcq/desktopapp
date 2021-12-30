using System;
using System.Xml.Linq;

namespace Framework.Utility
{
    public static class XElementExtensions
    {
        public static string GetString(this XElement element, string subElementName, string defaultValue = null)
        {
            if (defaultValue == null) defaultValue = string.Empty;
            var elem = element.Element(subElementName);
            return elem != null ? elem.Value : defaultValue;
        }

        public static int GetInt(this XElement element, string subElementName, int defaultValue = 0)
        {
            var elem = element.Element(subElementName);
            int value = elem != null && int.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static long GetLong(this XElement element, string subElementName, long defaultValue = 0)
        {
            var elem = element.Element(subElementName);
            long value = elem != null && long.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static double GetDouble(this XElement element, string subElementName, double defaultValue =default(double))
        {
            var elem = element.Element(subElementName);
            double value = elem != null && double.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static DateTime GetDateTime(this XElement element, string subElementName, DateTime defaultValue = default(DateTime))
        {
            var elem = element.Element(subElementName);
            DateTime value = elem != null && DateTime.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static string GetAttributeString(this XElement element, string attrName, string defaultValue = null)
        {
            if (defaultValue == null) defaultValue = string.Empty;
            var elem = element.Attribute(attrName);
            return elem != null ? elem.Value : defaultValue;
        }

        public static int GetAttributeInt(this XElement element, string attrName, int defaultValue = 0)
        {
            var elem = element.Attribute(attrName);
            int value = elem != null && int.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static double GetAttributeDouble(this XElement element, string attrName, double defaultValue = default(double))
        {
            var elem = element.Attribute(attrName);
            double value = elem != null && double.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }

        public static DateTime GetAttributeDateTime(this XElement element, string attrName, DateTime defaultValue = default(DateTime))
        {
            var elem = element.Attribute(attrName);
            DateTime value = elem != null && DateTime.TryParse(elem.Value, out value) ? value : defaultValue;
            return value;
        }
    }
}
