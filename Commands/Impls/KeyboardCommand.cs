﻿using System;
using System.Collections.Generic;
using System.Linq;

using ImGuiNET;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    [Serializable]
    internal class KeyboardCommand : Command
    {
        public enum ActionType
        {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
        }

        public ActionType actionType = ActionType.KEY_PRESS;
        public VirtualKey vk;

        private InputSimulator sim = new InputSimulator();

        public override bool TerminateCondition() => true;

        public override void Do()
        {
            switch (actionType)
            {
                case ActionType.KEY_DOWN:
                    sim.Keyboard.KeyDown((VirtualKeyCode)(int)vk);
                    break;
                case ActionType.KEY_UP:
                    sim.Keyboard.KeyUp((VirtualKeyCode)(int)vk);
                    break;
                case ActionType.KEY_PRESS:
                    sim.Keyboard.KeyPress((VirtualKeyCode)(int)vk);
                    break;
            }
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            // Type selector
            ImGui.Text("Type:");
            ImGui.SameLine();
            List<ActionType> actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
            int newActionTypeIndex = actionTypes.IndexOf(actionType);
            if (ImGui.Combo($"##KeyboardCommand__ActionTypeSelector__{GetHashCode()}", ref newActionTypeIndex,
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
            if (ImGui.Combo($"##KeyboardCommand__KeySelector__{GetHashCode()}", ref newVkIndex, Enum.GetNames(typeof(VirtualKey)), keys.Count))
            {
                vk = keys[newVkIndex];
            }

            // TODO: add key modifier selector
            ImGui.PopItemWidth();
        }
    }
}
