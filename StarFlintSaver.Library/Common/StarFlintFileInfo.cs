using System;
using System.IO;

namespace StarFlintSaver.Library.Common
{
    public sealed class StarFlintFileInfo : IStarFlintFileInfo
    {
        public StarFlintFileInfo(IConfigurationFileLoader configurationFileLoader)
        {
            var configuration = configurationFileLoader.GetConfiguration();

            StarFlintRootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "PANTANG Studio", "StarFlint");
            StarFlintSaveFileName = Path.Combine(StarFlintRootFolder, configuration.StarFlintSaveFileName);
        }

        public string StarFlintRootFolder { get; }

        public string StarFlintSaveFileName { get; }
    }
}
