using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Enums;

namespace CottonCollector.Config
{
    public class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showName = false;
        public bool showTime = false;
        public bool showObjects = false;
        public ObjectKind currKind = ObjectKind.None;
    }
}
