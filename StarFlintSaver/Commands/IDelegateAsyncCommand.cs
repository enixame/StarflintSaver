using System.Threading.Tasks;

namespace StarFlintSaver.Windows.Commands
{

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
