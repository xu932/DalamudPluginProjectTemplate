using System;
using System.Collections.Generic;

using Dalamud.Logging;

namespace CottonCollector.CharacterControl
{
    internal class Commands
    {
        public enum Type {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
            MOUSE_LBTN_DOWN = 3,
            MOUSE_LBTN_UP = 4,
            MOUSE_LBTN_CLICK = 5,
            MOUSE_RBTN_DOWN = 6,
            MOUSE_RBTN_UP = 7,
            MOUSE_RBTN_CLICK = 8,
            MOUSE_MOVE = 9,
        }

        private bool done = true;
        private Command currCommand;

        public Queue<Command> commands;

        public Commands ()
        {
            commands = new Queue<Command>();
        }

        public void Update()
        {
            if (done && commands.Count != 0)
            {
                done = false;
                PluginLog.Log("Exectuing Command");
                currCommand = commands.Dequeue();
                currCommand.Execute();
            }

            if (!done && currCommand != null && currCommand.IsFinished())
            {
                done = true;
                PluginLog.Log("Finished Command");
            }
        }
    }
}
