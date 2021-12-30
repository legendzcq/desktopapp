using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			_timerPlay.Tick +=TimerPlay_Tick;
			PlayerMain.BufferingStarted += (s, e) =>
			{
				Trace.WriteLine("BufferStart");
			};
			PlayerMain.BufferingEnded += (s, e) =>
			{
				Trace.WriteLine("BufferEnded");
			};
			PlayerMain.MediaOpened += (s, e) =>
			{
				Trace.WriteLine("MediaOpened");
			};
		}

		private void TimerPlay_Tick(object sender, EventArgs e)
		{
			try
			{
				SliderMain.Maximum = PlayerMain.NaturalDuration.TimeSpan.TotalSeconds;
			}
			catch (Exception ex)
			{
				;
			}
			SliderMain.Value = PlayerMain.Position.TotalSeconds;
		}

		readonly System.Windows.Forms.Timer _timerPlay = new System.Windows.Forms.Timer { Interval = 1000 };

		private void BtnBeginPlay_OnClick(object sender, RoutedEventArgs e)
		{
			PlayerMain.Source = new Uri("http://res.chnedu.com/hd2015/congye/quanguo/xiti/kjdsh03/flash_hdg/kjdsh0304.mp4");
			PlayerMain.Play();
			_timerPlay.Enabled = true;
		}

		private void BtnPlay_OnClick(object sender, RoutedEventArgs e)
		{
			PlayerMain.Play();
			_timerPlay.Enabled = true;
		}

		private void BtnPause_OnClick(object sender, RoutedEventArgs e)
		{
			PlayerMain.Pause();
			_timerPlay.Enabled = false;
		}

		private void PlayerMain_OnMediaEnded(object sender, RoutedEventArgs e)
		{
			_timerPlay.Enabled = false;
		}
	}
}
