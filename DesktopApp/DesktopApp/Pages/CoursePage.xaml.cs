using System;
using System.Windows.Controls;

using DesktopApp.Controls;

using Framework.Remote;
using Framework.Utility;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        public CoursePage()
        {
            InitializeComponent();

#if CHINAACC|| JIANSHE || MED
            if (Util.IsShowAdv && Util.IsOnline)
            {
                SystemInfo.StartBackGroundThread("弹出广告", () =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var win = new AdvWindow();
                        win.ShowDialog();
                    }));
                });
            }
#endif
        }
        private void btnSelectCourse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Util.IsOnline)
            {
                var stu = new StudentRemote();
                stu.SelectCourse();
            }
            else
            {
                CustomMessageBox.Show("请链接网络");
            }
        }

        private void PurchaseCwareButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Loc.Course.ShowFreeCourse = false;
        }

        private void FreeCwareButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.Loc.Course.ShowFreeCourse = true;
        }

        public bool Test { get; set; }
    }


}
