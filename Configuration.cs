using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Enums;

using CottonCollector.CharacterControl;

namespace CottonCollector.Config
{
    [Serializable]
    internal class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showObjects = false;
        public ObjectKind currKind = ObjectKind.None;
        public bool showCameraInfo = false;

        public List<Preset> presets = new List<Preset>(); 
    }
}
