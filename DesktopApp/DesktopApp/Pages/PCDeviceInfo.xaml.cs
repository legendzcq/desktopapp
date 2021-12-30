
using DesktopApp.Logic;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PCDeviceInfo.xaml 的交互逻辑
    /// </summary>
    public partial class PCDeviceInfo : Window
    {
        public PCDeviceInfo()
        {
            InitializeComponent();
        }
        public PCDeviceInfo(string userName):this()
        {
            Loaded += (s, e) => BindData(userName);
            WebDevice.Navigating += WebDevice_Navigating;  
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
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] {silent});  
            }  
        }
        private void BindData(string userName)
        {
            string path = StudentLogic.KickDeviceInfo(userName);
            WebDevice.Source = new Uri(path);
        }
    } 
    
}
