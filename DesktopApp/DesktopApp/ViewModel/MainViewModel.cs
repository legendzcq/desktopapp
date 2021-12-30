using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
        }

        /// <summary>
        /// πÿ±’TabItem√¸¡Ó
        /// </summary>
        public ICommand CloseTabCommand { get; set; }
    }
}