using StarFlintSaver.Library.Data;
using StarFlintSaver.Windows.Commands;
using System.IO;
using System.Threading.Tasks;

namespace StarFlintSaver.Windows.ViewModel
{
    public sealed class SaveFileViewModel : ViewModelBase
    {
        public SaveFileViewModel(IFileSystemFeaturesViewModel parentViewModel, SaveFile saveFile)
        {
            ParentViewModel = parentViewModel;
            SaveFile = saveFile;

            SelectFileCommand = new DelegateAsyncCommand(SelectFileCommandAsync);
        }

        public IDelegateAsyncCommand SelectFileCommand { get; }

        public IFileSystemFeaturesViewModel ParentViewModel { get; }

        public SaveFile SaveFile { get; }

        public string FileName => SaveFile.FileName;

        public string ShortFileName => Path.GetFileName(SaveFile.FileName);

        public string SaveFileInfo => $"Save of {SaveFile.Date:dd/MM/yyyy HH:mm:ss}";

        public string Description
        {
            get
            {
                return SaveFile.Description;
            }

            set
            {
                SaveFile.Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private async Task SelectFileCommandAsync()
        {
            await Task.Run(() => ParentViewModel.SelectFile(SaveFile.FileName));
        }
    }
}
