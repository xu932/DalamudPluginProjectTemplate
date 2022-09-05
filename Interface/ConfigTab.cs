using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

using Dalamud.Logging;

namespace CottonCollector.Interface
{
    internal abstract class ConfigTab
    {
        private string name;

        public ConfigTab(string name)
        {
            this.name = name;
        }

        public void Draw(bool shouldShow)
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
