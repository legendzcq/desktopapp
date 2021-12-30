using Framework.Remote;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Pages
{
    /// <summary>
    /// MySevicePage.xaml 的交互逻辑
    /// </summary>
    public partial class MySevicePage : Page
    {
        public MySevicePage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var helper = new ComVisibleObjectForScripting();
                WebService.ObjectForScripting = helper;
                if (Util.IsOnline)
                {
                    ImgDefalt.Visibility = Visibility.Collapsed;
                    WebService.Visibility = Visibility.Visible;
                    string url = new StudentRemote().GetServiceUrl();
                    WebService.Source = new Uri(url);
                }
                else
                {
                    ImgDefalt.Visibility = Visibility.Visible;
                    WebService.Visibility = Visibility.Collapsed;
                }

            };
            WebService.Navigating += WebDevice_Navigating;
            WebService.LoadCompleted += WebService_LoadCompleted;
        }
        void WebDevice_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            SetWebBrowserSilent(sender as WebBrowser, true);

        }

        /// <summary>  
        /// 设置浏览器静默，不弹错误提示框  
        /// </summary>  
        /// <param name="webBrowser">要设置的WebBrowser控件浏览器</param>  
        /// <param name="silent">是否静默</param>  
        private void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }

        [ComVisible(true)]
        public class ComVisibleObjectForScripting
        {
            public void GoToUrl(string url)
            {
#if CHINAACC
                url = url.Replace("m.chinaacc.com", "www.chinaacc.com");
#endif
#if MED
                url = url.Replace("m.med66.com", "www.med66.com");
#endif
#if JIANSHE
                url = url.Replace("m.jianshe99.com", "www.jianshe99.com");
#endif
                var stu = new StudentRemote();
                stu.GoToUrl(url);
            }
        }

        private void WebService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
            }
            catch
            {

            }
        }
    }
}
