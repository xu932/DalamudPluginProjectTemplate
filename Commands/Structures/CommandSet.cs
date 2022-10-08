using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using ImGuiNET;

using Dalamud.Logging;

namespace CottonCollector.Commands.Structures
{
    [JsonObject(IsReference = true)]
    internal class CommandSet : Command
    {
        static public Dictionary<string, CommandSet> CommandSetMap { get; private set; }

        [JsonProperty] public string uniqueId { get; private set; }
        [JsonProperty] public List<Command> subCommands = new();
        [JsonProperty] public List<Command> triggers = new();

        private readonly CommandManager commandManager = new();
        private readonly SynchronousTriggersManager triggersManager = new();

        static CommandSet()
        {
            CommandSetMap = new Dictionary<string, CommandSet>();
        }

        public CommandSet(string uniqueId) : base()
        {
            PluginLog.Verbose($"new command set: {uniqueId}");
            this.uniqueId = uniqueId;

            if (CommandSetMap.ContainsKey(uniqueId))
            {
                throw new ArgumentException($"{uniqueId} already exists.");
            }

            CommandSetMap.Add(uniqueId, this);
        }

        protected override bool TerminateCondition() => commandManager.IsEmpty;

        protected override void Do()
        {
            CottonCollectorPlugin.Framework.Update += commandManager.Update;
            triggersManager.Enable();
        }

        protected override void OnStart()
        {
            PluginLog.Log($"Executing Command Set {uniqueId}");
            commandManager.Schedule(subCommands);
            triggersManager.Add(triggers);
        }

        protected override void OnFinish()
        {
            CottonCollectorPlugin.Framework.Update -= commandManager.Update;
            triggersManager.Disable();
        }


        internal void KillSwitch()
        {
            commandManager.KillSwitch();
            triggersManager.KillSwitch();

            CottonCollectorPlugin.Framework.Update -= commandManager.Update;
            CottonCollectorPlugin.Framework.Update -= triggersManager.Update;
        }

        #region GUI
        internal override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text($"{uniqueId}");
        }

        internal override void SelectorGui()
        {
            ImGui.Text($"{uniqueId}");
        }
        #endregion
    }
}
