using System;
using System.Numerics;
using System.Collections.Generic;

using Dalamud.Logging;

using Dalamud.Game.ClientState.Keys;

using CottonCollector.CameraManager;
using CottonCollector.CharacterControl.Commands;

namespace CottonCollector.CharacterControl
{
    using AtomicCommand = Queue<Command>;

    internal unsafe class CommandManager
    {
        private bool done = true;
        private Command currCommand;
        private AtomicCommand currCommandQueue = null; // A atomic list of commands.

        public Queue<AtomicCommand> commands;

        public CommandManager ()
        {
            commands = new Queue<AtomicCommand>();
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
            currCommandQueue = null;
            CottonCollectorPlugin.KeyState.ClearAll();
            done = true;
        }
    }
}
