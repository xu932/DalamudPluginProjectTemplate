using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Enums;

namespace CottonCollector.Config
{
    public class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showObjects = false;
        public bool showCharacterControl = false;
        public ObjectKind currKind = ObjectKind.None;
    }
}
