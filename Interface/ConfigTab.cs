using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal abstract class ConfigTab
    {
        private string name;
        protected CottonCollectorConfig config;

        public ConfigTab(string name, ref CottonCollectorConfig config)
        {
            this.name = name;
            this.config = config;
        }

        public void Draw(bool shouldShow = true)
        {
            if (!shouldShow) return;
            try
            {
                if (ImGui.BeginTabItem(name)) {
                    TabContent();
                    ImGui.EndTabItem();
                }
            }
            catch
            {
                PluginLog.Error($"{name} failed to render.");
            }
        }

        public abstract void TabContent();
    }
}
