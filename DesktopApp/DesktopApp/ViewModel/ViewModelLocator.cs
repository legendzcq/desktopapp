/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DesktopApp"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;

using GalaSoft.MvvmLight.Ioc;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CourseViewModel>();
            SimpleIoc.Default.Register<ChapterListViewModel>();
            //SimpleIoc.Default.Register<PlayViewModel>();

            SimpleIoc.Default.Register<CenterListViewModel>();
            SimpleIoc.Default.Register<PaperListViewModel>();
            SimpleIoc.Default.Register<PaperViewModel>();

            SimpleIoc.Default.Register<SetManageViewModel>();
            SimpleIoc.Default.Register<ImportViewModel>();
            SimpleIoc.Default.Register<DownloadCenterViewModel>();

            SimpleIoc.Default.Register<KcjyDownViewModel>();
            SimpleIoc.Default.Register<KcjyDetailViewModel>();

            SimpleIoc.Default.Register<MobileDownViewModel>();
            SimpleIoc.Default.Register<MobileDownChapterViewModel>();
            SimpleIoc.Default.Register<PushMessageViewModel>();

            SimpleIoc.Default.Register<CourseSettingViewModel>();
            SimpleIoc.Default.Register<CourseRecordViewModel>();

            SimpleIoc.Default.Register<PaperSocreListModel>();
            SimpleIoc.Default.Register<PaperSocreViewModel>();

            SimpleIoc.Default.Register<DeviceListViewModel>();
		}

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public CourseViewModel Course => ServiceLocator.Current.GetInstance<CourseViewModel>();

        public ChapterListViewModel ChapterList => ServiceLocator.Current.GetInstance<ChapterListViewModel>();

        public PaperListViewModel PaperList => ServiceLocator.Current.GetInstance<PaperListViewModel>();
        public PaperViewModel Paper => ServiceLocator.Current.GetInstance<PaperViewModel>();
        public CenterListViewModel CenterList => ServiceLocator.Current.GetInstance<CenterListViewModel>();
        public SetManageViewModel SetManage => ServiceLocator.Current.GetInstance<SetManageViewModel>();
        public ImportViewModel Import => ServiceLocator.Current.GetInstance<ImportViewModel>();
        public DownloadCenterViewModel DownloadCenter => ServiceLocator.Current.GetInstance<DownloadCenterViewModel>();

        public KcjyDownViewModel KcjyDown => ServiceLocator.Current.GetInstance<KcjyDownViewModel>();

        public KcjyDetailViewModel KcjyDetail => ServiceLocator.Current.GetInstance<KcjyDetailViewModel>();

        public MobileDownViewModel MobileDown => ServiceLocator.Current.GetInstance<MobileDownViewModel>();

        public MobileDownChapterViewModel MobileDownChapter => ServiceLocator.Current.GetInstance<MobileDownChapterViewModel>();

        public PushMessageViewModel PushMessage => ServiceLocator.Current.GetInstance<PushMessageViewModel>();

        public CourseSettingViewModel CourseSetting => ServiceLocator.Current.GetInstance<CourseSettingViewModel>();
        public CourseRecordViewModel CourseRecordView => ServiceLocator.Current.GetInstance<CourseRecordViewModel>();
        public PaperSocreListModel PaperScoreList => ServiceLocator.Current.GetInstance<PaperSocreListModel>();
        public PaperSocreViewModel PaperScoreView => ServiceLocator.Current.GetInstance<PaperSocreViewModel>();

        public DeviceListViewModel DeviceListVM => ServiceLocator.Current.GetInstance<DeviceListViewModel>();

        //public PlayViewModel Play
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<PlayViewModel>();
        //    }
        //}
        /// <summary>
        /// 只清理了ImportViewModel（导入课程视图模型）
        /// </summary>
        public static void Cleanup() => ServiceLocator.Current.GetInstance<ImportViewModel>().Cleanup();
    }
}