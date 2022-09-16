using System;
using System.Collections.Generic;

using ImGuiNET;

using CottonCollector.Config;
using Dalamud.Interface;
using System.Text.RegularExpressions;
using Dalamud.Utility;

using CottonCollector.CharacterControl;
using CottonCollector.CharacterControl.Commands;
using Dalamud.Interface.Colors;

namespace CottonCollector.Interface
{
    internal class PresetsTab : ConfigTab
    {
        internal static string filterString = "";
        internal static string newPresetName = "";
        internal static int selectedNewCommandIndex = 0;
        internal static Preset selectedPreset = null;
        internal static Command newCommand = new KeyboardCommand();

        public PresetsTab(ref CottonCollectorConfig config) : base("Presets", ref config)
        {
        }

        public override void TabContent()
        {
            ImGui.BeginChild("TableWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (ImGui.BeginTable("LayoutsTable", 2 ,ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("Preset list", ImGuiTableColumnFlags.None, 200);
                var displayPresetName = selectedPreset != null ? selectedPreset.name : "";
                ImGui.TableSetupColumn($"{displayPresetName}##selectedPresetName", ImGuiTableColumnFlags.None, 600);

                ImGui.TableHeadersRow();

                ImGui.TableNextColumn();
                ImGui.InputTextWithHint("##presetFilter", "search presets...", ref filterString, 100);
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddPreset"))
                {
                    ImGui.OpenPopup("Add preset");
                }
                ImGui.PopFont();

                if(ImGui.BeginPopup("Add preset"))
                {
                    ImGui.InputTextWithHint("##newPresetName", "Preset name", ref newPresetName, 100);
                    ImGui.SameLine();
                    if (ImGui.Button("Add")) {
                        if (newPresetName != "" && config.presets.Find((p) => { return p.name == newPresetName; }) == null)
                        {
                            ImGui.CloseCurrentPopup();
                            config.presets.Add(new Preset(newPresetName));
                        }
                    }
                    ImGui.EndPopup();
                }

                ImGui.BeginChild("PresetsTableSelector");
                foreach (var preset in config.presets)
                {
                    if (!filterString.IsNullOrEmpty() && !Regex.IsMatch(preset.name, filterString))
                    {
                        continue;
                    }
                    if (ImGui.Selectable($"{preset.name}", selectedPreset != null && preset.name == selectedPreset.name))
                    {
                        selectedPreset = preset;
                    }
                }
                ImGui.EndChild();

                ImGui.TableNextColumn();
                if (selectedPreset != null)
                {
                    if (ImGui.BeginTable("CommandsTable", 3, ImGuiTableFlags.Resizable))
                    {
                        ImGui.TableSetupColumn("##Tracking", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 10);
                        ImGui.TableSetupColumn("##Commands", ImGuiTableColumnFlags.None, 450);
                        ImGui.TableSetupColumn("##Btns", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 140);
                        ImGui.TableHeadersRow();

                        int index = 0;
                        for (LinkedListNode<CommandTreeNode> commandLN = selectedPreset.presetRoot.children.First; 
                            commandLN != null; commandLN = commandLN.Next, index++)
                        {
                            var commandTN = commandLN.ValueRef;
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (commandTN.IsCurrent()) {
                                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                                ImGui.Text("->");
                                ImGui.PopStyleColor();
                            }

                            if (commandTN.IsLeaf()) {
                                var command = commandTN.command;
                                ImGui.TableSetColumnIndex(1);
                                if (command != null)
                                {
                                    ImGui.Text($"{index} : {command.GetType().Name}");

                                    ImGui.SameLine();
                                    command.SelectorGui();
                                }
                                else {
                                    ImGui.Text("null: something might went wrong.");
                                }
                            }

                            ImGui.TableSetColumnIndex(2);
                            if (ImGui.Button($"Remove##PresetsTab__Btn__{index}"))
                            {
                                var prev = commandLN.Previous;
                                selectedPreset.presetRoot.children.Remove(commandLN);
                                commandLN = prev;
                                return;
                            }

                            ImGui.SameLine();
                            if (ImGui.Button($"Up##PresetsTab__Btn__{index}"))
                            {
                                var prev = commandLN.Previous;
                                if (prev != null)
                                {
                                    selectedPreset.presetRoot.children.Remove(commandLN);
                                    selectedPreset.presetRoot.children.AddBefore(prev, commandLN);
                                    return;
                                }
                            }

                            ImGui.SameLine();
                            if (ImGui.Button($"Down##PresetsTab__Btn__{index}"))
                            {
                                var next = commandLN.Next;
                                if (next != null)
                                {
                                    selectedPreset.presetRoot.children.Remove(commandLN);
                                    selectedPreset.presetRoot.children.AddAfter(next, commandLN);
                                    return;
                                }
                            }
                        }

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(1);

                        ImGui.Text("New ");
                        // TODO: fix this shit.
                        var commandTypes = new List<string>() { "Keyboard Command", "SleepCommand", "Till Moved To", "Till Looked At" };

                        ImGui.SetNextItemWidth(200);
                        ImGui.SameLine();
                        if (ImGui.Combo("##CommandTypeSelector", ref selectedNewCommandIndex, commandTypes.ToArray(), commandTypes.Count))
                        {
                            switch (selectedNewCommandIndex)
                            {
                                case 0:
                                    newCommand = new KeyboardCommand();
                                    break;
                                case 1:
                                    newCommand = new SleepCommand();
                                    break;
                                case 2:
                                    newCommand = new TillMovedToCommand();
                                    break;
                                case 3:
                                    newCommand = new TillLookedAtCommand();
                                    break;
                            }
                        }

                        ImGui.TableSetColumnIndex(2);
                        if (newCommand != null)
                        {
                            if (ImGui.Button("Add"))
                            {
                                selectedPreset.presetRoot.Add(newCommand);
                                newCommand = (Command)Activator.CreateInstance(newCommand.GetType());
                            }
                        }

                        ImGui.EndTable();
                    }

                    ImGui.Separator();
                    if (ImGui.Button("Play"))
                    {
                        CottonCollectorPlugin.cmdManager.root.Add(selectedPreset.presetRoot.ExecutableCopy());
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Kill Switch"))
                    {
                        CottonCollectorPlugin.cmdManager.KillSwitch();
                    }
                }
                ImGui.EndTable();
            }
            ImGui.EndChild();
        }

    }
}
