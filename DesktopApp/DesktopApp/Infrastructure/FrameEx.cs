using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DesktopApp.Infrastructure
{
    public static class FrameEx
    {
        public static void RegisteNavigationControl(this Frame frame)
        {
            var navigationMode = NavigationMode.New;
            frame.Navigating += (s, e) =>
            {
                var page = frame.Content as Page;
                if (page != null)
                {
                    navigationMode = e.NavigationMode;

                    var viewModel = page.DataContext as NavigationViewModelBase;
                    if (viewModel != null)
                    {
                        viewModel.OnNavigateFrom(e);
                    }
                }
            };

            frame.Navigated += (s, e) =>
            {
                var page = frame.Content as Page;
                if (page != null)
                {
                    var viewModel = page.DataContext as NavigationViewModelBase;
                    if (viewModel != null)
                    {
                        viewModel.NavigationService = frame.NavigationService;
                        viewModel.Dispatcher = Application.Current.Dispatcher;
                        viewModel.OnNavigateTo(e, navigationMode);
                    }
                }
            };
        }
    }
}
