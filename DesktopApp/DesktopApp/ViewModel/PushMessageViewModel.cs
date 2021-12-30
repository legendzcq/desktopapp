using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using DesktopApp.Controls;
using Framework.Local;
using Framework.Push;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
	public class PushMessageViewModel : ViewModelBase
	{
		public ObservableCollection<PushMessageItemViewModel> Items { get; set; }

		public ICommand GotoLinkCommand { get; private set; }

		public ICommand DeleteCommand { get; private set; }

		public PushMessageViewModel()
		{
			Items = new ObservableCollection<PushMessageItemViewModel>();

			DeleteCommand = new RelayCommand<PushMessageItemViewModel>(msg =>
			{
				if (CustomMessageBox.Show("是否真的删除该消息？", "确认", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
				{
					new StudentData().RemoveMessage(msg.MessageItem.MessageId);
					Items.Remove(msg);
				}
			});

			GotoLinkCommand = new RelayCommand<PushMessageItemViewModel>(msg =>
			{
				if (msg.MessageItem.MessageType == 2)
				{
					Process.Start(((PushLinkMessage)msg.MessageItem).LinkUrl);
				}
			});
		}

		public void BindData()
		{
			var lst = new StudentData().GetMessageList();
			Items.Clear();
			foreach (var item in lst)
			{
				Items.Add(new PushMessageItemViewModel(item));
			}
		}
	}

	public class PushMessageItemViewModel : ViewModelBase
	{
		private string _messageLink;
		private bool _canShowLink;
		private string _messageTime;
		private string _messageContent;

		public string MessageContent
		{
			get { return _messageContent; }
			set
			{
				_messageContent = value;
				RaisePropertyChanged(() => MessageContent);
			}
		}

		public bool CanShowLink
		{
			get { return _canShowLink; }
			set
			{
				_canShowLink = value;
				RaisePropertyChanged(() => CanShowLink);
			}
		}

		public string MessageLink
		{
			get { return _messageLink; }
			set
			{
				_messageLink = value;
				RaisePropertyChanged(() => MessageLink);
			}
		}

		public string MessageTime
		{
			get { return _messageTime; }
			set
			{
				_messageTime = value;
				RaisePropertyChanged(() => MessageTime);
			}
		}

		public PushMessage MessageItem { get; set; }

		public PushMessageItemViewModel(PushMessage item)
		{
			MessageItem = item;
			CanShowLink = item.MessageType == 2;
			MessageContent = item.MessageContent;
			MessageTime = item.MessageTime.ToString("yyyy-MM-dd HH:mm:ss");
			if (item.MessageType == 2)
			{
				MessageLink = ((PushLinkMessage)item).LinkUrl;
			}
		}
	}
}
