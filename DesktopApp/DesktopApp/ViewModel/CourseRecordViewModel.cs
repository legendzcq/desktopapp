using System;
//using System.Collections.Generic;
using System.IO;
//using System.Linq;
//using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using DesktopApp.Pages;

using Framework.Model;
using Framework.Utility;

using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
    public class CourseRecordViewModel : NavigationViewModelBase
    {
        public CourseRecordViewModel() => InitCmd();
        #region 属性
        private ListCollectionView _courseRecordList;

        public ListCollectionView CourseRecordList
        {
            get => _courseRecordList;
            set
            {
                _courseRecordList = value;
                RaisePropertyChanged(() => CourseRecordList);
            }
        }

        #endregion
        #region 命令
        /// <summary>
        /// 继续听课
        /// </summary>
        public ICommand ContinueLectureCommand { get; set; }
        #endregion

        #region 方法

        private void InitCmd() => ContinueLectureCommand = new RelayCommand<ViewCourseRecord>(item => ContinueLecture(item));

        private void BindData()
        {
            System.Collections.Generic.List<ViewCourseRecord> list = StudentWareLogic.GetCourseRecord();
            foreach (ViewCourseRecord item in list)
            {
                System.Collections.Generic.IEnumerable<CourseRecordOtherInfo> otherInfo = StudentWareLogic.GetCourseRecordByCwareId(item.CwareId);
                try
                {
                    foreach (CourseRecordOtherInfo detail in otherInfo)
                    {
                        var time = Convert.ToDateTime(detail.VideoLength).TimeOfDay.TotalSeconds;
                        item.TotalLength += time;
                        if (detail.SSOUID == Util.SsoUid & detail.MaxLastPosition != null)
                        {
                            item.FinishVideoLength += (double)detail.MaxLastPosition;
                        }
                    }
                    item.FinishPersent = Math.Ceiling(item.FinishVideoLength / item.TotalLength * 100).ToString() + "%";
                    //这个用于进度条显示，设置进度条最大值100来算，否则最大值太大的话百分比太小的话会导致进度显示不明显
                    item.FinishVideoLength = Math.Ceiling(item.FinishVideoLength / item.TotalLength * 100);
                }
                catch
                {

                }

            }
            CourseRecordList = new ListCollectionView(list);
            if (CourseRecordList.GroupDescriptions != null)
                CourseRecordList.GroupDescriptions.Add(new PropertyGroupDescription("CourseName"));


        }
        private void ContinueLecture(ViewCourseRecord item)
        {
            if (!File.Exists(item.LocalFile))
            {
                if (CustomMessageBox.Show("【" + item.VideoName + "】的视频文件可能被删除,是否将该记录清除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (StudentWareLogic.DeleteStudentVideoRecord(item.CwareId, item.VideoId))
                    {
                        CourseRecordList.Remove(item);
                    }
                }
                return;
            }
            ViewStudentCourseWare course = StudentWareLogic.GetStudentSubjectCourseWareItem(item.EduSubjectId, item.CwareId);
            ViewStudentWareDetail detailCourse = StudentWareLogic.GetViewStudentCwareDetailItem(item.CwareId, item.VideoId);
            var pageTitle = string.IsNullOrEmpty(course.CourseWareName) ? string.Format("{0} {1}({2})", course.CourseName, course.CWareClassName, course.CTeacherName) : course.CourseWareName;
            Window playWin = null;
            if (detailCourse.VideoType == 2)
            {
                playWin = new SFPlayWindow(pageTitle, detailCourse, course)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
            }
            else
            {
                playWin = new PlayWindow1(pageTitle, detailCourse, course)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
            }

            App.CurrentMainWindow.ShowInTaskbar = false;
            App.CurrentMainWindow.Hide();

            playWin.ShowDialog();
            BindData();
            App.CurrentMainWindow.ShowInTaskbar = true;
            App.CurrentMainWindow.Show();
        }
        #endregion

        public override void OnNavigateTo(NavigationEventArgs e, NavigationMode mode)
        {
            if (mode == NavigationMode.Forward || mode == NavigationMode.Back)
                return;
            if (mode == NavigationMode.New)
            {
                BindData();
            }
        }
    }
}
