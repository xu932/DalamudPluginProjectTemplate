using System.Collections.Generic;

using Dalamud.Logging;
using System.Linq;
using Dalamud.Game;

namespace CottonCollector.Commands.Structures
{
    internal class CommandManager
    {
        private bool done = true;
        private readonly LinkedList<Command> commands = new();
        private Command currCommand;

        internal bool IsEmpty => commands.Count == 0 && currCommand == null;

        internal void Update(Framework framework)
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
                if (!nextCommand.ShouldRepeat || currCommand.IsFinished())
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

        internal void KillSwitch()
        {
            PluginLog.Log($"Killed {commands.Count} commands");
            if (currCommand != null)
            {
                if (currCommand is CommandSet currCommandSet)
                {
                    currCommandSet.KillSwitch();
                }
                currCommand = null;
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

        internal void Schedule(IEnumerable<Command> newCommands)
        {
            foreach (var command in newCommands)
            {
                PluginLog.Log($"Scheduling {command.GetType()}");
                commands.AddLast(command);
            }
        }

        internal void Schedule(Command newCommand)
        {
            PluginLog.Log($"Scheduling {newCommand.GetType()}");
            PluginLog.Log($"Command set size {commands.Count}");
            commands.AddLast(newCommand);
        }
    }
}
