using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Game;
using Dalamud.Logging;

namespace CottonCollector.Commands.Structures
{
    internal class SynchronousTriggersManager
    {
        private readonly CommandManager commandManager = new();
        private readonly LinkedList<Trigger> triggers = new();
        
        public void Update(Framework framework)
        {
            if (commandManager.IsEmpty && triggers.Count > 0)
            {
                var triggeredTriggers = triggers.Where(t => t.TriggerCondition());
                if (triggeredTriggers.Count() > 0)
                {
                    var trigger = triggeredTriggers.First();
                    PluginLog.Log($"Scheduling Trigger");
                    commandManager.Schedule(trigger.command);
                }
            }
            commandManager.Update(framework);
        }

        public void Add(Trigger t)
        {
            triggers.AddLast(t);
        }

        public void Add(IEnumerable<Trigger> ts)
        {
            foreach (var t in ts)
            {
                triggers.AddLast(t);
            }
        }

        public void KillSwitch()
        {
            triggers.Clear();
        }
    }
}
