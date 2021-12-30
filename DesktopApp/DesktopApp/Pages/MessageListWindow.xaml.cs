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
using DesktopApp.ViewModel;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for MessageListWindow.xaml
	/// </summary>
	public partial class MessageListWindow : Window
	{
		public MessageListWindow()
		{
			InitializeComponent();
			var vm = (PushMessageViewModel)this.DataContext;
			vm.BindData();
		}

		private void BtnCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private bool _isPressd;
		private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressd = true;
			if (e.ClickCount == 2)
			{
				
			}
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
