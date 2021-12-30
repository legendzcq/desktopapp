using Framework.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PCDeviceConstraint.xaml 的交互逻辑
    /// </summary>
    public partial class PCDeviceConstraint : Window/*, INotifyPropertyChanged*/
    {
        public PCDeviceConstraint()
        {
            InitializeComponent();
            
            Loaded += PCDeviceConstraint_Loaded;
            Closing += PCDeviceConstraint_Closing;
        }

        private void PCDeviceConstraint_Closing(object sender, CancelEventArgs e)
        {
            if (this.ContainerFrame.CurrentSource.OriginalString == "Pages/PCDeviceCheckPhonePage.xaml")
            {
                this.ContainerFrame.Navigate(new Uri("/Pages/PCDeviceVerificationPage.xaml", UriKind.Relative));
                e.Cancel = true;
            }
            
        }

        private void PCDeviceConstraint_Loaded(object sender, RoutedEventArgs e)
        {
            ContainerFrame.Source = new Uri("/Pages/PCDeviceBindedListPage.xaml", UriKind.Relative);
            Messenger.Default.Register<NavigateTarget>(this, (target) => MsgHandler(target));
        }

        // 处理翻页信号
        public void MsgHandler(NavigateTarget target)
        {
            switch (target)
            {
                case NavigateTarget.BINDEDLISTPAGE:
                    this.ContainerFrame.Navigate(new Uri("/Pages/PCDeviceBindedListPage.xaml", UriKind.Relative));
                    break;
                case NavigateTarget.VERIFICATIONPAGE:
                    this.ContainerFrame.Navigate(new Uri("/Pages/PCDeviceVerificationPage.xaml", UriKind.Relative));
                    break;
                case NavigateTarget.CHECKPHONEPAGE:
                    this.ContainerFrame.Navigate(new Uri("/Pages/PCDeviceCheckPhonePage.xaml", UriKind.Relative));
                    break;
                case NavigateTarget.BINDPHONEPAGE:
                    this.ContainerFrame.Navigate(new Uri("/Pages/PCDeviceBindPhonePage.xaml", UriKind.Relative));
                    break;
                case NavigateTarget.CLOSEWINDOW:
                    var aaa = this.ContainerFrame.CurrentSource;
                    this.Close();
                    break;
                default:
                    return;
            }
        }

        public enum NavigateTarget
        {
            BINDEDLISTPAGE, // 设备绑定列表页面（选择要解绑的设备）
            VERIFICATIONPAGE, // 解绑设备之前的身份验证页面（验证当前操作者的身份页，向当前绑定手机号发送验证码）
            CHECKPHONEPAGE, // 更换绑定手机号之前的身份验证页面（验证当前操作者的身份页，向当前绑定手机号发送验证码）
            BINDPHONEPAGE, // 绑定新手机号页面
            CLOSEWINDOW // 关闭设备绑定解绑窗口
        }
    }
}
