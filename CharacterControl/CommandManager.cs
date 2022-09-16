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
                if (root.children.Count != 0)
                {
                    currCommand = root.PopCommand();
                    PluginLog.Log("Exectuing Command");
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
            root.command = null;
            root.children.Clear();
            CottonCollectorPlugin.KeyState.ClearAll();

            done = true;
        }
    }
}
