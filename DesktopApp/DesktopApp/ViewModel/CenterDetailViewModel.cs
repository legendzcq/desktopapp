using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 考试中心子项视图模型
    /// </summary>
    public class CenterDetailViewModel : ViewModelBase
    {
        public CenterDetailViewModel(ViewStudentCenter center)
        {
            Center = center;
            IsShowBtn = true;
            SiteCourseName = center.SiteCourseName;
            // 从ViewSetudentCenter转换过来的时候带上UpdateTime属性，@author ChW，@date 2021-05-17
            UpdateTime = center.UpdateTime;
        }

        public string SiteCourseName { get; set; }

        private bool _isLoading;
        private bool _isShowBtn;

        /// <summary>
        /// 是否显示Loading动画
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }
        /// <summary>
        /// 是否显示刷新按钮
        /// </summary>
        public bool IsShowBtn
        {
            get { return _isShowBtn; }
            set
            {
                _isShowBtn = value;
                RaisePropertyChanged(() => IsShowBtn);
            }
        }

        public ViewStudentCenter Center { get; private set; }

        /**
         * 显示更新时间
         * @author ChW
         * @date 2021-05-12
         */
        private string _updateTime;
        public string UpdateTime
        {
            get { return _updateTime; }
            set
            {
                _updateTime = value;
                RaisePropertyChanged(() => UpdateTime);
            }
        }
    }
}
