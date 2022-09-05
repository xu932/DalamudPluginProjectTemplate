using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class ConfigWindow
    {
        private bool IsOpen = false;
        private CottonCollectorConfig config;

        private SettingsTab settingsTab;
        private ObjectTableTab objectTableTab;
        private CharacterControlTab characterControlTab;

        public ConfigWindow()
        {
            config = CottonCollectorPlugin.DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            settingsTab = new SettingsTab();
            objectTableTab = new ObjectTableTab();
            characterControlTab = new CharacterControlTab();
        }

        public void Draw()
        {
            if (!IsOpen) return;

            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            if (imGuiReady)
            {
                if (ImGui.BeginTabBar("", ImGuiTabBarFlags.None))
                {
                    settingsTab.Draw();
                    objectTableTab.Draw(config.showObjects);
                    characterControlTab.Draw(config.showCharacterControl);

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
