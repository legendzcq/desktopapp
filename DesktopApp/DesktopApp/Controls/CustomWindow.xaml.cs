using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using DesktopApp.Infrastructure;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.Controls
{
	/// <summary>
	/// Interaction logic for CustomWindow.xaml
	/// </summary>
	public partial class CustomWindow
	{

		public CustomWindow(Uri navUri, string title, object extraData = null)
		{
			InitializeComponent();

			MainFrame.RegisteNavigationControl();

			Messenger.Default.Register<string>(this, TokenManager.CloseCustomWindow, s => Close());

			Loaded += (s, e) =>
			{
				App.CurrentCustomWindow = this;
				Title = title;
				WindowTitle.Text = title;
				if (extraData == null)
				{
					MainFrame.Navigate(navUri);
				}
				else
				{
					MainFrame.Navigate(navUri, extraData);
				}
			};
			Closed += (s, e) =>
			{
				App.CurrentCustomWindow = null;
			};
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
