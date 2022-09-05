using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class SettingsTab : ConfigTab
    {
        public SettingsTab(ref CottonCollectorConfig config) : base("Settings", ref config) { }

        public override void TabContent()
        {
            ImGui.Checkbox("Show Objects", ref config.showObjects);
            ImGui.Checkbox("Show Character Control", ref config.showCharacterControl);
            if (ImGui.Button("Save"))
            {
                CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                PluginLog.Log("Cotton Collector config saved.");
            }
        }

    }
}
