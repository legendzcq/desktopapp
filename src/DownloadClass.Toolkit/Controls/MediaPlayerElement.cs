using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using DownloadClass.Toolkit.Behaviros;

using LibVLCSharp;

using Microsoft.Extensions.DependencyInjection;

namespace DownloadClass.Toolkit.Controls
{
    [TemplatePart(Name = nameof(VideoView), Type = typeof(VideoView))]
    [TemplatePart(Name = nameof(PlaybackControls), Type = typeof(PlaybackControls))]
    public class MediaPlayerElement : ContentControl
    {
        private VideoView? VideoView { get; set; }
        private PlaybackControls? PlaybackControls { get; set; }

        static MediaPlayerElement() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaPlayerElement), new FrameworkPropertyMetadata(typeof(MediaPlayerElement)));

        public MediaPlayerElement()
        {
            Loaded += (_, _) =>
            {
                if (PlaybackControls!.Source == default)
                {
                    PlaybackControls.Source = Source;
                }
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VideoView = (GetTemplateChild(nameof(VideoView)) as VideoView)!;
            PlaybackControls = (GetTemplateChild(nameof(PlaybackControls)) as PlaybackControls)!;

            IServiceScope? scope = ServiceLocator.ServiceProvider.CreateScope();
            MediaPlayer mediaPlayer = scope.ServiceProvider.GetRequiredService<MediaPlayer>();
            VideoView.MediaPlayer = mediaPlayer;
            PlaybackControls.MediaPlayer = mediaPlayer;

            SubscribePropertyChanged();
            AddBindings();
        }

        private void AddBindings()
        {
            CommandBindings.Add(new CommandBinding(Commands.TogglePlayPause, (_, _) =>
                PlaybackControls!.IsPlaying = !PlaybackControls!.IsPlaying, (sender, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(Commands.ExitFullScreen, (_, _) =>
                PlaybackControls!.IsFullScreen = false, (sender, e) => e.CanExecute = PlaybackControls!.IsFullScreen == true));
            CommandBindings.Add(new CommandBinding(Commands.GoForward, (_, _) => Position = Position + TimeSpan.FromMinutes(1)));
            CommandBindings.Add(new CommandBinding(Commands.Backup, (_, _) => Position = Position - TimeSpan.FromMinutes(1)));

            InputBindings.Add(new KeyBinding(Commands.TogglePlayPause, Key.Space, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(Commands.ExitFullScreen, Key.Escape, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(Commands.Backup, Key.Left, ModifierKeys.None));
            InputBindings.Add(new KeyBinding(Commands.GoForward, Key.Right, ModifierKeys.None));

            InputBindingBehavior.SetPropagateInputBindingsToWindow(this, true);
        }

        private void SubscribePropertyChanged()
        {
            var isFullScreenDescriptor = DependencyPropertyDescriptor.FromProperty(PlaybackControls.IsFullScreenProperty, typeof(PlaybackControls));
            isFullScreenDescriptor.AddValueChanged(PlaybackControls!, (_, _) => IsFullScreen = PlaybackControls!.IsFullScreen);

            var durationDescriptor = DependencyPropertyDescriptor.FromProperty(PlaybackControls.DurationProperty, typeof(PlaybackControls));
            durationDescriptor.AddValueChanged(PlaybackControls!, (_, _) => Duration = PlaybackControls!.Duration);

            var lengthDescripter = DependencyPropertyDescriptor.FromProperty(PlaybackControls.LengthProperty, typeof(PlaybackControls));
            lengthDescripter.AddValueChanged(PlaybackControls!, (_, _) => Length = PlaybackControls!.Length);

            SetBinding(PositionProperty, new Binding(nameof(PlaybackControls.Position))
            {
                Source = PlaybackControls,
                Mode = BindingMode.TwoWay
            });
            SetBinding(IsPlayingProperty, new Binding(nameof(PlaybackControls.IsPlaying))
            {
                Source = PlaybackControls,
                Mode = BindingMode.TwoWay
            });
            SetBinding(SpeedRatioProperty, new Binding(nameof(PlaybackControls.SpeedRatio))
            {
                Source = PlaybackControls,
                Mode = BindingMode.TwoWay
            });
        }

        [TypeConverter(typeof(UriTypeConverter))]
        public Uri? Source
        {
            get => (Uri?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(MediaPlayerElement), new PropertyMetadata(default, OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement control = (d as MediaPlayerElement)!;
            if (control.PlaybackControls != default)
            {
                control.PlaybackControls.Source = e.NewValue as Uri;
            }
        }

        public bool IsFullScreen
        {
            get => (bool)GetValue(PlaybackControls.IsFullScreenProperty);
            protected set => SetValue(IsFullScreenPropertyKey, value);
        }

        public static readonly DependencyPropertyKey IsFullScreenPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsFullScreen), typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(false));

        public static readonly DependencyProperty IsFullScreenProperty = IsFullScreenPropertyKey.DependencyProperty;

        public event RoutedPropertyChangedEventHandler<TimeSpan> PositionChanged
        {
            add { AddHandler(PositonChangedEvent, value); }
            remove { RemoveHandler(PositonChangedEvent, value); }
        }

        public static readonly RoutedEvent PositonChangedEvent = EventManager.RegisterRoutedEvent(nameof(PositonChangedEvent), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<TimeSpan>), typeof(MediaPlayerElement));

        public TimeSpan Position
        {
            get => (TimeSpan)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(TimeSpan), typeof(MediaPlayerElement), new PropertyMetadata(default(TimeSpan), OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement control = (d as MediaPlayerElement)!;
            if (e.NewValue is TimeSpan position)
            {
                control.RaiseEvent(new RoutedPropertyChangedEventArgs<TimeSpan>((TimeSpan)e.OldValue, position, PositonChangedEvent));
            }
        }

        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationPropertyKey, value);
        }

        public static readonly DependencyPropertyKey DurationPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Duration), typeof(TimeSpan), typeof(MediaPlayerElement), new PropertyMetadata(default));

        public static readonly DependencyProperty DurationProperty = DurationPropertyKey.DependencyProperty;

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(default));

        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register(nameof(SpeedRatio), typeof(double), typeof(MediaPlayerElement), new PropertyMetadata(default));

        public TimeSpan Length
        {
            get => (TimeSpan)GetValue(LengthProperty);
            protected set => SetValue(LengthPropertyKey, value);
        }

        public static readonly DependencyPropertyKey LengthPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Length), typeof(TimeSpan), typeof(MediaPlayerElement), new PropertyMetadata(default));

        public static DependencyProperty LengthProperty = LengthPropertyKey.DependencyProperty;
    }
}
