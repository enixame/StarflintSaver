using System.Threading.Tasks;
using System.Windows.Input;

namespace StarFlintSaver.Windows.Commands
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public interface IDelegateAsyncCommand : IDelegateCommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public interface IDelegateAsyncCommand<in T> : IDelegateCommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }
}
