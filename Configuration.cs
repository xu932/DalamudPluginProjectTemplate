using Dalamud.IoC;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace CottonCollector
{
    public class Configuration : IPluginConfiguration
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        int IPluginConfiguration.Version { get; set; }

        public void Save()
        {
            DalamudPluginInterface.SavePluginConfig(this);
        }
    }
}
