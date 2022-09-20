using System.Collections.Generic;

using Dalamud.Logging;
using System.Linq;
using Dalamud.Game;

namespace CottonCollector.Commands.Structures
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

        public bool IsEmpty => commands.Count == 0;

        public void Update(Framework framework)
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

        // returns the first command before scheduling.
        public LinkedListNode<Command> ScheduleFront(IEnumerable<Command> newCommands)
        {
            var ret = commands.First;
            foreach (var command in newCommands.Reverse())
            {
                commands.AddFirst(command);
            }
            return ret;
        }

        // returns the first command before scheduling.
        public LinkedListNode<Command> ScheduleFront(Command newCommand)
        {
            var ret = commands.First;
            commands.AddFirst(newCommand);
            return ret;
        }

        public void ScheduleBefore(LinkedListNode<Command> commandNode, IEnumerable<Command> newCommands)
        {
            if (commandNode == null)
            {
                Schedule(newCommands);
            }
            else
            {
                foreach (var command in newCommands.Reverse())
                {
                    commands.AddBefore(commandNode, command);
                }
            }
        }

        public void ScheduleBefore(LinkedListNode<Command> commandNode, Command newCommand)
        {
            if (commandNode == null)
            {
                Schedule(newCommand);
            }
            else
            {
                commands.AddBefore(commandNode, newCommand);
            }
        }
    }
}
