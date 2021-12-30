using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DownloadClass.Toolkit.Controls
{
    internal class SeekBarSlider : Slider
    {
        private ToolTip? _autoToolTip;
        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip != default)
                {
                    return _autoToolTip;
                }
                _autoToolTip = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this) as ToolTip;
                return _autoToolTip!;
            }
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            FormatAutoToolTipContent();
            ThumbIsDragging = true;
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            FormatAutoToolTipContent();
            ThumbIsDragging = true;
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await Dispatcher.InvokeAsync(() => ThumbIsDragging = false);
            });
        }

        private void FormatAutoToolTipContent() => AutoToolTip.Content = TimeSpan.FromSeconds(double.Parse((AutoToolTip.Content as string)!)).ToString(@"hh\:mm\:ss");

        public bool ThumbIsDragging
        {
            get => (bool)GetValue(ThumbIsDraggingProperty);
            protected set => SetValue(ThumbIsDragingPropertyKey, value);
        }

        protected static readonly DependencyPropertyKey ThumbIsDragingPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ThumbIsDragging), typeof(bool), typeof(SeekBarSlider), new PropertyMetadata(false));

        public static readonly DependencyProperty ThumbIsDraggingProperty = ThumbIsDragingPropertyKey.DependencyProperty;
    }
}
