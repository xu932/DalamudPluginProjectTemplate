using System.Numerics;
using ImGuiNET;

using Dalamud.Logging;

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
            if (IsOpen) return;

            ImGui.SetNextWindowSize(new Vector2(750, 250));
            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            if (imGuiReady)
            {
                ImGui.Checkbox("TestCheckBox", ref config.ShowName);
                ImGui.Separator();
                if (ImGui.Button("Save and Close"))
                {
                    this.Close();
                    CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                    CottonCollectorPlugin.config = this.config;
                    PluginLog.Log("Cotton Collector config saved.");
                }
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
