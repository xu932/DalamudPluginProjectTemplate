using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;
using CottonCollector.Util;

namespace CottonCollector.Interface
{
    internal class ConfigWindow
    {
        private bool IsOpen = false;
        private CottonCollectorConfig config;

        private static SettingsTab settingsTab = new();
        private static ObjectTableTab objectTableTab = new();
        private static CameraInfoTab cameraInfoTab = new();
        private static CommandSetsTab commandSetsTab = new();
        private static KeybindSettingsTab keybindTab = new();

        public ConfigWindow()
        {
            config = CottonCollectorPlugin.config;
        }

        public void Draw()
        {
            if (!IsOpen) return;

            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            ImGui.BeginChild("RootWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (imGuiReady)
            {
                if (ImGui.BeginTabBar("##maintabbar", ImGuiTabBarFlags.None))
                {
                    settingsTab.Draw();
                    objectTableTab.Draw(config.showObjects);
                    cameraInfoTab.Draw(config.showCameraInfo);
                    keybindTab.Draw();
                    commandSetsTab.Draw();

                    ImGui.EndTabBar();  
                }
            }

            if (ImGui.Button("Save"))
            {
                CottonCollectorPlugin.config = this.config;
                CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                PluginLog.Log("Cotton Collector config saved.");
            }

            ImGui.SameLine();
            if (ImGui.Button("Save and Close"))
            {
                CottonCollectorPlugin.config = this.config;
                CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                PluginLog.Log("Cotton Collector config saved.");
                this.Close();
            }

            ImGui.EndChild();

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
