using System.Collections.Generic;

using Dalamud.Logging;

using CottonCollector.CharacterControl.Commands;
using CottonCollector.Commands.Structures;
using System.Linq;

namespace CottonCollector.Commands
{
    internal unsafe class CommandManager
    {
        private bool done = true;
        private Command currCommand;

        private LinkedList<Command> commands;

        public CommandManager()
        {
            commands = new LinkedList<Command>();
        }

        public void Update()
        {
            if (done && commands.Count > 0)
            {
                var nextCommand = commands.First.Value;
                commands.RemoveFirst();
                if (nextCommand != null)
                {
                    while (nextCommand.IsCommandSet())
                    {
                        foreach (var subCommand in nextCommand.subCommands.Reverse())
                        {
                            commands.AddFirst(subCommand);
                        }
                        nextCommand = commands.First.Value;
                    }

                    currCommand = nextCommand;
                    PluginLog.Log($"Exectuing {currCommand.type}");
                    currCommand.Execute();
                    done = false;
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
            CottonCollectorPlugin.KeyState.ClearAll();

            done = true;
        }

        public void Schedule(IEnumerable<Command> newCommands)
        {
            foreach (var command in newCommands)
            {
                commands.AddLast(command);
            } 
        }
    }
}
