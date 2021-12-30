using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

namespace DesktopApp.Controls
{
    public class IconButton : Button
    {
        static IconButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));

        public PackIconMaterialKind? Kind
        {
            get => (PackIconMaterialKind?)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(KindProperty), typeof(PackIconMaterialKind), typeof(IconButton), new PropertyMetadata(default));
    }
}
