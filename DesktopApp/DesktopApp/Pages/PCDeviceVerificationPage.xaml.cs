using DesktopApp.Controls;
using Framework.Model;
using Framework.Remote;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using static DesktopApp.Pages.PCDeviceConstraint;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PCDeviceVerificationPage.xaml 的交互逻辑
    /// </summary>
    public partial class PCDeviceVerificationPage : Page
    {
        public PCDeviceVerificationPage()
        {
            InitializeComponent();
        }

        private void ChangePhone_Click(object sender, RoutedEventArgs e)
        {
            // 切换到PCDeviceCheckPhonePage
            Messenger.Default.Send<NavigateTarget>(NavigateTarget.CHECKPHONEPAGE);
        }

        private void VertificationCode_Click(object sender, RoutedEventArgs e)
        {
            // 发送验证码
            StudentRemote stuRemote = new StudentRemote();
            SendVerificationResult item = stuRemote.SendVerificationCode(App.Loc.DeviceListVM.MobilePhone, StudentRemote.VerificationCodeType.CHECK_BINDED); // 从服务器获取绑定设备列表，“2”表示校验手机号是否已被绑定

            if (item != null)
            {
                if (item.Code == "1") // 验证码发送成功
                {
                    WaitTime = 60;
                    VerificationCodeButton.IsEnabled = false;

                    Tmr = new Timer(10);
                    Tmr.Elapsed += OnTimeOut;
                    Tmr.AutoReset = true;
                    Tmr.Start();
                }
                else
                {
                    CustomMessageBox.Show(item.Msg);
                }
            }
        }

        private void OnTimeOut(object sender, ElapsedEventArgs e)
        {
            if (Tmr.Interval != 1000)
            {
                Tmr.Interval = 1000;
            }

            if (WaitTime <= 0)
            {
                VerificationCodeButton.Dispatcher.Invoke(() => { VerificationCodeButton.Content = "重新发送"; VerificationCodeButton.IsEnabled = true; });
                Tmr.Stop();
                Tmr.Dispose();
                return;
            }

            VerificationCodeButton.Dispatcher.Invoke(() => { VerificationCodeButton.Content = WaitTime.ToString() + "秒之后重发"; });
            WaitTime--;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // 使用验证码校验身份
            StudentRemote stuRemote = new StudentRemote();
            var item = stuRemote.CheckUserIdentityByVerificationCode(App.Loc.DeviceListVM.MobilePhone, VerificationCodeTextBox.Text);

            if (item != null)
            {
                if (item.Code == "1") // 身份效验成功
                {
                    // 发送解绑请求
                    var retInfo = stuRemote.UnbindDevice(App.Loc.DeviceListVM.selectedMid, App.Loc.DeviceListVM.selectedMname);

                    if (retInfo != null)
                    {
                        if (retInfo.Code == "1")
                        {
                            CustomMessageBox.Show("设备解绑成功，请在登录界面重新登录。");
                        }
                    }

                    // 关闭设备主窗口
                    Messenger.Default.Send<NavigateTarget>(NavigateTarget.CLOSEWINDOW);
                }
                else
                {
                    CustomMessageBox.Show(item.Msg);
                }
            }
        }

        private Timer Tmr { get; set; }

        // 连续获取验证码的等待时间间隔
        private int WaitTime { get; set; }

    }
}
