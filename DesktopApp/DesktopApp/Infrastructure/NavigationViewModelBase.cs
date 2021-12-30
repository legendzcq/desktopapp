using System.Windows.Navigation;
using System.Windows.Threading;
using GalaSoft.MvvmLight;

namespace DesktopApp.Infrastructure
{
    /// <summary>
    /// 导航视图模型抽象基类
    /// </summary>
    public abstract class NavigationViewModelBase : ViewModelBase
    {
        #region Loading

        private bool _isShowLoading;
        /// <summary>
        /// 是否显示Loading控件
        /// </summary>
        public bool IsShowLoading
        {
            get { return _isShowLoading; }
            set
            {
                _isShowLoading = value;
                RaisePropertyChanged(() => IsShowLoading);
            }
        }

        /// <summary>
        /// 显示Loading
        /// </summary>
        public void ShowLoading()
        {
            IsShowLoading = true;
        }

        /// <summary>
        /// 隐藏Loading控件
        /// </summary>
        public void HideLoading()
        {
            IsShowLoading = false;
        }

        #endregion

        public Dispatcher Dispatcher { get; set; }

        public NavigationService NavigationService { get; set; }

        public virtual void OnNavigateTo(NavigationEventArgs e, NavigationMode mode) { }

        public virtual void OnNavigateFrom(NavigatingCancelEventArgs e) { }
    }
}
