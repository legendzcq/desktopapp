using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;
using DesktopApp.ViewModel;
using Framework.Model;
using Framework.NewModel;
using Framework.Player;
using Framework.Utility;
using GalaSoft.MvvmLight.Messaging;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DesktopApp.Controls;
using System.Globalization;
using Framework.Local;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for SFPlayWindow.xaml
    /// </summary>
    public partial class SFPlayWindow
    {

        static int _lastrate = 10;
        private static double _lastpr;

        private int _rate = 10;
        private int _seeTime;
        private string _lastNode = string.Empty;
        private readonly Dictionary<string, int> _nodeTime = new Dictionary<string, int>();
        private ViewStudentWareDetail _videoItem;

        private readonly ViewStudentCourseWare _course;
        private readonly string _cwName; // 班次名称

        private bool _sliderShowValue = true;

        /// <summary>
        /// 弹出知识点列表
        /// </summary>
        private List<PointTestStartTimeItem> _pointTestStartTimeList;

        /// <summary>
        /// 正在切换视频的时候不能更改视频窗口，否则视频窗口会显示不出来。
        /// </summary>
        private bool _isInChange;

        private PlayerCore _ctlPlayerCore;
        private List<ChapterDetailViewModel> _playList;// 播放列表
        private WindowResizer _windowResizer;

        //开始学习位置
        private double startPos = 0;

        //用于定位听课时间点位置
        private double pos1 = 0;
        //定位学习时间
        private string currentTime;

        public SFPlayWindow()
        {
            InitializeComponent();
        }
        public SFPlayWindow(string cwName, ViewStudentWareDetail videoItem, ViewStudentCourseWare course)
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
            Log.RecordData("PlayVideo", course.CourseId, videoItem.CwareId, videoItem.VideoId, "SFP");
            // 从课程导入接收刷新消息
            Messenger.Default.Register<string>(this, TokenManager.RefreshList, s =>
            {
                if (_course != null)
                {
                    Dispatcher.Invoke(new Action(BindPlayList));
                }
            });


            Title = Util.AppName;

            ChangSize();

            FullScreenManager.RepairWpfWindowFullScreenBehavior(this);
            PreviewKeyDown += Window_PreviewKeyDown;
            PreviewKeyUp += Window_PreviewKeyUp;
            SizeChanged += Window_SizeChanged;

            _windowResizer = new WindowResizer(this);
            _windowResizer.AddResizers(left, right, top, bottom, leftTop, leftBottom, rightTop, rightBottom);
            _windowResizer.ConnectResizer();

            _cwName = cwName;
            _videoItem = videoItem;
            _course = course;
            BindPlayList();

            _seeTime = 0;

            Loaded += (s, e) => PlayVideo();
            Closing += (s, e) =>
            {
                _ctlPlayerCore.Stop();
                StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, (int)SliderBar.Value, _seeTime);
                //学习记录添加到数据库 dgh 2018.06.01
                AddTimebase((int)startPos, (int)pos1, currentTime);
                if (Util.IsOnline)
                {
                    //同步视频记录 dgh 2017.12.01
                    UpLoadLastPos();
                }
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
        }

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
            sliderVol.Minimum = SystemMultimediaController.MinValue;
            sliderVol.Maximum = SystemMultimediaController.MaxValue;
            sliderVol.Value = SystemMultimediaController.CurrentValue;
            if (WinHost.Child != null) WinHost.Child.Dispose();
            _ctlPlayerCore = new PlayerCore();
            _ctlPlayerCore.Playing += PlayerCore_Playing;
            _ctlPlayerCore.Disposed += ctlPlayerCore_Disposed;
            _ctlPlayerCore.OnError += () =>
            {
                //检查用户显卡状态
                if (!SystemInfo.CheckVideoControllerDriver())
                {
                    //未安装驱动（双显卡机器，如果有一个未装，也会影响到播放）
                    MessageBox.Show(@"系统检测到您的显卡未安装驱动，请先安装显卡驱动" + Environment.NewLine + @"请用IE浏览器打开以下网址安装驱动： " + Environment.NewLine + @"http://up.mydrivers.com/", @"提示");
                    return;
                }
                if (
                    MessageBox.Show(@"播放时发生错误，系统可以尝试修复错误，是否要修复错误？", @"提示", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
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

        private bool _isInPoint = false;

        private PointTestStartTimeItem _currentTest = null;

        private void PlayerCore_Playing(double currentPosition)
        {
            _seeTime++;

            if (Math.Abs(currentPosition - pos1) > 2 && pos1 > 0)
            {
                if (currentPosition - pos1 > 0)//向前拖动添加到数据库中
                {
                    AddTimebase((int)startPos, (int)pos1, currentTime);
                }
                startPos = (int)currentPosition;

            }
            //3分钟向库里存一次 dgh 2018.06.01,
            if (_seeTime % 180 == 0)
            {
                AddTimebase((int)startPos, (int)pos1,currentTime);
                startPos = pos1;
                if (Util.IsOnline)
                {
                    UpLoadLastPos();
                }

            }
            pos1 = currentPosition;
            currentTime = Util.GetNowTimeStamp13().ToString(CultureInfo.InvariantCulture);

            if (_sliderShowValue) SliderBar.Value = currentPosition;

            // 播放时间
            var val = (int)SliderBar.Value;
            var total = (int)SliderBar.Maximum;
            TxtTime.Text = string.Format("{0}/{1}", (val / 60).ToString("00") + ":" + (val % 60).ToString("00"), (total / 60).ToString("00") + ":" + (total % 60).ToString("00"));
            if (Math.Abs(SliderBar.Maximum - SliderBar.Value) < 0.5)
            {
                SetPlayState();
                StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, 0, _seeTime);
                Thread.Sleep(500);
                Log.RecordData("PlayVideoOver", _videoItem.CwareId, _videoItem.VideoId);
                Dispatcher.Invoke(new Action(() => PlayNextVideo(false)));
            }

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
                        _isInPoint = true;
                        SetPlayState();
                        var win = new PointTest(item, _course) { Owner = this };
                        if (win.ShowDialog() == true)
                        {
                            _ctlPlayerCore.SetPosition(item.BackTime);
                        }
                        SetPlayState();
                        _isInPoint = false;
                        break;
                    }
                }
            }
            if (currentPosition < 2)
            {
                int pos = StudentWareLogic.GetVideoPosition(_videoItem.CwareId, _videoItem.VideoId);
                if(pos>60)
                {
                    AddTimebase(pos, (int)SliderBar.Maximum, currentTime);
                }
               
                return;
            }
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

        private void InitKcjy()
        {
            try
            {
                var webObj = new PlayWindow.ObjectForWeb();
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
                var fname = Guid.NewGuid().ToString() + ".htm";

                if (!Directory.Exists(SystemInfo.AppDataPath + "web"))
                {
                    Directory.CreateDirectory(SystemInfo.AppDataPath + "web");
                }
                File.WriteAllText(SystemInfo.AppDataPath + "web" + "\\" + fname, html);
                WebMain.Navigate("http://127.0.0.1:" + Util.HttpPort + "/" + fname);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            SetPlayState();
        }
        /// <summary>
        /// 暂停或继续
        /// </summary>
        /// <param name="isOnlyPause">是否只暂停不做任何操作 true:是，false：否</param>
        /// <param name="initiali">是否初始化来的数据</param>
        private void SetPlayState(bool isOnlyPause = false, bool initiali = false)
        {
            if (isOnlyPause)
            {
                Log.RecordData("PlayVideoPause", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
                BtnPlayPause.NormalImage = "/Images/SFPlay/btn_play_normal.png";
                BtnPlayPause.HoverImage = "/Images/SFPlay/btn_play_hover.png";
                BtnPlayPause.PressedImage = "/Images/SFPlay/btn_play_hover.png";

                _ctlPlayerCore.Pause();
                if (!initiali)
                {
                    AddTimebase((int)startPos, (int)pos1,currentTime);
                    startPos = pos1;
                }
            }
            else
            {
                if (_ctlPlayerCore.PlayState == PlayState.Running)
                {
                    Log.RecordData("PlayVideoPause", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
                    BtnPlayPause.NormalImage = "/Images/SFPlay/btn_play_normal.png";
                    BtnPlayPause.HoverImage = "/Images/SFPlay/btn_play_hover.png";
                    BtnPlayPause.PressedImage = "/Images/SFPlay/btn_play_hover.png";

                    _ctlPlayerCore.Pause();
                    if (!initiali)
                    {
                        AddTimebase((int)startPos, (int)pos1, currentTime);
                        startPos = pos1;
                    }
                }
                else
                {
                    Log.RecordData("PlayVideoStart", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value, (((double)_rate) / 10).ToString("0.0"));
                    Log.RecordData("PlayVideoRate", _videoItem.CwareId, _videoItem.VideoId, (((double)_rate) / 10).ToString("0.0"));
                    BtnPlayPause.NormalImage = "/Images/SFPlay/btn_pause_normal.png";
                    BtnPlayPause.HoverImage = "/Images/SFPlay/btn_pause_hover.png";
                    BtnPlayPause.PressedImage = "/Images/SFPlay/btn_pause_hover.png";

                    _ctlPlayerCore.Play();
                    SliderBar.Maximum = _ctlPlayerCore.VideoLen;
                }
            }

        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Log.RecordData("PlayVideoPres", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
            _playList.Single(x => x.VideoState == VideoState.Playing).VideoState = VideoState.Done;
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
            //切换视频的时候添加数据库
            AddTimebase((int)startPos, (int)pos1, currentTime);
            _ctlPlayerCore.Stop();
            _isInChange = true;
            _videoItem = item;

            PlayVideo();
            _isInChange = false;
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            PlayNextVideo();
        }

        private void PlayNextVideo(bool isShowInfo = true)
        {
            Log.RecordData("PlayVideoNext", _videoItem.CwareId, _videoItem.VideoId, SliderBar.Value);
            _playList.Single(x => x.VideoState == VideoState.Playing).VideoState = VideoState.Done;
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
            //切换视频的时候添加数据库
            AddTimebase((int)startPos, (int)pos1, currentTime);
            _ctlPlayerCore.Stop();
            _isInChange = true;
            _videoItem = item;
            PlayVideo();
            _isInChange = false;
        }

        private void PlayVideo()
        {
            _seeTime = 0;
            //设置进程的优先级
            _playList.Single(x => x.DownId == _videoItem.DownId).VideoState = VideoState.Playing;
            //
            Trace.WriteLine("SetAboveNormal");
            var thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.AboveNormal;
            Trace.WriteLine("SetPlayerTitle");
            PageTitle.Text = string.Format("【正在讲授】{0} - {1}", _cwName, _videoItem.VideoName);
            Trace.WriteLine("InitKcjy");
            InitKcjy();
            Trace.WriteLine("InitPlayer");
            //初始化播放器
            InitPlayer();
            _ctlPlayerCore.FileName = _videoItem.VideoPath;
            Trace.WriteLine("SetPlayState");
            SetPlayState(false,true);
            int pos = StudentWareLogic.GetVideoPosition(_videoItem.CwareId, _videoItem.VideoId);
            pos1 = startPos = pos;
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
            //开始学习时间
            currentTime =Util.GetNowTimeStamp13().ToString(CultureInfo.InvariantCulture);
            //获取弹出知识点
            SystemInfo.StartBackGroundThread("获取弹出知识点列表", () =>
            {
                _pointTestStartTimeList = StudentWareLogic.GetPointTestStartTimeList(_videoItem.CwareId, _videoItem.VideoId).ToList();
            });
        }

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            //变速的时候也需要添加到数据库
            AddTimebase((int)startPos, (int)pos1, currentTime);
            startPos = pos1;
            double r, pr = 0;
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
            r = ((double)_rate) / 10;
            _ctlPlayerCore.SetRate(r);
            _ctlPlayerCore.SetPitch(pr);
            _lastrate = _rate;
            _lastpr = pr;
            Trace.WriteLine("手动变调：" + _lastrate + _lastpr);
            var speed = r.ToString("0.0");
            TxtSpeed.Text = speed == "1.0" ? "正常" : speed + "X";
            Log.RecordData("PlayVideoRate", _videoItem.CwareId, _videoItem.VideoId, speed);
        }

        private void SliderBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, SliderBar.Value);
            _ctlPlayerCore.SetPosition(SliderBar.Value);
            _sliderShowValue = true;
        }

        private void ctlPlayerCore_Disposed(object sender, EventArgs e)
        {
            _ctlPlayerCore.Stop();
        }

        private void SliderBar_LostMouseCapture(object sender, MouseEventArgs e)
        {
            Log.RecordData("PlayVideoSetPosition", _videoItem.CwareId, _videoItem.VideoId, _ctlPlayerCore.CurrentPosition, SliderBar.Value);
            _ctlPlayerCore.SetPosition(SliderBar.Value);
            //_ctlPlayerCore.Play();
            _sliderShowValue = true;
        }

        private void SliderBar_GotMouseCapture(object sender, MouseEventArgs e)
        {
            _sliderShowValue = false;
            //_ctlPlayerCore.Pause();
        }

        private void SliderBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _sliderShowValue = false;
        }


        /// <summary>
        /// 将学习记录添加到数据库中
        /// </summary>
        /// <param name="prepos">开始时间点</param>
        /// <param name="nextpos">结束时间点</param>
        /// <param name="studyTimeEnd">结束时间</param>
        private void AddTimebase(int prepos, int nextpos, string studyTimeEnd)
        {
            StudentWareData sw = new StudentWareData();
            if (prepos == nextpos || prepos < 0 || nextpos < 0 || nextpos - prepos < 0) return;
            if (string.IsNullOrWhiteSpace(studyTimeEnd)) return;
            var count = sw.StudyVideoStrById(_course.CwareId, _videoItem.VideoId, prepos, nextpos);
            if (count > 0) return;
            if (prepos == 1)
            {
                prepos = 0;
            }
            var cm = (nextpos - prepos) * 1000;
            var speed = TxtSpeed.Text == "正常" ? "1.0" : TxtSpeed.Text.TrimEnd('X');
            double sp = Convert.ToDouble(speed);
            string studyTimeStart = (Convert.ToInt64(studyTimeEnd) - Math.Round(cm / sp)).ToString(CultureInfo.InvariantCulture);
            Trace.WriteLine("添加数据库1：" + studyTimeEnd);
            Trace.WriteLine("添加数据库1：" + cm);
            Trace.WriteLine("添加数据库1：" + studyTimeStart);
            TimebaseStr timestr = new TimebaseStr
            {
                VideoStartTime = prepos.ToString(CultureInfo.InvariantCulture),
                VideoEndTime = nextpos.ToString(CultureInfo.InvariantCulture),
                Speed = speed,
                StudyTimeEnd = studyTimeEnd,
                StudyTimeStart = studyTimeStart,
                CwareId = _course.CwareId,
                VideoID = _videoItem.VideoId,
            };

            sw.AddStudentVideoTimebaseItem(timestr);
        }

        /// <summary>
        /// 上传最后一次听课记录
        /// </summary>
        private void UpLoadLastPos()
        {
            var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var lastPlayPosition = (int)SliderBar.Value;
            var svr = new StudentVideoRecord
            {
                CwareUrl = _course.CwareUrl,
                CwareId = _course.CwareId,
                LastPosition = lastPlayPosition,
                LastTime = datetime,
                VideoID = _videoItem.VideoId,
                Uid = Util.SsoUid
            };
            SystemInfo.StartBackGroundThread("上传视频记录", () =>
            {
                StudentWareLogic.SaveNextBeginTime(svr);
            });
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
        }

        private void BtnSound_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SystemMultimediaController.CurrentValue = (int)sliderVol.Value;
        }

        #region 播放列表

        /// <summary>
        /// 绑定播放列表数据
        /// </summary>
        private void BindPlayList()
        {
            var viewStudentWareDetails = StudentWareLogic.GetStudentWareDetail(_course.CwareId);
            _playList = new List<ChapterDetailViewModel>();
            viewStudentWareDetails.ForEach(x =>
            {
                var vm = new ChapterDetailViewModel();
                vm.FromModel(x);
                if (_videoItem.DownId == x.DownId)
                {
                    vm.VideoState = VideoState.Playing;
                }
                _playList.Add(vm);
            });
            var groupList = new ListCollectionView(_playList);
            if (groupList.GroupDescriptions != null)
                groupList.GroupDescriptions.Add(new PropertyGroupDescription("ChapterName"));
            DgData.ItemsSource = groupList;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var selItem = DgData.SelectedItem as ChapterDetailViewModel;
                if (selItem == null || selItem.VideoState != VideoState.Done || selItem.ViewStudentWare.VideoId == _videoItem.VideoId)
                {
                    return;
                }
                _ctlPlayerCore.Stop();
                StudentWareLogic.UpdateTime(_videoItem.CwareId, _videoItem.VideoId, (int)SliderBar.Value, _seeTime);
                _videoItem = selItem.ViewStudentWare;
                _playList.Single(x => x.VideoState == VideoState.Playing).VideoState = VideoState.Done;
                PlayVideo();
            }
        }
        #endregion

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

        private void SelectFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = SelectFont.SelectedIndex + 1;
            if (WebMain != null && WebMain.Document != null) WebMain.Document.InvokeScript("SetFont", new object[] { index });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Log.RecordData("PlayVideoClose", _videoItem.CwareId, _videoItem.VideoId);
            base.OnClosing(e);
        }

        private void BtnAsk_Click(object sender, RoutedEventArgs e)
        {
            if (!Util.IsOnline)
            {
                CustomMessageBox.Show("只能在联网的状态下才能提问");
                return;
            }
            SetPlayState(true);//暂停
//#if CHINAACC || JIANSHE || MED || LAW || CHINATAT || KAOYAN || G12E||ZIKAO||FOR68
            var boardId = new Framework.Local.StudentWareData().GetStudentCwareBordId(_course.CwareId, _course.EduSubjectId);
            int _boardId = boardId.FirstOrDefault();
            string Jy_url = string.Format("[KCJYGET,{0},{1},{2},ISNEW]", _course.CwareId, _videoItem.VideoId, _lastNode);
            StudentWareLogic.GotoLoginedWebSite(_course.CwareId, _boardId, _lastNode, Jy_url);
//#endif
        }
        private void BtnTimeSelect_Click(object sender, RoutedEventArgs e)
        {
            BtnTimeSelect.IsEnabled = false;
            SetPlayState();//暂停
            string[] str = TxtTime.Text.Split('/');
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
                    var value = DateTime.MinValue.AddSeconds(pos).ToString("mm:ss");
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
