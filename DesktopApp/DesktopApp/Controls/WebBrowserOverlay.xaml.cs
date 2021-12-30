using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using Framework.Utility;

namespace DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for WebBrowserOverlay.xaml
    /// </summary>
    public partial class WebBrowserOverlay : Window
    {

        readonly FrameworkElement _placementTarget;

        public System.Windows.Forms.WebBrowser WebBrowser { get { return Wb; } }

        public WebBrowserOverlay(FrameworkElement placementTarget)
        {
            InitializeComponent();

            _placementTarget = placementTarget;
            Window owner = GetWindow(placementTarget);

            //owner.SizeChanged += delegate { OnSizeLocationChanged(); };
            owner.LocationChanged += delegate { OnSizeLocationChanged(); };
            _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };

            if (owner.IsVisible)
            {
                Owner = owner;
                Show();
                OnSizeLocationChanged();

            }
            else
            {
                owner.IsVisibleChanged += delegate
                    {
                        if (owner.IsVisible)
                        {
                            Owner = owner;
                            Show();
                            OnSizeLocationChanged();
                        }
                    };
            }

        }

        void OnSizeLocationChanged()
        {
            Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
            CompositionTarget ct = hwndSource.CompositionTarget;
            offset = ct.TransformToDevice.Transform(offset);
            size = ct.TransformToDevice.Transform(size);

            NativeMethod.POINT screenLocation = new NativeMethod.POINT(offset);
            NativeMethod.ClientToScreen(hwndSource.Handle, ref screenLocation);
            NativeMethod.POINT screenSize = new NativeMethod.POINT(size);
            try
            {
                NativeMethod.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
            }
            catch (Exception)
            {
                Debug.WriteLine("WebBrowserOverlay Win32 Error");
            }
        }

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(WebBrowserOverlay), new PropertyMetadata(OnHtmlChanged));

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wbo = d as WebBrowserOverlay;

            if (wbo == null || e.NewValue == null)
                return;

            var html = e.NewValue.ToString();

            wbo.WebBrowser.DocumentText = html;
            //wbo.WebBrowser.NavigateToString(html);
        }
    }
}
