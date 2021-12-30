using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
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
    /// 考试中心列表视图模型
    /// </summary>
    public class CenterListViewModel : NavigationViewModelBase
    {
        private List<CenterDetailViewModel> _centerDetailViewModels;

        public CenterListViewModel()
        {
            InitCmd();
        }

        #region 命令

        /// <summary>
        /// 导航至试卷列表事件命令
        /// </summary>
        public ICommand NavPaperCommand { get; private set; }
        /// <summary>
        /// 刷新全部考试中心命令
        /// </summary>
        public ICommand RefreshListCommand { get; private set; }
        /// <summary>
        /// 刷新单个考试中心命令
        /// </summary>
        public ICommand RefreshCommand { get; private set; }

        #endregion

        #region 属性

        private ListCollectionView _courseList;
        public ListCollectionView Items
        {
            get { return _courseList; }
            set
            {
                _courseList = value;
                RaisePropertyChanged(() => Items);
            }
        }

        #endregion

        #region 方法

        private void InitCmd()
        {
            NavPaperCommand = new RelayCommand<CenterDetailViewModel>(item => NavigationService.Navigate(new Uri("/Pages/PaperListPage.xaml", UriKind.Relative), item.Center));

            RefreshCommand = new RelayCommand<int>(centerId =>
            {
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }

                var item = _centerDetailViewModels.FirstOrDefault(c => c.Center.CenterId == centerId);
                if (item != null)
                {
                    item.IsLoading = true;
                    item.IsShowBtn = false;
                    StudentQuestionLogic.GetCenterInfo(centerId, (time) =>
                    {
                        item.IsLoading = false;
                        item.IsShowBtn = true;

                        // 打上更新操作的时间标签
                        item.UpdateTime = time.ToString();
                    });
                }
            });

            RefreshListCommand = new RelayCommand(RefreshData);
        }

        private void RefreshData()
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }

            ShowLoading();

            Action nextAction = () => StudentQuestionLogic.GetQuestionBaseInfoFromRemote(re =>
            {
                if (!re.State)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CustomMessageBox.Show(re.Message);
                        HideLoading();
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        BindData(StudentQuestionLogic.GetCenterList());
                        HideLoading();
                    }));
                }
            });
            if (string.IsNullOrEmpty(Util.SessionId))
            {
                StudentLogic.ExecuteLogin(Util.UserName, Util.Password, re =>
                {
                    if (re.State)
                    {
                        Dispatcher.Invoke(nextAction);
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            CustomMessageBox.Show(re.Message);
                            HideLoading(); // 隐藏Loading
                        }));
                    }
                });
            }
            else
            {
                nextAction();
            }
        }

        private void BindData(List<ViewStudentCenter> list)
        {
            _centerDetailViewModels = new List<CenterDetailViewModel>(list.Count);
            list.ForEach(c => _centerDetailViewModels.Add(new CenterDetailViewModel(c)));

            Items = new ListCollectionView(_centerDetailViewModels);

            if (Items.GroupDescriptions != null)
                Items.GroupDescriptions.Add(new PropertyGroupDescription("SiteCourseName"));
        }

        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {

            var list = StudentQuestionLogic.GetCenterList();
            if (list.Count == 0 && Util.IsOnline)
            {
                RefreshData();
                return;
            }
            //BindData(list);
            ShowLoading();
            SystemInfo.StartBackGroundThread("加载题库数据", () => Dispatcher.Invoke(new Action(() =>
            {
                BindData(list);
                HideLoading();

            })));
        }
    }
}
