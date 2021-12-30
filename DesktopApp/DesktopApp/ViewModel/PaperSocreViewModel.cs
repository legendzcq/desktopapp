using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DesktopApp.ViewModel
{
    public class PaperSocreViewModel : NavigationViewModelBase
    {
        private int _paperId;
        private List<ViewStudentQuestion> _questionList;// 试题列表（不包含父级试题）
        private int _index;// 题目索引
        private StudentPaperScore _papeSocre;

        private ViewStudentPaper _paper;
        /// <summary>
        /// 做题时间
        /// </summary>
        public PaperSocreViewModel()
        {
            InitCmd();
        }

        #region 属性

        private string _pageTitle;
        private PaperSocreQuesViewModel _currentItem;
        private ListCollectionView _btnItems;
        private int _totalCount;
        private int _testedCount;
        private int _rightCount;
        private int _errorCount;
        private string _correctRate;
        private double _userScore;

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        /// <summary>
        /// 当前试题
        /// </summary>
        public PaperSocreQuesViewModel CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                RaisePropertyChanged(() => CurrentItem);
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
                RaisePropertyChanged(() => TotalCount);
            }
        }
        /// <summary>
        /// 已做题数
        /// </summary>
        public int TestedCount
        {
            get { return _testedCount; }
            set
            {
                _testedCount = value;
                RaisePropertyChanged(() => TestedCount);
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
        public ObservableCollection<PaperSocreQuesViewModel> Items { get; private set; }

        public ListCollectionView BtnItems
        {
            get { return _btnItems; }
            set
            {
                _btnItems = value;
                RaisePropertyChanged(() => BtnItems);
            }
        }
        #endregion

        #region 命令

        /// <summary>
        /// 选择试题
        /// </summary>
        public ICommand SelectQuestionCommand { get; private set; }
        /// <summary>
        /// 上一题
        /// </summary>
        public ICommand PrevCommand { get; private set; }
        /// <summary>
        /// 下一题
        /// </summary>
        public ICommand NextCommand { get; private set; }

        #endregion

        #region 方法

        private void InitCmd()
        {
            SelectQuestionCommand = new RelayCommand<PaperSocreQuesViewModel>(item =>
            {
                CurrentItem = item;
                _index = Items.IndexOf(item);
            });
            PrevCommand = new RelayCommand(Previous);

            NextCommand = new RelayCommand(Next);
        }
        /// <summary>
        /// 上一题
        /// </summary>
        private void Previous()
        {

            if (_index == 0)
            {
                CustomMessageBox.Show("已经是第一题");
                return;
            }
            CurrentItem = Items[--_index];
        }
        /// <summary>
        /// 下一题
        /// </summary>
        private void Next()
        {
            if (_index == Items.Count - 1)
            {
                CustomMessageBox.Show("已经是最后一题");
                return;
            }
            CurrentItem = Items[++_index];
        }
        private void BindData(ViewStudentPaper paper)
        {
            _index = 0;
            PageTitle = paper.PaperViewName;

            var paperViewId = paper.PaperViewId;
            var list = new List<PaperSocreQuesViewModel>();
            _questionList = StudentQuestionLogic.GetPaperScoreDetail(paperViewId,_papeSocre.PaperScoreID).ToList();
            _questionList.ForEach(model => list.Add(new PaperSocreQuesViewModel(_paperId, model)));
            BindBtnData(list);
            Items = new ObservableCollection<PaperSocreQuesViewModel>(list);
           
            if (Items.Any())
            {
                CurrentItem = Items[_index];
            }
            GetPaperResult();
        }

        private void BindBtnData(IEnumerable<PaperSocreQuesViewModel> items)
        {
            var groupList = items.GroupBy(i => i.Question.PartSequence).OrderBy(i => i.Key);
            // 组装题目列表
            var typeNum = 0;
            foreach (var typeGroup in groupList)
            {
                var questionType = typeGroup.First().Question.PartName;
                var questionTitle = GetChineseNum(typeNum++) + "、" + questionType + "（" + typeGroup.Count() + "题）";
                foreach (var item in typeGroup)
                {
                    item.TypeTitle = questionTitle;
                    item.Number = item.Question.Parent == null
                                      ? item.Question.MainId.ToString(CultureInfo.InvariantCulture)
                                      : item.Question.MainId + "<" + item.Question.SubId + ">";
                }
            }

            BtnItems = new ListCollectionView(items.ToList());
            if (BtnItems.GroupDescriptions != null)
                BtnItems.GroupDescriptions.Add(new PropertyGroupDescription("TypeTitle"));
        }

        private string GetChineseNum(int num)
        {
            var chs = new[] { "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            return num >= chs.Length ? string.Empty : chs[num];
        }
        /// <summary>
        /// 显示统计的做题结果
        /// </summary>
        private void GetPaperResult()
        {
            TotalCount = _questionList.Count;// 总题数
            TestedCount = _questionList.Count(q => !string.IsNullOrWhiteSpace(q.UserAnswer));
            RightCount = _questionList.Where(q=>("1,2,3,9").Contains(q.QuesTypeId.ToString())).Count(q => q.Answer == q.UserAnswer);
            ErrorCount = TestedCount-RightCount;
            UserScore = _questionList.Sum(q => q.UserScore);
            CorrectRate = (RightCount * 100.0 / TotalCount).ToString("F2") + "%";
           
        }
        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Back)
                return;
            if (mode == NavigationMode.Forward)
            {
                _paperId = _papeSocre.PaperViewID;
                BindData(_paper);
            }
            else
            {
                Cleanup();
                dynamic parms = e.ExtraData;
                _papeSocre = parms.Param1;
                //var vsPaper = parms.Param2;
                _paper = parms.Param2;

                _paperId = _papeSocre.PaperViewID;
                BindData(_paper);
            }
        }
        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
            }
        }
        public override void Cleanup()
        {
            _index = 0;
            _currentItem = null;
            Items = null;
        }
    }
}
