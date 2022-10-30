using StarFlintSaver.Windows.Utils;
using System.Windows;

namespace StarFlintSaver.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxHelper.ShowMessage($"An unhandled exception just occurred: {e.Exception.Message}", "Unhandled error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
