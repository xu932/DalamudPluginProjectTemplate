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
                var nextCommand = commands.First.Value;
                PluginLog.Log($"next command: {nextCommand.GetType().Name}");
                if (nextCommand != null && nextCommand.TriggerCondition())
                {
                    commands.RemoveFirst();
                    currCommand = nextCommand;
                    currCommand.Execute();
                    done = false;
                }
            }

            if (!done && currCommand != null)
            {
                if (currCommand.ShouldRepeat)
                {
                    currCommand.Execute();
                }
                if (currCommand.IsFinished())
                {
                    done = true;
                    currCommand = null;
                }
            }
        }

        internal void KillSwitch()
        {
            if (currCommand != null)
            {
                if (currCommand is CommandSet currCommandSet)
                {
                    currCommandSet.KillSwitch();
                }
                PluginLog.Log($"Killed current command: {currCommand.GetType().Name}");
                currCommand.ResetExecutionState();
                currCommand = null;
            }

            foreach (var command in commands)
            {
                if (command is CommandSet commandSet)
                {
                    commandSet.KillSwitch();
                }
                command.ResetExecutionState();
            }
            PluginLog.Log($"Killed {commands.Count} queued commands");
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
            commands.AddLast(newCommand);
        }
    }
}
