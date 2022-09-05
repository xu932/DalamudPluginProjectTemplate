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

        private ObjectTableTab objectTableTab;

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
                    objectTableTab.Draw(config.showObjects);
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
