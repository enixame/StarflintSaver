using System;
using System.Windows;
using System.Windows.Interop;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    internal sealed class WindowWrapper : IWin32Window
    {
        private readonly IntPtr _hwnd;

        public static WindowWrapper CurrentWindow => new WindowWrapper(new WindowInteropHelper(Application.Current.MainWindow).Handle);

        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }
    }
}
