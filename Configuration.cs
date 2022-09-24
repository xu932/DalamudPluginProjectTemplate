using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Enums;
using CottonCollector.Commands.Structures;
using Newtonsoft.Json;

namespace CottonCollector.Config
{
    internal class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showObjects = false;
        public ObjectKind currKind = ObjectKind.None;
        public bool showCameraInfo = false;
        public bool nouse = false;

        public List<CommandSet> commandSets = new(); 
    }
}
