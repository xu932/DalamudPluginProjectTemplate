﻿using System;
using System.Collections.Generic;
using System.Linq;

using ImGuiNET;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

namespace CottonCollector.CharacterControl.Commands
{
    internal class KeyboardCommand : Command
    {
        public enum Type {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
        }

        private Type type = Type.KEY_PRESS;
        private VirtualKey vk;

        static int newTypeIndex = 0;
        static int newVkIndex = 0;

        private InputSimulator sim = new InputSimulator();

        public KeyboardCommand() { }

        public override bool TerminateCondition() => true;

        public override void Do()
        {
            switch (type)
            {
                case Type.KEY_DOWN:
                    sim.Keyboard.KeyDown((VirtualKeyCode)(int)vk);
                    break;
                case Type.KEY_UP:
                    sim.Keyboard.KeyUp((VirtualKeyCode)(int)vk);
                    break;
                case Type.KEY_PRESS:
                    sim.Keyboard.KeyPress((VirtualKeyCode)(int)vk);
                    break;
            }
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            // Type selector
            ImGui.SameLine();
            ImGui.Text("Type:");
            ImGui.SameLine();
            List<Type> types = Enum.GetValues(typeof(Type)).Cast<Type>().ToList();
            if (ImGui.Combo("##KeyboardCommand__TypeSelector", ref newTypeIndex, Enum.GetNames(typeof(Type)), types.Count))
            {
                type = types[newTypeIndex];
            }

            // Key Selector
            ImGui.SameLine();
            ImGui.Text("Key:");
            ImGui.SameLine();
            List<VirtualKey> keys = Enum.GetValues(typeof(VirtualKey)).Cast<VirtualKey>().ToList();
            int key_index = types.IndexOf(type); 
            if (ImGui.Combo("##KeyboardCommand__KeySelector", ref newVkIndex, Enum.GetNames(typeof(VirtualKey)), keys.Count))
            {
                vk = keys[newVkIndex];
            }

            // TODO: add key modifier selector

            ImGui.PopItemWidth();
        }
    }
}