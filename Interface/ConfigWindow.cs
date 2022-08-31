using System;
using System.Numerics;
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

            ImGui.SetNextWindowSize(new Vector2(750, 250));
            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            if (imGuiReady)
            {
                ImGui.Checkbox("Show Name", ref config.showName);
                if (config.showName)
                    ImGui.Text($"{CottonCollectorPlugin.ClientState.LocalPlayer.Name}");
                ImGui.Separator();

                ImGui.Checkbox("Show Time", ref config.showTime);
                if (config.showTime)
                    ImGui.Text($"{DateTime.Now}");
                ImGui.Separator();

                ImGui.Checkbox("Show Objects", ref config.showObjects);
                if (config.showObjects)
                {
                    if (ImGui.BeginListBox(config.currKind.ToString()))
                    {
                        foreach (ObjectKind kind in Enum.GetValues(typeof(ObjectKind)))
                        {
                            bool isSelected = false;
                            ImGui.Selectable(kind.ToString(), ref isSelected);
                            if (isSelected) {
                                config.currKind = kind;
                            }
                        }
                        ImGui.EndListBox();
                    }
                    foreach (GameObject obj in CottonCollectorPlugin.ObjectTable)
                    {
                        if (obj.ObjectKind == config.currKind)
                            ImGui.Text($"{obj.Name}:{obj.ObjectId}");
                    }
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
