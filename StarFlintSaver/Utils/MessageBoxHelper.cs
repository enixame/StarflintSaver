using System;
using System.Windows;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.Utils
{
    public static class MessageBoxHelper
    {
        public static MessageBoxResult ShowMessage(
            string message,
            string caption,
            MessageBoxButton button,
            MessageBoxImage image)
        {
            var currentDispatcher = Application.Current.Dispatcher;
            bool checkAccess = currentDispatcher.CheckAccess();

            MessageBoxResult result = MessageBoxResult.None;
            if (!checkAccess)
            {
                currentDispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    result = ShowMessageBox(message, caption, button, image);
                }));
            }
            else
            {
                result = ShowMessageBox(message, caption, button, image);
            }

            return result;
        }

        private static MessageBoxResult ShowMessageBox(
            string message,
            string caption,
            MessageBoxButton button,
            MessageBoxImage image)
        {
            return Application.Current.MainWindow != null
                ? MessageBox.Show(Application.Current.MainWindow, message, caption, button, image)
                : MessageBox.Show(message, caption, button, image);
        }
    }
}
