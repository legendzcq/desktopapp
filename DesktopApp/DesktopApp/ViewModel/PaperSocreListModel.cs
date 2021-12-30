using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using Framework.Model;
using Framework.NewModel;
using Framework.Remote;
using Framework.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DesktopApp.ViewModel
{
    public class PaperSocreListModel : NavigationViewModelBase
    {
        private ViewStudentPaper VsPaper;
        public PaperSocreListModel()
        {
            InitCmd();
        }

        #region 属性

        private ListCollectionView _paperScores;
        private string _pageTitle;

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }
        public ListCollectionView PaperScores
        {
            get { return _paperScores; }
            set
            {
                _paperScores = value;
                RaisePropertyChanged(() => PaperScores);
            }
        }
        #endregion
        #region 命令
        public ICommand NavPaperOperCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        #endregion

        #region 方法

        private void InitCmd()
        {
            NavPaperOperCommand = new RelayCommand<StudentPaperScore>(paper => NavPaperSocre(paper));
            RefreshCommand = new RelayCommand(() => RefreshData());
        }

        private void BindData()
        {
            var list = new StudentQuestionRemote().GetPaperSocres(Util.SsoUid, VsPaper.PaperViewId);
            PaperScores = new ListCollectionView(list);
            if(PaperScores==null||PaperScores.Count==0)
            {
                CustomMessageBox.Show("暂无做题记录");
            }
        }

        private void NavPaperSocre(StudentPaperScore paper)
        {
            NavigationService.Navigate(new Uri("/Pages/PaperSocreDetail.xaml", UriKind.Relative), new { Param1 = paper, Param2 = VsPaper});
        }
        private void RefreshData()
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }
            Action act = () =>
            {
                BindData();
                HideLoading();
            };
            StudentQuestionLogic.GetPaperSocresFromWeb(() => Dispatcher.Invoke(act));
            ShowLoading();
        }
        #endregion


        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Forward || mode == NavigationMode.Back)
                return;
            if (mode == NavigationMode.New)
            {
                Cleanup();

                VsPaper = e.ExtraData as ViewStudentPaper;
                if (VsPaper == null)
                    return;
                if (Util.IsOnline)
                {
                    RefreshData();
                }
                else
                {
                    BindData();
                }
            }
            PageTitle = VsPaper.PaperViewName;
           
        }
        public override void Cleanup()
        {
            PageTitle = string.Empty;
            PaperScores = null;
        }

    }
}
