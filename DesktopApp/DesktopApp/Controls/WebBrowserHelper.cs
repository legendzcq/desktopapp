using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Controls
{
    /// <summary>
    /// Binding Html to the Web Browser Control
    /// </summary>
    public class WebBrowserHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(WebBrowserHelper), new PropertyMetadata(OnHtmlChanged));
        
        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebBrowser;

            if (browser == null || e.NewValue == null)
                return;

            var html = e.NewValue.ToString();

            browser.NavigateToString(html);
        }
    }
}
