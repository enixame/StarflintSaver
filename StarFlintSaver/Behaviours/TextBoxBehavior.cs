using System.Windows;
using System.Windows.Controls;

namespace StarFlintSaver.Windows.Behaviours
{
    public static class TextBoxBehavior
    {
        public static bool GetEnforceFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnforceFocusProperty);
        }

        public static void SetEnforceFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(EnforceFocusProperty, value);
        }

        public static readonly DependencyProperty EnforceFocusProperty =
            DependencyProperty.RegisterAttached("EnforceFocus", typeof(bool), typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnTextBoxEnforceFocusChanged));

        private static void OnTextBoxEnforceFocusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is TextBox textBox))
            {
                return;
            }

            if (eventArgs.NewValue is bool == false)
            {
                return;
            }

            if ((bool)eventArgs.NewValue)
            {
                textBox.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(TextBoxLoadedHandler));
            }
            else
            {
                textBox.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(TextBoxLoadedHandler));
            }
        }

        private static void TextBoxLoadedHandler(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textbox))
            {
                return;
            }

            textbox.Focus();
            textbox.CaretIndex = textbox.Text.Length;
        }
    }
}
