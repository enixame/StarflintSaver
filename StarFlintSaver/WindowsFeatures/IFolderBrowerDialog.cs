using System;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    public interface IFolderBrowerDialog
    {
        string InitialFolder { get; set; }
        string DefaultFolder { get; set; }
        string SelectFolder { get; }
        DialogResult ShowDialog();
        DialogResult ShowDialog(IntPtr owner);
    }
}
