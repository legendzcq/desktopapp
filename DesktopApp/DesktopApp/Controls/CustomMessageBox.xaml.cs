using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using DesktopApp.Logic;
using Framework.Utility;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace DesktopApp.Controls
{
	/// <summary>
	/// 自定义弹出框
	/// </summary>
	public partial class CustomMessageBox
	{
		private CustomMessageBox()
		{
			InitializeComponent();

			if (App.CurrentCustomWindow != null)
			{
				Owner = App.CurrentCustomWindow;
			}
			else
			{
				Owner = App.CurrentMainWindow;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg">要在消息框中显示的文本</param>
		/// <param name="title">要在消息框的标题栏中显示的文本</param>
		/// <param name="boxButton">在消息框中显示哪些按钮</param>
		/// <param name="width">宽度</param>
		/// <param name="height">高度</param>
		/// <param name="owner"></param>
		/// <param name="checkBoxVisible">协议提示是否显示</param>
		/// <param name="checkBoxText">协议内容</param>
		/// <returns></returns>
		public static MessageBoxResult Show(string msg, string title = "提示", MessageBoxButton boxButton = MessageBoxButton.OK, int width = 500, int height = 180, bool checkBoxVisible = false, string checkBoxText = "", Window owner = null)
		{
			var msgBox = new CustomMessageBox
			{
				TxtTitle = { Text = title },
				TxtContent = { Text = msg },
				Width = width,
				Height = height,
				ChkAgree = { Content = checkBoxText, Visibility = Visibility.Collapsed }
			};
			if (owner != null)
			{
				msgBox.Owner = owner;
			}

			Grid.SetRow(msgBox.GridBtns, 3);
			switch (boxButton)
			{
				case MessageBoxButton.OKCancel:
					msgBox.BtnYes.Content = "确定";
					msgBox.BtnNo.Content = "取消";
					msgBox.BtnYes.Visibility = Visibility.Visible;
					msgBox.BtnNo.Visibility = Visibility.Visible;
					break;
				case MessageBoxButton.YesNo:
					msgBox.BtnYes.Content = "是";
					msgBox.BtnNo.Content = "否";
					msgBox.BtnYes.Visibility = Visibility.Visible;
					msgBox.BtnNo.Visibility = Visibility.Visible;
					break;
				case MessageBoxButton.YesNoCancel:
					msgBox.BtnYes.Visibility = Visibility.Visible;
					msgBox.BtnNo.Visibility = Visibility.Visible;
					break;
				default:
					msgBox.BtnYes.Content = "确定";
					msgBox.BtnYes.IsDefault = true;
					msgBox.BtnYes.Visibility = Visibility.Visible;
					msgBox.BtnNo.Visibility = Visibility.Collapsed;
					break;
			}

			var msgboxResult = new MessageBoxResult();

			msgBox.BtnYes.Click += (s, e) =>
			{
				switch (boxButton)
				{
					case MessageBoxButton.OKCancel:
						msgboxResult = MessageBoxResult.OK;
						break;
					case MessageBoxButton.YesNo:
						msgboxResult = MessageBoxResult.Yes;
						break;
					case MessageBoxButton.YesNoCancel:
						msgboxResult = MessageBoxResult.Yes;
						break;
					default:
						msgboxResult = MessageBoxResult.OK;
						break;
				}
				msgBox.Close();
			};

			msgBox.ImgClose.MouseLeftButtonDown += (s, e) =>
			{
				switch (boxButton)
				{
					case MessageBoxButton.OKCancel:
						msgboxResult = MessageBoxResult.Cancel;
						break;
					case MessageBoxButton.YesNo:
						msgboxResult = MessageBoxResult.No;
						break;
					case MessageBoxButton.YesNoCancel:
						msgboxResult = MessageBoxResult.No;
						break;
					default:
						msgboxResult = MessageBoxResult.OK;
						break;
				}
				msgBox.Close();
			};
			msgBox.BtnNo.Click += (s, e) =>
			{
				switch (boxButton)
				{
					case MessageBoxButton.OKCancel:
						msgboxResult = MessageBoxResult.Cancel;
						break;
					case MessageBoxButton.YesNo:
						msgboxResult = MessageBoxResult.No;
						break;
					case MessageBoxButton.YesNoCancel:
						msgboxResult = MessageBoxResult.No;
						break;
					default:
						msgboxResult = MessageBoxResult.OK;
						break;
				}
				msgBox.Close();
			};
			msgBox.ChkAgree.Visibility = checkBoxVisible ? Visibility.Visible : Visibility.Collapsed;
			msgBox.ChkAgree.IsChecked = !checkBoxVisible;
			msgBox.ShowDialog();
			return msgboxResult;
		}

		/// <summary>
		/// 选择视频格式
		/// </summary>
		/// <returns>True为高清，False为标清，NULL为未选择</returns>
		public static int ShowSelectVideoType()
		{
			var msgBox = new CustomMessageBox
			{
				Height = 350,
				TxtTitle = { Text = "初始设置" },
				TxtContent = { Visibility = Visibility.Collapsed },
				BtnYes = { Content = "确定", IsDefault = true, Visibility = Visibility.Visible, Margin = new Thickness(0, 0, 20, 0) },
				BtnNo = { Visibility = Visibility.Collapsed },
				PanelSelect = { Margin = new Thickness(20, 0, 20, 0), Visibility = Visibility.Visible },
				TxtPath = { Text = Util.VideoPath },
				TxtDownloadPath = { Text = Util.VideoDownSavePath },
				RdHd = { IsChecked = true },
				ChkAgree = { Visibility = Visibility.Collapsed, IsChecked = true }
			};
			// 初始赋值

			int result = 0;
			var action = new Action(() =>
			{
				if (msgBox.RdHd.IsChecked != null && msgBox.RdHd.IsChecked.Value)
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
				msgBox.Close();
			});
			msgBox.BtnYes.Click += (s, e) => action();
			msgBox.ImgClose.MouseLeftButtonDown += (s, e) => action();

			msgBox.ShowDialog();
			return result;
		}

		private void SelectDirectory_OnClick(object sender, RoutedEventArgs e)
		{
			var fbd = new FolderBrowserDialog { SelectedPath = TxtPath.Text };
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TxtPath.Text = fbd.SelectedPath;
				SaveConfig();
			}
		}

		private void SelectDownloadDirectory_OnClick(object sender, RoutedEventArgs e)
		{
			var fbd = new FolderBrowserDialog { SelectedPath = TxtDownloadPath.Text };
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				TxtDownloadPath.Text = fbd.SelectedPath;
				SaveConfig();
			}
		}

		private void SaveConfig()
		{
			try
			{
				Util.VideoPath = TxtPath.Text;
				Util.VideoDownSavePath = TxtDownloadPath.Text;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}
	}
}
