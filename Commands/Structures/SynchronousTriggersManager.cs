using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Game;
using Dalamud.Logging;
using Windows.Devices.Display.Core;

namespace CottonCollector.Commands.Structures
{
    internal class SynchronousTriggersManager
    {
        private readonly CommandManager commandManager = new();
        private readonly LinkedList<Command> triggers = new();
        private bool enabled = false;
        private bool updating = false;

        internal bool IsFinished => !updating;

        internal void Update(Framework framework)
        {
            if (commandManager.IsEmpty && triggers.Count > 0 && enabled)
            {
                var triggeredTriggers = triggers.Where(t => t.TriggerCondition());
                if (triggeredTriggers.Any())
                {
                    var trigger = triggeredTriggers.First();
                    PluginLog.Log($"Scheduling Trigger");
                    commandManager.Schedule(trigger);
                }
            }
            commandManager.Update(framework);

            if (!enabled && updating && commandManager.IsEmpty)
            {
                updating = false;
                CottonCollectorPlugin.Framework.Update -= Update; 
            }
        }

        internal void Add(Command t)
        {
            triggers.AddLast(t);
        }

        internal void Add(IEnumerable<Command> ts)
        {
            foreach (var t in ts)
            {
                triggers.AddLast(t);
            }
        }

        internal void Enable()
        {
            enabled = true;
            updating = true;
            CottonCollectorPlugin.Framework.Update += Update;
        }

        internal void Disable()
        {
            enabled = false;
        }

        internal void KillSwitch()
        {
            triggers.Clear();
            commandManager.KillSwitch();
            enabled = updating = false;
        }
    }
}
