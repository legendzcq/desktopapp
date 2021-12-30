using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using Framework.Remote;
using Framework.Utility;

namespace DesktopApp.Pages
{
    /// <summary>
    /// AdvWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdvWindow : Window
    {
        public AdvWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var helper = new ComVisibleObjectForScripting();
                WebAdv.ObjectForScripting = helper;
                var url = new StudentRemote().GetAdvUrl();
                WebAdv.Source = new Uri(url);
            };
            WebAdv.Navigating += WebDevice_Navigating;
        }

        private void WebDevice_Navigating(object sender, NavigatingCancelEventArgs e) => SetWebBrowserSilent(sender as WebBrowser, true);

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
                var browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }
        private void BtnCloseClick(object sender, RoutedEventArgs e) => Close();

        private bool _isPressd;
        private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isPressd = true;
            if (e.ClickCount == 2)
            {

            }
        }

        private void GridTop_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isPressd = false;
        private void ckTip_Click(object sender, RoutedEventArgs e)
        {
            if (ckTip.IsChecked == true) Util.IsShowAdv = false;
        }
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerStepThrough]
        private void GridTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isPressd)
            {
                if (WindowState == WindowState.Maximized)
                {
                    Point pos = Mouse.GetPosition(this);
                    Top = SystemParameters.WorkArea.Y - pos.Y;
                }
                DragMove();
            }
        }

        [ComVisible(true)]
        public class ComVisibleObjectForScripting
        {
            public void GoToUrl(string url) => SystemInfo.ShellExecute(url, true, false);
        }
    }
}
