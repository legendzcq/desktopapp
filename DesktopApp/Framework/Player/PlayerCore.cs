using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Framework.Player.DShow;
using Framework.Utility;

namespace Framework.Player
{
    [Guid("0A878A55-A929-4f68-BF81-E1B7B4A9BDEF"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioPitch
    {
        void setPitchValue([In] double nVal);
        void getPitchValue([Out] out double nVal);
        void setTempoValue([In] int nVal);
        void getTempoValue([Out] out int nVal);
        void setLicenseKey([In] int nVal);
    }

    public enum PlayState
    {
        Stopped = 0,
        Paused = 1,
        Running = 2,
    }

    public class PlayerCore : Panel
    {
        private const string Mp4DemuxFilterType = "95F2E3CB-32CD-4D3F-BF3D-437259FC1CA3";

        //#if CHINAACC
        //        private const string Mp4DemuxFilterType = "95F2E3CB-32CD-4D3F-BF3D-437259FC1CA3";
        //#endif
        //#if MED
        //        private const string Mp4DemuxFilterType = "1DD6B052-037A-4EA7-A576-A5A441658E5B";
        //#endif
        //#if JIANSHE
        //        private const string Mp4DemuxFilterType = "E5142115-20B3-4E98-B955-B9B46B6BEF73";
        //#endif
        //#if LAW
        //        private const string Mp4DemuxFilterType = "0F356728-7834-42A3-85B1-AD240668D0D1";
        //#endif
        //#if CHINATAT
        //        private const string Mp4DemuxFilterType = "6AEFCCE5-011A-4073-8486-1B2AA9771575";
        //#endif
        //#if G12E
        //        private const string Mp4DemuxFilterType = "2AC3F2B9-19C2-4A37-8E4B-78152DF1DD5D";
        //#endif
        //#if ZIKAO
        //        private const string Mp4DemuxFilterType = "D3B05333-93CE-4B9D-B604-2FFCC3E28CA6";
        //#endif
        //#if CHENGKAO
        //        private const string Mp4DemuxFilterType = "E3CCADC7-32ED-4988-A85F-1831CC0A6DB3";
        //#endif
        //#if KAOYAN
        //        private const string Mp4DemuxFilterType = "77ED84C7-BECD-4C49-A362-D69F5DF94B01";
        //#endif
        //#if FOR68
        //        private const string Mp4DemuxFilterType = "AB2FD7F0-924B-4546-90DF-A1A0C65ACD66";
        //#endif
        public event Action<double> Playing;

        public event Action OnError;

        //以下是一堆dshow里需要的接口
        private IGraphBuilder _graphBuilder;
        private IMediaControl _mediaControl;
        private IMediaEventEx _mediaEventEx;
        private IVideoWindow _videoWindow;
        private IBasicAudio _basicAudio;
        private IBasicVideo _basicVideo;
        private IMediaSeeking _mediaSeeking;
        private IMediaPosition _mediaPosition;
        private IBaseFilter _audioPitchFilter;
        private IBaseFilter _mp4Filter;

        private bool _isInStop;

        private double _scaleRate;

        private readonly int _dbClickTime;

        //private readonly RECT _rec = new RECT();

        /// <summary>
        /// 当前正在播放的文件名称
        /// </summary>
        public string FileName { private get; set; }

        public double CurrentPosition { get; private set; }

        /// <summary>
        /// 当前的播放状态
        /// </summary>
        public PlayState PlayState
        {
            get
            {
                FilterState runState = FilterState.Stopped;
                if (_mediaControl != null)
                {
                    try
                    {
                        _mediaControl.GetState(1000, out runState);
                    }
                    catch
                    {
                        ;
                    }
                }
                return (PlayState)runState;
            }
        }

        /// <summary>
        /// 视频长度
        /// </summary>
        public double VideoLen
        {
            get
            {
                var time = 0.0;
                if (_mediaPosition != null)
                {
                    _mediaPosition.get_Duration(out time);
                }
                return time;
            }
        }

        /// <summary>
        /// 当前音量
        /// </summary>
        public int Volumn
        {
            get
            {
                var vol = 0;
                if (_basicAudio != null)
                {
                    _basicAudio.get_Volume(out vol);
                    vol = (int)(100 + vol * 0.01);
                }
                return vol;
            }
            set
            {
                if (_basicAudio != null)
                {
                    var vol = (value - 100) * 100;
                    _basicAudio.put_Volume(vol);
                }
            }
        }

        public PlayerCore() => _dbClickTime = NativeMethod.GetDoubleClickTime();

        /// <summary>
        /// 播放或者继续
        /// </summary>
        public void Play()
        {
            _isInStop = false;
            PlayState playstate = PlayState;
            if (playstate == PlayState.Running)
            {
                return;
            }
            if (playstate == PlayState.Paused && _mediaControl != null)
            {
                _mediaControl.Run();
                SystemInfo.StartBackGroundThread("播放异步更新时间", UpcontrolTime);
                return;
            }
            if (string.IsNullOrWhiteSpace(FileName))
            {
                return;
            }
            ReleaseComObject();
            InternalPlay();
            SystemInfo.StartBackGroundThread("播放异步更新时间", UpcontrolTime);
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        private void UpcontrolTime()
        {
            var active = false;
            NativeMethod.SystemParametersInfo(NativeMethod.SPI_GETSCREENSAVEACTIVE, false, ref active, NativeMethod.SPIF_SENDWININICHANGE);
            NativeMethod.SystemParametersInfo(NativeMethod.SPI_SETSCREENSAVEACTIVE, false, ref active, NativeMethod.SPIF_SENDWININICHANGE);
            while (true)
            {
                Thread.Sleep(1000);
                if (_mediaPosition == null) break;
                _mediaPosition.get_CurrentPosition(out var time);
                CurrentPosition = time;
                if (Playing != null && !IsDisposed)
                {
                    //防止系统待机
                    NativeMethod.SetThreadExecutionState(NativeMethod.ES_CONTINUOUS | NativeMethod.ES_DISPLAY_REQUIRED | NativeMethod.ES_SYSTEM_REQUIRED);
                    try
                    {
                        Invoke(new MethodInvoker(() => Playing(time)));
                    }
                    catch (InvalidOperationException ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        Log.RecordLog(ex.ToString());
                    }
                    PlayState state = PlayState;
                    if (state == PlayState.Stopped)
                    {
                        ReleaseComObject();
                    }
                    if (state == PlayState.Stopped || state == PlayState.Paused)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            //恢复系统可以待机
            NativeMethod.SystemParametersInfo(NativeMethod.SPI_SETSCREENSAVEACTIVE, active, ref active, NativeMethod.SPIF_SENDWININICHANGE);
            NativeMethod.SetThreadExecutionState(NativeMethod.ES_CONTINUOUS);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (_mediaControl != null && !_isInStop)
            {
                _isInStop = true;
                try
                {
                    _mediaControl.Stop();
                }
                catch (Exception ex)
                {
                    Log.RecordLog(ex.ToString());
                }
            }
            //if (_timeThread == null) return;
            //_timeThread.Abort();
            //_timeThread = null;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (_mediaControl == null) return;
            _mediaControl.Pause();
            //_timeThread.Abort();
            //_timeThread = null;
        }

        private int _muteVolumn;
        /// <summary>
        /// 静音
        /// </summary>
        public void SetMute()
        {
            if (_basicAudio == null) return;
            _basicAudio.get_Volume(out _muteVolumn);
            _basicAudio.put_Volume(-10000);
        }

        /// <summary>
        /// 静音恢复
        /// </summary>
        public void ResumeMute()
        {
            if (_basicAudio != null)
            {
                _basicAudio.put_Volume(_muteVolumn);
            }
        }

        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="rate"></param>
        public void SetRate(double rate)
        {
            if (_mediaPosition != null)
            {
                _mediaPosition.put_Rate(rate);
            }
        }

        public void SetPitch(double rate)
        {
            var myaudiopitch = _audioPitchFilter as IAudioPitch;
            if (myaudiopitch != null)
            {
                myaudiopitch.setPitchValue(rate);
            }
        }

        /// <summary>
        /// 设置播放位置
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(double position)
        {
            if (_mediaPosition != null)
            {
                _mediaPosition.put_CurrentPosition(position);
            }
        }

        /// <summary>
        /// 向前
        /// </summary>
        public void SetBack()
        {
            if (_mediaPosition != null)
            {
                _mediaPosition.get_CurrentPosition(out var pos);
                {
                    pos = pos > 15 ? pos - 15 : 0;
                    _mediaPosition.put_CurrentPosition(pos);
                }
            }
        }

        /// <summary>
        /// 向后
        /// </summary>
        public void SetForward()
        {
            if (_mediaPosition != null)
            {
                _mediaPosition.get_CurrentPosition(out var pos);
                {
                    pos = pos < VideoLen - 15 ? pos + 15 : VideoLen - 0.5;
                    _mediaPosition.put_CurrentPosition(pos);
                }
            }
        }

        /// <summary>
        /// 释放COM对象
        /// </summary>
        private void ReleaseComObject()
        {
            if (_audioPitchFilter != null)
            {
                Marshal.ReleaseComObject(_audioPitchFilter);
                _audioPitchFilter = null;
            }
            if (_mp4Filter != null)
            {
                Marshal.ReleaseComObject(_mp4Filter);
                _mp4Filter = null;
            }
            if (_mediaSeeking != null)
            {
                Marshal.ReleaseComObject(_mediaSeeking);
                _mediaSeeking = null;
            }
            if (_mediaPosition != null)
            {
                Marshal.ReleaseComObject(_mediaPosition);
                _mediaPosition = null;
            }
            if (_basicAudio != null)
            {
                Marshal.ReleaseComObject(_basicAudio);
                _basicAudio = null;
            }
            if (_basicVideo != null)
            {
                Marshal.ReleaseComObject(_basicVideo);
                _basicVideo = null;
            }
            if (_mediaEventEx != null)
            {
                Marshal.ReleaseComObject(_mediaEventEx);
                _mediaEventEx = null;
            }
            if (_videoWindow != null)
            {
                Marshal.ReleaseComObject(_videoWindow);
                _videoWindow = null;
            }
            if (_mediaControl != null)
            {
                Marshal.ReleaseComObject(_mediaControl);
                _videoWindow = null;
            }
            if (_graphBuilder != null)
            {
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }
        }

        /// <summary>
        /// 播放代码
        /// </summary>
        private void InternalPlay()
        {
            try
            {
                int hr;

                _graphBuilder = (IGraphBuilder)new FilterGraph();


                //音频输出类型：0：默认(Default DirectSound Device)，1：Default WaveOut Device
                if (Util.AudioType == 1)
                {
                    var audiodv = new Guid("E30629D1-27E5-11CE-875D-00608CB78066");
                    var audiodvt = Type.GetTypeFromCLSID(audiodv);
                    var audioFilter = (IBaseFilter)Activator.CreateInstance(audiodvt);
                    _graphBuilder.AddFilter(audioFilter, "Default WaveOut Device");
                }
                //禁用声音加速  解决某些学员机器不支持 变调的功能
                if (!Util.IsNotUseSpeed)
                {
                    //变调器
                    var guid = new Guid("5811086B-1D07-4ce9-A398-8DEC9A2DBD89");
                    var comtype = Type.GetTypeFromCLSID(guid);
                    _audioPitchFilter = (IBaseFilter)Activator.CreateInstance(comtype);
                    _graphBuilder.AddFilter(_audioPitchFilter, "Audio Pitch");

                    //变调器
                    var myaudiopitch = (IAudioPitch)_audioPitchFilter;
                    myaudiopitch.setPitchValue(0);
                    myaudiopitch.setLicenseKey(5212);
                }
                //解码器
                var mp4Guid = new Guid(Mp4DemuxFilterType);
                var mp4Comtype = Type.GetTypeFromCLSID(mp4Guid);
                _mp4Filter = (IBaseFilter)Activator.CreateInstance(mp4Comtype);
                _graphBuilder.AddFilter(_mp4Filter, "MP4Filter");

                //部分机器不能正确创建 h264和aac的解码器，需要手动加进去
                Version winver = Environment.OSVersion.Version;
                IBaseFilter aacFilter, h264Filter;
                if (winver.Major == 5 || (winver.Major == 6 && winver.Minor == 0) || Util.IsUseffDshow)
                {
                    //手动加载ffdshow
                    var ffa = new Guid("0F40E1E5-4F79-4988-B1A9-CC98794E6B55");
                    var ffat = Type.GetTypeFromCLSID(ffa);
                    aacFilter = (IBaseFilter)Activator.CreateInstance(ffat);
                    _graphBuilder.AddFilter(aacFilter, "ffdshow Audio Decoder");

                    var ffv = new Guid("04FE9017-F873-410E-871E-AB91661A4EF7");
                    var ffvt = Type.GetTypeFromCLSID(ffv);
                    h264Filter = (IBaseFilter)Activator.CreateInstance(ffvt);
                    _graphBuilder.AddFilter(h264Filter, "ffdshow Video Decoder");
                    Trace.WriteLine("采用Video Renderer输出");
                }
                else
                {
                    var msdvdv = new Guid("212690FB-83E5-4526-8FD7-74478B7939CD");
                    var msdvdvt = Type.GetTypeFromCLSID(msdvdv);
                    h264Filter = (IBaseFilter)Activator.CreateInstance(msdvdvt);
                    _graphBuilder.AddFilter(h264Filter, "Microsoft DTV-DVD Video Decoder");
                    Trace.WriteLine("采用Video Renderer输出");
                    var msdvda = new Guid("E1F1A0B8-BEEE-490D-BA7C-066C40B5E2B9");
                    var msdvdat = Type.GetTypeFromCLSID(msdvda);
                    aacFilter = (IBaseFilter)Activator.CreateInstance(msdvdat);
                    _graphBuilder.AddFilter(aacFilter, "Microsoft DTV-DVD Audio Decoder");
                }

                IBaseFilter vrFilter;

                ////采用vmr9作为视频输出
                if (Util.IsUsevmr9)
                {
                    var vr = new VideoMixingRenderer9();
                    vrFilter = (IBaseFilter)vr;
                    _graphBuilder.AddFilter(vrFilter, "Video Mixing Renderer 9");
                    Trace.WriteLine("采用vmr9作为视频输出");
                }
                //else
                //{
                //	var vr = new VideoRendererDefault();
                //	vrFilter = (IBaseFilter)vr;
                //	_graphBuilder.AddFilter(vrFilter, "Video Renderer");
                //}


                //var ad = new AudioRender();
                //var audioFilter = (IBaseFilter)ad;
                //_graphBuilder.AddFilter(audioFilter, "Audio Renderer");

                //IBaseFilter sourseFilter;
                //_graphBuilder.AddSourceFilter(FileName, null, out sourseFilter);
                //ConnectFilter(sourseFilter, _mp4Filter);
                //ConnectFilter(_mp4Filter, h264Filter);
                //ConnectFilter(h264Filter, vrFilter);

                //ConnectFilter(_mp4Filter, _audioPitchFilter,1);
                //ConnectFilter(_audioPitchFilter, audioFilter);

                _graphBuilder.RenderFile(FileName, null);

                _mediaControl = _graphBuilder as IMediaControl;
                _mediaEventEx = _graphBuilder as IMediaEventEx;
                _mediaSeeking = _graphBuilder as IMediaSeeking;
                _mediaPosition = _graphBuilder as IMediaPosition;

                _videoWindow = _graphBuilder as IVideoWindow;
                _basicVideo = _graphBuilder as IBasicVideo;
                _basicAudio = _graphBuilder as IBasicAudio;

                if (_mediaEventEx != null)
                {
                    hr = _mediaEventEx.SetNotifyWindow(Handle, NativeMethod.WMGraphNotify, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                if (_videoWindow != null)
                {
                    try
                    {
                        hr = _videoWindow.put_Owner(Handle);
                        DsError.ThrowExceptionForHR(hr);

                        hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
                        DsError.ThrowExceptionForHR(hr);
                        Application.DoEvents();

                        hr = _videoWindow.get_Width(out var winWidth);
                        hr = _videoWindow.get_Height(out var winHeight);

                        //winWidth -= _rec.right - _rec.left;
                        //winHeight -= _rec.bottom - _rec.top;

                        winWidth = winWidth - SystemInformation.HorizontalResizeBorderThickness * 2;
                        winHeight = winHeight - SystemInformation.VerticalResizeBorderThickness * 2 -
                                    SystemInformation.CaptionHeight;
                        _scaleRate = (double)winWidth / winHeight;

                        //MessageBox.Show(winWidth + " : " + winHeight);

                        ResizeVideoWindow();
                    }
                    catch (Exception ex)
                    {
                        Log.RecordLog(ex.ToString());
                        if (OnError != null) OnError();
                    }
                }
                hr = _mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                if (OnError != null) OnError();
            }
        }

        private void ConnectFilter(IBaseFilter outF, IBaseFilter inF, int idxOut = 0, int idxIn = 0)
        {
            IPin pinOut = DsFindPin.ByDirection(outF, PinDirection.Output, idxOut);
            IPin pinIn = DsFindPin.ByDirection(inF, PinDirection.Input, idxIn);
            var hr = _graphBuilder.Connect(pinOut, pinIn);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// 响应控件大小改变
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_videoWindow != null)
            {
                ResizeVideoWindow();
            }
        }

        private void ResizeVideoWindow()
        {
            if (_videoWindow != null)
            {
                int tw, th, tt, tl;

                var tr = (double)Width / Height;

                if (tr > _scaleRate)
                {
                    th = Height;
                    tw = (int)(Height * _scaleRate);
                    tt = 0;
                    tl = (Width - tw) / 2;
                }
                else
                {
                    tw = Width;
                    th = (int)(Width / _scaleRate);
                    tl = 0;
                    tt = (Height - th) / 2;
                }
                _videoWindow.put_Left(tl);
                _videoWindow.put_Top(tt);
                _videoWindow.put_Width(tw);
                _videoWindow.put_Height(th);
            }
        }

        private long _lastClickTime;

        protected override void WndProc(ref Message m)
        {
            var param = m.WParam.ToInt32();
            if (param == 513 || param == 516 || param == 519)
            {
                var tick = Util.GetNow().Ticks / 10000;
                if (_lastClickTime + _dbClickTime > tick)
                {
                    _lastClickTime = 0;
                    base.OnDoubleClick(EventArgs.Empty);
                }
                else
                {
                    _lastClickTime = tick;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    Stop();
        //    //ReleaseComObject();
        //    Log.RecordLog("ControlDisposed");
        //}
    }
}
