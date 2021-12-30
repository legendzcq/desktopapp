using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopApp.Controls
{
    /// <summary>
    /// WinTimeSelect.xaml 的交互逻辑
    /// </summary>
    public partial class WinTimeSelect : Window
    {
        public double TimeValue { get; set; }
        public WinTimeSelect()
        {
            InitializeComponent();

            ImgClose.MouseLeftButtonDown += (s, e) =>
            {
                this.Close();
            };
        }
        public WinTimeSelect(string timeValue)
            : this()
        {
            if (!string.IsNullOrWhiteSpace(timeValue))
            {
                string[] str = timeValue.Split(':');
                if (str.Length == 3)
                {
                    txtHour.Text = str[0];
                    txtMin.Text = str[1];
                    txtSS.Text = str[2];
                }
                else if (str.Length == 2)
                {
                    txtHour.Text = "00";
                    txtMin.Text = str[0];
                    txtSS.Text = str[1];
                }

            }

        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtHour_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
              (e.Key >= Key.D0 && e.Key <= Key.D9) ||
              e.Key == Key.Back ||
              e.Key == Key.Left || e.Key == Key.Right)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHour.Text) && string.IsNullOrWhiteSpace(txtMin.Text) && string.IsNullOrWhiteSpace(txtSS.Text))
            {
                MessageBox.Show("数据不能为空");
                return;
            }
            int hour = 0, min = 0, second = 0;
            int.TryParse(txtHour.Text.Trim(), out hour);
            int.TryParse(txtMin.Text.Trim(), out min);
            int.TryParse(txtSS.Text.Trim(), out second);
            TimeValue = hour * 60 * 60 + min * 60 + second;
            this.DialogResult = true;
        }
    }
}
