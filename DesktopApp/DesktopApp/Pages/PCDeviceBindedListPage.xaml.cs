using DesktopApp.Controls;
using Framework.Remote;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
using static DesktopApp.Pages.PCDeviceConstraint;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PCDeviceBindedListPage.xaml 的交互逻辑
    /// </summary>
    public partial class PCDeviceBindedListPage : Page
    {
        public PCDeviceBindedListPage()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var children = FindVisualChildren<RadioButton>(DeviceListBox);

            foreach (var item in children)
            {
                if ((bool)item.IsChecked)
                {
                    App.Loc.DeviceListVM.selectedMid = item.Tag.ToString();
                    App.Loc.DeviceListVM.selectedMname = item.Content.ToString();

                    // 切换到PCDeviceVerificationPage
                    Messenger.Default.Send<NavigateTarget>(NavigateTarget.VERIFICATIONPAGE);
                    return;
                }
            }

            // 没有选中任何设备
            CustomMessageBox.Show("请选择要解绑的设备！");
            e.Handled = true;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    public class SelectedMachineMessage
    {
        public SelectedMachineMessage(string mid, string mname)
        {
            this.mid = mid;
            this.mname = mname;
        }
        // 机器key
        public string mid { get; set; }
        // 机器name
        public string mname { get; set; }
    }
}
