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

namespace CottonCollector.Interface
{
    internal class PresetsTab : ConfigTab
    {
        internal static string filterString = "";
        internal static string newPresetName = "";
        internal static int selectedNewCommandIndex = 0;
        internal static Preset selectedPreset = null;
        internal static Command newCommand = null;

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
                    int index = 0;
                    foreach(Command command in selectedPreset.atomicCommands)
                    {
                        ImGui.Text($"{index} : {command.GetType().Name}");

                        ImGui.SameLine();
                        command.SelectorGui();

                        ImGui.SameLine();
                        if (ImGui.Button("Remove")) 
                        {
                            selectedPreset.atomicCommands.RemoveAt(index);
                            return;
                        }

                        if (index > 0)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("Up"))
                            {
                                selectedPreset.atomicCommands.RemoveAt(index);
                                selectedPreset.atomicCommands.Insert(index - 1, command);
                                return;
                            }
                        }

                        if (index < selectedPreset.atomicCommands.Count - 1)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("Down"))
                            {
                                selectedPreset.atomicCommands.RemoveAt(index);
                                if (index + 1 == selectedPreset.atomicCommands.Count)
                                {
                                    selectedPreset.atomicCommands.Add(command);
                                }
                                else
                                {
                                    selectedPreset.atomicCommands.Insert(index + 1, command);
                                }
                                return;
                            }
                        }
                        index++;
                    }

                    ImGui.Text("New Command");
                    // TODO: fix this shit.
                    var commandTypes = new List<string>() { "Keyboard Command", "SleepCommand", "Till Moved To", "Till Looked At" };

                    ImGui.SetNextItemWidth(100);
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

                    if (newCommand != null)
                    {
                        ImGui.SameLine();
                        newCommand.SelectorGui();
                        if (ImGui.Button("Add"))
                        {
                            selectedPreset.atomicCommands.Add(newCommand);
                            newCommand = null;
                        }
                    }

                    if (ImGui.Button("Play"))
                    {
                        CottonCollectorPlugin.cmdManager.commands.Enqueue(new Queue<Command>(selectedPreset.atomicCommands));
                    }
                }
                ImGui.EndTable();
            }
            ImGui.EndChild();
        }

    }
}
