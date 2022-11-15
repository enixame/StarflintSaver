using StarFlintSaver.Library.Data;

namespace StarFlintSaver.Library.Common
{
    public interface IConfigurationFileLoader
    {
        StarFlintSaverConfiguration GetConfiguration();

        void UpdateStarFlintSaverBaseDirectory(string starFlintSaverBaseDirectory);
    }
}
