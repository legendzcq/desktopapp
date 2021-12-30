using DesktopApp.Controls;
using Framework.Model;
using Framework.Remote;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DesktopApp.Pages.PCDeviceConstraint;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PCDeviceBindPhonePage.xaml 的交互逻辑
    /// </summary>
    public partial class PCDeviceBindPhonePage : Page
    {
        public PCDeviceBindPhonePage()
        {
            InitializeComponent();
        }

        private void VertificationCode_Click(object sender, RoutedEventArgs e)
        {
            // 向手机发送验证码（用于校验操作者身份）
            StudentRemote stuRemote = new StudentRemote();
            var item = stuRemote.SendVerificationCode(NewPhoneTextBox.Text, StudentRemote.VerificationCodeType.NOCHECK_REGISTER); // 从服务器获取绑定设备列表，“2”表示校验手机号是否已被绑定

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
            // 绑定新手机号
            StudentRemote stuRemote = new StudentRemote();
            BindPhoneResult item = stuRemote.BindPhone(NewPhoneTextBox.Text, VerificationCodeTextBox.Text);

            if (item != null)
            {
                if (item.Code == "1") // 验证码发送成功
                {
                    CustomMessageBox.Show("手机号更换成功，请继续解绑设备操作。");
                    Messenger.Default.Send<NavigateTarget>(NavigateTarget.VERIFICATIONPAGE);
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
