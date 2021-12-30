using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace DesktopApp.Infrastructure
{
    public class EnumStateBehavior : Behavior<FrameworkElement>
    {
        public object EnumProperty
        {
            get => GetValue(EnumPropertyProperty);
            set => SetValue(EnumPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnumProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumPropertyProperty =
            DependencyProperty.Register("EnumProperty", typeof(object), typeof(EnumStateBehavior), new UIPropertyMetadata(null, EnumPropertyChanged));

        private static void EnumPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var eb = sender as EnumStateBehavior;

            VisualStateManager.GoToElementState(eb.AssociatedObject, e.NewValue.ToString(), true);
        }

    }
}
