using StarFlintSaver.Library.Common;
using StarFlintSaver.Windows.Commands;
using StarFlintSaver.Windows.ConcurrentTask;
using StarFlintSaver.Windows.Utils;
using StarFlintSaver.Windows.WindowsFeatures;
using System.Threading.Tasks;
using System.Windows;
using Clipboard = StarFlintSaver.Windows.WindowsFeatures.Clipboard;

namespace StarFlintSaver.Windows.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        private readonly ITaskDispatcher _taskDispatcher;
        public SaveManagerViewModel FileManager { get; }

        public IDelegateCommand WindowsClosingEventCommand { get; }

        public MainViewModel()
        {
            WindowsClosingEventCommand = new DelegateCommand(ClosingAction);

            IClipboard clipboard = new Clipboard();
            ISystemFeatures systemFeatures = new WindowsSystemFeatures(clipboard);
            IConfigurationFileLoader configurationFileLoader = new ConfigurationFileLoader();

            IStarFlintFileInfo starFlintFileInfo = new StarFlintFileInfo(configurationFileLoader);
            IDirectoryManager directoryManager = new DirectoryManager(configurationFileLoader, systemFeatures);

            IJsonDataRepository jsonDataRepository = new JsonDataRepository(starFlintFileInfo);
            IStarFlintFilesManager starFlintFilesManager = new StarFlintFilesManager(directoryManager, starFlintFileInfo);
            IFileSynchronisationProcess fileSynchronisationProcess = new FileSynchronisationProcess(directoryManager, jsonDataRepository);

            _taskDispatcher = new TaskDispatcher();
            _taskDispatcher.OnError += TaskDispatcherOnError;

            FileManager = new SaveManagerViewModel(jsonDataRepository, starFlintFilesManager, fileSynchronisationProcess, directoryManager, _taskDispatcher);
            var task = Task.Run(async () => await FileManager.LoadDataAsync());
            task.ThrowExceptionInUiThread();
        }

        private void ClosingAction(object obj)
        {
            var saveTask = Task.Run(async () => await CloseAndSaveAsync());
            saveTask.Wait();
        }

        private void TaskDispatcherOnError(object sender, System.Exception e)
        {
            MessageBoxHelper.ShowMessage($"An error just occurred: {e.Message}", "Error while executing task", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task CloseAndSaveAsync()
        {
            _taskDispatcher.OnError -= TaskDispatcherOnError;
            await FileManager.CloseAndSaveAsync();
        }
    }
}
