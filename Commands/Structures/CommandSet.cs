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

        public LinkedList<Command> subCommands = null;

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

        public override void Do()
        {
            // Do nothing. This should never get called.
        }

        public override bool TerminateCondition() => true;

        public override void SelectorGui()
        {
            // Not editable.
            ImGui.Text($"{uniqueId}");
        }
    }
}
