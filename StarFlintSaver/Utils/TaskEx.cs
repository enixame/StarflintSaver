using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.Utils
{
    internal static class TaskEx
    {
        public static void ThrowExceptionInUiThread(this Task task)
        {
            task.ContinueWith(previous =>
            {
                if (previous.Exception != null)
                {
                    var currentDispatcher = Application.Current.Dispatcher;
                    currentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        var exception = previous.Exception;
                        throw exception;
                    }));
                }
            }, default, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
