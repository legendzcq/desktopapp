using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for ImportPage.xaml
	/// </summary>
	public partial class ImportPage : Page
	{
		public ImportPage()
		{
			InitializeComponent();

			Loaded += (s, e) =>
			{
				var iv = DataContext as ImportViewModel;
				if (iv != null) iv.CleanUpImported();
			};

#if CHINAACC
			BtnStopImport.Visibility = System.Windows.Visibility.Visible;
#else
            BtnStopImport.Visibility = System.Windows.Visibility.Collapsed;
#endif
		}
	}
}
