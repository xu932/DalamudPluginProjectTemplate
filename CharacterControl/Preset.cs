using System;
using System.Collections.Generic;

using CottonCollector.CharacterControl.Commands;

namespace CottonCollector.CharacterControl
{
    [Serializable]
    internal class Preset
    {
        public string name { get; private set; }
        public CommandTreeNode presetRoot = new CommandTreeNode();

        public Preset(string name) {
            this.name = name;
        }
    }
}
