using System;
using System.Collections.Generic;

using ImGuiNET;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    internal class CommandSet : Command
    {
        public string uniqueId;

        static public Dictionary<string, CommandSet> CommandSetMap;

        static CommandSet()
        {
            CommandSetMap = new Dictionary<string, CommandSet>();
        }

        public CommandSet(string uniqueId) : base(Type.COMMAND_SET)
        {
            this.uniqueId = uniqueId;
            this.subCommands = new LinkedList<Command>();
            this.timeOutMili = -1;

            if (CommandSetMap.ContainsKey(uniqueId))
            {
                throw new ArgumentException("Duplicated command set name.");
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
