using System;
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
        internal static Command newCommand = new KeyboardCommand();
        internal static Command newTrigger = new KeyboardCommand();

        public CommandSetsTab() : base("CommandSets") { }

        private void CommandList(LinkedList<Command> commands)
        {
            if (ImGui.BeginTable(Ui.Uid(), 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg))
            {
                ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 10);
                ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize, 400);
                ImGui.TableSetupColumn(Ui.Uid(), ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthFixed, 120);
                ImGui.TableHeadersRow();

                int index = 0;
                for (var commandLN = commands.First;
                    commandLN != null; commandLN = commandLN.Next, index++)
                {
                    var command = commandLN.Value;

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (command.IsCurrent)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                        ImGui.Text("->");
                        ImGui.PopStyleColor();
                    }

                    ImGui.TableSetColumnIndex(1);
                    command.MinimalInfo();

                    ImGui.TableSetColumnIndex(2);
                    var editUid = Ui.Uid("Edit Command", index);
                    if (ImGui.BeginPopup(editUid))
                    {
                        command.BuilderGui();

                        if (ImGui.Button(Ui.Uid("Save & Close", index)))
                        {
                            ImGui.CloseCurrentPopup();
                            CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                        }
                        ImGui.EndPopup();
                    }

                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Edit.ToIconString()}", index)))
                    {
                        ImGui.OpenPopup(editUid);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Minus.ToIconString()}", index)))
                    {
                        var prev = commandLN.Previous;
                        commands.Remove(commandLN);
                        commandLN = prev;
                    }

                    ImGui.SameLine();
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.ArrowUp.ToIconString()}", index)))
                    {
                        var prev = commandLN.Previous;
                        if (prev != null)
                        {
                            commands.Remove(commandLN);
                            commands.AddBefore(prev, commandLN);
                        }
                    }

                    ImGui.SameLine();
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.ArrowDown.ToIconString()}", index)))
                    {
                        var next = commandLN.Next;
                        if (next != null)
                        {
                            commands.Remove(commandLN);
                            commands.AddAfter(next, commandLN);
                        }
                    }
                    ImGui.PopFont();
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

                ImGui.TableSetColumnIndex(2);
                if (newCommand != null)
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                    {
                        commands.AddLast(newCommand);
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

                ImGui.TableSetColumnIndex(2);
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}")))
                {
                    commands.AddLast(CommandSet.CommandSetMap[commandSetKeys[selectedCommandSetLinkIndex]]);
                }
                ImGui.PopFont();

                ImGui.EndTable();
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
                            CommandList(selectedCommandSet.subCommands);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem(Ui.Uid("Triggers")))
                        {
                            CommandList(selectedCommandSet.triggers);
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
                    }
                    ImGui.SameLine();

                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Trash.ToIconString()}")))
                    {
                        config.commandSets.Remove(selectedCommandSet);
                        selectedCommandSet = null;
                    }
                    ImGui.PopFont();
                }
                ImGui.EndTable();
                #endregion

            }
            ImGui.EndChild();
        }

    }
}
