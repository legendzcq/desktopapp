using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 试卷列表视图模型
    /// </summary>
    public class PaperListViewModel : NavigationViewModelBase
    {
        private ViewStudentCenter _center;

        public PaperListViewModel()
        {
            InitCmd();
        }

        #region 属性

        private List<ViewStudentPaper> _modelList;
        private ObservableCollection<ViewStudentPaper> _papers;
        private string _pageTitle;
        private ViewStudentPaper _selectedItem;

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public ViewStudentPaper SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public ObservableCollection<ViewStudentPaper> Papers
        {
            get { return _papers; }
            set
            {
                _papers = value;
                RaisePropertyChanged(() => Papers);
            }
        }

        #endregion

        #region 命令

        public ICommand NavPaperCommand { get; private set; }
        public ICommand NavPaperDoCommand { get; private set; }
        public ICommand NavPaperFavCommand { get; private set; }
        public ICommand NavPaperWrongCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand NavPaperResultCommand { get; private set; }

        #endregion

        #region 方法

        private void InitCmd()
        {
            NavPaperCommand = new RelayCommand<ViewStudentPaper>(paper => NavPaper(paper, 0));
            NavPaperDoCommand = new RelayCommand<ViewStudentPaper>(paper => NavPaper(paper, 1));
            NavPaperWrongCommand = new RelayCommand<ViewStudentPaper>(paper => NavPaper(paper, 2));
            NavPaperFavCommand = new RelayCommand<ViewStudentPaper>(paper => NavPaper(paper, 3));
            NavPaperResultCommand = new RelayCommand<ViewStudentPaper>(paper => NavPaperRecored(paper));

            RefreshCommand = new RelayCommand(() =>
            {
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }

                Action act = () =>
                {
                    _modelList = StudentQuestionLogic.GetCenterPaperListFromLocal(_center.CenterId);
                    BindData();
                    HideLoading();
                };
                StudentQuestionLogic.GetCenterInfo(_center.CenterId, (x) => Dispatcher.Invoke(act));
                ShowLoading();
            });
        }

        private void BindData()
        {
            Papers = new ObservableCollection<ViewStudentPaper>(_modelList);
        }

        private void NavPaper(ViewStudentPaper paper, int type)
        {
            if (!Util.IsOnline && !StudentQuestionLogic.CheckPaperDetailExists(paper.PaperViewId))
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }
           int contestTimes = int.Parse(paper.ContestTimes);
           if (contestTimes > 0)
            {
                int subCount = 0;
                StudentQuestionLogic.GetPaperSubmitCnt(paper.PaperViewId, ref subCount);
                //如果提交的次数已超过限制的次数则不需要提交给线上
                if (subCount >= contestTimes)
                {
                    CustomMessageBox.Show("您已超过答题次数限制");
                    return;
                }
            }
            var isNav = false;
            switch (type)
            {
                case 0:
                    isNav = paper.AllCnt > 0;
                    break;
                case 1:
                    isNav = paper.DoCnt > 0;
                    break;
                case 2:
                    isNav = paper.WrongCnt > 0;
                    break;
                case 3:
                    isNav = paper.FavCnt > 0;
                    break;
            }

            if (isNav)
                NavigationService.Navigate(new Uri("/Pages/PaperPage.xaml", UriKind.Relative), new { Param1 = paper, Param2 = type, Param3 = _center });
        }
        private void NavPaperRecored(ViewStudentPaper paper)
        {
            NavigationService.Navigate(new Uri("/Pages/PaperScoreList.xaml", UriKind.Relative), paper);

        }
        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Forward)
                return;
            if (mode == NavigationMode.New)
            {
                Cleanup();

                _center = e.ExtraData as ViewStudentCenter;
                if (_center == null)
                    return;
            }

            PageTitle = "【" + _center.SiteCourseName + "】" + _center.CenterName;

            _modelList = StudentQuestionLogic.GetCenterPaperListFromLocal(_center.CenterId);
            if (_modelList.Count == 0 && Util.IsOnline)
            {
                RefreshCommand.Execute(null);
                return;
            }
            BindData();
           
        }

        public override void OnNavigateFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                _modelList = StudentQuestionLogic.GetCenterPaperListFromLocal(_center.CenterId);
                BindData();
            }
        }

        public override void Cleanup()
        {
            PageTitle = string.Empty;
            Papers = null;
        }
    }
}
