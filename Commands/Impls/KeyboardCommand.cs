using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

using CottonCollector.BackgroundInputs;
using CottonCollector.Commands.Structures;
using CottonCollector.Util;

namespace CottonCollector.Commands.Impls
{
    internal class KeyboardCommand : Command
    {
        public enum ActionType
        {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
        }

        [JsonProperty] private ActionType actionType = ActionType.KEY_PRESS;
        [JsonProperty] private VirtualKey vk;
        [JsonProperty] private BgInput.Modifier mod = BgInput.Modifier.NONE;

        protected override int MinTimeMili { get; } = 10;

        protected override bool TerminateCondition() => true;

        protected override void Do()
        {
            switch (actionType)
            {
                case ActionType.KEY_DOWN:
                    PluginLog.Verbose($"KeyDown {mod} {vk}");
                    BgInput.KeyDown(vk, mod);
                    break;
                case ActionType.KEY_UP:
                    PluginLog.Verbose($"KeyUp {mod} {vk}");
                    BgInput.KeyUp(vk, mod);
                    break;
                case ActionType.KEY_PRESS:
                    PluginLog.Verbose($"KeyPress {mod} {vk}");
                    BgInput.KeyPress(vk, mod);
                    break;
            }
        }

        #region GUI
        internal override void MinimalInfo()
        {
            base.MinimalInfo();
            var info = $"{actionType} ";
            if (mod != BgInput.Modifier.NONE)
            {
                info += $"{mod}+";
            }
            info += vk.ToString();

            ImGui.Text(info);
        }

        internal override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            // Type Selector
            ImGui.Text("Type:");
            ImGui.SameLine();
            List<ActionType> actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
            int newActionTypeIndex = actionTypes.IndexOf(actionType);
            if (ImGui.Combo(Ui.Uid(index: uid), ref newActionTypeIndex,
                Enum.GetNames(typeof(ActionType)), actionTypes.Count))
            {
                actionType = actionTypes[newActionTypeIndex];
            }

            // Modifier Selector
            ImGui.SameLine();
            ImGui.Text("Modifier:");
            ImGui.SameLine();
            List<BgInput.Modifier> mods = Enum.GetValues(typeof(BgInput.Modifier)).Cast<BgInput.Modifier>().ToList();
            int newModIndex = mods.IndexOf(mod);
            if (ImGui.Combo(Ui.Uid(index: uid), ref newModIndex, mods.Select(t=>Enum.GetName(t)).ToArray(), mods.Count))
            {
                mod = mods[newModIndex];
            }

            // Key Selector
            ImGui.SameLine();
            ImGui.Text("Key:");
            ImGui.SameLine();
            List<VirtualKey> keys = CottonCollectorPlugin.KeyState.GetValidVirtualKeys().ToList();
            int newVkIndex = keys.IndexOf(vk);
            if (ImGui.Combo(Ui.Uid(index: uid), ref newVkIndex, keys.Select(t=>Enum.GetName(t)).ToArray(), keys.Count))
            {
                vk = keys[newVkIndex];
            }

            ImGui.PopItemWidth();
        }
        #endregion
    }
}
