using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.CharacterControl
{
    internal class Commands
    {
        public Queue<Tuple<VirtualKey, int>> commands;

        public Commands ()
        {
            commands = new Queue<Tuple<VirtualKey, int>>();
        }
    }
}
