using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
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
using System.Windows.Shapes;
using Framework.Download;
using Framework.Utility;

namespace DesktopApp.Pages
{
	/// <summary>
	/// AdvanceSetting.xaml 的交互逻辑
	/// </summary>
	public partial class AdvanceSetting : Window
	{
		public AdvanceSetting()
		{
			InitializeComponent();
			var fonts = new InstalledFontCollection();
			foreach (var f in fonts.Families)
			{
				var item = new ComboBoxItem();
				item.Content = f.Name;
				item.FontSize = 14;
				item.FontFamily = new FontFamily(f.Name);
				CbFonts.Items.Add(item);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TbDeviceId.Text = Util.DeviceId;

			RbDnsPublic.IsChecked = Util.DnsType == Util.DnsState.PublicDnsServer;
			RbDnsOfficial.IsChecked = Util.DnsType == Util.DnsState.OfficalDnsServer;
			RbDnsDefault.IsChecked = Util.DnsType == Util.DnsState.Default;

			RbOffLine.IsChecked = !Util.DisableOnlineCheck;
			RbOnline.IsChecked = Util.DisableOnlineCheck;

			RbDisCookieN.IsChecked = !Util.DisableCookie;
			RbDisCookieY.IsChecked = Util.DisableCookie;

			TxtDownLoadBlock.Text = (MultiBlockDownloader.MinBlockSize / 1024).ToString(CultureInfo.InvariantCulture);
			TxtPackSize.Text = MultiBlockDownloader.PackSize.ToString(CultureInfo.InvariantCulture);
			//TxtThreadCount.Text = MultiBlockDownloader.ThreadCount.ToString(CultureInfo.InvariantCulture);

			foreach (object t in CbFonts.Items)
			{
				var item = t as ComboBoxItem;
				if (item != null && item.Content as string == Util.BaseFont)
					item.IsSelected = true;
			}
		}

		private void RbDnsDefault_Click(object sender, RoutedEventArgs e)
		{
			Util.DnsType = Util.DnsState.Default;
		}

		private void RbDnsOfficial_Click(object sender, RoutedEventArgs e)
		{
			Util.DnsType = Util.DnsState.OfficalDnsServer;
		}

		private void RbDnsPublic_Click(object sender, RoutedEventArgs e)
		{
			Util.DnsType = Util.DnsState.PublicDnsServer;
		}

		private void RbOnline_Click(object sender, RoutedEventArgs e)
		{
			var chk = RbOffLine.IsChecked;
			Util.DisableOnlineCheck = chk.HasValue && chk.Value;
		}

		private void RbOffLine_Click(object sender, RoutedEventArgs e)
		{
			var chk = RbOffLine.IsChecked;
			Util.DisableOnlineCheck = chk.HasValue && chk.Value;
		}

		private void btnNetSet_Click(object sender, RoutedEventArgs e)
		{
			int blockSize = int.TryParse(TxtDownLoadBlock.Text, out blockSize) ? blockSize : 64;
			if (blockSize <= 10) blockSize = 64;
			if (blockSize > 1024) blockSize = 1024;
			int packSize = int.TryParse(TxtPackSize.Text, out packSize) ? packSize : 4096;
			if (packSize <= 512) packSize = 512;
			if (packSize > 32768) packSize = 32768;
			int threadCount = int.TryParse(TxtThreadCount.Text, out threadCount) ? threadCount : 2;
			if (threadCount < 2) threadCount = 2;
			if (threadCount > 10) threadCount = 10;
			MultiBlockDownloader.MinBlockSize = blockSize * 1024;
			MultiBlockDownloader.PackSize = packSize;
			//MultiBlockDownloader.ThreadCount = threadCount;
		}

		private void RbDisCookieN_Click(object sender, RoutedEventArgs e)
		{
			var chk = RbDisCookieY.IsChecked;
			Util.DisableCookie = chk.HasValue && chk.Value;
		}

		private void RbDisCookieY_Click(object sender, RoutedEventArgs e)
		{
			var chk = RbDisCookieY.IsChecked;
			Util.DisableCookie = chk.HasValue && chk.Value;
		}


		private void CbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBoxItem = CbFonts.SelectedValue as ComboBoxItem;
			if (comboBoxItem != null)
			{
				var fontName = comboBoxItem.Content as string;
				if (fontName != null)
				{
					var font = new FontFamily(fontName);
					LbFontTest.FontFamily = font;
				}
			}
		}

		private void btnFont_Click(object sender, RoutedEventArgs e)
		{
			var comboBoxItem = CbFonts.SelectedValue as ComboBoxItem;
			if (comboBoxItem != null)
			{
				var fontName = comboBoxItem.Content as string;
				Util.BaseFont = fontName;
				MessageBox.Show("您更改了系统字体，系统即将重新加载设置");
				Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
				Process.GetCurrentProcess().Kill();
			}
		}
	}
}
