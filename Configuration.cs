﻿using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects.Enums;

namespace CottonCollector.Config
{
    public class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showObjects = false;
        public bool showCharacterControl = false;
        public ObjectKind currKind = ObjectKind.None;
        public bool showCameraInfo = false;
    }
}
