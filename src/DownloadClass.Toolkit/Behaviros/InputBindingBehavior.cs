using System.Windows;
using System.Windows.Input;

namespace DownloadClass.Toolkit.Behaviros
{
    public static class InputBindingBehavior
    {
        public static bool GetPropagateInputBindingsToWindow(FrameworkElement frameworkElement)
        {
            return frameworkElement == null
                ? throw new System.ArgumentNullException(nameof(frameworkElement))
                : (bool)frameworkElement.GetValue(PropagateInputBindingsToWindowProperty);
        }

        public static void SetPropagateInputBindingsToWindow(FrameworkElement frameworkElement, bool value)
        {
            if (frameworkElement == null)
            {
                throw new System.ArgumentNullException(nameof(frameworkElement));
            }

            frameworkElement.SetValue(PropagateInputBindingsToWindowProperty, value);
        }

        public static readonly DependencyProperty PropagateInputBindingsToWindowProperty =
            DependencyProperty.RegisterAttached("PropagateInputBindingsToWindow", typeof(bool), typeof(InputBindingBehavior),
            new PropertyMetadata(false, OnPropagateInputBindingsToWindowChanged));

        private static void OnPropagateInputBindingsToWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FrameworkElement)d).Loaded += FrameworkElement_Loaded;

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= FrameworkElement_Loaded;

            var window = Window.GetWindow(frameworkElement);
            if (window == null)
            {
                return;
            }

            for (var i = frameworkElement.InputBindings.Count - 1; i >= 0; i--)
            {
                InputBinding inputBinding = frameworkElement.InputBindings[i];
                window.InputBindings.Add(inputBinding);
                frameworkElement.InputBindings.Remove(inputBinding);
            }
        }
    }
}
