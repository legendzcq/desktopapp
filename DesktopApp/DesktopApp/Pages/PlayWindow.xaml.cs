using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using DesktopApp.Logic;
using Framework.Model;
using Framework.NewModel;
using Framework.Player;
using Framework.Utility;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows.Navigation;
using DesktopApp.Controls;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow
    {

        /// <summary>
        /// 讲义内容文本
        /// </summary>
        private const string Html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">" + "\r\n" + @"<html xmlns=""http://www.w3.org/1999/xhtml"" >" + "\r\n" +
            @"<head>" + "\r\n" +
            @"    <title>讲义内容</title><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">" + "\r\n" +
            @"    <style type=""text/css"">" + "\r\n" +
            @"        * { margin: 0; padding: 0; }" + "\r\n" +
            @"        body { line-height: 150%; font-size: {$0}px; background-color:#fffee2;margin:4px; }" + "\r\n" +
            @"        a { text-decoration: none; color:#333; }" + "\r\n" +
            @"        .ant{ display:block; height:5px; overflow:hidden }" + "\r\n" +
            @"        .textnode{ margin-top:5px; padding:5px; border:1px solid #fffee2;}" + "\r\n" +
            @"        .select{ border:2px dashed #777;}" + "\r\n" +
            @"        .textnode.hover{background-color:#fffec9; border:1px solid gray;}" + "\r\n" +
            @"        .font14zd{color:red;}" + "\r\n" +
            @"    </style>" + "\r\n" +
            @"    <script type=""text/javascript"">" + "\r\n" +
            @"        var lastnode;" + "\r\n" +
            @"        function setPosition(node) {" + "\r\n" +
            @"            window.external.SetPos(node);" + "\r\n" +
            @"        }" + "\r\n" +
            @"        function gotoNode(node) {" + "\r\n" +
            @"            try{" + "\r\n" +
            @"                if(lastnode) document.getElementById(lastnode).className='textnode';" + "\r\n" +
            @"                location.href = ""#A"" + node;" + "\r\n" +
            @"                lastnode=node;" + "\r\n" +
            @"                document.getElementById(lastnode).className='textnode select';" + "\r\n" +
            @"            } catch(e){}" + "\r\n" +
            @"        }" + "\r\n" +
            @"        function SetFont(type){" + "\r\n" +
            @"            switch(type){" + "\r\n" +
            @"                case 1:" + "\r\n" +
            @"                    document.body.style.fontSize = '{$1}px';" + "\r\n" +
            @"                    break;" + "\r\n" +
            @"                case 2:" + "\r\n" +
            @"                    document.body.style.fontSize = '{$2}px';" + "\r\n" +
            @"                    break;" + "\r\n" +
            @"                case 3:" + "\r\n" +
            @"                    document.body.style.fontSize = '{$3}px';" + "\r\n" +
            @"                    break;" + "\r\n" +
            @"                case 4:" + "\r\n" +
            @"                    document.body.style.fontSize = '{$4}px';" + "\r\n" +
            @"                    break;" + "\r\n" +
            @"            }" + "\r\n" +
            @"        }" + "\r\n" +
            @"    </script>" + "\r\n" +
            @"</head>" + "\r\n" +
            @"<body>{$body}" + "\r\n" +
            @"</body>" + "" + "\r\n" +
            @"</html>";

        static int _lastrate = 10;
        private static double _lastpr = 0.0;

        private int _rate = 10;
        private int _seeTime = 0;

        /// <summary>
        /// 最后一个播放节点
        /// </summary>
        private string _lastNode = string.Empty;

        /// <summary>
        /// 讲义节点
        /// </summary>
        private readonly Dictionary<string, int> _nodeTime = new Dictionary<string, int>();

        /// <summary>
        /// 当前视频对象
        /// </summary>
        private ViewStudentWareDetail _videoItem;

        /// <summary>
        /// 当前状态
        /// </summary>
        private PageState _currentState = PageState.Play;

        private readonly ViewStudentCourseWare _course;

        /// <summary>
        /// 班次名称
        /// </summary>
        private readonly string _cwName;

        /// <summary>
        /// 是否显示进度条上的纽
        /// </summary>
        private bool _sliderShowValue = true;

        /// <summary>
        /// 弹出知识点列表
        /// </summary>
        private List<PointTestStartTimeItem> _pointTestStartTimeList;

        /// <summary>
        /// 正在切换视频的时候不能更改视频窗口，否则视频窗口会显示不出来。
        /// </summary>
        private bool _isInChange;

        /// <summary>
        /// 播放器
        /// </summary>
        private PlayerCore _ctlPlayerCore;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cwName"></param>
        /// <param name="videoItem"></param>
        /// <param name="course"></param>
        public PlayWindow(string cwName, ViewStudentWareDetail videoItem, ViewStudentCourseWare course)
        {
            InitializeComponent();
            //是否禁用声音加速功能：如果禁用则声音恢复正常播放
            if (Util.IsNotUseSpeed)
            {
                _lastrate = 10;
                _lastpr = 0.0;
                _rate = 10;
                btnSpeed.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnSpeed.Visibility = Visibility.Visible;
            }
            Log.RecordData("PlayVideo", course.CourseId, videoItem.CwareId, videoItem.VideoId, "HD");
            _course = course;
            Title = Util.AppName;
            Trace.WriteLine("InitPlayWindow");
            ChangSize();

            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
            PreviewKeyDown += Window_PreviewKeyDown;
            PreviewKeyUp += Window_PreviewKeyUp;
            KeyDown += (s, e) =>
            {
                // 全屏时按ESC退出全屏
                if (e.Key == Key.Escape && this.IsFullscreen())
                {
                    BtnFullScreen_Click(s, e);
                }
            };
            SizeChanged += Window_SizeChanged;

            _cwName = cwName;
            _videoItem = videoItem;
            _seeTime = 0;

            Loaded += (s, e) => PlayVideo();
            Closing += (s, e) =>
            {
                _ctlPlayerCore.Stop();
                StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, (int)SliderBar.Value, _seeTime);
                if (WinHost.Child != null) WinHost.Child.Dispose();

                //设置进程的优先级
                Trace.WriteLine("SetNormal");
                var thisProc = Process.GetCurrentProcess();
                thisProc.PriorityClass = ProcessPriorityClass.Normal;
                GC.Collect();
            };
            KeyUp += (s, e) =>
            {
                if (e.Key == Key.Space)
                {
                    SetPlayState();
                }
                else if (e.Key == Key.Left)
                {
                    var pos = SliderBar.Value - 60;
                    if (pos < 0) pos = 0.5;
                    Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, pos);
                    _ctlPlayerCore.SetPosition(pos);
                }
                else if (e.Key == Key.Right)
                {
                    var pos = SliderBar.Value + 60;
                    if (pos >= SliderBar.Maximum) pos = SliderBar.Maximum - 1;
                    Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, pos);
                    _ctlPlayerCore.SetPosition(pos);
                }
            };
            Trace.WriteLine("InitPlayWindowFinished");
        }

        /// <summary>
        /// 改变窗体大小
        /// </summary>
        private void ChangSize()
        {
            int h = Screen.PrimaryScreen.WorkingArea.Height;
            if (h <= 700)
            {
                Height = 570;
            }
        }

        /// <summary>
        /// 初始化播放器
        /// </summary>
        private void InitPlayer()
        {
            SliderVol.Minimum = SystemMultimediaController.MinValue;
            SliderVol.Maximum = SystemMultimediaController.MaxValue;
            SliderVol.Value = SystemMultimediaController.CurrentValue;

            if (WinHost.Child != null) WinHost.Child.Dispose();
            _ctlPlayerCore = new PlayerCore();
            _ctlPlayerCore.Playing += PlayerCore_Playing;
            _ctlPlayerCore.Disposed += ctlPlayerCore_Disposed;
            _ctlPlayerCore.DoubleClick += ctlPlayerCore_DoubleClick;
            _ctlPlayerCore.OnError += () =>
            {
                //检查用户显卡状态
                if (!SystemInfo.CheckVideoControllerDriver())
                {
                    //未安装驱动（双显卡机器，如果有一个未装，也会影响到播放）
                    MessageBox.Show(@"系统检测到您的显卡未安装驱动，请先安装显卡驱动" + Environment.NewLine + @"请用IE浏览器打开以下网址安装驱动： " + Environment.NewLine + @"http://up.mydrivers.com/", @"提示");
                    return;
                }
                if (MessageBox.Show(@"播放时发生错误，系统可以尝试修复错误，是否要修复错误？", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (!SystemInfo.FixReg())
                    {
                        MessageBox.Show(@"修复时发生错误，您可能需要重新安装本软件", @"提示");
                    }
                    else
                    {
                        MessageBox.Show(@"修复完成！", @"提示");
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
                        Process.GetCurrentProcess().Kill();
                    }
                }
            };
            WinHost.Child = _ctlPlayerCore;
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// 初始化讲义
        /// </summary>
        private void InitKcjy()
        {
            try
            {
                var webObj = new ObjectForWeb();
                webObj.WebClick += node =>
                {
                    if (_nodeTime.ContainsKey(node))
                    {
                        _ctlPlayerCore.SetPosition(_nodeTime[node]);
                        Log.RecordData("PlayVideoJyClick", _videoItem.CwareId, _videoItem.VideoId, node, _nodeTime[node]);
                    }
                };
                WebMain.ObjectForScripting = webObj;
                _nodeTime.Clear();
                var list = StudentWareLogic.GetStudentWareKcjyList(_videoItem.CwareId, _videoItem.VideoId);
                var sb = new StringBuilder();
                list.ForEach(x =>
                {
                    _nodeTime.Add(x.NodeId, x.VideoTime);
                    sb.AppendLine(@"<a class=""ant"" name=""A" + x.NodeId + @""">" + @"</a><div id=""" + x.NodeId +
                                  @""" class=""textnode""  onmouseout=""if(this.className=='textnode hover') this.className='textnode'"" onmouseover=""if(this.className=='textnode') this.className='textnode hover'"" ondblclick=""setPosition('" +
                                  x.NodeId + @"');"">");
                    sb.AppendLine(x.NodeText);
                    sb.AppendLine(@"</div>");
                });
                int smallFontSize = Util.KcjyFontSize - 2;
                int bigFontSize = Util.KcjyFontSize + 2;
                int biggerFontSize = Util.KcjyFontSize + 4;
                string html = Html.Replace("{$body}", sb.ToString()).Replace("{$0}", Util.KcjyFontSize.ToString()).Replace("{$1}", smallFontSize.ToString()).Replace("{$2}", Util.KcjyFontSize.ToString()).Replace("{$3}", bigFontSize.ToString()).Replace("{$4}", biggerFontSize.ToString());
                var fname = Guid.NewGuid() + ".htm";
                if (!Directory.Exists(SystemInfo.AppDataPath + "web"))
                {
                    Directory.CreateDirectory(SystemInfo.AppDataPath + "web");
                }
                File.WriteAllText(SystemInfo.AppDataPath + "web\\" + fname, html);
                WebMain.Navigate("http://127.0.0.1:" + Util.HttpPort + "/" + fname);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private bool _isInPoint = false;

        private PointTestStartTimeItem _currentTest = null;

        /// <summary>
        /// 播放进度事件
        /// </summary>
        /// <param name="currentPosition"></param>
        private void PlayerCore_Playing(double currentPosition)
        {
            _seeTime++;
            if (_sliderShowValue) SliderBar.Value = currentPosition;

            // 播放时间显示处理
            var val = DateTime.MinValue.AddSeconds(SliderBar.Value).ToString("HH:mm:ss");
            var total = DateTime.MinValue.AddSeconds(SliderBar.Maximum).ToString("HH:mm:ss");
            TxtTime.Text = string.Format("{0}/{1}", val, total);
            if (Math.Abs(SliderBar.Maximum - SliderBar.Value) < 0.5)
            {
                //播放结束跳转
                SetPlayState();
                StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, 0, _seeTime);
                Thread.Sleep(500);
                Log.RecordData("PlayVideoOver", _videoItem.CwareId, _videoItem.VideoId);
                Dispatcher.Invoke(new Action(() => PlayNextVideo(false)));
                return;
            }
            if (currentPosition < 2) return;

            //弹出知识点
            if (!_isInPoint && _pointTestStartTimeList != null && _pointTestStartTimeList.Count > 0)
            {
                foreach (var item in _pointTestStartTimeList)
                {
                    //显示知识点测试按钮
                    if (item.PointOpenType == "t" && item.PointTestStartTime - 30 < currentPosition && item.PointTestStartTime - 29 > currentPosition)
                    {
                        _currentTest = item;
                        BtnPointTest.Visibility = Visibility.Visible;
                    }
                    if (item == _currentTest && item.PointOpenType == "t" && (item.PointTestStartTime < currentPosition || item.PointTestStartTime - 30 > currentPosition))
                    {
                        _currentTest = null;
                        BtnPointTest.Visibility = Visibility.Collapsed;
                    }
                    if (!_isInPoint && item.PointOpenType != "t" && item.PointTestStartTime < currentPosition && item.PointTestStartTime + 1 > currentPosition)
                    {
                        //弹出知识点测试或者单元测试
                        _isInPoint = true;
                        SetPlayState();
                        var win = new PointTest(item, _course) { Owner = this };
                        //如果返回时间点小于当前时间超过1秒，那么执行返回学习
                        if (win.ShowDialog() == true && Math.Abs(item.BackTime - currentPosition) > 1)
                        {
                            _ctlPlayerCore.SetPosition(item.BackTime);
                        }
                        SetPlayState();
                        _isInPoint = false;
                        break;
                    }
                }
            }

            //讲义跳转
            string currentNode = string.Empty;
            foreach (var de in _nodeTime)
            {
                if (de.Value <= currentPosition)
                {
                    currentNode = de.Key;
                }
                else
                {
                    break;
                }
            }

            if (currentNode != _lastNode)
            {
                _lastNode = currentNode;
                if (WebMain.Document != null) WebMain.Document.InvokeScript("gotoNode", new object[] { _lastNode });
            }
        }

        /// <summary>
        /// 知识点练习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPointTest_Click(object sender, RoutedEventArgs e)
        {
            _isInPoint = true;
            SetPlayState();
            var win = new PointTest(_currentTest, _course) { Owner = this };
            win.ShowDialog();
            SetPlayState();
            _isInPoint = false;
        }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            SetPlayState();
        }

        /// <summary>
        /// 暂停或继续
        /// </summary>
        /// <param name="isOnlyPause">是否只暂停不做任何操作 true:是，false：否</param>
        private void SetPlayState(bool isOnlyPause = false)
        {
            if (isOnlyPause)
            {
                Log.RecordData("PlayVideoPause", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
                BtnPlayPause.NormalImage = "/Images/Play/btn_play_normal.png";
                BtnPlayPause.HoverImage = "/Images/Play/btn_play_hover.png";
                BtnPlayPause.PressedImage = "/Images/Play/btn_play_hover.png";

                _ctlPlayerCore.Pause();
            }
            else
            {
                if (_ctlPlayerCore.PlayState == PlayState.Running)
                {
                    Log.RecordData("PlayVideoPause", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
                    BtnPlayPause.NormalImage = "/Images/Play/btn_play_normal.png";
                    BtnPlayPause.HoverImage = "/Images/Play/btn_play_hover.png";
                    BtnPlayPause.PressedImage = "/Images/Play/btn_play_hover.png";

                    _ctlPlayerCore.Pause();
                }
                else
                {
                    Log.RecordData("PlayVideoStart", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
                    Log.RecordData("PlayVideoRate", _videoItem.CwareId, _videoItem.VideoId, (((double)_rate) / 10).ToString("0.0"));
                    BtnPlayPause.NormalImage = "/Images/Play/btn_pause_normal.png";
                    BtnPlayPause.HoverImage = "/Images/Play/btn_pause_hover.png";
                    BtnPlayPause.PressedImage = "/Images/Play/btn_pause_hover.png";

                    _ctlPlayerCore.Play();
                    SliderBar.Maximum = _ctlPlayerCore.VideoLen;
                }
            }

        }

        /// <summary>
        /// 上一个视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var item = StudentWareLogic.GetStudentCwareDetailPreItem(_videoItem.CwareId, _videoItem.VideoId);
            if (item == null)
            {
                MessageBox.Show(@"当前已经是第一个视频");
                return;
            }
            if (item.VideoState == -1)
            {
                MessageBox.Show(@"课件《" + item.VideoName + @"》尚未下载");
                return;
            }
            if (item.VideoState != 3)
            {
                MessageBox.Show(@"课件《" + item.VideoName + @"》尚未下载完成");
                return;
            }
            if (string.IsNullOrEmpty(item.VideoPath) || !File.Exists(item.VideoPath))
            {
                MessageBox.Show(@"课件《" + item.VideoName + @"》文件不存在，请重新下载");
                return;
            }
            StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, (int)SliderBar.Value, _seeTime);
            _ctlPlayerCore.Stop();
            _isInChange = true;
            Log.RecordData("PlayVideoPres", _videoItem.CwareId, _videoItem.VideoId + " ", SliderBar.Value);
            _videoItem = item;
            _pointTestStartTimeList = null;
            PlayVideo();
            _isInChange = false;
        }

        /// <summary>
        /// 下一个视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            PlayNextVideo();
        }

        /// <summary>
        /// 播放下一个视频
        /// </summary>
        /// <param name="isShowInfo"></param>
        private void PlayNextVideo(bool isShowInfo = true)
        {
            var item = StudentWareLogic.GetStudentCwareDetailNextItem(_videoItem.CwareId, _videoItem.VideoId);
            if (item == null)
            {
                if (isShowInfo) MessageBox.Show(@"当前已经是最后一个视频");
                return;
            }
            if (item.VideoState == -1)
            {
                if (isShowInfo) MessageBox.Show(@"课件《" + item.VideoName + @"》尚未下载");
                return;
            }
            if (item.VideoState != 3)
            {
                if (isShowInfo) MessageBox.Show(@"课件《" + item.VideoName + @"》尚未下载完成");
                return;
            }
            if (string.IsNullOrEmpty(item.VideoPath) || !File.Exists(item.VideoPath))
            {
                if (isShowInfo) MessageBox.Show(@"课件《" + item.VideoName + @"》文件不存在，请重新下载");
                return;
            }
            StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, (int)SliderBar.Value, _seeTime);
            _ctlPlayerCore.Stop();
            _isInChange = true;
            Log.RecordData("PlayVideoNext", _videoItem.CwareId, _videoItem.VideoId + " ", SliderBar.Value);
            _videoItem = item;
            _pointTestStartTimeList = null;
            PlayVideo();
            _isInChange = false;
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        private void PlayVideo()
        {
            _seeTime = 0;
            //设置进程的优先级
            Trace.WriteLine("SetAboveNormal");
            var thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.AboveNormal;
            Trace.WriteLine("SetPlayerTitle");
            PageTitle.Text = string.Format("【正在讲授】{0} - {1}", _cwName, _videoItem.VideoName);
            Trace.WriteLine("InitPlayer");
            //初始化播放器
            InitPlayer();
            Trace.WriteLine("InitKcjy");
            InitKcjy();
            _ctlPlayerCore.FileName = _videoItem.VideoPath;
            Trace.WriteLine("SetPlayState");
            SetPlayState();
            int pos = StudentWareLogic.GetVideoPosition(_videoItem.CwareId, _videoItem.VideoId);
            Trace.WriteLine("SetPlayPosition");
            //如果直接设置为0的话，破Xp居然不播放了，诡异的问题，不予以深究，快淘汰的玩意。。。。
            _ctlPlayerCore.SetPosition(pos > 0 && pos < SliderBar.Maximum - 30 ? pos : 0.5);

            SliderBar.Value = pos;
            Trace.WriteLine("SetPlaySpeed");
            var r = ((double)_lastrate) / 10;
            _rate = _lastrate;
            _ctlPlayerCore.SetRate(r);
            _ctlPlayerCore.SetPitch(_lastpr);
            var speed = r.ToString("0.0");
            TxtSpeed.Text = speed == "1.0" ? "正常" : speed + "X";
            Trace.WriteLine("PlayVideoOk");

            //获取弹出知识点
            SystemInfo.StartBackGroundThread("获取弹出知识点列表", () =>
            {
                _pointTestStartTimeList = StudentWareLogic.GetPointTestStartTimeList(_videoItem.CwareId, _videoItem.VideoId).ToList();
            });
        }

        /// <summary>
        /// 播放速度选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            double pr = 0;
            switch (_rate)
            {
                case 10:
                    _rate = 12;
                    pr = -3;
                    break;
                case 12:
                    _rate = 13;
                    pr = -5;
                    break;
                case 13:
                    _rate = 15;
                    pr = -7;
                    break;
                case 15:
                    _rate = 18;
                    pr = -10;
                    break;
                case 18:
                    _rate = 20;
                    pr = -12;
                    break;
                case 20:
                    _rate = 10;
                    pr = 0;
                    break;
            }

            double r = ((double)_rate) / 10;
            _ctlPlayerCore.SetRate(r);
            _ctlPlayerCore.SetPitch(pr);
            _lastrate = _rate;
            _lastpr = pr;
            Trace.WriteLine("手动变调：" + _lastrate + _lastpr);
            var speed = r.ToString("0.0");
            TxtSpeed.Text = speed == "1.0" ? "正常" : speed + "X";
            Log.RecordData("PlayVideoRate", _videoItem.CwareId, _videoItem.VideoId, speed);
        }

        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (_isInChange) return;
            var isfull = this.IsFullscreen();
            Log.RecordData("PlayVideoFullScreen", _videoItem.CwareId, _videoItem.VideoId, !isfull);
            if (isfull)
            {
                BtnFullScreen.NormalImage = "/Images/Play/btn_fullscreen_normal.png";
                BtnFullScreen.HoverImage = "/Images/Play/btn_fullscreen_hover.png";
                BtnFullScreen.PressedImage = "/Images/Play/btn_fullscreen_hover.png";
                GridTop.Visibility = Visibility.Visible;

                this.ExitFullscreen();
            }
            else
            {
                BtnFullScreen.NormalImage = "/Images/Play/btn_fullback_normal.png";
                BtnFullScreen.HoverImage = "/Images/Play/btn_fullback_hover.png";
                BtnFullScreen.PressedImage = "/Images/Play/btn_fullback_hover.png";
                GridTop.Visibility = Visibility.Collapsed;

                this.GoFullscreen();
            }
        }

        /// <summary>
        /// 播放控件被卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlPlayerCore_Disposed(object sender, EventArgs e)
        {
            _ctlPlayerCore.Stop();
        }

        /// <summary>
        /// 进度条弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, SliderBar.Value);
            _ctlPlayerCore.SetPosition(SliderBar.Value);
            _sliderShowValue = true;
        }

        /// <summary>
        /// 进度条失去鼠标捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderBar_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, SliderBar.Value);
            _ctlPlayerCore.SetPosition(SliderBar.Value);
            //_ctlPlayerCore.Play();
            _sliderShowValue = true;
        }

        /// <summary>
        /// 进度条获取鼠标捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderBar_GotMouseCapture(object sender, MouseEventArgs e)
        {
            _sliderShowValue = false;
            //_ctlPlayerCore.Pause();
        }

        /// <summary>
        /// 进度条鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _sliderShowValue = false;
        }

        [ComVisible(true)]
        public class ObjectForWeb
        {
            public event Action<string> WebClick;

            public void SetPos(string node)
            {
                if (WebClick != null)
                {
                    WebClick(node);
                }
            }

            public void ExportToWord()
            {

            }
        }

        /// <summary>
        /// 视频窗口双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlPlayerCore_DoubleClick(object sender, EventArgs e)
        {
            BtnFullScreen_Click(sender, null);
        }

        /// <summary>
        /// 向左按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLeft_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentState == PageState.Split)//  分屏 -> 讲义
            {
                _currentState = PageState.Jy;
                BtnLeft.Visibility = Visibility.Collapsed;
                BtnRight.Visibility = Visibility.Visible;

                Col1.Width = new GridLength(0, GridUnitType.Star);
            }
            else if (_currentState == PageState.Play) // 播放 -> 分屏
            {
                _currentState = PageState.Split;

                BtnLeft.Visibility = Visibility.Visible;
                BtnRight.Visibility = Visibility.Visible;

                Col3.Width = new GridLength(1, GridUnitType.Star);
            }

            Log.RecordData("PlayVideoJyState", _videoItem.CwareId, _videoItem.VideoId, _currentState);
        }

        /// <summary>
        /// 向右按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRight_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentState == PageState.Split)// 分屏 -> 播放
            {
                _currentState = PageState.Play;

                BtnLeft.Visibility = Visibility.Collapsed;
                BtnRight.Visibility = Visibility.Collapsed;

                Col3.Width = new GridLength(0, GridUnitType.Star);
            }
            else if (_currentState == PageState.Jy) // 讲义 -> 分屏
            {
                _currentState = PageState.Split;

                BtnLeft.Visibility = Visibility.Visible;
                BtnRight.Visibility = Visibility.Visible;

                Col1.Width = new GridLength(2, GridUnitType.Star);
            }

            Log.RecordData("PlayVideoJyState", _videoItem.CwareId, _videoItem.VideoId, _currentState);
        }

        private enum PageState
        {
            Split,
            Play,
            Jy
        }

        /// <summary>
        /// 声音设置值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSound_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SystemMultimediaController.CurrentValue = (int)SliderVol.Value;
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource != null)
                SystemMultimediaController.SetMute(hwndSource.Handle);
        }

        #region ALT+F4直接退出主程序

        private bool _altDown = false;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = true;
            }
            else if (e.SystemKey == Key.F4 && _altDown)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = false;
            }
        }

        #endregion

        /// <summary>
        /// 是否显示讲义按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnJy_OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (_currentState == PageState.Split)
                BtnRight_OnClick(sender, e);
            else
                BtnLeft_OnClick(sender, e);
        }

        #region 主窗体最大化、最小化、全屏、拖动

        public void ToggleFullScreen()
        {
            var isFull = this.IsFullscreen();
            GridTop.Visibility = isFull ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnMin_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMax_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isInChange) return;
            WindowState = WindowState.Maximized;
        }

        private void BtnNormal_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isInChange) return;
            WindowState = WindowState.Normal;

            BtnMax.Visibility = Visibility.Visible;
            BtnNormal.Visibility = Visibility.Collapsed;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }



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
                    BtnNormal_OnClick(sender, e);

                    var pos = Mouse.GetPosition(this);
                    Top = SystemParameters.WorkArea.Y - pos.Y;
                }
                DragMove();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                BtnMax.Visibility = Visibility.Collapsed;
                BtnNormal.Visibility = Visibility.Visible;
            }
            else
            {
                BtnMax.Visibility = Visibility.Visible;
                BtnNormal.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        /// <summary>
        /// 字体大小选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = SelectFont.SelectedIndex + 1;
            if (WebMain != null && WebMain.Document != null) WebMain.Document.InvokeScript("SetFont", new object[] { index });
        }

        private void ExportJy_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = @"*.docx(Word文档)|*.docx" };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //using (var docx = DocX.Create(sfd.FileName))
                //{
                //    if (WebMain.Document != null && WebMain.Document.Body != null)
                //    {
                //        docx.InsertParagraph(WebMain.Document.Body.InnerHtml);
                //    }
                //    docx.Save();
                //}
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Log.RecordData("PlayVideoClose", _videoItem.CwareId, _videoItem.VideoId);
            base.OnClosing(e);
        }

        private void ChkTopMost_OnClick(object sender, RoutedEventArgs e)
        {
            Topmost = ChkTopMost.IsChecked.HasValue && ChkTopMost.IsChecked.Value;
        }

        private void BtnAsk_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("只能在联网的状态下才能提问");
                return;
            }
            SetPlayState(true);//暂停
//#if CHINAACC || JIANSHE || MED || LAW || CHINATAT || KAOYAN || G12E||ZIKAO ||FOR68
            var boardId = new Framework.Local.StudentWareData().GetStudentCwareBordId(_course.CwareId, _course.EduSubjectId);
            int _boardId = boardId.FirstOrDefault();
            string Jy_url = string.Format("[KCJYGET,{0},{1},{2},ISNEW]", _course.CwareId, Int32.Parse(_videoItem.VideoId), _lastNode);
            StudentWareLogic.GotoLoginedWebSite(_course.CwareId, _boardId, _lastNode, Jy_url);
//#endif
        }

        private void BtnTimeSelect_Click(object sender, RoutedEventArgs e)
        {
            BtnTimeSelect.IsEnabled = false;
            SetPlayState();//暂停
           
            //传的时间值有误差因此使用异步延迟弹出
            SystemInfo.StartBackGroundThread("异步时间选择", ChangeTime);
        }
        private void ChangeTime()
        {
            Thread.Sleep(700);
            Dispatcher.Invoke(new Action(() =>
            {
                string[] str = TxtTime.Text.Split('/');
                var win = new WinTimeSelect(str[0]);
                if (win.ShowDialog() == true)
                {
                    var pos = win.TimeValue;
                    if (pos < 0) pos = 0.5;
                    if (pos >= SliderBar.Maximum) pos = SliderBar.Maximum - 1;
                    SliderBar.Value = pos;
                    var value = DateTime.MinValue.AddSeconds(pos).ToString("HH:mm:ss");
                    TxtTime.Text = string.Format("{0}/{1}", value, str[1]);
                    Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, pos);
                    _ctlPlayerCore.SetPosition(pos);
                }
                SetPlayState();//继续
                BtnTimeSelect.IsEnabled = true;
            }));
        }
    }
}
