using StarFlintSaver.Windows.Commands;
using System.Threading.Tasks;

namespace StarFlintSaver.Windows.ViewModel
{
    public interface ISaveManagerViewModel
    {
        IDelegateAsyncCommand CreateSaveCommand { get; }

        IDelegateAsyncCommand OpenRootFolderCommand { get; }

        Task SelectFileAsync(string filePath);

        Task CopyFileAsync(string filePath);

        Task LoadSaveAsync(SaveFileViewModel saveFileViewModel);

        Task DeleteSaveAsync(SaveFileViewModel saveFileViewModel);
    }
}
