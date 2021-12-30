using System.Windows;
using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for ChapterPage.xaml
    /// </summary>
    public partial class ChapterPage : Page
    {
        public ChapterPage()
        {
            InitializeComponent();

            Loaded += ChapterPage_Loaded;
            Unloaded += ChapterPage_Unloaded;
        }

        private void ChapterPage_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigating += NavigationService_Navigating;
        }

        /**
         * 用于扩展返回按钮的功能，当进入批量删除or批量下载页面时，返回按钮用于返回章节显示页
         * @author ChW
         * @date 2021-05-07
         */
        private void NavigationService_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (App.Loc.ChapterList.State == DesktopApp.ViewModel.ChapterPageState.Select
                || App.Loc.ChapterList.State == DesktopApp.ViewModel.ChapterPageState.Delete)
            {
                // 返回章节显示页
                App.Loc.ChapterList.State = DesktopApp.ViewModel.ChapterPageState.View;

                // 阻止页面返回上一页
                e.Cancel = true;
            }
        }

        private void ChapterPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigating -= NavigationService_Navigating;
            }
        }
    }
}
