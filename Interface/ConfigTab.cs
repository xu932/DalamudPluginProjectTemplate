using System;

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
            catch (Exception e)
            {
                PluginLog.Error($"{name} failed to render.");
                PluginLog.Error(e.Message);
                PluginLog.Error(e.StackTrace);
            }
        }

        public abstract void TabContent();
    }
}
