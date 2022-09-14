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

        // temp
        internal static double X = 0.0f, Y = 0.0f, Z = 0.0f, gotoThreshold = 1.0;

        private string selectedPresetName = "";
        private Preset selectedPreset = null;

        public PresetsTab(ref CottonCollectorConfig config) : base("Presets", ref config)
        {
        }

        public override void TabContent()
        {
            ImGui.BeginChild("TableWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (ImGui.BeginTable("LayoutsTable", 2 ,ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("Preset list", ImGuiTableColumnFlags.None, 200);
                ImGui.TableSetupColumn($"{selectedPresetName}##selectedPresetName", ImGuiTableColumnFlags.None, 600);

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
                    if (ImGui.Selectable($"{preset.name}", preset.name == selectedPresetName))
                    {
                        selectedPresetName = preset.name;
                        selectedPreset = preset;
                    }
                }
                ImGui.EndChild();

                ImGui.TableNextColumn();
                if (!selectedPresetName.IsNullOrEmpty() && selectedPreset != null)
                {
                    foreach(Command command in selectedPreset.atomicCommands)
                    {
                        command.SelectorGui(); 
                        if (ImGui.Button("Remove")) 
                        {
                            selectedPreset.atomicCommands.Remove(command);
                        }
                    }

                    // TODO: fix this shit.
                    var commandTypes = new List<string>() { "Keyboard Command", "Till Moved To", "Till Looked At" };

                    if (ImGui.Button("Add"))
                    {
                        ImGui.SetNextItemWidth(100);
                        ImGui.Combo("##CommandTypeSelector", ref selectedNewCommandIndex, commandTypes.ToArray(), commandTypes.Count);

                        Command cmd = null;
                        switch (selectedNewCommandIndex)
                        {
                            case 0:
                                cmd = new KeyboardCommand();
                                break;
                            case 1:
                                cmd = new TillMovedToCommand();
                                break;
                            case 2:
                                cmd = new TillLookedAtCommand();
                                break;
                        }

                        if (cmd != null)
                        {
                            cmd.SelectorGui();
                            if (ImGui.Button("Add"))
                            {
                                selectedPreset.atomicCommands.Add(cmd);
                            }
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
