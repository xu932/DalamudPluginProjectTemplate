using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Utility;

using CottonCollector.Config;
using Dalamud.Interface.Colors;
using CottonCollector.Commands.Structures;
using CottonCollector.Commands.Impls;
using Dalamud.Logging;

namespace CottonCollector.Interface
{
    internal class CommandSetsTab : ConfigTab
    {
        internal static string filterString = "";
        internal static string newCommandSetName = "";
        internal static int selectedNewCommandIndex = 0;
        internal static int selectedNewTriggerIndex = 0;
        internal static int selectedCommandSetLinkIndex = 0;
        internal static int selectedTriggerLinkIndex = 0;
        internal static CommandSet selectedCommandSet = null;
        internal static Command newCommand = new KeyboardCommand();
        internal static Command newTrigger = new KeyboardCommand();

        public CommandSetsTab(ref CottonCollectorConfig config) : base("CommandSets", ref config)
        {
        }

        public override void TabContent()
        {
            ImGui.BeginChild("TableWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (ImGui.BeginTable("LayoutsTable", 2 ,ImGuiTableFlags.Resizable))
            {
                #region COMMAND_SET_SELECTOR
                ImGui.TableSetupColumn("Command sets list", ImGuiTableColumnFlags.None, 200);
                var displayCommandSetName = selectedCommandSet != null ? selectedCommandSet.uniqueId : "";
                ImGui.TableSetupColumn($"{displayCommandSetName}##selectedCommandSetName", ImGuiTableColumnFlags.None, 1000);
                ImGui.TableHeadersRow();

                ImGui.TableNextColumn();
                ImGui.InputTextWithHint("##presetFilter", "search presets...", ref filterString, 100);
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddCommandSet"))
                {
                    ImGui.OpenPopup("Add preset");
                }
                ImGui.PopFont();

                if(ImGui.BeginPopup("Add preset"))
                {
                    ImGui.InputTextWithHint("##newCommandSetName", "CommandSet name", ref newCommandSetName, 100);
                    ImGui.SameLine();
                    if (ImGui.Button("Add")) {
                        if (newCommandSetName != "" && !CommandSet.CommandSetMap.ContainsKey(newCommandSetName))
                        {
                            ImGui.CloseCurrentPopup();
                            config.commandSets.Add(new CommandSet(newCommandSetName));
                        }
                    }
                    ImGui.EndPopup();
                }

                ImGui.BeginChild("CommandSetsTableSelector");
                foreach (var commandSet in config.commandSets)
                {
                    if(commandSet == null)
                    {
                        PluginLog.Log("Null CommandSet!!");
                        continue;
                    }
                    if (!filterString.IsNullOrEmpty() && !Regex.IsMatch(commandSet.uniqueId, filterString))
                    {
                        continue;
                    }
                    if (ImGui.Selectable($"{commandSet.uniqueId}", selectedCommandSet != null && commandSet.uniqueId == selectedCommandSet.uniqueId))
                    {
                        selectedCommandSet = commandSet;
                    }
                }
                ImGui.EndChild();
                #endregion

                #region COMMAND_SET_EDITOR
                ImGui.TableNextColumn();
                if (selectedCommandSet != null)
                {
                    selectedCommandSet.BuilderGui();
                    if (ImGui.BeginTable("CommandSetEditorWrapper", 2, ImGuiTableFlags.Resizable))
                    {
                        ImGui.TableSetupColumn("CommandSequenceEditor", ImGuiTableColumnFlags.NoResize, 600);
                        ImGui.TableSetupColumn("TriggersEditor", ImGuiTableColumnFlags.NoResize, 600);
                        ImGui.TableHeadersRow();

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.BeginTable("CommandsTable", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg))
                        {
                            ImGui.TableSetupColumn("##Tracking", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 10);
                            ImGui.TableSetupColumn("Command Sequence", ImGuiTableColumnFlags.NoResize, 400);
                            ImGui.TableSetupColumn("##Command__Btns", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 190);
                            ImGui.TableHeadersRow();

                            int index = 0;
                            for (var commandLN = selectedCommandSet.subCommands.First;
                                commandLN != null; commandLN = commandLN.Next, index++)
                            {
                                var command = commandLN.Value;
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                if (command.IsCurrent())
                                {
                                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                                    ImGui.Text("->");
                                    ImGui.PopStyleColor();
                                }

                                ImGui.TableSetColumnIndex(1);
                                command.MinimalInfo();

                                ImGui.TableSetColumnIndex(2);

                                if (ImGui.BeginPopup($"Edit Command##CommandSetsTab__Popup__{index}"))
                                {
                                    command.BuilderGui();

                                    if (ImGui.Button("Save & Close"))
                                    {
                                        ImGui.CloseCurrentPopup();
                                        CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                                    }
                                    ImGui.EndPopup();
                                }

                                if (ImGui.Button($"Edit##CommandSetsTab__Btn__{index}"))
                                {
                                    ImGui.OpenPopup($"Edit Command##CommandSetsTab__Popup__{index}");
                                }

                                ImGui.SameLine();
                                if (ImGui.Button($"Remove##CommandSetsTab__Btn__{index}"))
                                {
                                    var prev = commandLN.Previous;
                                    selectedCommandSet.subCommands.Remove(commandLN);
                                    commandLN = prev;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button($"Up##CommandSetsTab__Btn__{index}"))
                                {
                                    var prev = commandLN.Previous;
                                    if (prev != null)
                                    {
                                        selectedCommandSet.subCommands.Remove(commandLN);
                                        selectedCommandSet.subCommands.AddBefore(prev, commandLN);
                                        return;
                                    }
                                }

                                ImGui.SameLine();
                                if (ImGui.Button($"Down##CommandSetsTab__Btn__{index}"))
                                {
                                    var next = commandLN.Next;
                                    if (next != null)
                                    {
                                        selectedCommandSet.subCommands.Remove(commandLN);
                                        selectedCommandSet.subCommands.AddAfter(next, commandLN);
                                        return;
                                    }
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(1);
                            ImGui.Separator();

                            ImGui.Text("New ");
                            var commandTypes = Command.AllTypes.Where(t => !t.Equals(typeof(CommandSet))).ToArray();

                            ImGui.SetNextItemWidth(200);
                            ImGui.SameLine();
                            if (ImGui.Combo("##CommandTypeSelector", ref selectedNewCommandIndex,
                                commandTypes.Select(t => t.Name).ToArray(), commandTypes.Length))
                            {
                                newCommand = (Command)Activator.CreateInstance(commandTypes[selectedNewCommandIndex]);
                            }

                            ImGui.TableSetColumnIndex(2);
                            if (newCommand != null)
                            {
                                if (ImGui.Button("Add##CommandSetsTab__Btn__AddCommand"))
                                {
                                    selectedCommandSet.subCommands.AddLast(newCommand);
                                    newCommand = (Command)Activator.CreateInstance(newCommand.GetType());
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(1);

                            ImGui.Text("Link CommandSet ");

                            ImGui.SetNextItemWidth(200);
                            ImGui.SameLine();
                            string[] commandSetKeys = CommandSet.CommandSetMap.Keys.ToArray();
                            ImGui.Combo("##CommandSetSelector", ref selectedCommandSetLinkIndex,
                                commandSetKeys, CommandSet.CommandSetMap.Count);

                            ImGui.TableSetColumnIndex(2);
                            if (ImGui.Button("Add##CommandSetsTab__Btn__AddCommandSet"))
                            {
                                // TODO: this is not safe for adding loop dependencies. Fix.
                                selectedCommandSet.subCommands.AddLast(CommandSet.CommandSetMap[commandSetKeys[selectedCommandSetLinkIndex]]);
                            }

                            ImGui.EndTable();

                        }

                        ImGui.TableSetColumnIndex(1);
                        if (ImGui.BeginTable("TriggerTable", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg))
                        {
                            ImGui.TableSetupColumn("##tracking", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 10);
                            ImGui.TableSetupColumn("Triggers", ImGuiTableColumnFlags.NoResize, 400);
                            ImGui.TableSetupColumn("##Trigger__Btns", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 190);
                            ImGui.TableHeadersRow();

                            int index = 0;
                            for (var triggerLN = selectedCommandSet.triggers.First;
                                triggerLN != null; triggerLN = triggerLN.Next, index++)
                            {
                                var command = triggerLN.Value;
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                if (command.IsCurrent())
                                {
                                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                                    ImGui.Text("->");
                                    ImGui.PopStyleColor();
                                }

                                ImGui.TableSetColumnIndex(1);
                                command.MinimalInfo();

                                ImGui.TableSetColumnIndex(2);

                                if (ImGui.BeginPopup($"Edit Trigger##CommandSetsTab__Popup__{index}"))
                                {
                                    command.BuilderGui();

                                    if (ImGui.Button("Save & Close"))
                                    {
                                        ImGui.CloseCurrentPopup();
                                        CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                                    }
                                    ImGui.EndPopup();
                                }

                                if (ImGui.Button($"Edit##CommandSetsTab__Trigger__Btn__{index}"))
                                {
                                    ImGui.OpenPopup($"Edit Trigger##CommandSetsTab__Popup__{index}");
                                }

                                ImGui.SameLine();
                                if (ImGui.Button($"Remove##CommandSetsTab__Trigger__Btn__{index}"))
                                {
                                    var prev = triggerLN.Previous;
                                    selectedCommandSet.triggers.Remove(triggerLN);
                                    triggerLN = prev;
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(1);
                            ImGui.Separator();

                            ImGui.Text("New ");
                            var commandTypes = Command.AllTypes.Where(t => !t.Equals(typeof(CommandSet))).ToArray();

                            ImGui.SetNextItemWidth(200);
                            ImGui.SameLine();
                            if (ImGui.Combo("##TriggerTypeSelector", ref selectedNewTriggerIndex,
                                commandTypes.Select(t => t.Name).ToArray(), commandTypes.Count()))
                            {
                                newTrigger = (Command)Activator.CreateInstance(commandTypes[selectedNewTriggerIndex]);
                            }

                            ImGui.TableSetColumnIndex(2);
                            if (newTrigger != null)
                            {
                                if (ImGui.Button("Add##CommandSetsTab__Btn__AddTrigger"))
                                {
                                    selectedCommandSet.triggers.AddLast(newTrigger);
                                    newTrigger = (Command)Activator.CreateInstance(newTrigger.GetType());
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(1);

                            ImGui.Text("Link CommandSet ");

                            ImGui.SetNextItemWidth(200);
                            ImGui.SameLine();
                            string[] commandSetKeys = CommandSet.CommandSetMap.Keys.ToArray();
                            ImGui.Combo("##TriggerSetSelector", ref selectedTriggerLinkIndex,
                                commandSetKeys, CommandSet.CommandSetMap.Count);

                            ImGui.TableSetColumnIndex(2);
                            if (ImGui.Button("Add##CommandSetsTab__Btn__AddTriggerSet"))
                            {
                                // TODO: this is not safe for adding loop dependencies. Fix.
                                selectedCommandSet.triggers.AddLast(CommandSet.CommandSetMap[commandSetKeys[selectedTriggerLinkIndex]]);
                            }

                            ImGui.EndTable();
                        }

                        ImGui.EndTable();
                    }

                    ImGui.Separator();
                    if (ImGui.Button("Play"))
                    {
                        CottonCollectorPlugin.rootCmdManager.Schedule(selectedCommandSet);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Kill Switch"))
                    {
                        CottonCollectorPlugin.rootCmdManager.KillSwitch();
                    }
                }
                ImGui.EndTable();
                #endregion

            }
            ImGui.EndChild();
        }

    }
}
