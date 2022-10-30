using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace StarFlintSaver.Windows.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase()
        {
        }

        protected readonly Dispatcher UiDispatcher = Application.Current.Dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Null or Empty", nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
