using StarFlintSaver.Windows.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.Commands
{
    public class DelegateCommand : IDelegateCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
            // do Nothing
        }

        public void RaiseCanExecuteChanged()
        {
            RaiseEventInUiThread(CanExecuteChanged);
        }

        private void RaiseEventInUiThread(EventHandler eventHandler)
        {
            var currentDispatcher = Application.Current.Dispatcher;
            bool checkAccess = currentDispatcher.CheckAccess();

            if (!checkAccess)
            {
                currentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    eventHandler?.Invoke(this, EventArgs.Empty);
                }));
            }
            else
            {
                eventHandler?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public sealed class DelegateAsyncCommand : DelegateCommand, IDelegateAsyncCommand
    {
        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public DelegateAsyncCommand(Func<Task> execute)
            : this(execute, null)
        {
        }

        public DelegateAsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            var task = Task.Run(async () => await ExecuteAsync());
            task.ThrowExceptionInUiThread();
        }

        #endregion
    }

    public sealed class DelegateAsyncCommand<T> : DelegateCommand, IDelegateAsyncCommand<T>
    {
        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public DelegateAsyncCommand(Func<T, Task> execute)
            : this(execute, null)
        {
        }

        public DelegateAsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            var task = Task.Run(async () => await ExecuteAsync((T)parameter));
            task.ThrowExceptionInUiThread();
        }

        #endregion
    }
}
