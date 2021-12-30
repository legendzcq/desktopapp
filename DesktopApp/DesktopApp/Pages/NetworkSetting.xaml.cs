using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
using Framework.Utility;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for NetworkSetting.xaml
	/// </summary>
	public partial class NetworkSetting
	{
		public NetworkSetting()
		{
			InitializeComponent();

			Loaded += NetworkSetting_Loaded;
		}

		void NetworkSetting_Loaded(object sender, RoutedEventArgs e)
		{
			RdProxyDefault.IsChecked = Util.ProxyType == 0;
			RdProxyNot.IsChecked = Util.ProxyType == 1;
			RdProxyYes.IsChecked = Util.ProxyType == 2;

			TxtProxyAddress.Text = Util.ProxyAddress;
			TxtProxyPort.Text = Util.ProxyPort.ToString(CultureInfo.InvariantCulture);
			TxtProxyUserName.Text = Util.ProxyUserName;
			TxtProxyPassword.Password = Util.ProxyUserPassword;
		}

		private void BtnSaveSettings_OnClick(object sender, RoutedEventArgs e)
		{
			int port = int.TryParse(TxtProxyPort.Text, out port) ? port : 0;

			if (RdProxyYes.IsChecked.HasValue && RdProxyYes.IsChecked.Value)
			{
				Util.ProxyType = 2;
			}
			else
			{
				if (RdProxyNot.IsChecked.HasValue && RdProxyNot.IsChecked.Value)
				{
					Util.ProxyType = 1;
				}
				else
				{
					Util.ProxyType = 0;
				}
			}

			Util.ProxyAddress = TxtProxyAddress.Text;
			Util.ProxyPort = port;
			Util.ProxyUserName = TxtProxyUserName.Text;
			Util.ProxyUserPassword = TxtProxyPassword.Password;

			this.Close();
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private bool _isPressd;
		private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressd = true;
		}

		private void GridTop_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isPressd = false;
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
					var pos = Mouse.GetPosition(this);
					Top = SystemParameters.WorkArea.Y - pos.Y;
				}
				DragMove();
			}
		}
	}
}
