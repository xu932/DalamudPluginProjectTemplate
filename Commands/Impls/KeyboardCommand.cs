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

        [JsonProperty] public ActionType actionType = ActionType.KEY_PRESS;
        [JsonProperty] public VirtualKey vk;

        protected override int MinTimeMili { get; } = 10;

        protected override bool TerminateCondition() => true;

        protected override void Do()
        {
            switch (actionType)
            {
                case ActionType.KEY_DOWN:
                    PluginLog.Verbose($"KeyDown {vk}");
                    BgInput.KeyDown(vk);
                    break;
                case ActionType.KEY_UP:
                    PluginLog.Verbose($"KeyUp {vk}");
                    BgInput.KeyUp(vk);
                    break;
                case ActionType.KEY_PRESS:
                    PluginLog.Verbose($"KeyPress {vk}");
                    BgInput.KeyPress(vk);
                    break;
            }
        }

        #region GUI
        internal override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text(actionType.ToString() + " " + vk.ToString());
        }

        internal override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            // Type selector
            ImGui.Text("Type:");
            ImGui.SameLine();
            List<ActionType> actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
            int newActionTypeIndex = actionTypes.IndexOf(actionType);
            if (ImGui.Combo(Ui.Uid(index: uid), ref newActionTypeIndex,
                Enum.GetNames(typeof(ActionType)), actionTypes.Count))
            {
                actionType = actionTypes[newActionTypeIndex];
            }

            // Key Selector
            ImGui.SameLine();
            ImGui.Text("Key:");
            ImGui.SameLine();
            List<VirtualKey> keys = Enum.GetValues(typeof(VirtualKey)).Cast<VirtualKey>().ToList();
            int newVkIndex = keys.IndexOf(vk);
            if (ImGui.Combo(Ui.Uid(index: uid), ref newVkIndex, Enum.GetNames(typeof(VirtualKey)), keys.Count))
            {
                vk = keys[newVkIndex];
            }

            // TODO: add key modifier selector
            ImGui.PopItemWidth();
        }
        #endregion
    }
}
