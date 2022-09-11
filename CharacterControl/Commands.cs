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
        private Queue<Command> currCommandQueue = null; // A atomic list of commands.

        public Queue<Queue<Command>> commands;

        public Commands ()
        {
            commands = new Queue<Queue<Command>>();
        }

        public void Update()
        {
            if (done)
            {
                if (currCommandQueue == null || currCommandQueue.Count == 0)
                {
                    if (commands.Count != 0) {
                        currCommandQueue = commands.Dequeue();
                    }
                }

                if (currCommandQueue != null && currCommandQueue.Count != 0)
                {
                    done = false;
                    PluginLog.Log("Exectuing Command");
                    currCommand = currCommandQueue.Dequeue();
                    currCommand.Execute();
                }
            }

            if (!done && currCommand != null && currCommand.IsFinished())
            {
                done = true;
                PluginLog.Log("Finished Command");
            }
        }

        public void KillSwitch()
        {
            commands.Clear();
        }
    }
}
