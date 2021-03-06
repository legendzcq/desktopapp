using System.Windows;
using System.Windows.Input;

namespace DesktopApp.Utils
{
    /// <summary>
    /// for custom window
    /// </summary>
    internal partial class WindowService
    {
        /// <summary>
        /// 标识 IsBindingToSystemCommands 依赖项属性。
        /// </summary>
        public static readonly DependencyProperty IsBindingToSystemCommandsProperty =
            DependencyProperty.RegisterAttached("IsBindingToSystemCommands", typeof(bool), typeof(WindowService), new PropertyMetadata(default(bool), OnIsBindingToSystemCommandsChanged));

        /// <summary>
        /// 标识 IsDragMoveEnabled 依赖项属性。
        /// </summary>
        public static readonly DependencyProperty IsDragMoveEnabledProperty =
            DependencyProperty.RegisterAttached("IsDragMoveEnabled", typeof(bool), typeof(WindowService), new PropertyMetadata(default(bool), OnIsDragMoveEnabledChanged));

        /// <summary>
        /// 从指定元素获取 IsBindingToSystemCommands 依赖项属性的值。
        /// </summary>
        /// <param name="obj">从中读取属性值的元素。</param>
        /// <returns>从属性存储获取的属性值。</returns>
        public static bool GetIsBindingToSystemCommands(Window obj) => (bool)obj.GetValue(IsBindingToSystemCommandsProperty);

        /// <summary>
        /// 将 IsBindingToSystemCommands 依赖项属性的值设置为指定元素。
        /// </summary>
        /// <param name="obj">对其设置属性值的元素。</param>
        /// <param name="value">要设置的值。</param>
        public static void SetIsBindingToSystemCommands(Window obj, bool value) => obj.SetValue(IsBindingToSystemCommandsProperty, value);

        /// <summary>
        /// 从指定元素获取 IsDragMoveEnabled 依赖项属性的值。
        /// </summary>
        /// <param name="obj">从中读取属性值的元素。</param>
        /// <returns>从属性存储获取的属性值。</returns>
        public static bool GetIsDragMoveEnabled(DependencyObject obj) => (bool)obj.GetValue(IsDragMoveEnabledProperty);

        /// <summary>
        /// 将 IsDragMoveEnabled 依赖项属性的值设置为指定元素。
        /// </summary>
        /// <param name="obj">对其设置属性值的元素。</param>
        /// <param name="value">要设置的值。</param>
        public static void SetIsDragMoveEnabled(DependencyObject obj, bool value) => obj.SetValue(IsDragMoveEnabledProperty, value);

        private static void OnIsBindingToSystemCommandsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var newValue = (bool)args.NewValue;
            if (obj is Window window && newValue)
            {
                var service = new WindowCommandHelper(window);
                service.ActiveCommands();
            }
        }

        private static void OnIsDragMoveEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var oldValue = (bool)args.OldValue;
            var newValue = (bool)args.NewValue;
            if (oldValue == newValue)
                return;

            var target = obj as Window;
            if (newValue)
                target.MouseLeftButtonDown += OnWindowMouseLeftButtonDown;
            else
                target.MouseLeftButtonDown -= OnWindowMouseLeftButtonDown;
        }

        private static void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                (sender as Window).DragMove();
            }
        }
    }
}
