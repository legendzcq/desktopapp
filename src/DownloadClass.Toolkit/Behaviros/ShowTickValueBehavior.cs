using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace DownloadClass.Toolkit.Behaviros
{
    public class ShowTickValueBehavior : Behavior<Slider>
    {
        private Track _track = default!;
        private ToolTip _toolTip = default!;

        public FormatType FormatType
        {
            get => (FormatType)GetValue(FormatTypeProperty);
            set => SetValue(FormatTypeProperty, value);
        }

        public static readonly DependencyProperty FormatTypeProperty =
            DependencyProperty.Register(nameof(FormatType), typeof(FormatType), typeof(ShowTickValueBehavior), new PropertyMetadata(default));

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            _track.MouseMove -= TrackOnMouseMove;
            base.OnDetaching();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
            _track = (Track)AssociatedObject.Template.FindName("PART_Track", AssociatedObject);
            _toolTip = new ToolTip();
            _track.ToolTip = _toolTip;
            _track.MouseMove += TrackOnMouseMove;
        }

        private void TrackOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            Point position = mouseEventArgs.GetPosition(_track);
            var valueFromPoint = _track.ValueFromPoint(position);
            var floorOfValueFromPoint = (int)Math.Floor(valueFromPoint);
            var toolTip = string.Empty;
            switch (FormatType)
            {
                case FormatType.Default:
                    toolTip = $"{floorOfValueFromPoint}";
                    break;
                case FormatType.TimeSpan:
                    toolTip = TimeSpan.FromSeconds(floorOfValueFromPoint).ToString(@"hh\:mm\:ss");
                    break;
                default:
                    break;
            }
            _toolTip.Content = toolTip;
            _toolTip.Placement = PlacementMode.Relative;
            _toolTip.HorizontalOffset = position.X;
            _toolTip.VerticalOffset = position.Y + 16;
        }
    }

    public enum FormatType
    {
        Default = 0,
        TimeSpan
    }
}
