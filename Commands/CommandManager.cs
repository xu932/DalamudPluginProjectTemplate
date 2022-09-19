using System.Collections.Generic;

using Dalamud.Logging;

using CottonCollector.Commands.Structures;
using System.Linq;

namespace CottonCollector.Commands
{
    internal class CommandManager
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

                while (nextCommand.IsCommandSet())
                {
                    ScheduleFront(((CommandSet)nextCommand).subCommands);
                    nextCommand = commands.First.Value;
                    commands.RemoveFirst();
                }

                currCommand = nextCommand;
                PluginLog.Log($"Exectuing {currCommand.GetType().Name}");
                currCommand.Execute();
                done = false;
            }

            if (!done && currCommand != null && currCommand.IsFinished())
            {
                done = true;
                PluginLog.Log("Finished Command");
            }
        }

        public void KillSwitch()
        {
            PluginLog.Log($"Killed {commands.Count} commands");
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

        public void Schedule(Command newCommand)
        {
            commands.AddLast(newCommand);
        }

        public void ScheduleFront(IEnumerable<Command> newCommands)
        {
            foreach (var command in newCommands.Reverse())
            {
                commands.AddFirst(command);
            }
        }

        public void ScheduleFront(Command newCommand)
        {
            commands.AddFirst(newCommand);
        }
    }
}
