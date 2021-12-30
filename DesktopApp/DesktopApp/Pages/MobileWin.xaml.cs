using System;
using System.Linq;
using System.Timers;
using System.Windows;
using DesktopApp.Controls;
using DesktopApp.ViewModel;
using Framework.Local;

namespace DesktopApp.Pages
{
    /// <summary>
    /// MobileWin.xaml 的交互逻辑
    /// </summary>
    public partial class MobileWin
    {
        public MobileWin()
        {
            InitializeComponent();
        }
#if MOBILE
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (App.MobileConnector.IsConnected)
            {
                TxtCode.IsReadOnly = true;
                BtnConnect.Visibility = Visibility.Collapsed;
                BtnDisConnect.Visibility = Visibility.Visible;
            }
            else
            {
                TxtCode.IsReadOnly = false;
                BtnConnect.Visibility = Visibility.Visible;
                BtnDisConnect.Visibility = Visibility.Collapsed;
            }
            App.MobileConnector.OnConnectSuccess += ConnectSuccess;
            App.MobileConnector.OnConnectFail += ConnectFail;
            App.MobileConnector.OnDisConnected += ConnectClosed;
            App.MobileConnector.OnDeviceNameChanged += MobileConnector_OnDeviceNameChanged;

        }

        void MobileConnector_OnDeviceNameChanged(string obj)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMobileName.Text = "当前连接设备：" + obj;
            }));
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            var code = TxtCode.Text.Trim();
            if (code.Length != 4)
            {
                CustomMessageBox.Show("请正确输入验证码", owner: this);
                return;
            }
            BtnConnect.IsEnabled = false;
            CProgress.Visibility = Visibility.Visible;
            App.MobileConnector.BeginConnect(code);
        }


        private void ConnectSuccess()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                CProgress.Visibility = Visibility.Collapsed;
                TxtCode.IsReadOnly = true;
                BtnConnect.Visibility = Visibility.Collapsed;
                BtnDisConnect.Visibility = Visibility.Visible;
                CustomMessageBox.Show("连接成功", owner: this);

                var list = new StudentWareData().GetDonwloadedList().Select(x => new CwareDownItemViewModel(x)).ToList();
                DgData.ItemsSource = list;
                DgData.Visibility = Visibility.Visible;
                var timer = new Timer { Interval = 2000 };
                timer.Elapsed += (s, ee) =>
                {
                    if (!App.MobileConnector.IsInTrasfer)
                    {
                        var item = list.FirstOrDefault(x => x.State == TransState.NeedTrasfer);
                        if (item != null) item.StartTransfer();
                    }
                };
                timer.Start();
            }));
        }

        private void ConnectFail()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                BtnConnect.IsEnabled = true;
                TxtCode.IsReadOnly = false;
                BtnConnect.Visibility = Visibility.Visible;
                BtnDisConnect.Visibility = Visibility.Collapsed;
                TxtCode.Text = string.Empty;

                CProgress.Visibility = Visibility.Collapsed;
                CustomMessageBox.Show("连接失败", owner: this);
                BtnConnect.IsEnabled = true;
            }));
        }

        private void ConnectClosed()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                BtnConnect.IsEnabled = true;
                TxtCode.IsReadOnly = false;
                BtnConnect.Visibility = Visibility.Visible;
                BtnDisConnect.Visibility = Visibility.Collapsed;
                TxtCode.Text = string.Empty;
            }));
        }

        private void BtnDisConnect_Click(object sender, RoutedEventArgs e)
        {
            App.MobileConnector.CloseConnection();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            App.MobileConnector.OnConnectSuccess -= ConnectSuccess;
            App.MobileConnector.OnConnectFail -= ConnectFail;
            App.MobileConnector.OnDisConnected -= ConnectClosed;
            App.MobileConnector.OnDeviceNameChanged -= MobileConnector_OnDeviceNameChanged;
            App.MobileConnector.CloseConnection();
        }
#else
        private void BtnTest_Click(object sender, RoutedEventArgs e) { }
        private void Window_Loaded(object sender, RoutedEventArgs e) { }

        private void Window_Unloaded(object sender, RoutedEventArgs e) { }

        private void BtnConnect_Click(object sender, RoutedEventArgs e) { }

        private void BtnDisConnect_Click(object sender, RoutedEventArgs e) { }

        private void BtnSend_Click(object sender, RoutedEventArgs e) { }
#endif
    }
}
