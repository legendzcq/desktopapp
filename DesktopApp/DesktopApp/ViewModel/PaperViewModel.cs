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
using System.Windows.Threading;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;
using Framework.NewModel;

namespace DesktopApp.ViewModel
{
    public class PaperViewModel : NavigationViewModelBase
    {
        private int _paperId;
        private List<ViewStudentQuestion> _questionList;// 试题列表（不包含父级试题）
        private int _index;// 题目索引
        private int contestTimes;//限制提交次数

        // 计时器
        private TimeSpan _elapsedTime;
        private DispatcherTimer _timer;

        public PaperViewModel()
        {
            PageStatus = PageStatus.Testing;
            IsBtnShow = true;
            InitCmd();
        }

        #region 属性

        private string _pageTitle;
        private QuestionItemViewModel _currentItem;
        private PaperResultViewModel _paperResult;
        private ListCollectionView _btnItems;
        private int _totalCount;
        private int _testedCount;
        private bool _isShowResult;
        private string _usedTime;
        private bool _isBtnHide;

        // 公用试卷信息，在获取试卷下的题目的时得到，用于处理题目提交时线上不显示答案的Bug
        public int _version = 1;

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
        public QuestionItemViewModel CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                RaisePropertyChanged(() => CurrentItem);
            }
        }

        /// <summary>
        /// 做题结果视图模型
        /// </summary>
        public PaperResultViewModel PaperResult
        {
            get { return _paperResult; }
            set
            {
                _paperResult = value;
                RaisePropertyChanged(() => PaperResult);
            }
        }
        /// <summary>
        /// 做题状态
        /// </summary>
        public PageStatus PageStatus { get; set; }
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
        /// 是否显示答题结果
        /// </summary>
        public bool IsShowResult
        {
            get { return _isShowResult; }
            set
            {
                _isShowResult = value;
                RaisePropertyChanged(() => IsShowResult);
            }
        }

        /// <summary>
        /// 用时
        /// </summary>
        public string UsedTime
        {
            get { return _usedTime; }
            set
            {
                _usedTime = value;
                RaisePropertyChanged(() => UsedTime);
            }
        }

        /// <summary>
        /// 交卷后隐藏按钮
        /// </summary>
        public bool IsBtnShow
        {
            get { return _isBtnHide; }
            set
            {
                _isBtnHide = value;
                RaisePropertyChanged(() => IsBtnShow);
            }
        }

        public ObservableCollection<QuestionItemViewModel> Items { get; private set; }

        public ListCollectionView BtnItems
        {
            get { return _btnItems; }
            set
            {
                _btnItems = value;
                RaisePropertyChanged(() => BtnItems);
            }
        }

        public ObservableCollection<QuestionItemViewModel> ErrorRecordList { get; private set; }
        public ViewStudentCenter Center { get; set; }

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
        /// <summary>
        /// 自动下一题
        /// </summary>
        public ICommand AutoNextCommand { get; private set; }
        /// <summary>
        /// 试题选择状态切换
        /// </summary>
        public ICommand QuestionStateCommand { get; private set; }
        /// <summary>
        /// 交卷
        /// </summary>
        public ICommand HandInCommand { get; private set; }
        /// <summary>
        /// 交卷
        /// </summary>
        public ICommand ErrorRecordCommand { get; private set; }

        #endregion

        #region 方法

        private void InitCmd()
        {
            SelectQuestionCommand = new RelayCommand<QuestionItemViewModel>(item =>
            {
                if (IsBtnShow) Log.RecordData("TikuSelectQuestion", _paperId, item.Question.QuestionId);
                CurrentItem = item;
                _index = Items.IndexOf(item);
            });

            HandInCommand = new RelayCommand(() =>
            {
                if (!CheckTested())
                {
                    CustomMessageBox.Show("本试卷您没有答题，请答题后再交卷");
                    return;
                }
                if (CustomMessageBox.Show("交卷后对试卷判分，是否交卷？", "提示", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel)
                {
                    return;
                }
                Log.RecordData("TikuHandIn", _paperId);
                HandIn();
            });

            PrevCommand = new RelayCommand(Previous);

            NextCommand = new RelayCommand(Next);

            AutoNextCommand = new RelayCommand(() =>
            {
                if (_index != Items.Count - 1)
                    Next();
            });

            QuestionStateCommand = new RelayCommand<string>(userAnswer =>
            {
                CurrentItem.StatusImage = string.IsNullOrEmpty(userAnswer) ? "/Images/Paper/exam_undo.png" : "/Images/Paper/exam_do.png";
                CurrentItem.UserAnswer = userAnswer;// 获取用户答案

                TestedCount = Items.Count(q => !string.IsNullOrEmpty(q.UserAnswer));
            });

            ErrorRecordCommand = new RelayCommand(() =>
            {
                var sb = new StringBuilder();
                var list = StudentQuestionLogic.GetStudentQuestionRecord(_paperId, CurrentItem.Question.QuestionId);
                if (list.Count == 0)
                {
                    CustomMessageBox.Show("没有之前的出错记录");
                    return;
                }

                foreach (var item in list)
                {
                    sb.Append(string.Format("{0}         {1}\r\n", item.UserAnswer, item.SaveTime.ToString("yyyy/MM/dd HH:mm:ss")));
                }

                if (CustomMessageBox.Show(sb.ToString(), "清除出错记录", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    StudentQuestionLogic.SetQuestionResolved(_paperId, CurrentItem.Question.QuestionId);
                }

                Log.RecordData("TikuErrorRecord", _paperId, CurrentItem.Question.QuestionId);
            });
        }
        /// <summary>
        /// 交卷
        /// </summary>
        private void HandIn()
        {
            //PageStatus = PageStatus.Done;
            StopTimer();
            //////IsBtnShow = false;

            //////// 获取并显示做题结果
            //////GetPaperResult();
            //////IsShowResult = true;
            //////// 保存做题记录
            //////SaveResult();
            //////// 跳转至第一题
            //////CurrentItem = null;
            //////_index = 0;
            //////CurrentItem = Items[_index];
            var anwserlist = new List<StudentPaperScoreAnswer>();
            var userAswersList = Items.ToList().Where(i => !string.IsNullOrEmpty(i.UserAnswer)).ToList();
            userAswersList.ForEach(q => anwserlist.Add(new StudentPaperScoreAnswer
            {
                QuestionID = q.Question.QuestionId,
                UserAnswer = q.UserAnswer,
                UserScore = q.IsRight == 1 ? q.Question.Score.ToString() : "0"
            }));

            // 修复Bug：上次记录时，如果是大题（主观题）的小题，需要一起记录大题的questionID上传，否则小题在服务器不被记录
            // 过滤出属于大题中的小题
            var userAswersList_isZhuguan = Items.ToList().Where(i => !string.IsNullOrEmpty(i.UserAnswer) && i.IsZhuguanHasAnswer).ToList();
            // 向parentList内添加大题（主观题）的questionID，HashSet不会包含重复ID
            var parentList = new HashSet<int>();
            userAswersList_isZhuguan.ForEach(q => parentList.Add(q.Question.ParentId));
            // 将大题ID加入anwserlist
            foreach (var item in parentList)
            {
                anwserlist.Add(new StudentPaperScoreAnswer
                {
                    QuestionID = item,
                    UserAnswer = "",
                    UserScore = "0"
                });
            }

            var score = Items.Where(q => q.IsRight == 1).Sum(q => q.Question.Score);
            var sumSocre = Items.Sum(q => q.Question.Score);
            StudentPaperScore stuQuesRecord = new StudentPaperScore()
            {
                Answers = anwserlist,
                AutoScore = score.ToString(CultureInfo.InvariantCulture),
                CenterID = Center.CenterId,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                LastScore = score.ToString(CultureInfo.InvariantCulture),
                PaperScore = sumSocre.ToString(CultureInfo.InvariantCulture),
                PaperViewID = _paperId,
                SpendTime = _elapsedTime.TotalSeconds.ToString(CultureInfo.InvariantCulture),
                SiteCourseID = Center.SiteCourseId,
                UserID = Util.SsoUid,
                Version = _version
            };
            //如果是断网状态则存到本地，加标识“a”
            if (!Util.IsOnline)
            {
                int count = StudentQuestionLogic.GetPaperScoreCount(Util.SsoUid);
                stuQuesRecord.PaperScoreID = count + "a";
                new Framework.Local.StudentQuestionData().AddPaperSocres(stuQuesRecord);
            }
            else
            {
                //if (contestTimes > 0)
                //{
                //    int subCount = 0;
                //    StudentQuestionLogic.GetPaperSubmitCnt(_paperId, ref subCount);
                //    //如果提交的次数已超过限制的次数则不需要提交给线上
                //    if (subCount >= contestTimes)
                //    {
                //        CustomMessageBox.Show("您已超过答题次数限制");
                //        return;
                //    }
                //}
                stuQuesRecord.PaperScoreID = "0";
                StudentQuestionLogic.BatchPaperScoreSubmit(stuQuesRecord);
            }
            PageStatus = PageStatus.Done;
            IsBtnShow = false;
            // 获取并显示做题结果
            GetPaperResult();
            IsShowResult = true;
            // 保存做题记录
            SaveResult();
            // 跳转至第一题
            CurrentItem = null;
            _index = 0;
            CurrentItem = Items[_index];
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
            if (IsBtnShow) Log.RecordData("TikuPrev", _paperId, CurrentItem.Question.QuestionId);
        }
        /// <summary>
        /// 下一题
        /// </summary>
        private void Next()
        {
            if (_index == Items.Count - 1)
            {
                if (IsShowResult || !CheckTested())
                {
                    CustomMessageBox.Show("已经是最后一题");
                    return;
                }

                if (CustomMessageBox.Show("已经是最后一题，是否交卷评分？", "提示", MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                {
                    HandIn();
                }
                return;
            }
            CurrentItem = Items[++_index];
            if (IsBtnShow) Log.RecordData("TikuNext", _paperId, CurrentItem.Question.QuestionId);
        }

        /// <summary>
        /// 保存做题数据
        /// </summary>
        private void SaveResult()
        {
            // 做题数据保存到数据库
            var resultList = new List<ViewStudentQuestionResult>();

            Items.ToList().ForEach(i => resultList.Add(new ViewStudentQuestionResult
            {
                IsFav = i.IsFav,
                IsRight = i.IsRight,
                PaperViewId = _paperId,
                QuestionId = i.Question.QuestionId,
                UserAnswer = i.UserAnswer
            }));

            StudentQuestionLogic.SaveUserResult(resultList);
        }
        /// <summary>
        /// 保存收藏
        /// </summary>
        private void SaveFav()
        {
            // 收藏数据保存到数据库
            var resultList = new List<ViewStudentQuestionResult>();

            Items.ToList().ForEach(i => resultList.Add(new ViewStudentQuestionResult
            {
                PaperViewId = _paperId,
                QuestionId = i.Question.QuestionId,
                IsFav = i.IsFav,
                IsRight = 0// 已保存，设置为未做
            }));

            StudentQuestionLogic.SaveUserResult(resultList);
        }
        /// <summary>
        /// 显示统计的做题结果
        /// </summary>
        private void GetPaperResult()
        {
            var errorCount = Items.Count(q => q.IsRight == 2);
            var rightCount = Items.Count(q => q.IsRight == 1);
            var score = Items.Where(q => q.IsRight == 1).Sum(q => q.Question.Score);
            var rate = (rightCount * 100.0 / Items.Count).ToString("F2") + "%";
            PaperResult = new PaperResultViewModel
            {
                RightCount = rightCount,
                ErrorCount = errorCount,
                CorrectRate = rate,
                UserScore = score,
                TotalTime = UsedTime,
                TotalCount = _totalCount
            };

            Items.ToList().ForEach(q =>
            {
                if (q.IsRight == 0) return;

                q.StatusImage = q.IsRight == 1
                    ? "/Images/Paper/exam_right.png"
                    : "/Images/Paper/exam_error.png";
            });
        }

        /// <summary>
        /// 检查是否已经做题
        /// </summary>
        /// <returns></returns>
        private bool CheckTested()
        {
            return Items.Any(q => !string.IsNullOrEmpty(q.UserAnswer));
        }

        /// <summary>
        /// 初始化计时器
        /// </summary>
        private void InitTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _elapsedTime = new TimeSpan();
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", _elapsedTime.Hours, _elapsedTime.Minutes,
                                     _elapsedTime.Seconds);
            UsedTime = time;
        }

        private void DisposeTimer()
        {
            _timer.Tick -= _timer_Tick;
            _timer = null;
        }

        private void StartTimer()
        {
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
        }

        private void BindData(ViewStudentPaper paper, int type)
        {
            _version = paper.Version;
            PageTitle = paper.PaperViewName;

            var paperViewId = paper.PaperViewId;

            Action act = () =>
            {
                switch (type)
                {
                    case 1:
                        _questionList = _questionList.Where(q => !q.IsDone).ToList();
                        break;
                    case 2:
                        _questionList = _questionList.Where(q => q.IsWrong).ToList();
                        break;
                    case 3:
                        _questionList = _questionList.Where(q => q.IsFav).ToList();
                        break;
                }

                var list = new List<QuestionItemViewModel>();
                _questionList.ForEach(model => list.Add(new QuestionItemViewModel(_paperId, model)));
                BindBtnData(list);
                Items = new ObservableCollection<QuestionItemViewModel>(list);
                TotalCount = Items.Count;// 总题数
                if (Items.Any())
                {
                    CurrentItem = Items[_index];
                }
            };

            _questionList = StudentQuestionLogic.GetPaperDetail(paperViewId).ToList();
            if (!_questionList.Any())
            {
                if (!Util.IsOnline) return;
                StudentQuestionLogic.GetPaperDetailFromRemote(paperViewId, re =>
                {
                    _questionList = StudentQuestionLogic.GetPaperDetail(paperViewId).ToList();
                    Dispatcher.Invoke(act);
                });
            }
            else
            {
                act();
            }
        }

        private void BindBtnData(IEnumerable<QuestionItemViewModel> items)
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

        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Back)
                return;

            dynamic parms = e.ExtraData;
            var paper = parms.Param1;
            var type = parms.Param2;
            var center = parms.Param3;

            _paperId = paper.PaperViewId;
            contestTimes = int.Parse(paper.ContestTimes);
            InitTimer();
            StartTimer();
            BindData(paper, type);
            Center = center;
            Log.RecordData("TikuStart", center.SiteCourseId, center.CenterId, _paperId, type);
        }

        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                SaveFav();// 保存收藏记录
                NavigationService.RemoveBackEntry();
                Cleanup();
                Log.RecordData("TikuLeave", _paperId);
            }
        }

        public override void Cleanup()
        {
            _index = 0;
            _currentItem = null;

            IsBtnShow = true;
            UsedTime = "00:00:00";
            PageStatus = PageStatus.Testing;
            Items = null;
            IsShowResult = false;
            TestedCount = 0;
            PaperResult = null;
            ErrorRecordList = null;
            DisposeTimer();
        }
    }
    /// <summary>
    /// 做题状态
    /// </summary>
    public enum PageStatus
    {
        /// <summary>
        /// 正在做题
        /// </summary>
        Testing,
        /// <summary>
        /// 已做完
        /// </summary>
        Done
    }
}