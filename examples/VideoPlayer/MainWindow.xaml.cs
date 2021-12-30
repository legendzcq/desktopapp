using System;
using System.Windows;

using Microsoft.Win32;

namespace VideoPlayer
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

        private void OpenZipFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                DefaultExt = ".zip",
                Filter = "下载课堂压缩包|*.zip"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                MediaPlayerElement.Source = new Uri(dialog.FileName);
            }
        }
    }
}
