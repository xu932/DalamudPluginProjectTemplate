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
            settingsTab = new SettingsTab(ref config);
            objectTableTab = new ObjectTableTab(ref config);
            characterControlTab = new CharacterControlTab(ref config);
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

            }

            if (ImGui.Button("Close"))
            {
                CottonCollectorPlugin.config = this.config;
                this.Close();
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
