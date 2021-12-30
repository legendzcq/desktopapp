using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;

using Framework.Model;
#if CHINAACC
#endif
using Framework.Utility;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using Framework.Remote;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 课程列表视图模型
    /// </summary>
    public class CourseViewModel : NavigationViewModelBase
    {
        public CourseViewModel()
        {
            InitCmd();
            // 从课程导入接收刷新消息
            Messenger.Default.Register<string>(this, TokenManager.RefreshList, s =>
                Dispatcher.Invoke(new Action(() => BindData(StudentWareLogic.GetStudentCourseWareList()))));
        }

        private bool ShowFreeCourseFilter(object obj)
        {
            ViewStudentCourseWare item = obj as ViewStudentCourseWare;
            
            if (item == null)
                return false;

            return ShowFreeCourse == (item.IsFree == 1);
        }

        #region 命令

        /// <summary>
        /// 导航章节事件命令
        /// </summary>
        public ICommand NavChapterCommand { get; private set; }
        /// <summary>
        /// 刷新所有课程命令
        /// </summary>
        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// 浏览讲义下载命令
        /// </summary>
        public ICommand NavKcjyDownLoadCommand { get; private set; }

        /// <summary>
        /// 刷新本地数据命令
        /// </summary>
        public ICommand RefreshLocalCommand { get; set; }

        public ICommand CourseSettingCommand { get; set; }
        /// <summary>
        /// 听课记录 dgh 2017.03.20
        /// </summary>
        public ICommand CourseRecordCommand { get; set; }

        #endregion

        #region 属性

        private ListCollectionView _courseList;
        private bool _isShowNoData;

        public ListCollectionView CourseList
        {
            get => _courseList;
            set
            {
                _courseList = value;
                RaisePropertyChanged(() => CourseList);

                // 暂时隐藏课程分类功能
                //CourseList.Filter = ShowFreeCourseFilter;
            }
        }

        public bool IsShowNoData
        {
            get => _isShowNoData;
            set
            {
                _isShowNoData = value;
                RaisePropertyChanged(() => IsShowNoData);
            }
        }

        private bool _showFreeCourse;
        public bool ShowFreeCourse
        {
            get { return _showFreeCourse; }
            set
            {
                _showFreeCourse = value;
                RaisePropertyChanged(() => ShowFreeCourse);
                CourseList.Refresh();
            }
        }

        #endregion

        #region 方法

        private void InitCmd()
        {
            NavChapterCommand = new RelayCommand<ViewStudentCourseWare>(item =>
            {
                if (item.IsOpen)
                {
                    NavigationService.Navigate(new Uri("/Pages/ChapterPage.xaml", UriKind.Relative), item);
                }
                else
                {
                    if (!Util.IsOnline)
                    {
                        CustomMessageBox.Show(item.CanDownload
                            ? string.Format("《{0}-{1}》 未开通!", item.CourseName, item.CWareClassName, " 未开通")
                            : string.Format("《{0}-{1}》下载权限暂未开放。\r\n提示：开通课程满七天后自动开放下载权限，如果您开通课程已经满七天，请点击“更新列表”获取权限！",
                                item.CourseName, item.CWareClassName));
                    }
                    else
                    {
                        if (item.CanDownload)
                        {
                            CustomMessageBox.Show(string.Format("《{0}-{1}》 未开通!", item.CourseName, item.CWareClassName, " 未开通"));
                        }
                        else
                        {
                            /*在联网的情况下：如果学员听课权限开通未满七天，但又想提前开通下载权限，学员点击如下“确定”按钮后，技术需要实现为学员提前开通下载权限的功能，如果学员不点击“确定”，则不开通下载权限。从而最终实现系统自动处理此类用户需求，不再需要客服参与的目的！ dgh 2015.06.23*/
                            if (CustomMessageBox.Show(string.Format("《{0}-{1}》下载权限暂未开放。\r\n提示：开通课程满七天后自动开放下载权限，如果您开通课程已经满七天，请点击“更新列表”获取权限！\r\n若申请提前开通下载权限，申请成功后，网校对所提前开通下载权限的课程不予以任何形式的退课或换课。",
                                item.CourseName, item.CWareClassName), "提示", System.Windows.MessageBoxButton.OKCancel, 500, 240, true, "本人了解并遵守上述规定，申请开通") == System.Windows.MessageBoxResult.OK)
                            {
                                //对于网络请求开启异步线程
                                SystemInfo.StartBackGroundThread("开通不允许下载的课程", () =>
                                {
                                    var stud = new StudentWareRemote();
                                    ReturnItem re = stud.GetOpenCourseDownloadMsg();
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        CustomMessageBox.Show(re.Message);
                                        if (re.State) //执行成功再刷新列表
                                        {
                                            RefreshData(); //刷新列表
                                        }
                                    }));
                                });
                            }
                        }
                    }
                }
            });

            NavKcjyDownLoadCommand = new RelayCommand<ViewStudentCourseWare>(item =>
            {
                if (item.IsOpen)
                {
                    NavigationService.Navigate(new Uri("/Pages/KcjyDetail.xaml", UriKind.Relative), item);
                }
                else
                {
                    CustomMessageBox.Show(item.CanDownload
                        ? string.Format("《{0}-{1}》 未开通!", item.CourseName, item.CWareClassName)
                        : string.Format("《{0}-{1}》下载权限暂未开放。\r\n提示：开通课程满七天后自动开放下载权限，如果您开通课程已经满七天，请点击“更新列表”获取权限！",
                            item.CourseName, item.CWareClassName));
                }
            });

            RefreshCommand = new RelayCommand(RefreshData);

            RefreshLocalCommand = new RelayCommand(() => BindData(StudentWareLogic.GetStudentCourseWareList()));

            CourseSettingCommand = new RelayCommand<string>(ShowSubjectSetting);
            #region 听课记录
            CourseRecordCommand = new RelayCommand<ViewStudentCourseWare>(item =>
            {
                List<ViewCourseRecord> list = StudentWareLogic.GetCourseRecord();
                if (list == null || list.Count == 0)
                {
                    CustomMessageBox.Show("暂无听课记录");
                    return;
                }
                NavigationService.Navigate(new Uri("/Pages/CourseRecord.xaml", UriKind.Relative));
            });
            #endregion
        }

        private void ShowSubjectSetting(string subjectName)
        {
            var win = new CustomWindow(new Uri("/Pages/CourseSettingPage.xaml", UriKind.Relative), "班次设置", subjectName) { Owner = App.CurrentMainWindow };
            if (win.ShowDialog() == true)
            {
                BindData(StudentWareLogic.GetStudentCourseWareList());
            }

        }

        private void RefreshData()
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }

            ShowLoading();

            Action nextAction = () => StudentWareLogic.GetStudentWareFromRemote(re =>
            {
                if (!re.State)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //CustomMessageBox.Show(re.Message);
                        BindData(StudentWareLogic.GetStudentCourseWareList());
                        HideLoading();
                    }));
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        List<ViewStudentCourseWare> lst = StudentWareLogic.GetStudentCourseWareList();
                        BindData(lst);
                        StudentWareLogic.GetWareDetailFromRemoteAll(lst.Where(x => x.CanDownload || x.IsOpen).Select(x => x.CwId));
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

        private void BindData(List<ViewStudentCourseWare> list)
        {
            IsShowNoData = !list.Any();
            CourseList = new ListCollectionView(list);
            //当获取到科目的时候需要更改推送包里的课程列表
            IEnumerable<string> edulist = list.Select(x => x.CourseEduId.ToString(CultureInfo.InvariantCulture)).Distinct();
            App.CurrentMainWindow.PushClient.Course = list.Select(x => x.CourseId).Distinct().Union(edulist).Where(x => !string.IsNullOrEmpty(x));
            if (CourseList.GroupDescriptions != null)
                CourseList.GroupDescriptions.Add(new PropertyGroupDescription("CourseName"));

        }

        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Back)
            {
                BindData(StudentWareLogic.GetStudentCourseWareList());
                return;
            }
            List<ViewStudentCourseWare> list = StudentWareLogic.GetStudentCourseWareList();
            if (list.Count == 0 && Util.IsOnline)
            {
                RefreshData();
                return;
            }
            BindData(list);
        }
    }
}
