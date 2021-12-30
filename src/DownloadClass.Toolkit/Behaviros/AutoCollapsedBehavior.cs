using System;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace DownloadClass.Toolkit.Behaviros
{
    internal class AutoCollapsedBehavior : Behavior<UIElement>
    {
        private Visibility _backup;

        protected override void OnAttached()
        {
            base.OnAttached();

            _backup = AssociatedObject.Visibility;

            if (Environment.OSVersion.Version.Major < 10)
                AssociatedObject.Visibility = Visibility.Collapsed;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Visibility = _backup;
        }
    }
}
