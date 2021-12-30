using Framework.Utility;
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
using System.Windows.Shapes;

namespace DesktopApp.Pages
{
    /// <summary>
    /// OtherPlaySetting.xaml 的交互逻辑
    /// </summary>
    public partial class OtherPlaySetting : Window
    {
        public OtherPlaySetting()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CbAutoTest.Text = Util.IsAutoShowPoint ? "启用" : "不启用";
                CbAutoPlay.Text = Util.IsAutoPlay ? "启用" : "不启用";

            };
        }


        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CbAutoTest.Text))
            {
                Util.IsAutoShowPoint = CbAutoTest.Text == "启用" ? true : false;
            }
            if (!string.IsNullOrWhiteSpace(CbAutoPlay.Text))
            {
                Util.IsAutoPlay = CbAutoPlay.Text == "启用" ? true : false;
            }
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


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

    }
}
