using System;
using System.Collections.Generic;

using CottonCollector.CharacterControl.Commands;

namespace CottonCollector.CharacterControl
{
    using AtomicCommand = List<Command>;

    [Serializable]
    internal class Preset
    {
        public string name { get; private set; }
        public AtomicCommand atomicCommands = new AtomicCommand();

        public Preset(string name) {
            this.name = name;
        }
    }
}
