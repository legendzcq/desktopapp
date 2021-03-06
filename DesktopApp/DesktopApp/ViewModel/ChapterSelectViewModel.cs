using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 选择下载章节视图模型
    /// </summary>
    public class ChapterSelectViewModel : ViewModelBase
    {
        //public ChapterSelectViewModel(ViewStudentWareDetail detail)
        //{
        //    ViewStudentWare = detail;

        //    ChapterName = detail.ChapterName;
        //    VideoName = string.IsNullOrEmpty(detail.VideoName) ?  detail.Title :detail.VideoName;
        //    VideoLength = detail.VideoLength;
        //    IsCanSelect = detail.VideoState == -1;
        //    IsSelected = !IsCanSelect;
        //}
        /// <summary>
        /// 批量删除使用
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="multDelete"></param>
        public ChapterSelectViewModel(ViewStudentWareDetail detail, int videoSt =-1)
        {
            ViewStudentWare = detail;

            ChapterName = detail.ChapterName;
            VideoName = string.IsNullOrEmpty(detail.VideoName) ? detail.Title : detail.VideoName;
            VideoLength = detail.VideoLength;
            IsCanSelect = detail.VideoState == videoSt;
            IsSelected = !IsCanSelect;
        }
        public ViewStudentWareDetail ViewStudentWare { get; private set; }

        private bool _isSelected;
        /// <summary>
        /// 当前项是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public string ChapterName { get; set; }

        public string VideoName { get; set; }

        public string VideoLength { get; set; }
        /// <summary>
        /// 未下载（状态-1）的章节可以选择
        /// </summary>
        public bool IsCanSelect { get; set; }
    }
}
