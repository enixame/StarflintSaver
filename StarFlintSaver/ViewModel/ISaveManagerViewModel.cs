using System.Threading.Tasks;

namespace StarFlintSaver.Windows.ViewModel
{
    public interface ISaveManagerViewModel
    {
        void SelectFile(string filePath);

        void CopyFile(string filePath);

        Task LoadSaveAsync(SaveFileViewModel saveFileViewModel);

        Task DeleteSaveAsync(SaveFileViewModel saveFileViewModel);
    }
}
