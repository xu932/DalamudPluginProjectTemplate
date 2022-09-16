using System.Collections.Generic;

using Dalamud.Logging;

using CottonCollector.CharacterControl.Commands;

namespace CottonCollector.CharacterControl
{
    internal unsafe class CommandManager
    {
        private bool done = true;
        private Command currCommand;

        // public Queue<AtomicCommand> commands;
        public CommandTreeNode root;

        public CommandManager ()
        {
            root = new CommandTreeNode();
        }

        public void Update()
        {
            if (done)
            {
                var nextCommand = root.NextCommand();
                if (nextCommand != null)
                {
                    PluginLog.Log("BAKA!!");
                    currCommand = nextCommand;
                    if (currCommand != null)
                    {
                        PluginLog.Log($"Exectuing {currCommand.type}");
                        currCommand.Execute();
                        done = false;
                    }
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
            root.command = null;
            root.children.Clear();
            CottonCollectorPlugin.KeyState.ClearAll();

            done = true;
        }
    }
}
