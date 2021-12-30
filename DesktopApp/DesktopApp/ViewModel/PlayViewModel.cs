//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Navigation;
//using DesktopApp.Infrastructure;
//using DesktopApp.Pages;
//using Framework.Model;

//namespace DesktopApp.ViewModel
//{
//    public class PlayViewModel : NavigationViewModelBase
//    {
//        public PlayWindow PlayWindow { get; set; }
//        public ViewStudentWareDetail VideoItem { get; private set; }

//        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
//        {
//            VideoItem = e.ExtraData as ViewStudentWareDetail;
//        }

//        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
//        {
//            NavigationService.RemoveBackEntry();
//            Cleanup();
//        }

//        public override void Cleanup()
//        {
//            PlayWindow = null;
//            VideoItem = null;
//        }
//    }
//}
