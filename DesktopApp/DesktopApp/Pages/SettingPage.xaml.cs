using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using DesktopApp.Controls;
using DesktopApp.Logic;
using DesktopApp.Utils;

using Framework.Utility;

using MessageBox = System.Windows.MessageBox;

namespace DesktopApp.Pages
{
    /// <summary>
    /// 设置界面
    /// </summary>
    public partial class SettingPage
    {
        //音频设备存储
        private readonly Dictionary<string, int> deviceDictonary = new Dictionary<string, int>();
        public SettingPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                TxtUserName.Text = Util.UserName;
                TxtVersion.Text = Util.AppVersion;
                TxtPath.Text = Util.VideoPath;
                TxtDownloadPath.Text = Util.VideoDownSavePath;
                TxtJiangyiPath.Text = Util.JiangyiSavePath;
                RdHd.IsChecked = Util.DownType == 1;
                RdSd.IsChecked = Util.DownType == 2;
                RdDownDirect.IsChecked = !Util.IsUseMirrorDown;
                RdDownMirror.IsChecked = Util.IsUseMirrorDown;
                CbTaskCount.SelectedIndex = Util.DownloadTaskCount - 1;
                CbThreadCount.SelectedIndex = Util.DownloadThreadCount - 1;
                RdSystem.IsChecked = !Util.IsUseffDshow;
                RdFfdshow.IsChecked = Util.IsUseffDshow;
                ckShowSpeed.IsChecked = Util.IsNotUseSpeed;
                ckVmr.IsChecked = Util.IsUsevmr9;
                ckCheckFile.IsChecked = Util.IsCheckFile;
                Version winver = Environment.OSVersion.Version;
                CbQuestionFontSize.Text = Util.QuestionFontSize.ToString();
                CbKcjyFontSize.Text = Util.KcjyFontSize.ToString();
                CbAudioDevice.SelectedIndex = Util.AudioType;
                CbAdv.Text = Util.IsShowAdv ? "显示" : "不显示";
                if (winver.Major == 5 || (winver.Major == 6 && winver.Minor == 0))
                {
                    //GbUseFfDshow.Visibility = Visibility.Collapsed;
                    RdSystem.Visibility = Visibility.Collapsed;
                    RdFfdshow.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // GbUseFfDshow.Visibility = Visibility.Visible;
                    RdSystem.Visibility = Visibility.Visible;
                    RdFfdshow.Visibility = Visibility.Visible;
                }

#if CK100
                //财考网不显示选择效果
                gbDownType.Visibility = Visibility.Collapsed;
#endif
#if CK100
                GroupJySavePath.Visibility = Visibility.Collapsed;
#endif
#if CHINAACC || JIANSHE || MED
                gpAdv.Visibility = Visibility.Visible;
#else
                 gpAdv.Visibility = Visibility.Collapsed;
#endif
                var fonts = new InstalledFontCollection();
                foreach (System.Drawing.FontFamily f in fonts.Families)
                {
                    var item = new ComboBoxItem
                    {
                        Content = f.Name,
                        FontSize = 14,
                        FontFamily = new FontFamily(f.Name)
                    };
                    if (Util.BaseFont == f.Name)
                    {
                        item.IsSelected = true;
                    }
                    CbFonts.Items.Add(item);
                }
            };
        }
        private void SelectDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog { SelectedPath = TxtPath.Text };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TxtPath.Text = fbd.SelectedPath;
                try
                {
                    Util.VideoPath = TxtPath.Text;
                    //new StudentWareData().UpdateDownloadPath(TxtPath.Text);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void SelectDownloadDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog { SelectedPath = TxtDownloadPath.Text };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TxtDownloadPath.Text = fbd.SelectedPath;
                try
                {
                    Util.VideoDownSavePath = TxtDownloadPath.Text;
                    //new StudentWareData().UpdateDownloadPath(TxtDownloadPath.Text);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void SelectKcjyDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog { SelectedPath = TxtJiangyiPath.Text };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TxtJiangyiPath.Text = fbd.SelectedPath;
                try
                {
                    Util.JiangyiSavePath = TxtJiangyiPath.Text;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                return;
            }
            Updater.Start();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.Show("真的要退出吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                StudentLogic.ExecuteLogout();
            }
        }

        private void VideoType_Click(object sender, RoutedEventArgs e)
        {
            if (RdHd.IsChecked != null && RdHd.IsChecked.Value) Util.DownType = 1;
            if (RdSd.IsChecked != null && RdSd.IsChecked.Value) Util.DownType = 2;
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (RdDownMirror.IsChecked != null && RdDownMirror.IsChecked.Value)
            {
                Util.IsUseMirrorDown = true;
            }
            else
            {
                Util.IsUseMirrorDown = false;
            }
        }

        private void TxtPath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            const ModifierKeys key = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift;
            if ((e.KeyboardDevice.Modifiers | key) == key && e.Key == Key.F6)
            {
                var win = new AdvanceSetting { Owner = App.CurrentMainWindow };
                win.ShowDialog();
            }
        }

        private void CbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = CbFonts.SelectedValue as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var fontName = comboBoxItem.Content as string;
                var font = new FontFamily(fontName);
                LbFontTest.FontFamily = font;
            }
        }

        private void BtnFont_Click(object sender, RoutedEventArgs e)
        {
            var comboBoxItem = CbFonts.SelectedValue as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var fontName = comboBoxItem.Content as string;
                Util.BaseFont = fontName;
                MessageBox.Show("您更改了显示字体，系统即将重新启动");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void CbTaskCount_SelectionChanged(object sender, SelectionChangedEventArgs e) => Util.DownloadTaskCount = CbTaskCount.SelectedIndex + 1;

        private void CbThreadCount_SelectionChanged(object sender, SelectionChangedEventArgs e) => Util.DownloadThreadCount = CbThreadCount.SelectedIndex + 1;

        private void UserFfDshow_OnClick(object sender, RoutedEventArgs e)
        {
            Util.IsUseffDshow = RdFfdshow.IsChecked ?? false;
            if (Util.IsUseffDshow)
            {
                SystemInfo.FixReg();
            }
        }

        private void BtnProxySetting_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new NetworkSetting() { Owner = App.CurrentMainWindow };
            win.ShowDialog();
        }

        private void SelectDbRecover_OnClick(object sender, RoutedEventArgs e)
        {
            var f1 = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "数据库备份(*.bak)|*.bak"
            };
            if (f1.ShowDialog() == DialogResult.OK)
            {
                var ss = f1.FileName;
                var origRoot = SystemInfo.AppDataPath + "\\db";
                try
                {
                    var newFile = origRoot + "\\db.bak";
                    File.Copy(f1.FileName, newFile, true);//覆盖原文件
                    if (!File.Exists(origRoot + "\\存在备份数据文件.txt"))
                    {
                        //用该文件做标识：表示是要恢复数据的操作则在重启程序时自动恢复数据
                        File.Create(origRoot + "\\存在备份数据文件.txt");
                    }
                    CustomMessageBox.Show("数据恢复成功,将重新启动程序！");
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据恢复失败！", "提示");
                    Log.RecordLog(ex.Message);
                    throw;
                }
            }
        }

        private void SelectDbBackUp_OnClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                //设置文件类型
                Filter = "数据库备份文件（*.bak）|*.bak",
                //设置默认文件类型显示顺序
                FilterIndex = 1,
                //保存对话框是否记忆上次打开的目录
                RestoreDirectory = true,
                //如果文件已存在给与提示
                OverwritePrompt = true
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var localFilePath = sfd.FileName.ToString();
                var orignFile = SystemInfo.AppDataPath + "\\db\\db.db";
                try
                {
                    File.Copy(orignFile, localFilePath, true);//覆盖原文件
                    MessageBox.Show("数据备份成功！", "备份结果");
                }
                catch (Exception ex)
                {
                    Log.RecordLog(ex.Message);
                    MessageBox.Show("数据备份失败！", "备份结果");
                }

            }
        }

        private void ckShowSpeed_Unchecked(object sender, RoutedEventArgs e) => Util.IsNotUseSpeed = false;

        private void ckShowSpeed_Checked(object sender, RoutedEventArgs e) => Util.IsNotUseSpeed = true;

        private void ckVmr_Unchecked(object sender, RoutedEventArgs e) => Util.IsUsevmr9 = false;

        private void ckVmr_Checked(object sender, RoutedEventArgs e) => Util.IsUsevmr9 = true;

        private void ckCheckFile_Checked(object sender, RoutedEventArgs e) => Util.IsCheckFile = true;

        private void ckCheckFile_Unchecked(object sender, RoutedEventArgs e) => Util.IsCheckFile = false;

        private void CbKcjyFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = CbKcjyFontSize.SelectedValue as ComboBoxItem;
            if (comboBoxItem != null)
            {
                Util.KcjyFontSize = int.Parse(comboBoxItem.Content.ToString());
            }
        }

        private void CbQuestionFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = CbQuestionFontSize.SelectedValue as ComboBoxItem;
            if (comboBoxItem != null)
            {
                Util.QuestionFontSize = int.Parse(comboBoxItem.Content.ToString());
            }
        }

        private void BtnAudioDevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(CbAudioDevice.Text))
                {
                    Util.AudioType = CbAudioDevice.SelectedIndex;
                }
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private void CbAdv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = CbAdv.SelectedValue as ComboBoxItem;
            if (comboBoxItem != null)
            {
                Util.IsShowAdv = comboBoxItem.Content.ToString() == "显示" ? true : false;
            }
        }

        private void btnPlaySet_Click(object sender, RoutedEventArgs e)
        {
            var win = new OtherPlaySetting() { Owner = App.CurrentMainWindow };
            win.ShowDialog();
        }
    }
}
