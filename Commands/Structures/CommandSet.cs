using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using ImGuiNET;

using Dalamud.Logging;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    [JsonObject(IsReference = true)]
    internal class CommandSet : Command
    {
        static public Dictionary<string, CommandSet> CommandSetMap;

        public string uniqueId;
        public LinkedList<Command> subCommands = new();
        public LinkedList<Trigger> triggers = new();

        private readonly CommandManager commandManager = new();
        private readonly SynchronousTriggersManager triggersManager = new();

        static CommandSet()
        {
            CommandSetMap = new Dictionary<string, CommandSet>();
        }

        public CommandSet(string uniqueId)
        {
            this.uniqueId = uniqueId;
            this.timeOutMili = -1;

            if (CommandSetMap.ContainsKey(uniqueId))
            {
                throw new ArgumentException($"Duplicated command set name: {uniqueId}");
            }

            CommandSetMap.Add(uniqueId, this);
        }

        public override bool TerminateCondition() => commandManager.IsEmpty;

        public override void OnStart()
        {
            PluginLog.Log($"Executing Command Set {uniqueId}");
            commandManager.Schedule(subCommands);
            triggersManager.Add(triggers);
        }

        public override void Do()
        {
            CottonCollectorPlugin.Framework.Update += commandManager.Update;
            CottonCollectorPlugin.Framework.Update += triggersManager.Update;
        }

        public override void OnFinish()
        {
            CottonCollectorPlugin.Framework.Update -= commandManager.Update;
            CottonCollectorPlugin.Framework.Update -= triggersManager.Update;
        }


        public override void SelectorGui()
        {
            // Not editable.
            ImGui.Text($"{uniqueId}");
        }

        public void KillSwitch()
        {
            commandManager.KillSwitch();
            triggersManager.KillSwitch();

            CottonCollectorPlugin.Framework.Update -= commandManager.Update;
            CottonCollectorPlugin.Framework.Update -= triggersManager.Update;
        }
    }
}
