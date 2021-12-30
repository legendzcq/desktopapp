using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Xaml.Behaviors;

namespace DownloadClass.Toolkit.Behaviros
{
    public class AutoHideContentBehavior : Behavior<Border>
    {

        private readonly DispatcherTimer _dispatcherTimer = new();

        [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval
        {
            get => (TimeSpan)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(TimeSpan), typeof(AutoHideContentBehavior), new PropertyMetadata(TimeSpan.FromSeconds(5), OnIntervalChanged));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoHideContentBehavior? behavior = (d as AutoHideContentBehavior)!;
            if (e.NewValue is TimeSpan interval)
                behavior._dispatcherTimer.Interval = interval;
        }

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AutoHideContentBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoHideContentBehavior behavior = (d as AutoHideContentBehavior)!;

            if (e.NewValue is bool isEnabled && behavior.AssociatedObject.Child is UIElement content)
            {
                if (isEnabled)
                {
                    behavior._dispatcherTimer.Start();
                }
                else
                {
                    behavior._dispatcherTimer.Stop();
                    content.Visibility = Visibility.Visible;
                }

            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            _dispatcherTimer.Interval = Interval;
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            if (IsEnabled)
                _dispatcherTimer.Start();

            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (AssociatedObject.Child is UIElement content)
            {
                content.Visibility = Visibility.Visible;
                _dispatcherTimer.Start();
            }
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (AssociatedObject.Child is UIElement content)
            {
                content.Visibility = Visibility.Visible;
                _dispatcherTimer.Stop();
            }
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (AssociatedObject.Child is UIElement content)
            {
                content.Visibility = Visibility.Hidden;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            _dispatcherTimer.Tick -= DispatcherTimer_Tick;
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
}
