using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Utility;

using Dalamud.Interface.Colors;
using Dalamud.Logging;

using CottonCollector.Config;
using CottonCollector.Commands.Structures;
using CottonCollector.Commands.Impls;
using CottonCollector.Util;
using CottonCollector.BackgroundInputs;

namespace CottonCollector.Interface
{
    internal class CommandSetsTab : ConfigTab
    {
        internal static string filterString = "";
        internal static string newCommandSetName = "";
        internal static int selectedNewCommandIndex = 0;
        internal static int selectedNewTriggerIndex = 0;
        internal static int selectedCommandSetLinkIndex = 0;
        internal static int selectedCommandIndex = 0;
        internal static int selectedTriggerLinkIndex = 0;
        internal static Command newCommand = new KeyboardCommand();
        internal static Command newTrigger = new KeyboardCommand();

        public CommandSetsTab() : base("CommandSets") { }

        private void ToolBelt(List<Command> commands, int tabIndex)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.PlayCircle.ToIconString()}", tabIndex)))
            {
                CottonCollectorPlugin.rootCmdManager.Schedule(commands[selectedCommandIndex]);
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Minus.ToIconString()}", tabIndex)))
            {
                commands.Remove(commands[selectedCommandIndex]);
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            var disableUp = selectedCommandIndex == 0;
            if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.ArrowUp.ToIconString()}", tabIndex)) && !disableUp)
            {
                var temp = commands[selectedCommandIndex];
                commands.Remove(commands[selectedCommandIndex]);
                commands.Insert(--selectedCommandIndex, temp);
            }
            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            var disableDown = selectedCommandIndex == commands.Count - 1;
            if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.ArrowDown.ToIconString()}", tabIndex)) && !disableDown)
            {
                var temp = commands[selectedCommandIndex];
                commands.Remove(commands[selectedCommandIndex]);
                commands.Insert(++selectedCommandIndex, temp);
            }
            ImGui.PopFont();

            var editUid = Ui.Uid("Edit Command");
            if (ImGui.BeginPopup(editUid))
            {
                commands[selectedCommandIndex].BuilderGui();

                if (ImGui.Button(Ui.Uid("Save & Close")))
                {
                    ImGui.CloseCurrentPopup();
                    CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Edit.ToIconString()}")))
            {
                ImGui.OpenPopup(editUid);
            }
            ImGui.PopFont();
        }

        private void CommandList(List<Command> commands, int tabIndex)
        {
            ToolBelt(commands, tabIndex);

            Vector2 region = ImGui.GetContentRegionAvail();
            region.Y -= 30;
            if (ImGui.BeginChild(Ui.Uid(index: tabIndex), region, false, ImGuiWindowFlags.AlwaysVerticalScrollbar))
            {
                if (ImGui.BeginTable(Ui.Uid(), 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 10);
                    ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize, 400);
                    ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 120);
                    ImGui.TableHeadersRow();

                    int index = 0;
                    foreach (var command in commands)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        if (command.IsCurrent)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                            ImGui.Text("->");
                            ImGui.PopStyleColor();
                        }

                        ImGui.TableSetColumnIndex(1);
                        if(ImGui.Selectable(Ui.Uid(index: index), index == selectedCommandIndex, 
                            ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowItemOverlap)) {
                            PluginLog.Log($"SELECT index: {index}");
                            selectedCommandIndex = index;
                        }

                        ImGui.SameLine();
                        command.MinimalInfo();

                        index++;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Separator();

                    ImGui.Text("New ");
                    var commandTypes = Command.AllTypes.Where(t => !t.Equals(typeof(CommandSet))).ToArray();

                    ImGui.SetNextItemWidth(200);
                    ImGui.SameLine();
                    if (ImGui.Combo(Ui.Uid(), ref selectedNewCommandIndex,
                        commandTypes.Select(t => t.Name).ToArray(), commandTypes.Length))
                    {
                        newCommand = (Command)Activator.CreateInstance(commandTypes[selectedNewCommandIndex]);
                    }

                    if (newCommand != null)
                    {
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                        {
                            if (newCommand is MoveToCommand moveTo)
                            {
                                moveTo.SetCurrOrTargetPos();
                            }
                            commands.Add(newCommand);
                            newCommand = (Command)Activator.CreateInstance(newCommand.GetType());
                        }
                        ImGui.PopFont();
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(1);

                    ImGui.Text("Link CommandSet ");

                    ImGui.SetNextItemWidth(200);
                    ImGui.SameLine();
                    string[] commandSetKeys = CommandSet.CommandSetMap.Keys.ToArray();
                    ImGui.Combo(Ui.Uid(), ref selectedCommandSetLinkIndex,
                        commandSetKeys, CommandSet.CommandSetMap.Count);

                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                    {
                        commands.Add(CommandSet.CommandSetMap[commandSetKeys[selectedCommandSetLinkIndex]]);
                    }
                    ImGui.PopFont();

                    ImGui.EndTable();
                }
                ImGui.EndChild();
            }
        }

        public override void TabContent()
        {
            ref var selectedCommandSet = ref CottonCollectorPlugin.selectedCommandSet;
            var tableRegion = ImGui.GetContentRegionAvail();
            tableRegion.Y -= 30;
            ImGui.BeginChild("TableWrapper", tableRegion, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (ImGui.BeginTable(Ui.Uid(), 2, ImGuiTableFlags.Resizable))
            {
                #region COMMAND_SET_SELECTOR
                ImGui.TableSetupColumn(Ui.Uid("Command Sets List"), ImGuiTableColumnFlags.None, 200);

                var displayCommandSetName = selectedCommandSet != null ? selectedCommandSet.uniqueId : "";
                ImGui.TableSetupColumn(Ui.Uid(displayCommandSetName), ImGuiTableColumnFlags.None, 1000);
                ImGui.TableHeadersRow();

                ImGui.TableNextColumn();
                ImGui.InputTextWithHint(Ui.Uid(), "search presets...", ref filterString, 100);
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);

                var addPresetPopupUid = Ui.Uid();
                if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                {
                    ImGui.OpenPopup(addPresetPopupUid);
                }
                ImGui.PopFont();

                if (ImGui.BeginPopup(addPresetPopupUid))
                {
                    ImGui.InputTextWithHint(Ui.Uid(), "New Command Set Name", ref newCommandSetName, 100);
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                    {
                        if (newCommandSetName != "" && !CommandSet.CommandSetMap.ContainsKey(newCommandSetName))
                        {
                            ImGui.CloseCurrentPopup();
                            config.commandSets.Add(new CommandSet(newCommandSetName));
                        }
                    }
                    ImGui.PopFont();
                    ImGui.EndPopup();
                }

                ImGui.BeginChild(Ui.Uid());

                int setIndex = 0;
                foreach (var commandSet in config.commandSets)
                {
                    if (commandSet == null)
                    {
                        PluginLog.Verbose("Null CommandSet!!");
                        continue;
                    }
                    if (!filterString.IsNullOrEmpty() && !Regex.IsMatch(commandSet.uniqueId, filterString))
                    {
                        continue;
                    }
                    if (ImGui.Selectable(Ui.Uid($"{commandSet.uniqueId}", setIndex), selectedCommandSet != null && 
                        commandSet.uniqueId == selectedCommandSet.uniqueId))
                    {
                        selectedCommandSet = commandSet;
                    }

                    setIndex++;
                }
                ImGui.EndChild();
                #endregion

                #region COMMAND_SET_EDITOR
                ImGui.TableNextColumn();
                if (selectedCommandSet != null)
                {
                    selectedCommandSet.BuilderGui();
                    if (ImGui.BeginTabBar(Ui.Uid(), ImGuiTabBarFlags.None))
                    {
                        if (ImGui.BeginTabItem(Ui.Uid("Command Sequence")))
                        {
                            CommandList(selectedCommandSet.subCommands, 0);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem(Ui.Uid("Triggers")))
                        {
                            CommandList(selectedCommandSet.triggers, 1);
                            ImGui.EndTabItem();
                        }
                        ImGui.EndTabBar();
                    }

                    ImGui.Separator();
                    if (ImGui.Button(Ui.Uid("Play")))
                    {
                        CottonCollectorPlugin.rootCmdManager.Schedule(selectedCommandSet);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button(Ui.Uid("Kill Switch")))
                    {
                        CottonCollectorPlugin.rootCmdManager.KillSwitch();
                        CottonCollectorPlugin.KeyState.ClearAll();
                        BgInput.Clear();
                    }
                    ImGui.SameLine();

                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Trash.ToIconString()}")))
                    {
                        config.commandSets.Remove(selectedCommandSet);
                        CommandSet.CommandSetMap.Remove(selectedCommandSet.uniqueId);
                        selectedCommandSet = null;
                    }
                    ImGui.PopFont();
                }
                #endregion
                ImGui.EndTable();
            }
            ImGui.EndChild();
        }

    }
}
