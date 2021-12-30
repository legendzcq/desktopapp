using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DesktopApp.Controls;
using DesktopApp.Logic;

using Framework.Download;
using Framework.Model;
using Framework.Utility;
using System.Text;
using System.Windows.Controls;
using Framework.Remote;
using System.Collections.ObjectModel;
using DesktopApp.Pages;

namespace DesktopApp
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            Title = Util.AppName;
        }

        private readonly List<RemoteLogin.LoginedDevice> _loginedDeviceList;

        /// <summary>
        ///用于存放密码字符
        /// </summary>
        private StringBuilder PasswordBuilder;

        /// <summary>
        /// 明文密码
        /// </summary>
        private string PasswordStr = string.Empty;

        /// <summary>
        /// 响应事件标识，替换字符时，不处理后续逻辑
        /// </summary>
        private bool IsResponseChange = true;

        /// <summary>
        /// 判断是否按的delete键
        /// </summary>
        private bool isDelKey = true;

        private void Close_OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {

#if DEBUG
            if (TxtUserName.Text == "" && TxtPassword.Text == "")
            {
                TxtUserName.Text = "jiayou963";
                TxtPassword.Text = "123456pan";
            }
            if (TxtUserName.Text == "z" && TxtPassword.Text == "")
            {
                TxtUserName.Text = "zhaobin1990";
                TxtPassword.Text = "123456";
            }
            if (TxtUserName.Text == "c" && TxtPassword.Text == "")
            {
                TxtUserName.Text = "chenhaibin415";
                TxtPassword.Text = "chb123456789";
            }
#endif
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }

            if (string.IsNullOrEmpty(TxtUserName.Text.Trim()))
            {
                CustomMessageBox.Show("请输入学员代码");
                return;
            }
            if (string.IsNullOrEmpty(TxtPassword.Text.Trim()))
            {
                CustomMessageBox.Show("请输入密码");
                return;
            }

            LoadingCtl.Visibility = Visibility.Visible;
            Log.RecordData("ExecuteLogin", TxtUserName.Text.Trim());
            StudentLogic.ExecuteLogin(TxtUserName.Text.Trim(), PasswordStr, re =>
			{
				if (re.State)
				{
					StudentLogic.RecordUser();
					Dispatcher.Invoke(new Action(() =>
					{
						LoadingCtl.Visibility = Visibility.Collapsed;
						Visibility = Visibility.Collapsed;
						var win = new MainWindow();
						win.ShowDialog();
						Close();
					}));
				}
				else
				{
					Dispatcher.Invoke(new Action(() =>
					{
						CustomMessageBox.Show(re.Message);
						LoadingCtl.Visibility = Visibility.Collapsed;
					}));
				}
			}, (re, remoteLogin) => Dispatcher.Invoke(new Action(() =>
			{
                /**新版设备窗口
                 * @author ChW
                 * @date 2021-06-22
                 */
                StudentRemote stuRemote = new StudentRemote();
                var bindedDeviceList = stuRemote.GetBindedEquipment(TxtUserName.Text.Trim()); // 从服务器获取绑定设备列表
                App.Loc.DeviceListVM.fillBindedDeviceList(bindedDeviceList);
                App.Loc.DeviceListVM.MobilePhone = remoteLogin.MobilePhone; // 从登录接口获取的手机号

                //// 保存绑定设备列表和当前绑定的手机号
                //App.Loc.DeviceListVM.fillBindedDeviceList(remoteLogin.Mlist);
                //App.Loc.DeviceListVM.MobilePhone = remoteLogin.MobilePhone; // 从登录接口获取的手机号

                // 显示设备列表窗口
                var deviceWin = new PCDeviceConstraint();
                deviceWin.ShowDialog();
                LoadingCtl.Visibility = Visibility.Collapsed;

                //var win = new PCDeviceInfo(TxtUserName.Text.Trim());
                //win.ShowDialog();
                //LoadingCtl.Visibility = Visibility.Collapsed;
            })));
		}

        private void Relogin_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }
            LoadingCtl.Visibility = Visibility.Visible;
            Log.RecordData("ExecuteLogin", TxtUserName.Text.Trim());

            var deviceid = string.Empty;
            if (Device1.IsChecked == true) deviceid = _loginedDeviceList[0].DeviceId;
            if (Device2.IsChecked == true) deviceid = _loginedDeviceList[1].DeviceId;

            StudentLogic.KickDeviceAndRelogin(TxtUserName.Text.Trim(), PasswordStr, deviceid, re =>
            {
                if (re.State)
                {
                    StudentLogic.RecordUser();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        LoadingCtl.Visibility = Visibility.Collapsed;
                        Visibility = Visibility.Collapsed;
                        var win = new MainWindow();
                        win.ShowDialog();
                        Close();
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CustomMessageBox.Show(re.Message);
                        LoadingCtl.Visibility = Visibility.Collapsed;
                    }));
                }
            });
        }

        private void ReClose_Click(object sender, RoutedEventArgs e)
        {
            DeviceList.Visibility = Visibility.Collapsed;
            NormalLogin.Visibility = Visibility.Visible;
        }

        private void ShowDeviceList()
        {
            Device1Name.Text = _loginedDeviceList[0].DeviceName;
            if (_loginedDeviceList.Count > 1)
            {
                Device2Name.Text = _loginedDeviceList[1].DeviceName;
            }
            else
            {
                GridDevice2.Visibility = Visibility.Collapsed;
            }

            DeviceList.Visibility = Visibility.Visible;
            NormalLogin.Visibility = Visibility.Collapsed;
        }

        private void TxtPassword_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                Login_OnClick(null, null);
            }
        }

        private void BtnMin_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void BtnClose_OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private bool _isPressd;
        private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _isPressd = true;

        private void GridTop_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isPressd = false;

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
                DragMove();
            }
        }

        private void Device1Name_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Device1.IsChecked = true;

        private void Device2Name_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Device2.IsChecked = true;

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new NetworkSetting() { Owner = this };
            win.ShowDialog();
        }
        private void TxtPassword_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!IsResponseChange) return;
            foreach (TextChange c in e.Changes)
            {
                PasswordStr = PasswordStr.Remove(c.Offset, c.RemovedLength); //从密码文中根据本次Change对象的索引和长度删除对应个数的字符
                PasswordStr = PasswordStr.Insert(c.Offset, TxtPassword.Text.Substring(c.Offset, c.AddedLength));   //将Text新增的部分记录给密码文
            }
            Console.WriteLine(PasswordStr);
            IsResponseChange = false;
            TxtPassword.Text = ConvertToPasswordChar(TxtPassword.Text.Length);  //将输入的字符替换为密码字符
            IsResponseChange = true;
            if (isDelKey) return;
            TxtPassword.SelectionStart = TxtPassword.Text.Length + 1; //设置光标索引
        }
        /// <summary>
        /// 按照指定的长度生成密码字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string ConvertToPasswordChar(int length)
        {
            if (PasswordBuilder != null)
                PasswordBuilder.Clear();
            else
                PasswordBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                PasswordBuilder.Append('●');
            }
            return PasswordBuilder.ToString();
        }
        private void TxtPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                isDelKey = true;
                return;
            }
            isDelKey = false;
        }
    }
}
