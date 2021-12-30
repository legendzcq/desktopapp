using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Framework.Push;
using Framework.Remote;
using Framework.Utility;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for MessageWindow.xaml
	/// </summary>
	public partial class MessageWindow : Window
	{
		public MessageWindow()
		{
			InitializeComponent();
		}

		public PushMessage Message { get; set; }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
			Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
			BtnDetail.Visibility = Visibility.Collapsed;
			if (Message.MessageType == 1)
			{
				try
				{
					TxtContent.Text = Message.MessageContent;
					SavePushState(Message);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					Close();
				}
			}
			if (Message.MessageType == 2)
			{
				try
				{
					TxtContent.Text = Message.MessageContent;
					SavePushState(Message);
					BtnDetail.Visibility = Visibility.Visible;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					Close();
				}
			}
		}

		private void SavePushState(PushMessage obj)
		{
			//var setting = SystemInfo.GetSetting("LastMessage");
			//if (setting != null)
			//{
			//	var lastId = Convert.ToInt32(setting);
			//	if (lastId > obj.MessageId)
			//	{
			//		Close();
			//		return;
			//	}
			//}
			//SystemInfo.SaveSetting("LastMessage", obj.MessageId);
			SystemInfo.StartBackGroundThread("异步更新消息状态",() =>
			{
				var web = new StudentRemote();
				web.SavePcPushInfo(obj.MessageId);
			});
		}

		private void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void BtnDetail_Click(object sender, RoutedEventArgs e)
		{
			if (Message.GetType() == typeof(PushLinkMessage))
			{
				Process.Start(((PushLinkMessage)Message).LinkUrl);
			}
		}
	}
}
