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

        private LinkedList<Command> commands = new();

        public bool IsEmpty => commands.Count == 0 && currCommand == null;

        public void Update(Framework framework)
        {
            if (done && commands.Count > 0)
            {
                PluginLog.Log($"Command {commands.Count}");
                var nextCommand = commands.First.Value;
                if (nextCommand != null && nextCommand.TriggerCondition())
                {
                    currCommand = nextCommand;
                    currCommand.Execute();
                    done = false;
                }
                if (!nextCommand.Repeate || currCommand.IsFinished())
                {
                    commands.RemoveFirst();
                }
                else
                {
                    done = true;
                }
            }

            if (!done && currCommand != null && currCommand.IsFinished())
            {
                done = true;
                currCommand = null;
            }
        }

        public void KillSwitch()
        {
            PluginLog.Log($"Killed {commands.Count} commands");
            if (currCommand != null && currCommand is CommandSet currCommandSet)
            {
                currCommandSet.KillSwitch();
            }

            foreach (var command in commands)
            {
                if (command is CommandSet commandSet)
                {
                    commandSet.KillSwitch();
                }
            }
            commands.Clear();
            CottonCollectorPlugin.KeyState.ClearAll();

            done = true;
        }

        public void Schedule(IEnumerable<Command> newCommands)
        {
            foreach (var command in newCommands)
            {
                PluginLog.Log($"Scheduling {command.Description()}");
                commands.AddLast(command);
            }
        }

        public void Schedule(Command newCommand)
        {
            PluginLog.Log($"Scheduling {newCommand.Description()}");
            PluginLog.Log($"Command set size {commands.Count}");
            commands.AddLast(newCommand);
        }
    }
}
