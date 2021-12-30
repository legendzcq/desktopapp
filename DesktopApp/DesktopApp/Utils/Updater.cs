using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

using AutoUpdaterDotNET;
using DesktopApp.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopApp.Utils
{
    public static class Updater
    {
#if DEBUG && CHINAACC
        private const string AutoUpdaterUrl = "http://localhost:8080/AutoUpdate.xml";
#elif CHINAACC
        private const string AutoUpdaterUrl = "http://game.chinaacc.com/CourseClientUpdate/Chinaacc/AutoUpdate.xml";
#endif

        private static DispatcherTimer s_timer;

        public static void EnableAuto()
        {
            if (s_timer != null)
                return;

            s_timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
            s_timer.Tick += delegate
            {
                AutoUpdater.Start(AutoUpdaterUrl);
            };
            s_timer.Start();
        }

        public static void initial()
        {
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }

        public static void Start(bool Silent = false)
        {
            AutoCheck = Silent;
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.Start(AutoUpdaterUrl);
        }

        // 是否自动更新
        private static bool? AutoCheck { get;set; }

        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"新版本 {args.CurrentVersion} 可用。当前版本 {args.InstalledVersion} 需要更新，点击OK按钮进行更新。", @"Update Available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    }
                    else
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"新版本 {args.CurrentVersion} 可用。当前版本 {args.InstalledVersion} 。是否现在进行更新？", @"更新可用",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    }

                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                    {
                        try
                        {
                            AutoUpdater.ShowUpdateForm(args);
                            
                            //if (AutoUpdater.DownloadUpdate(args))
                            //{
                            //    //System.Windows.Forms.Application.Exit();
                            //    //Dispatcher.CurrentDispatcher.Invoke(new Action(delegate
                            //    //{
                            //    //    App.CurrentMainWindow.Close();
                            //    //}
                            //    //));

                            //    // 发送消息，关闭主界面，直接调用和Dispatcher调用都会提示线程问题
                            //    Messenger.Default.Send<string>("exit");
                            //}
                        }
                        catch (Exception exception)
                        {
                            System.Windows.Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    if (AutoCheck == true)
                    {
                        AutoCheck = null;
                        return; // 自动更新的情况不弹窗
                    }

                    System.Windows.Forms.MessageBox.Show($@"目前没有更新，当前版本 {args.InstalledVersion} 已经是最新版。", @"更新不可用",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (AutoCheck == true)
                {
                    AutoCheck = null;
                    return; // 自动更新的情况不弹窗
                }

                if (args.Error is System.Net.WebException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        @"访问服务器出现问题，请检查网络后重试。",
                        @"更新检查失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
