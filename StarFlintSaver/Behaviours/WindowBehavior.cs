using System.Windows;
using System.Windows.Input;

namespace StarFlintSaver.Windows.Behaviours
{
    public static class WindowBehavior
    {
        public static readonly DependencyProperty ClosingCommandActionProperty =
            DependencyProperty.RegisterAttached(
                "ClosingCommandAction",
                typeof(ICommand),
                typeof(WindowBehavior),
                new UIPropertyMetadata(null));

        public static ICommand GetClosingCommandAction(DependencyObject sender) => (ICommand)sender.GetValue(ClosingCommandActionProperty);

        public static void SetClosingCommandAction(DependencyObject sender, ICommand value) => sender.SetValue(ClosingCommandActionProperty, value);

        public static bool GetWindowClosingEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(WindowClosingEnabledProperty);
        }

        public static void SetWindowClosingEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(WindowClosingEnabledProperty, value);
        }

        public static readonly DependencyProperty WindowClosingEnabledProperty =
            DependencyProperty.RegisterAttached("WindowClosingEnabled", typeof(bool), typeof(WindowBehavior),
                new UIPropertyMetadata(false, OnWindowClosingEnabled));

        private static void OnWindowClosingEnabled(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is Window window))
            {
                return;
            }

            if (eventArgs.NewValue is bool == false)
            {
                return;
            }

            if ((bool)eventArgs.NewValue)
            {
                window.Closing += WindowClosing;
                
            }
            else
            {
                window.Closing -= WindowClosing;
            }
        }

        private static void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(sender is Window window))
            {
                return;
            }

            ICommand commandAction = GetClosingCommandAction(window);
            if(commandAction == null)
            {
                return;
            }

            if (commandAction.CanExecute(null))
            {
                commandAction.Execute(null);
            }
        }
    }
}
