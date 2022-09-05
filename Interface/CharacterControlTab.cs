using System;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.Interface
{
    internal class CharacterControlTab : ConfigTab
    {
        public CharacterControlTab() : base("Character Control") { }

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
