using System;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class CharacterControlTab : ConfigTab
    {
        public CharacterControlTab(ref CottonCollectorConfig config) : base("Character Control", ref config) { }

        public override void TabContent()
        {
            KeyState keyState = CottonCollectorPlugin.KeyState;

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
