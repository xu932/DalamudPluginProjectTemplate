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
                var nextCommand = commands.First.Value;
                PluginLog.Log($"trigger condition is {nextCommand.condition}");
                if (nextCommand != null && nextCommand.TriggerCondition())
                {
                    commands.RemoveFirst();
                    currCommand = nextCommand;
                    currCommand.Execute();
                    done = false;
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
            commands.AddLast(newCommand);
        }
    }
}
