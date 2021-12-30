using DesktopApp.Controls;
using Framework.Model;
using Framework.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Pages
{
    /// <summary>
    /// 提问界面
    /// </summary>
    public partial class MyQuestionPage
    {
        readonly string _siteCourseId;
        readonly string _qNo;
        readonly string _lecFromStr;
        readonly string _type;
        public MyQuestionPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        ///  <param name="type">类型：0:课堂提问，1:题库提问</param>
        /// <param name="siteCourseId">对外课程ID</param>
        /// <param name="qNo">讲义号</param>
        /// <param name="lecFromStr">讲义片</param>
        public MyQuestionPage(string type, string siteCourseId, string qNo, string lecFromStr)
            : this()
        {
            txtTitle.Text = lecFromStr;
            _siteCourseId = siteCourseId;
            _qNo = qNo;
            _lecFromStr = lecFromStr;
            _type = type;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #region 窗体拖动
        private bool _isPressd;
        private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isPressd = true;
        }

        private void GridTop_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isPressd = false;
        }
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerStepThrough]
        private void GridTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isPressd)
            {
                if (WindowState == WindowState.Maximized)
                {
                    var pos = Mouse.GetPosition(this);
                    Top = SystemParameters.WorkArea.Y - pos.Y;
                }
                DragMove();
            }
        }
        #endregion
        private void btnQues_Click(object sender, RoutedEventArgs e)
        {
            var stuf = new StudentFaqRemote();
            var re = new ReturnItem();
            switch(_type)
            {
                case "0"://课堂提问
                    re = stuf.GetSaveFaqLecture(_siteCourseId, _qNo, txtTitle.Text, txtQuesDesc.Text, _lecFromStr);
                    break;
                case"1"://题库提问
                    re = stuf.GetSaveQuestionFaq(_siteCourseId, _qNo, txtTitle.Text, txtQuesDesc.Text);
                    //stuf.GetQueListByQuesID(_siteCourseID, _qNo);
                    break;
            }
            CustomMessageBox.Show(re.Message, "提示", MessageBoxButton.OK, 300, 180, false, "", this);
        }
    }
}
