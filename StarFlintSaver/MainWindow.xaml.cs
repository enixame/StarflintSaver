using StarFlintSaver.Windows.ViewModel;
using System.Threading.Tasks;
using System.Windows;

namespace StarFlintSaver.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var saveTask = Task.Run(async () => await _mainViewModel.CloseAndSaveAsync());
            saveTask.Wait();
        }
    }
}
