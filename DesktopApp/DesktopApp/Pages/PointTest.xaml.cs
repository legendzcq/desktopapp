using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DesktopApp.Controls;
using DesktopApp.ViewModel;
using Framework.Model;
using Framework.NewModel;
using Framework.Remote;
using Framework.Utility;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for PointTest.xaml
	/// </summary>
	public partial class PointTest
	{
		private WebBrowserOverlay _webBrowserOverlay;

		private readonly ViewStudentCourseWare _course;

		private PointTestStartTimeItem _item;

		public PointTest(PointTestStartTimeItem item, ViewStudentCourseWare course)
		{
			InitializeComponent();

			_course = course;
			_item = item;
			var vm = new PointTestViewModel(item);
			TxtPointName.Text = _item.PointName;
			DataContext = vm;

			Loaded += (s, e) =>
			{
				// 解决WPF Window设置AllowsTransparency="True"后，WebBrowser无法显示的问题
				_webBrowserOverlay = new WebBrowserOverlay(BorderOwner);
				// 绑定ViewModel的HtmlContent到WebBrowser控件
				var binding = new Binding
				{
					Path = new PropertyPath("CurrentItem.HtmlContent"),
					Source = DataContext
				};
				BindingOperations.SetBinding(_webBrowserOverlay, WebBrowserOverlay.HtmlProperty, binding);

				_webBrowserOverlay.WebBrowser.DocumentCompleted += _webBrowserOverlay_LoadCompleted;
				var helper = new ObjectForScript(this);
				_webBrowserOverlay.WebBrowser.ObjectForScripting = helper;
			};
		}

		void _webBrowserOverlay_LoadCompleted(object s, EventArgs e)
		{
			var viewModel = DataContext as PointTestViewModel;
			if (viewModel == null || viewModel.CurrentItem == null)
				return;
			try
			{
				if (viewModel.HasSubmit)
				{
					ShowAnswer();
				}
				if (_webBrowserOverlay.WebBrowser.Document != null)
					_webBrowserOverlay.WebBrowser.Document.InvokeScript("setAnswer", new object[] { viewModel.CurrentItem.UserAnswer });
			}
			catch (Exception ex)
			{
				Debug.WriteLine("setAnswer JS调用错误：" + ex.Message);
			}
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

		private void BtnCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		[ComVisible(true)]
		public class ObjectForScript
		{
			private readonly PointTest _page;

			public ObjectForScript(PointTest page)
			{
				_page = page;
			}

			public void ClickEvent()
			{
				//donothing
			}

			/// <summary>
			/// 试题选择状态更改事件
			/// </summary>
			/// <param name="userAnswer"></param>
			public void SelectStateChang(string userAnswer)
			{
				var viewModel = _page.DataContext as PointTestViewModel;
				if (viewModel != null)
				{
					viewModel.CurrentItem.UserAnswer = userAnswer;
				}
			}
		}

		private void BtnReStudy_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void BtnContinue_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void ShowAnswer()
		{
			var viewModel = DataContext as PointTestViewModel;
			if (viewModel == null)
				return;
			try
			{
				string isRight = string.Empty, isDisabled = string.Empty;

				isDisabled = "true";

				if (viewModel.CurrentItem.IsRight == 1)
					isRight = "true";
				else if (viewModel.CurrentItem.IsRight == 2)
					isRight = "false";
				if (_webBrowserOverlay.WebBrowser.Document != null)
					_webBrowserOverlay.WebBrowser.Document.InvokeScript("showAnswer", new object[]
					{
						viewModel.CurrentItem.QuestionItem.RightAnswer, viewModel.CurrentItem.QuestionItem.Analysis ?? string.Empty, isRight, isDisabled
					});
			}
			catch (Exception ex)
			{
				Debug.WriteLine("showAnswer JS调用错误：" + ex.Message);
			}
		}

		private void BtnSubmit_Click(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as PointTestViewModel;
			if (viewModel != null)
			{
				if (viewModel.HasAnswer())
				{
					viewModel.HasSubmit = true;

					ShowAnswer();
					viewModel.CurrentItem = viewModel.QuestionList[viewModel.CurrentIndex];
					if (Util.IsOnline)
					{
						SystemInfo.StartBackGroundThread("", () =>
						{
							var questionInfo = viewModel.GetQuestionInfo();
							new StudentWareRemote().SavePointTestResult(_item.TestId, _course.CwId, _course.BoardId, questionInfo,
								_item.PointOpenType);
						});
					}
					BtnSubmit.Visibility = Visibility.Collapsed;
					if (_item.PointOpenType != "t") BtnReStudy.Visibility = Visibility.Visible;
					BtnContinue.Visibility = Visibility.Visible;
				}
				else
				{
					CustomMessageBox.Show("您还没有答题，请答题后再提交！");
				}
			}
		}
	}
}
