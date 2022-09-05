using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using ImGuiNET;

using Dalamud.Logging;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class ConfigWindow
    {
        private bool IsOpen = false;
        private CottonCollectorConfig config;

        public ConfigWindow()
        {
            config = CottonCollectorPlugin.DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
        }

        public void Draw()
        {
            if (!IsOpen) return;

            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            if (imGuiReady)
            {
                if (ImGui.BeginTabBar("", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Settings"))
                    {
                        ImGui.Checkbox("Show Name", ref config.showName);
                        ImGui.Checkbox("Show Time", ref config.showTime);
                        ImGui.Checkbox("Show Objects", ref config.showObjects);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Info"))
                    {
                        if (config.showName)
                            ImGui.Text($"{CottonCollectorPlugin.ClientState.LocalPlayer.Name}");
                        if (config.showTime)
                            ImGui.Text($"{DateTime.Now}");
                        ImGui.EndTabItem();
                    }
                    if (config.showObjects)
                    {
                        if (ImGui.BeginTabItem("Object Table"))
                        {
                            ObjectKindSelector();
                            if (ImGui.BeginChild("ObjDataTable"))
                            {
                                ObjectTable();
                                ImGui.EndChild();
                            }

                            ImGui.EndTabItem();
                        }
                    }
                    ImGui.EndTabBar();  
                }

                if (ImGui.Button("Save and Close"))
                {
                    this.Close();
                    CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                    CottonCollectorPlugin.config = this.config;
                    PluginLog.Log("Cotton Collector config saved.");
                }
            }
            ImGui.End();
        }

        private void ObjectKindSelector()
        {
            List<ObjectKind> objKindList = Enum.GetValues(typeof(ObjectKind)).Cast<ObjectKind>().ToList();
            int selectedIndex = objKindList.IndexOf(config.currKind);
            ImGui.Text($"kind size: {objKindList.Count}");
            if (ImGui.Combo("ObjectKindSelector", ref selectedIndex, Enum.GetNames(typeof(ObjectKind)), objKindList.Count))
            {
                config.currKind = objKindList[selectedIndex];
            }
        }

        private void ObjectTable()
        {
            if (ImGui.BeginTable("Objects", 4)) {
                // Table header
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("ObjectId");
                ImGui.TableSetupColumn("DataId");
                ImGui.TableSetupColumn("Pos");
                ImGui.TableHeadersRow();

                // Object table
                foreach (GameObject obj in CottonCollectorPlugin.ObjectTable)
                {
                    if (obj.ObjectKind != config.currKind) continue;
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{obj.Name}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text($"{obj.ObjectId}");
                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text($"{obj.DataId}");
                    ImGui.TableSetColumnIndex(3);
                    ImGui.Text($"X:{obj.Position.X}, Y:{obj.Position.Y}, Z:{obj.Position.Z}");
                }
                ImGui.EndTable();
            }
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }
    }
}
