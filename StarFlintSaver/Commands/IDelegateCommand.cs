using System.Windows.Input;

namespace StarFlintSaver.Windows.Commands
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
