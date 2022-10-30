using StarFlintSaver.Library.Data;
using System.IO;

namespace StarFlintSaver.Windows.ViewModel
{
    public sealed class SaveFileViewModel : ViewModelBase
    {
        public SaveFileViewModel(ViewModelBase parentViewModel, SaveFile saveFile)
        {
            ParentViewModel = parentViewModel;
            SaveFile = saveFile;
        }

        public ViewModelBase ParentViewModel { get; }
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
