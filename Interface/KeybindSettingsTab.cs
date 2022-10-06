using System;
using System.Collections.Generic;
using System.Linq;

using ImGuiNET;

using CottonCollector.BackgroundInputs;
using Dalamud.Game.ClientState.Keys;
using CottonCollector.Commands.Impls;

namespace CottonCollector.Interface
{
    internal class KeybindSettingsTab : ConfigTab
    {
        internal KeybindSettingsTab() : base("KeyBindSetting") { }

        private void KeyInput(ref Tuple<BgInput.Modifier, VirtualKey> keybind, string name)
        {
            ImGui.PushItemWidth(100);
            ImGui.Text("Modifier:");

            List<BgInput.Modifier> mods = Enum.GetValues(typeof(BgInput.Modifier)).Cast<BgInput.Modifier>().ToList();
            int newModIndex = mods.IndexOf(keybind.Item1);
            BgInput.Modifier newMod = keybind.Item1;
            ImGui.SameLine();
            if (ImGui.Combo($"##KeyBindSetting__KeyInput__Mod__{name}", ref newModIndex, 
                mods.Select(t=>Enum.GetName(t)).ToArray(), mods.Count))
            {
                newMod = mods[newModIndex];
            }
            ImGui.SameLine();
            ImGui.Text("Key:");

            List<VirtualKey> keys = CottonCollectorPlugin.KeyState.GetValidVirtualKeys().ToList();
            int newVkIndex = keys.IndexOf(keybind.Item2);
            VirtualKey newVk = keybind.Item2;
            ImGui.SameLine();
            if (ImGui.Combo($"##KeyBindSetting__KeyInput__VK__{name}", ref newVkIndex, 
                keys.Select(t=>Enum.GetName(t)).ToArray(), keys.Count))
            {
                newVk = keys[newVkIndex];
            }
            ImGui.PopItemWidth();

            keybind = new Tuple<BgInput.Modifier, VirtualKey>(newMod, newVk);
        }

        public override void TabContent()
        {
            var config = CottonCollectorPlugin.config;
            ImGui.Text("Move To Command");
            ImGui.Separator();

            if (ImGui.BeginTable("##MoveToCommandKeyBind", 2))
            {
                ImGui.TableSetupColumn("##Label", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn("##Selector", ImGuiTableColumnFlags.None);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Forward");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveForward, "Forward");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Backward");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveBackward, "Backward");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Left");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveLeft, "Left");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Right");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveRight, "Right");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Up");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveUpward, "Upward");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Move Down");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.moveDownward, "Downward");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Rotate Camera Left");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.rotateCameraLeft, "RotateLeft");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Rotate Camera Right");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.rotateCameraRight, "RotateRight");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("Jump");
                ImGui.TableSetColumnIndex(1);
                KeyInput(ref config.keybind.jump, "Jump");

                ImGui.EndTable();
            }
            ImGui.Separator();
        }
    }
}
