﻿using System;
using System.Collections.Generic;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;

using CottonCollector.Config;
using CottonCollector.CharacterControl;

namespace CottonCollector.Interface
{
    internal class CharacterControlTab : ConfigTab
    {
        private Commands commands;

        public CharacterControlTab(ref CottonCollectorConfig config, ref Commands commands) : base("Character Control", ref config) {
            this.commands = commands;
        }

        public override void TabContent()
        {
            KeyState keyState = CottonCollectorPlugin.KeyState;

            if (ImGui.Button("Press W for 1 sec"))
            {
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_DOWN, VirtualKey.W, 1000));
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_UP, VirtualKey.W, 100));
            }

            if (ImGui.Button("Press W for 1 sec then Press D for 1 sec"))
            {
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_DOWN, VirtualKey.W, 1000));
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_UP, VirtualKey.W, 100));
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_DOWN, VirtualKey.D, 1000));
                commands.commands.Enqueue(new TimedCommand(Commands.Type.KEY_UP, VirtualKey.D, 100));
            }

            if (keyState[VirtualKey.W]) {
                ImGui.Text("W");
            }
            if (keyState[VirtualKey.S]) {
                ImGui.Text("S");
            }
            if (keyState[VirtualKey.A]) {
                ImGui.Text("A");
            }
            if (keyState[VirtualKey.D]) {
                ImGui.Text("D");
            }

        }
    }
}
