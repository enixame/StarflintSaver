using System;
using System.IO;

namespace StarFlintSaver.Library.Common
{
    public sealed class StarFlintFileInfo : IStarFlintFileInfo
    {
        private const string DefaultSaveFileName = "StarFlint_save_0.save";

        public StarFlintFileInfo()
        {
            StarFlintRootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "PANTANG Studio", "StarFlint");
            StarFlintSaveFileName = Path.Combine(StarFlintRootFolder, DefaultSaveFileName);
        }

        public string StarFlintRootFolder { get; }

        public string StarFlintSaveFileName { get; }
    }
}
