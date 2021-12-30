using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace TestVlc
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void BtnOpenFile_OnClick(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog() {Filter = "*.mp4|*.mp4"};
			ofd.ShowDialog();
			if (!string.IsNullOrEmpty(ofd.FileName))
			{
				PlayerMain.MediaPlayer.Play(new Uri(ofd.FileName));
			}
		}
	}
}
