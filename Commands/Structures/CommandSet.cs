using System;
using System.Collections.Generic;

using ImGuiNET;
using Newtonsoft.Json;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    [JsonObject(IsReference = true)]
    internal class CommandSet : Command
    {
        static public Dictionary<string, CommandSet> CommandSetMap;

        public string uniqueId;
        public LinkedList<Command> subCommands = new();

        private CommandManager commandManager = new();

        static CommandSet()
        {
            CommandSetMap = new Dictionary<string, CommandSet>();
        }

        public CommandSet(string uniqueId)
        {
            this.uniqueId = uniqueId;
            this.subCommands = new LinkedList<Command>();
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
            commandManager.Schedule(subCommands);
        }

        public override void Do()
        {
            CottonCollectorPlugin.Framework.Update += commandManager.Update;
        }

        public override void OnFinish()
        {
            CottonCollectorPlugin.Framework.Update -= commandManager.Update;
        }


        public override void SelectorGui()
        {
            // Not editable.
            ImGui.Text($"{uniqueId}");
        }
    }
}
