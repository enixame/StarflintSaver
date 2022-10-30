﻿using StarFlintSaver.Library.Common;
using StarFlintSaver.Library.Data;
using StarFlintSaver.Windows.Commands;
using StarFlintSaver.Windows.ConcurrentTask;
using StarFlintSaver.Windows.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.ViewModel
{
    public sealed class SaveManagerViewModel : ViewModelBase
    {
        private readonly IJsonDataRepository _jsonDataRepository;
        private readonly IStarFlintFilesManager _starFlintFilesManager;
        private readonly IFileSynchronisationProcess _fileSynchronisationProcess;
        private readonly ITaskDispatcher _taskDispatcher;

        private readonly ViewModelAnimationManager<SaveManagerViewModel> _messageAnimationManager;
        private readonly ViewModelAnimationManager<SaveManagerViewModel> _saveAnimationManager;

        private SaveFileViewModel _saveFileViewModel;
        private string _message;
        private string _saveMessage;
        private bool _messageAnimationStarting;
        private bool _savingInProgress;
        private bool _resyncProcessIsRuning;

        public SaveManagerViewModel(
            IJsonDataRepository jsonDataRepository,
            IStarFlintFilesManager starFlintFilesManager,
            IFileSynchronisationProcess fileSynchronisationProcess,
            ITaskDispatcher taskDispatcher)
        {
            _jsonDataRepository = jsonDataRepository;
            _starFlintFilesManager = starFlintFilesManager;
            _fileSynchronisationProcess = fileSynchronisationProcess;
            _taskDispatcher = taskDispatcher;

            _messageAnimationManager = new ViewModelAnimationManager<SaveManagerViewModel>(5, (viewModel) => viewModel.MessageAnimationStarting, (ViewModel) => ViewModel.Message);
            _saveAnimationManager = new ViewModelAnimationManager<SaveManagerViewModel>(2, (viewModel) => viewModel.SavingInProgress, (ViewModel) => ViewModel.SaveMessage);

            CreateSaveCommand = new DelegateAsyncCommand(CreateSaveAsync, () => !ResyncProcessIsRunning);
            LoadSaveCommand = new DelegateAsyncCommand(LoadSaveAsync, () => SelectedSaveFile != null && !ResyncProcessIsRunning);
            DeleteSaveCommand = new DelegateAsyncCommand(DeleteSaveAsync, () => SelectedSaveFile != null && !ResyncProcessIsRunning);
            ResyncCommand = new DelegateAsyncCommand(ResyncSaveFilesAsync, () => !ResyncProcessIsRunning);
        }

        public ObservableCollection<SaveFileViewModel> SaveFiles { get; } = new ObservableCollection<SaveFileViewModel>();

        public string TotalSaveFilesCountDescription => $"Total saves: {SaveFiles.Count}";

        public string Message
        {
            get
            {
                return _message;
            }

            private set
            {
                _message = value;
                NotifyPropertyChanged(nameof(Message));
            }
        }

        public bool MessageAnimationStarting
        {
            get
            {
                return _messageAnimationStarting;
            }

            private set
            {
                _messageAnimationStarting = value;
                NotifyPropertyChanged(nameof(MessageAnimationStarting));
            }
        }

        public string SaveMessage
        {
            get
            {
                return _saveMessage;
            }

            private set
            {
                _saveMessage = value;
                NotifyPropertyChanged(nameof(SaveMessage));
            }
        }

        public bool SavingInProgress
        {
            get
            {
                return _savingInProgress;
            }

            private set
            {
                _savingInProgress = value;
                NotifyPropertyChanged(nameof(SavingInProgress));
            }
        }

        public bool ResyncProcessIsRunning
        {
            get
            {
                return _resyncProcessIsRuning;
            }

            private set
            {
                _resyncProcessIsRuning = value;

                CreateSaveCommand.RaiseCanExecuteChanged();
                LoadSaveCommand.RaiseCanExecuteChanged();
                DeleteSaveCommand.RaiseCanExecuteChanged();
                ResyncCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged(nameof(ResyncProcessIsRunning));
            }
        }

        public SaveFileViewModel SelectedSaveFile
        {
            get
            {
                return _saveFileViewModel;
            }

            set
            {
                _saveFileViewModel = value;

                LoadSaveCommand.RaiseCanExecuteChanged();
                DeleteSaveCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged(nameof(SelectedSaveFile));
            }
        }

        public IDelegateAsyncCommand CreateSaveCommand { get; }

        public IDelegateAsyncCommand LoadSaveCommand { get; }

        public IDelegateAsyncCommand DeleteSaveCommand { get; }

        public IDelegateAsyncCommand ResyncCommand { get; }

        private async Task CreateSaveAsync()
        {
            await Task.Run(async () =>
            {
                var fileInfo = _starFlintFilesManager.CopyFromCurrentSaveFile();
                var saveFile = new SaveFile()
                {
                    Date = fileInfo.CreationTime,
                    FileName = fileInfo.FullName,
                    Description = string.Empty,
                };
                _jsonDataRepository.AddSaveFile(saveFile);

            _messageAnimationManager.StartAnimation(this, "Save created !");

                await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    var saveFileViewModel = new SaveFileViewModel(this, saveFile);
                    saveFileViewModel.PropertyChanged += SaveFileViewModelPropertyChanged;
                    SaveFiles.Add(saveFileViewModel);
                    NotifyPropertyChanged(nameof(SaveFiles));
                    NotifyPropertyChanged(nameof(TotalSaveFilesCountDescription));
                    SelectedSaveFile = saveFileViewModel;
                }));

                _taskDispatcher.ExecuteTask(SaveDataAsync);
            });
        }

        private async Task LoadSaveAsync()
        {
            _messageAnimationManager.StartAnimation(this, "Save loaded !");

            await Task.Run(() =>
            {
                _starFlintFilesManager.LoadSave(SelectedSaveFile.FileName);
            });
        }

        private async Task DeleteSaveAsync()
        {
            var messageBoxResult = MessageBoxHelper.ShowMessage($"Do you want to delete {SelectedSaveFile.SaveFileInfo} ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }

            _starFlintFilesManager.DeleteSave(SelectedSaveFile.FileName);
            _jsonDataRepository.DeleteSaveFile(SelectedSaveFile.SaveFile);

            await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                SelectedSaveFile.PropertyChanged -= SaveFileViewModelPropertyChanged;
                SaveFiles.Remove(SelectedSaveFile);
                NotifyPropertyChanged(nameof(SaveFiles));
                NotifyPropertyChanged(nameof(TotalSaveFilesCountDescription));
                SelectedSaveFile = null;
            }));

            _taskDispatcher.ExecuteTask(SaveDataAsync);
            _messageAnimationManager.StartAnimation(this, "Save deleted !");
        }

        private async Task ResyncSaveFilesAsync()
        {
            ResyncProcessIsRunning = true;

            try
            {
                var allFiles = await _fileSynchronisationProcess.GetAllFilesFromRootDirectoryAsync();

                await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    _jsonDataRepository.SaveFiles.Clear();

                    foreach (var savefileViewModel in SaveFiles)
                    {
                        savefileViewModel.PropertyChanged -= SaveFileViewModelPropertyChanged;
                    }
                    SaveFiles.Clear();

                    foreach (var saveFile in allFiles)
                    {
                        _jsonDataRepository.AddSaveFile(saveFile);
                        var saveFileViewModel = new SaveFileViewModel(this, saveFile);
                        saveFileViewModel.PropertyChanged += SaveFileViewModelPropertyChanged;
                        SaveFiles.Add(saveFileViewModel);
                    }

                    NotifyPropertyChanged(nameof(SaveFiles));
                    NotifyPropertyChanged(nameof(TotalSaveFilesCountDescription));
                }));

                await SaveDataAsync();

            }
            finally
            {
                ResyncProcessIsRunning = false;
            }
        }

        public async Task LoadDataAsync()
        {
            var saveFiles = await _jsonDataRepository.LoadFromJsonDataAsync();

            await UiDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (var savefile in saveFiles)
                {
                    var viewModel = new SaveFileViewModel(this, savefile);
                    viewModel.PropertyChanged += SaveFileViewModelPropertyChanged;
                    SaveFiles.Add(viewModel);
                }

                NotifyPropertyChanged(nameof(SaveFiles));
                NotifyPropertyChanged(nameof(TotalSaveFilesCountDescription));
            }));
        }

        private async Task SaveDataAsync()
        {
            if (SaveFiles.Count > 0)
            {
                _saveAnimationManager.StartAnimation(this, "Saving...");
               
                await _jsonDataRepository.SaveAsJsonDataAsync(); 
            }
        }

        private async Task StartAnimationAsync(Action<bool> actionToRun, int animationDurationInSeconds)
        {
            actionToRun?.Invoke(true);

            var duration = TimeSpan.FromSeconds(animationDurationInSeconds);

            await Task.Delay(duration);
            actionToRun?.Invoke(false);
        }

        public async Task CloseAndSaveAsync()
        {
            _taskDispatcher.Dispose();
            await SaveDataAsync();
        }

        private void SaveFileViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(SaveFileViewModel.Description), StringComparison.Ordinal))
            {
                _taskDispatcher.ExecuteTask(SaveDataAsync);
            }
        }
    }
}