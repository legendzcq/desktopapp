using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
    public class PaperResultViewModel : ViewModelBase
    {
        private double _userScore;
        private string _totalTime;
        private int _rightCount;
        private int _errorCount;
        private string _correctRate;
        private int _totalCount;

        /// <summary>
        /// 用户得分
        /// </summary>
        public double UserScore
        {
            get { return _userScore; }
            set
            {
                _userScore = value;
                RaisePropertyChanged(() => UserScore);
            }
        }
        /// <summary>
        /// 用时
        /// </summary>
        public string TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                RaisePropertyChanged(() => TotalTime);
            }
        }
        /// <summary>
        /// 正确题数
        /// </summary>
        public int RightCount
        {
            get { return _rightCount; }
            set
            {
                _rightCount = value;
                RaisePropertyChanged(() => RightCount);
            }
        }
        /// <summary>
        /// 错误题数
        /// </summary>
        public int ErrorCount
        {
            get { return _errorCount; }
            set
            {
                _errorCount = value;
                RaisePropertyChanged(() => ErrorCount);
            }
        }
        /// <summary>
        /// 正确率
        /// </summary>
        public string CorrectRate
        {
            get { return _correctRate; }
            set
            {
                _correctRate = value;
                RaisePropertyChanged(() => CorrectRate);
            }
        }

        /// <summary>
        /// 总题数
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => _totalCount);
            }
        }
    }
}
