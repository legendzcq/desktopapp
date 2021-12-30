using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using DesktopApp.ViewModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for DownloadCenterPage.xaml
    /// </summary>
    public partial class DownloadCenterPage : Page
    {
        public DownloadCenterPage()
        {
            InitializeComponent();
			Messenger.Default.Register<string>(this, TokenManager.RefreshDownloadList, s => ChkItem_OnClick(null, null));
            Loaded += (s, e) =>
            {
                var vm = DataContext as DownloadCenterViewModel;
                if (vm != null)
                {
	                vm.Dispatcher = Dispatcher;
                    vm.RefreshCommand.Execute(null);
                    ChkItem_OnClick(null, null);
                }
            };
        }

        private void ChkItem_OnClick(object sender, RoutedEventArgs e)
        {
            var items = DgData.ItemsSource as IList<DownloadItemViewModel>;
            if (items != null)
            {
                if (!items.Any() || items.Any(i => !i.IsSelected))
                {
                    ChkAll.IsChecked = false; 
                }
                else if (items.All(i => i.IsSelected))
                {
                    ChkAll.IsChecked = true; 
                }

                RefreshBtnState(items);
            }
        }

        private void ChkAll_OnClick(object sender, RoutedEventArgs e)
        {
            var items = DgData.ItemsSource as IList<DownloadItemViewModel>;
            if (items != null)
            {
                items.ToList().ForEach(s =>
                {
                    s.IsSelected = ChkAll.IsChecked == true;
                });
                RefreshBtnState(items);
            }
        }

        private void RefreshBtnState(IList<DownloadItemViewModel> items)
        {
            ImgDel.IsEnabled = items.Any(i => i.IsSelected);
            ImgPause.IsEnabled = items.Any(i => i.IsSelected && (i.VideoState == VideoState.Downloading || i.VideoState == VideoState.Wait));
            ImgStart.IsEnabled = items.Any(i => i.IsSelected && (i.VideoState == VideoState.Pause));
        }
    }
}
