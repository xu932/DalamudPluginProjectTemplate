using System;

using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;
using CottonCollector.Util;

namespace CottonCollector.Interface
{
    internal abstract class ConfigTab
    {
        protected CottonCollectorConfig config;
        private string name;

        public ConfigTab(string name)
        {
            this.name = name;
            this.config = CottonCollectorPlugin.config;
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
                PluginLog.Error($"{e}");
            }
        }

        public abstract void TabContent();
    }
}
