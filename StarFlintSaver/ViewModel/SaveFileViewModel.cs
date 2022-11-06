using StarFlintSaver.Library.Data;
using StarFlintSaver.Windows.Commands;
using System.IO;

namespace StarFlintSaver.Windows.ViewModel
{
    public sealed class SaveFileViewModel : ViewModelBase
    {
        public SaveFileViewModel(ISaveManagerViewModel parentViewModel, SaveFile saveFile)
        {
            ParentViewModel = parentViewModel;
            SaveFile = saveFile;

            SelectFileCommand = new DelegateAsyncCommand(async () => await ParentViewModel.SelectFileAsync(SaveFile.FileName));
            CopyFileCommand = new DelegateAsyncCommand(async() => await ParentViewModel.CopyFileAsync(SaveFile.FileName));
            LoadSaveCommand = new DelegateAsyncCommand(async () => await ParentViewModel.LoadSaveAsync(this));
            DeleteSaveCommand = new DelegateAsyncCommand(async () => await ParentViewModel.DeleteSaveAsync(this));
        }

        public IDelegateAsyncCommand SelectFileCommand { get; }

        public IDelegateAsyncCommand CopyFileCommand { get; }

        public IDelegateAsyncCommand LoadSaveCommand { get; }

        public IDelegateAsyncCommand DeleteSaveCommand { get; }

        public ISaveManagerViewModel ParentViewModel { get; }

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
    }
}
