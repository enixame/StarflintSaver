using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.Utils
{
    internal static class UiThreadDispatcher
    {
        private static readonly SynchronizationContext s_synchronizationContext = new DispatcherSynchronizationContext(Application.Current.Dispatcher, DispatcherPriority.Normal);

        public static void Invoke(Action action) => s_synchronizationContext.Send((obj) => action?.Invoke(), null);

        public static void BeginInvoke(Action action) => s_synchronizationContext.Post((obj) => action?.Invoke(), null);
    }
}
