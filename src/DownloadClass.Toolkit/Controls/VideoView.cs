using System.Windows;
using System.Windows.Controls;

using LibVLCSharp;

namespace DownloadClass.Toolkit.Controls
{
    [TemplatePart(Name = nameof(PART_VideoView), Type = typeof(LibVLCSharp.WPF.VideoView))]
    public class VideoView : ContentControl
    {
        private LibVLCSharp.WPF.VideoView? PART_VideoView { get; set; }

        static VideoView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoView), new FrameworkPropertyMetadata(typeof(VideoView)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_VideoView = (GetTemplateChild(nameof(PART_VideoView)) as LibVLCSharp.WPF.VideoView)!;
            if (MediaPlayer != default && PART_VideoView.MediaPlayer == default)
                PART_VideoView.MediaPlayer = MediaPlayer;

            Unloaded += VideoView_Unloaded;
        }

        private void VideoView_Unloaded(object sender, RoutedEventArgs e)
        {
            PART_VideoView?.Dispose();
            MediaPlayer?.Dispose();
        }

        public MediaPlayer? MediaPlayer
        {
            get => (MediaPlayer?)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        public static readonly DependencyProperty MediaPlayerProperty =
            DependencyProperty.Register(nameof(MediaPlayer), typeof(MediaPlayer), typeof(VideoView), new PropertyMetadata(default, OnMediaPlayerChanged));

        private static void OnMediaPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoView control = (d as VideoView)!;
            if (control.PART_VideoView is null)
                return;
            if (e.NewValue is MediaPlayer newMediaPlayer)
                control.PART_VideoView.MediaPlayer = newMediaPlayer;
        }
    }
}
