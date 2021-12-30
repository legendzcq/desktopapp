using DesktopApp.Controls;
using DesktopApp.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DesktopApp.Pages
{
    [ComVisible(true)]//将该类设置为com可访问 
    /// <summary>
    /// PaperSocreDetail.xaml 的交互逻辑
    /// </summary>
    public partial class PaperSocreDetail : Page
    {
        private WebBrowserOverlay _webBrowserOverlay;
        public PaperSocreDetail()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                // 解决WPF Window设置AllowsTransparency="True"后，WebBrowser无法显示的问题
                _webBrowserOverlay = new WebBrowserOverlay(BorderOwner);
                // 绑定ViewModel的HtmlContent到WebBrowser控件
                var binding = new Binding
                {
                    Path = new PropertyPath("CurrentItem.HtmlContent"),
                    Source = DataContext
                };
                BindingOperations.SetBinding(_webBrowserOverlay, WebBrowserOverlay.HtmlProperty, binding);

                _webBrowserOverlay.WebBrowser.DocumentCompleted += _webBrowserOverlay_LoadCompleted;
                _webBrowserOverlay.WebBrowser.ObjectForScripting = this;
            };

            Unloaded += (s, e) =>
            {
                _webBrowserOverlay.WebBrowser.DocumentCompleted -= _webBrowserOverlay_LoadCompleted;
                _webBrowserOverlay.Close();
            };
        }

        void _webBrowserOverlay_LoadCompleted(object s, EventArgs e)
        {
            var viewModel = DataContext as PaperSocreViewModel;
            if (viewModel == null || viewModel.CurrentItem == null)
                return;
        }
    }
}
