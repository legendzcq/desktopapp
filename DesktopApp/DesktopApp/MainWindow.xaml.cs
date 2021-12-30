using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using DesktopApp.Pages;

using Framework.Local;
using Framework.NewModel;
using Framework.Push;
using Framework.Remote;
using Framework.Utility;

using GalaSoft.MvvmLight.Messaging;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Windows.Forms.Timer;

namespace DesktopApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
#if CHINAACC
        private const string DefaultMsg = "关课提示：下载的课程在考试结束后一周关闭，请抓紧时间学习！"; //（会计从业课程关闭时间以招生方案公布的内容为准！）";
#else
#if G12E
        private const string DefaultMsg = "关课提示：所有学员的听课权限于辅导期结束后关闭，请学员抓紧时间学习。";
#else
#if JIANSHE
         private const string DefaultMsg = "下载的课程在考试结束后一周关闭，请抓紧时间学习！（造价员及实务大讲堂以招生方案公布为准）";
#else
        private const string DefaultMsg = "关课提示：下载的课程在考试结束后一周关闭，请抓紧时间学习！";
#endif
#endif
#endif
#if CHINAACC
        private const string PhoneMsg = "24小时咨询电话\r\n010-82318888\r\n400-810-4588";
#endif
#if MED
        private const string PhoneMsg = "24小时咨询电话\r\n010-82311666\r\n400-650-1888";
#endif
#if JIANSHE
        private const string PhoneMsg = "24小时咨询电话\r\n010-82326699\r\n400-810-5999";
#endif
#if LAW
        private const string PhoneMsg = "24小时咨询电话\r\n010-82332233\r\n400-650-0111";
#endif
#if FOR68
        private const string PhoneMsg = "24小时咨询电话\r\n010-82332888\r\n400-810-0888";
#endif
#if G12E
        private const string PhoneMsg = "24小时咨询电话\r\n010-82330666\r\n400-650-0666";
#endif
#if CHINATAT
        private const string PhoneMsg = "24小时咨询电话\r\n010-82333888";
#endif
#if KAOYAN
        private const string PhoneMsg = "24小时咨询电话\r\n010-82335666\r\n400-810-2666";
#endif
#if ZIKAO
        private const string PhoneMsg = "24小时咨询电话\r\n010-82335555\r\n400-813-5555";
#endif
#if CHENGKAO
        private const string PhoneMsg = "24小时咨询电话\r\n010-82335555\r\n400-813-5555";
#endif

        private readonly List<MarqueeInfoListItem> _messageList = new List<MarqueeInfoListItem>();

        private const string CopyRightMsg = "严正重申：学习课程仅供注册学员个人独自使用，不得以任何方式向任何第三方公开课程内容，否则将停止学习权限并追究违规法律责任！";

        private WindowResizer _windowResizer;

        public MainWindow()
        {
            InitializeComponent();
            Title = Util.AppName;
            TxtPhone.Text = PhoneMsg;

            _messageList.Add(new MarqueeInfoListItem { PushContent = DefaultMsg });
            _messageList.Add(new MarqueeInfoListItem { PushContent = CopyRightMsg });

            Util.OnlineStateChanged += (s, e) => Dispatcher.BeginInvoke(new Action(SetNetworkState));

            // 服务器时间戳和服务器公钥
            RemoteBase.GetPublicKey();
            StudentRemote.GetServerTimeStamp13();
            var t = new Timer { Interval = 5 * 60 * 1000 };
            t.Tick += (s, e) =>
            {
                StudentRemote.GetServerTimeStamp13();
            };
            t.Enabled = true;

            // 客户端更新流程初始化
            Utils.Updater.initial();

            SetNetworkState();
            InitFrameNavigation();
            InitWindow();
            ChangSize();
            RegistMesseger();
            CheckUpdate();
            DoCheckSid();
            BeginPushClient();

            //this.BtnFaq.Visibility = System.Windows.Visibility.Collapsed;
#if CHINAACC|| JIANSHE || MED
            BtnFaq.Visibility = System.Windows.Visibility.Collapsed;
            tabService.Visibility = System.Windows.Visibility.Visible;
#else
            BtnFaq.Visibility = System.Windows.Visibility.Visible;
            tabService.Visibility = System.Windows.Visibility.Collapsed;
#endif
            UploadCwareRecord();

            //// 用于更新流程中的程序关闭功能
            //Messenger.Default.Register<string>(this, (msg) => MessengerStringHandler(msg));
        }

        //private void MessengerStringHandler(string msg)
        //{
        //    if (msg == "exit")
        //    {
        //        Dispatcher.Invoke(new Action(delegate
        //        {
        //            //Close();
        //            Application.Current.Shutdown();
        //        }
        //        ));
        //    }
        //}

        // 任务调度器
        private readonly System.Threading.Tasks.TaskScheduler _syncContext = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

        // 用于在主线程内更新客户端
        public void AutoUpdaterStart()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Utils.Updater.Start(true);
            },
                new CancellationTokenSource().Token,
                System.Threading.Tasks.TaskCreationOptions.None,
                _syncContext).Wait();
        }

        #region 窗体启动时要执行的操作
        /// <summary>
        /// 推送客户端
        /// </summary>
        internal PushClient PushClient;

        /// <summary>
        /// 推送的消息的队列
        /// </summary>
        private Queue<PushMessage> _messageQueue;

        /// <summary>
        /// 消息窗体
        /// </summary>
        private MessageWindow _messageWin;

        /// <summary>
        /// 启动推送
        /// </summary>
        private void BeginPushClient()
        {
            //获取推送服务器IP
            var serverIp = Network.GetPushServerIp();
            _messageQueue = new Queue<PushMessage>();
            //初始化推送
            PushClient = new PushClient
            {
                ServerIp = serverIp,
                ServerPort = 7000,
                Course = new string[0]
            };
            PushClient.OnPushMessage += m =>
            {
                var ret = new StudentData().AddMessage(m);
                if (ret)
                {
                    _messageQueue.Enqueue(m);
                }
            };
            //循环处理推送消息
            var timer = new Timer { Interval = 10000 };
            long cnt = 0;
            timer.Tick += (s, e) =>
            {
                cnt++;
                if (cnt == 2)
                {
                    timer.Interval = 30000;
                    //推送线程开始工作
                    if (Util.IsOnline) PushClient.StartWork();
                    return;
                }
                if (cnt == 3)
                {
                    PushClient.SendQuery(Util.GetNow().AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (_messageWin != null)
                {
                    _messageWin.Close();
                    _messageWin = null;
                }
                if (_messageQueue.Count > 0)
                {
                    PushMessage item = _messageQueue.Dequeue();
                    _messageWin = new MessageWindow
                    {
                        Message = item
                    };
                    _messageWin.Show();
                }
            };
            timer.Enabled = true;

            Util.OnlineStateChanged += (o, e) =>
            {
                if (Util.IsOnline)
                {
                    PushClient.StartWork();
                }
                else
                {
                    PushClient.StopWork();
                }
            };
        }

        /// <summary>
        /// 系统启动后5秒开始检查更新
        /// </summary>
        private void CheckUpdate()
        {
            SystemInfo.StartBackGroundThread("异步检查更新", () =>
            {
                Thread.Sleep(5000);
                if (!Util.IsOnline)
                    return;
                StudentLogic.GetMarqueeInfo(list =>
                {
                    if (list != null && list.Any())
                    {
                        Dispatcher.Invoke(new MethodInvoker(() =>
                        {
                            _messageList.Clear();
                            //_messageList.Add(new MarqueeInfoListItem() { PushContent = DefaultMsg });
                            //_messageList.Add(new MarqueeInfoListItem() { PushContent = CopyRightMsg });
                            _messageList.AddRange(list);
                        }));
                    }
                });
            });
        }

        /// <summary>
        /// 设置网络在线状态
        /// </summary>
        private void SetNetworkState()
        {
            TxtOnline.Text = Util.IsOnline ? "在线" : "离线";
            //BtnFaq.Visibility = Util.IsOnline ? Visibility.Visible : Visibility.Collapsed;
            ImgNetwork.Source = Util.IsOnline ?
                new BitmapImage(new Uri("/Images/network_connected.png", UriKind.Relative)) :
                new BitmapImage(new Uri("/Images/network_disconnected.png", UriKind.Relative));
        }

        /// <summary>
        /// 屏幕分辨率不够时调整窗体大小
        /// </summary>
        private void ChangSize()
        {
            var h = Screen.PrimaryScreen.WorkingArea.Height;
            if (h <= 700)
            {
                Height = 570;
            }
        }


        /// <summary>
        /// 初始化窗体
        /// </summary>
        [DebuggerStepThrough]
        private void InitWindow()
        {
            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
            _windowResizer = new WindowResizer(this);
            _windowResizer.AddResizers(Rleft, Rright, Rtop, Rbottom, LeftTop, LeftBottom, RightTop, RightBottom);
            _windowResizer.ConnectResizer();
            App.CurrentMainWindow = this;
            var tidx = 0;
            var t = new Timer { Interval = 10000 };
            t.Tick += (s, e) =>
            {
                var idx = tidx % _messageList.Count;
                TxtMsg.Text = _messageList[idx].PushContent;
                if (string.IsNullOrEmpty(_messageList[idx].FontColor))
                {
                    TxtMsg.Foreground = Application.Current.Resources["MainBgBrush"] as Brush;
                }
                else
                {
                    try
                    {
                        var colors = _messageList[idx].FontColor.Split(',');
                        if (colors.Length == 3)
                        {
                            var c = Color.FromRgb(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]));
                            var b = new SolidColorBrush(c);
                            TxtMsg.Foreground = b;
                        }
                        else if (colors.Length == 4)
                        {
                            var c = Color.FromArgb(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]), byte.Parse(colors[3]));
                            var b = new SolidColorBrush(c);
                            TxtMsg.Foreground = b;
                        }
                        else
                        {
                            TxtMsg.Foreground = Application.Current.Resources["MainBgBrush"] as Brush;
                        }
                    }
                    catch (Exception)
                    {
                        TxtMsg.Foreground = Application.Current.Resources["MainBgBrush"] as Brush;
                    }
                }
                tidx++;
                /*去掉离线时长显示  20150703*/
                //#if CHINAACC || JIANSHE
                //				//离线时长显示
                //				if (Util.IsOnline || Util.LastOffLineTime > 2 * 60 * 60)
                //				{
                //					TxtOfflineLeftTime.Visibility = Visibility.Collapsed;
                //				}
                //				else
                //				{
                //					TxtOfflineLeftTime.Visibility = Visibility.Visible;
                //					var ts = TimeSpan.FromSeconds(Util.LastOffLineTime);
                //					var th = ts.Hours > 0 ? ts.Hours + "小时" : string.Empty;
                //					var tm = ts.Minutes > 0 ? ts.Minutes + "分钟" : th == string.Empty ? "0分钟" : string.Empty;
                //					TxtOfflineLeftTime.Text = "离线使用时长剩余：" + th + tm;
                //				}
                //#endif
            };
            t.Enabled = true;
            TxtMsg.Text = DefaultMsg;
        }

        /// <summary>
        /// 检查是否最后一个客户端，踢人的动作
        /// </summary>
        private void DoCheckSid()
        {
            //在线踢人设置
            var timerCheckId = new Timer { Interval = 1000 };
            long checkCount = 0;
            timerCheckId.Tick += (s, e) =>
            {
                timerCheckId.Interval = 60000;
                if (Util.IsOnline)
                {
                    if (checkCount % 2 == 0)
                    {
                        StudentLogic.CheckUserCanUseClient(item =>
                        {
                            if (!item.State)
                            {
                                Dispatcher.Invoke(new MethodInvoker(() =>
                                {
                                    if (timerCheckId.Enabled)
                                    {
                                        timerCheckId.Enabled = false;
                                        Hide();
                                        Log.RecordData("StudentKickOut");
                                        MessageBox.Show(this, item.Message, @"下线通知");
                                        StudentLogic.ExecuteLogout();
                                        //Process.GetCurrentProcess().Kill();
                                    }
                                }));
                            }
                        }, checkCount % 30 == 0 /*60分钟检查一次账户是否被冻结*/);
                    }
                    checkCount++;
                }
            };
            timerCheckId.Start();
        }

        /// <summary>
        /// 初始化基于Frame和Page的导航框架
        /// </summary>
        private void InitFrameNavigation()
        {
            ContainerFrame.RegisteNavigationControl();
            PaperContainerFrame.RegisteNavigationControl();
            KcjyContainerFrame.RegisteNavigationControl();
            /**
             * 注释掉“移动课堂”代码
             * @author ChW
             * @date 2021-4-13
             */
            /*
            MobileContainerFrame.RegisteNavigationControl();
             */

            // 初始导航
            ContainerFrame.Source = new Uri("/Pages/CoursePage.xaml", UriKind.Relative);
        }

        /// <summary>
        /// 注册下载开始消息
        /// 用于打开下载管理标签
        /// </summary>
        private void RegistMesseger()
        {
            //Messenger.Default.Register<string>(this, TokenManager.DownStart, msg =>
            //	Dispatcher.Invoke(new Action(() => BtnDownload_OnClick(null, null))));
            Messenger.Default.Register<string>(this, TokenManager.ShowSetting, msg =>
                Dispatcher.Invoke(new Action(() => BtnSet_OnClick(null, null))));
            Messenger.Default.Register<bool>(this, TokenManager.ImportState, state => Dispatcher.Invoke(new Action(() =>
            {
                BtnImport.IsShowNotice = state;
            })));
            Messenger.Default.Register<bool>(this, TokenManager.DownloadState, state => Dispatcher.Invoke(new Action(() =>
            {
                BtnDownload.IsShowNotice = state;
            })));
            BtnDownload.IsShowNotice = App.Loc.DownloadCenter.IsDownLoading;
        }
        /// <summary>
        /// 界面启动是上传一次学习记录 dgh 2018.06.01
        /// </summary>
        private void UploadCwareRecord()
        {

            SystemInfo.StartBackGroundThread("上传学习记录", () =>
            {
                while (true)
                {
                    if (!Util.IsOnline) return;
                    StudentWareLogic.SaveCwareRecord();
                    //3分钟执行一次
                    Thread.Sleep(180000);
                }

            });
        }
        #endregion

        #region 主窗体最大化、最小化、全屏、拖动

        private void BtnMin_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void BtnMax_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;

        private void BtnNormal_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;

            BtnMax.Visibility = Visibility.Visible;
            BtnNormal.Visibility = Visibility.Collapsed;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e) => Close();

        private bool _isPressd;
        private void GridTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isPressd = true;
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                {
                    BtnNormal_OnClick(null, null);
                }
                else
                {
                    BtnMax_OnClick(null, null);
                }
            }
        }

        private void GridTop_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isPressd = false;

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
                    BtnNormal_OnClick(sender, e);

                    Point pos = Mouse.GetPosition(this);
                    Top = SystemParameters.WorkArea.Y - pos.Y;
                }
                DragMove();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                if (BtnMax.Visibility == Visibility.Collapsed
                    && BtnNormal.Visibility == Visibility.Visible)
                {
                    return;
                }

                _windowResizer.RemoveResizer();
                BtnMax.Visibility = Visibility.Collapsed;
                BtnNormal.Visibility = Visibility.Visible;
            }
            else
            {
                if (BtnMax.Visibility == Visibility.Visible
                    && BtnNormal.Visibility == Visibility.Collapsed)
                {
                    return;
                }

                _windowResizer.ConnectResizer();
                BtnMax.Visibility = Visibility.Visible;
                BtnNormal.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region 导航

        private void MainBackOnClick(object sender, RoutedEventArgs e)
        {
            if (ContainerFrame.CanGoBack)
                ContainerFrame.NavigationService.GoBack();
        }

        private void MainForwardOnClick(object sender, RoutedEventArgs e)
        {
            if (ContainerFrame.CanGoForward)
                ContainerFrame.NavigationService.GoForward();
        }

        private void PaperBackOnClick(object sender, RoutedEventArgs e)
        {
            if (PaperContainerFrame.CanGoBack)
                PaperContainerFrame.NavigationService.GoBack();
        }

        private void PaperForwardOnClick(object sender, RoutedEventArgs e)
        {
            if (PaperContainerFrame.CanGoForward)
                PaperContainerFrame.NavigationService.GoForward();
        }

        private void KcjyBackOnClick(object sender, RoutedEventArgs e)
        {
            if (KcjyContainerFrame.CanGoBack)
                KcjyContainerFrame.NavigationService.GoBack();
        }

        private void KcjyForwardOnClick(object sender, RoutedEventArgs e)
        {
            if (KcjyContainerFrame.CanGoForward)
                KcjyContainerFrame.NavigationService.GoForward();
        }
        private void MobileBackOnClick(object sender, RoutedEventArgs e)
        {
            /**
             * 注释掉“移动课堂”代码
             * @author ChW
             * @date 2021-4-13
             */
            /*
            if (MobileContainerFrame.CanGoBack)
                MobileContainerFrame.NavigationService.GoBack();
            */
        }

        private void MobileForwardOnClick(object sender, RoutedEventArgs e)
        {
            /**
             * 注释掉“移动课堂”代码
             * @author ChW
             * @date 2021-4-13
             */
            /*
            if (MobileContainerFrame.CanGoForward)
                MobileContainerFrame.NavigationService.GoForward();
            */
        }
        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TabMain.SelectedIndex)
            {
                case 0: // 我的课程TabItem
                    PanelCourseBtns.Visibility = Visibility.Visible;
                    PanelPaperBtns.Visibility = Visibility.Collapsed;
                    PanelKcjyBtns.Visibility = Visibility.Collapsed;
                    //PanelMobileBtns.Visibility = Visibility.Collapsed;
                    break;
                case 1: // 课程讲义
                    if (KcjyContainerFrame.Source == null)
                    {

                        KcjyContainerFrame.Source = new Uri("/Pages/KcjyDownload.xaml", UriKind.Relative);
                    }
                    PanelCourseBtns.Visibility = Visibility.Collapsed;
                    PanelPaperBtns.Visibility = Visibility.Collapsed;
                    PanelKcjyBtns.Visibility = Visibility.Visible;
                    //PanelMobileBtns.Visibility = Visibility.Collapsed;

                    KcjyContainerFrame.NavigationService.LoadCompleted += RefreshKcjyPageState;
                    break;
                case 2: // 我的题库TabItem
                    if (PaperContainerFrame.Source == null)
                    {
                        PaperContainerFrame.Source = new Uri("/Pages/CenterListPage.xaml", UriKind.Relative);
                    }
                    PanelCourseBtns.Visibility = Visibility.Collapsed;
                    PanelPaperBtns.Visibility = Visibility.Visible;
                    PanelKcjyBtns.Visibility = Visibility.Collapsed;
                    //PanelMobileBtns.Visibility = Visibility.Collapsed;
                    break;
                case 3: // 移动课堂
                /**
                * 注释掉“移动课堂”代码
                * @author ChW
                * @date 2021-4-13
                */
                /*
                    if (MobileContainerFrame.Source == null)
                    {
                        MobileContainerFrame.Source = new Uri("/Pages/MobileDownload.xaml", UriKind.Relative);
                    }
                    PanelCourseBtns.Visibility = Visibility.Collapsed;
                    PanelPaperBtns.Visibility = Visibility.Collapsed;
                    PanelKcjyBtns.Visibility = Visibility.Collapsed;
                    PanelMobileBtns.Visibility = Visibility.Visible;
                    break;
                */
#if CHINAACC || JIANSHE || MED
                case 4: // 我的服务
                    if (MyserviceContainerFrame.Source == null)
                    {
                        MyserviceContainerFrame.Source = new Uri("/Pages/MySevicePage.xaml", UriKind.Relative);
                    }
                    PanelCourseBtns.Visibility = Visibility.Collapsed;
                    PanelPaperBtns.Visibility = Visibility.Collapsed;
                    PanelKcjyBtns.Visibility = Visibility.Collapsed;
                    //PanelMobileBtns.Visibility = Visibility.Collapsed;
                    break;
#endif
                default://常规设置、课程导入 TabItem
                    PanelCourseBtns.Visibility = Visibility.Collapsed;
                    PanelPaperBtns.Visibility = Visibility.Collapsed;
                    PanelKcjyBtns.Visibility = Visibility.Collapsed;
                    //PanelMobileBtns.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void RefreshKcjyPageState(object sender, NavigationEventArgs e)
        {
            App.Loc.KcjyDetail.BindLocalData();
        }

        #endregion

        #region 常规设置、课程导入、下载管理 TabItem

        private void BtnMessage_Click(object sender, RoutedEventArgs e)
        {
            var win = new MessageListWindow { Owner = this };
            win.ShowDialog();
        }

        private void BtnSet_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new CustomWindow(new Uri("/Pages/SettingPage.xaml", UriKind.Relative), "系统设置") { Owner = this };
            win.ShowDialog();
        }

        private void BtnImport_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new CustomWindow(new Uri("/Pages/ImportPage.xaml", UriKind.Relative), "课程导入") { Owner = this };
            win.ShowDialog();
        }

        private void BtnDownload_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new CustomWindow(new Uri("/Pages/DownloadCenterPage.xaml", UriKind.Relative), "下载管理") { Owner = this, Width = 800 };
            win.ShowDialog();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            var win = new CustomWindow(new Uri("/Pages/AboutPage.xaml", UriKind.Relative), "关于") { Owner = this };
            win.ShowDialog();
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            if (App.Loc.DownloadCenter.IsDownLoading)
            {
                if (CustomMessageBox.Show("当前有正在下载的任务，是否关闭？", "提示", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (CustomMessageBox.Show("是否关闭下载课堂？", "提示", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnFaq_OnClick(object sender, RoutedEventArgs e)
        {
            if (Util.IsOnline)
            {
                if (string.IsNullOrEmpty(Util.SessionId))
                {
                    StudentLogic.ExecuteLogin(Util.UserName, Util.Password, item => Dispatcher.Invoke(new Action(() =>
                    {
                        if (item.State)
                        {
                            Process.Start(new StudentRemote().GetStudentFaqUrl());
                        }
                        else
                        {
                            CustomMessageBox.Show(item.Message);
                        }
                    })));
                }
                else
                {
                    Process.Start(new StudentRemote().GetStudentFaqUrl());
                }
            }
            else
            {
                CustomMessageBox.Show("请在联网状态下使用答疑板功能");
            }
        }
    }
}
