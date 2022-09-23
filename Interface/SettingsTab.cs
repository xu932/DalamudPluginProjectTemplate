using ImGuiNET;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class SettingsTab : ConfigTab
    {
        public SettingsTab(ref CottonCollectorConfig config) : base("Settings", ref config) { }

        public override void TabContent()
        {
            ImGui.Checkbox("Show Objects", ref config.showObjects);
            ImGui.Checkbox("Show Camera Info", ref config.showCameraInfo);
        }

    }
}
