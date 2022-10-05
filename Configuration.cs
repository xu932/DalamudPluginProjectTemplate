using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Keys;

using CottonCollector.Commands.Structures;
using CottonCollector.BackgroundInputs;

namespace CottonCollector.Config
{
    using static BgInput;

    [Serializable]
    internal class KeybindConfig
    {
        internal Tuple<Modifier, VirtualKey> moveForward = new(Modifier.NONE, VirtualKey.W);
        internal Tuple<Modifier, VirtualKey> moveBackward = new(Modifier.NONE, VirtualKey.S);
        internal Tuple<Modifier, VirtualKey> moveLeft = new(Modifier.NONE, VirtualKey.A);
        internal Tuple<Modifier, VirtualKey> moveRight = new(Modifier.NONE, VirtualKey.D);
        internal Tuple<Modifier, VirtualKey> moveUpward = new(Modifier.NONE, VirtualKey.SPACE);
        internal Tuple<Modifier, VirtualKey> moveDownward = new(Modifier.CTRL, VirtualKey.SPACE);
        internal Tuple<Modifier, VirtualKey> rotateCameraLeft = new(Modifier.NONE, VirtualKey.LEFT);
        internal Tuple<Modifier, VirtualKey> rotateCameraRight = new(Modifier.NONE, VirtualKey.RIGHT);
        internal Tuple<Modifier, VirtualKey> jump = new(Modifier.NONE, VirtualKey.SPACE);
    }

    internal class CottonCollectorConfig : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public bool showObjects = false;
        public ObjectKind currKind = ObjectKind.None;
        public bool showCameraInfo = false;
        public bool nouse = false;

        internal KeybindConfig keybind = new();

        public List<CommandSet> commandSets = new(); 
    }
}
