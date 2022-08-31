using Dalamud.IoC;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace CottonCollector.Config
{
    public class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool ShowName = false;
    }
}
