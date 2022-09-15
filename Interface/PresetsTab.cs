using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

using ImGuiNET;

using CottonCollector.Config;
using Dalamud.Interface;
using System.Text.RegularExpressions;
using Dalamud.Utility;

using CottonCollector.CharacterControl;
using CottonCollector.CharacterControl.Commands;
using Windows.Devices.AllJoyn;
using Microsoft.VisualBasic.FileIO;
using Dalamud.Logging;
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
                        ImGui.TableSetupColumn("##Tracking", ImGuiTableColumnFlags.None, 20);
                        ImGui.TableSetupColumn("##Commands", ImGuiTableColumnFlags.None, 500);
                        ImGui.TableSetupColumn("##Btns", ImGuiTableColumnFlags.None, 80);
                        ImGui.TableHeadersRow();

                        int index = 0;
                        foreach (Command command in selectedPreset.atomicCommands)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (command.IsCurrent()) {
                                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                                ImGui.Text("->");
                                ImGui.PopStyleColor();
                            }

                            ImGui.TableSetColumnIndex(1);
                            ImGui.Text($"{index} : {command.GetType().Name}");

                            ImGui.SameLine();
                            command.SelectorGui();

                            ImGui.TableSetColumnIndex(2);
                            if (ImGui.Button($"Remove##PresetsTab__Btn__{index}"))
                            {
                                selectedPreset.atomicCommands.RemoveAt(index);
                                return;
                            }

                            ImGui.SameLine();
                            if (ImGui.Button($"Up##PresetsTab__Btn__{index}"))
                            {
                                if (index > 0)
                                {
                                    selectedPreset.atomicCommands.RemoveAt(index);
                                    selectedPreset.atomicCommands.Insert(index - 1, command);
                                    return;
                                }
                            }

                            ImGui.SameLine();
                            if (ImGui.Button($"Down##PresetsTab__Btn__{index}"))
                            {
                                if (index < selectedPreset.atomicCommands.Count - 1)
                                {
                                    selectedPreset.atomicCommands.RemoveAt(index);
                                    selectedPreset.atomicCommands.Insert(index + 1, command);
                                    return;
                                }
                            }
                            index++;
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
                                selectedPreset.atomicCommands.Add(newCommand);
                                newCommand = (Command)Activator.CreateInstance(newCommand.GetType());
                            }
                        }

                        ImGui.EndTable();
                    }

                    ImGui.Separator();
                    if (ImGui.Button("Play"))
                    {
                        CottonCollectorPlugin.cmdManager.commands.Enqueue(new Queue<Command>(selectedPreset.atomicCommands));
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
