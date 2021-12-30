using System.Windows.Navigation;
using DesktopApp.Infrastructure;

namespace DesktopApp.ViewModel
{
    public class SetManageViewModel : NavigationViewModelBase
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            SelectedIndex = 0;
        }

        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.RemoveBackEntry();
                ViewModelLocator.Cleanup();
            }
        }
    }
}
