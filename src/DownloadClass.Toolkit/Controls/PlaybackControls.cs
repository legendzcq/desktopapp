using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using DownloadClass.Toolkit.Services;
using DownloadClass.Toolkit.Utils;

using LibVLCSharp;

using Microsoft.Extensions.DependencyInjection;

using MediaPlayer = LibVLCSharp.MediaPlayer;

namespace DownloadClass.Toolkit.Controls
{
    public class PlaybackControls : ContentControl
    {
        private StreamMediaInput? _currentInput;
        private (WindowState state, WindowStyle style, ResizeMode resizeMode) _preWindowsStatus;
        private bool _isLoaded;
        private Stream? _currentStream;
        private readonly DispatcherTimer _durationTimer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        static PlaybackControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaybackControls), new FrameworkPropertyMetadata(typeof(PlaybackControls)));
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }

        private static readonly LibVLC s_libVLC = ServiceLocator.ServiceProvider.GetRequiredService<LibVLC>();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Loaded += PlaybackControls_Loaded;
            Unloaded += PlaybackControls_Unloaded;

            _durationTimer.Tick += DurationTimer_Tick;
        }

        private void DurationTimer_Tick(object? sender, EventArgs e) => Duration += TimeSpan.FromSeconds(1);

        private void PlaybackControls_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryUtil.CancelNavigatingSound();

            _isLoaded = true;
            IsPlaying = true;
        }

        private void PlaybackControls_Unloaded(object sender, RoutedEventArgs e)
        {
            RegistryUtil.RecoverNavigatingSound();

            if (MediaPlayer == default)
                return;

            MediaPlayer mediaPlayer = MediaPlayer;
            mediaPlayer.LengthChanged -= MediaPlayer_LengthChanged;
            mediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
            mediaPlayer.EndReached -= MediaPlayer_EndReached;
            Source = default;
        }

        private void MediaPlayer_EndReached(object? sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                IsPlaying = false;
                Position = TimeSpan.Zero;
                IsPlaying = true;
            });
        }

        private void MediaPlayer_PositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (!IsSyncing)
                    return;
                Position = Length.Multiply(e.Position);
            });
        }

        private void MediaPlayer_LengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e) =>
            Dispatcher.InvokeAsync(() => Length = TimeSpan.FromMilliseconds(e.Length));

        public MediaPlayer? MediaPlayer
        {
            get => (MediaPlayer?)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        public static readonly DependencyProperty MediaPlayerProperty =
            DependencyProperty.Register(nameof(MediaPlayer), typeof(MediaPlayer), typeof(PlaybackControls), new PropertyMetadata(default, OnMediaPlayerChanged));

        private static void OnMediaPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            if (e.NewValue is MediaPlayer newMediaPlayer)
            {
                control.IsPlaying = newMediaPlayer.IsPlaying;
                control.SpeedRatio = newMediaPlayer.Rate;
                control.Volume = newMediaPlayer.Volume;
                control.IsMuted = newMediaPlayer.Mute;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    newMediaPlayer.LengthChanged += control.MediaPlayer_LengthChanged;
                    newMediaPlayer.PositionChanged += control.MediaPlayer_PositionChanged;
                    newMediaPlayer.EndReached += control.MediaPlayer_EndReached;
                });
            }

            if (e.OldValue is MediaPlayer oldMediaPlayer)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    oldMediaPlayer.LengthChanged -= control.MediaPlayer_LengthChanged;
                    oldMediaPlayer.PositionChanged -= control.MediaPlayer_PositionChanged;
                    oldMediaPlayer.EndReached -= control.MediaPlayer_EndReached;
                });
            }
        }

        private static object CoerceMediaPlayerValue(DependencyObject d, object baseValue)
        {
            PlaybackControls control = (d as PlaybackControls)!;
            return control.MediaPlayer == default ? DependencyProperty.UnsetValue : baseValue;
        }

        public Uri? Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(PlaybackControls), new PropertyMetadata(default, OnSourceChanged, CoerceMediaPlayerValue));

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            control._durationTimer.Stop();
            control.Duration = default;

            if (e.OldValue is Uri oldSource)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                ThreadPool.QueueUserWorkItem(_ => mediaPlayer!.Stop());
                if (control._currentInput != default)
                {
                    control._currentInput.Dispose();
                    control._currentStream!.Dispose();
                }
            }

            if (e.NewValue is Uri source)
            {
                ISourceProvider sourceProvider = ServiceLocator.ServiceProvider.GetRequiredService<ISourceProvider>();
                (var isSuccessful, Stream? stream) = await sourceProvider.TryProvideAsync(source);
                if (isSuccessful)
                {
                    control._currentStream = stream;
                    var input = new StreamMediaInput(stream!);
                    control._currentInput = input;
                    using var media = new Media(s_libVLC, input);
                    MediaPlayer mediaPlayer = control.MediaPlayer!;
                    mediaPlayer.Media = media;
                    if (control._isLoaded)
                        control.IsPlaying = true;
                }
            }
        }

        public int Volume
        {
            get => (int)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(nameof(Volume), typeof(int), typeof(PlaybackControls), new PropertyMetadata(50, OnVolumeChanged, CoerceMediaPlayerValue));

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            if (e.NewValue is int volume)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                ThreadPool.QueueUserWorkItem(_ => mediaPlayer.SetVolume(volume));
                control.IsMuted = volume == 0;
            }
        }

        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register(nameof(IsMuted), typeof(bool), typeof(PlaybackControls), new PropertyMetadata(false, OnIsMutedChanged, CoerceIsMutedValue));

        private static object CoerceIsMutedValue(DependencyObject d, object baseValue)
        {
            if (CoerceMediaPlayerValue(d, baseValue) == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            PlaybackControls control = (d as PlaybackControls)!;
            return baseValue is bool isMuted
                ? isMuted && control.Volume == 0 && control.IsMuted ? DependencyProperty.UnsetValue : baseValue
                : DependencyProperty.UnsetValue;
        }

        private static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            if (e.NewValue is bool isMuted)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Mute = isMuted);
            }
        }

        public TimeSpan Length
        {
            get => (TimeSpan)GetValue(LengthProperty);
            protected set => SetValue(LengthPropertyKey, value);
        }

        public static readonly DependencyPropertyKey LengthPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Length), typeof(TimeSpan), typeof(PlaybackControls), new PropertyMetadata(default(TimeSpan), default, CoerceMediaPlayerValue));

        public static readonly DependencyProperty LengthProperty = LengthPropertyKey.DependencyProperty;

        public TimeSpan Position
        {
            get => (TimeSpan)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(TimeSpan), typeof(PlaybackControls), new PropertyMetadata(default(TimeSpan), OnPositionChanged, CoercePositionValue));

        private static object CoercePositionValue(DependencyObject d, object baseValue)
        {
            baseValue = CoerceMediaPlayerValue(d, baseValue);
            PlaybackControls control = (d as PlaybackControls)!;
            var positon = (TimeSpan)baseValue;

            if (positon > control.Length)
                return control.Length;

            if (positon < TimeSpan.Zero)
                return TimeSpan.Zero;

            return baseValue;
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;
            if (e.NewValue is TimeSpan position)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                TimeSpan interval = position - mediaPlayer.Position * control.Length;
                if (interval >= TimeSpan.FromSeconds(1) || interval <= TimeSpan.FromSeconds(-1))
                {
                    TimeSpan length = control.Length;
                    ThreadPool.QueueUserWorkItem(_ => mediaPlayer.SetPosition((float)(position / length)));
                }
            }
        }

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(PlaybackControls), new FrameworkPropertyMetadata(false, default, CoerceIsPlayingValue));

        private static object CoerceIsPlayingValue(DependencyObject d, object baseValue)
        {
            if (CoerceMediaPlayerValue(d, baseValue) == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            PlaybackControls control = (d as PlaybackControls)!;
            if (baseValue is bool isPlaying)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                if (isPlaying == mediaPlayer.IsPlaying)
                    return isPlaying;
                if (mediaPlayer.Media == default)
                    return DependencyProperty.UnsetValue;

                if (isPlaying)
                {
                    ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Play());
                    control._durationTimer.Start();
                    return isPlaying;
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Pause());
                    control._durationTimer.Stop();
                    return isPlaying;
                }
            }
            else
            {

                throw new NotSupportedException("can't support non-boolean type");
            }
        }

        public bool IsFullScreen
        {
            get => (bool)GetValue(IsFullScreenProperty);
            set => SetValue(IsFullScreenProperty, value);
        }

        public static readonly DependencyProperty IsFullScreenProperty =
            DependencyProperty.Register(nameof(IsFullScreen), typeof(bool), typeof(PlaybackControls), new PropertyMetadata(false, OnIsFullScreenChanged));

        private static void OnIsFullScreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            if (e.NewValue is bool isFullScreen)
            {
                var window = Window.GetWindow(control);
                var isPlayingBackup = control.IsPlaying;
                if (isFullScreen)
                {
                    if (isPlayingBackup)
                        control.IsPlaying = !control.IsPlaying;

                    control._preWindowsStatus = (window.WindowState, window.WindowStyle, window.ResizeMode);
                    //window.ResizeMode = ResizeMode.NoResize;
                    window.WindowStyle = WindowStyle.None;
                    if (window.WindowState == WindowState.Maximized)
                        window.WindowState = WindowState.Normal;
                    window.WindowState = WindowState.Maximized;

                    if (isPlayingBackup)
                        control.IsPlaying = !control.IsPlaying;
                }
                else
                {
                    if (isPlayingBackup)
                        control.IsPlaying = !control.IsPlaying;

                    //window.ResizeMode = control._preWindowsStatus.resizeMode;
                    window.WindowStyle = control._preWindowsStatus.style;
                    window.WindowState = control._preWindowsStatus.state;

                    if (isPlayingBackup)
                        control.IsPlaying = !control.IsPlaying;
                }
            }
        }

        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register(nameof(SpeedRatio), typeof(double), typeof(PlaybackControls), new PropertyMetadata(1.0, OnSpeedRatioChanged, CoerceMediaPlayerValue));

        private static void OnSpeedRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls control = (d as PlaybackControls)!;

            if (e.NewValue is double speedRatio)
            {
                MediaPlayer mediaPlayer = control.MediaPlayer!;
                ThreadPool.QueueUserWorkItem(_ => mediaPlayer.SetRate((float)speedRatio));
            }
        }

        public bool IsSyncing
        {
            get => (bool)GetValue(IsSyncingProperty);
            set => SetValue(IsSyncingProperty, value);
        }

        public static readonly DependencyProperty IsSyncingProperty =
            DependencyProperty.Register(nameof(IsSyncing), typeof(bool), typeof(PlaybackControls), new PropertyMetadata(true));

        public bool Topmost
        {
            get => (bool)GetValue(TopmostProperty);
            set => SetValue(TopmostProperty, value);
        }

        public static readonly DependencyProperty TopmostProperty =
            DependencyProperty.Register(nameof(Topmost), typeof(bool), typeof(PlaybackControls), new PropertyMetadata(false, OnTopmostChanged));

        private static void OnTopmostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlaybackControls? control = (d as PlaybackControls)!;
            if (e.NewValue is bool topmost)
            {
                var window = Window.GetWindow(control);
                window.Topmost = topmost;
            }
        }

        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationPropertyKey, value);
        }

        public static readonly DependencyPropertyKey DurationPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Duration), typeof(TimeSpan), typeof(PlaybackControls), new PropertyMetadata(default));

        public static readonly DependencyProperty DurationProperty = DurationPropertyKey.DependencyProperty;
    }
}