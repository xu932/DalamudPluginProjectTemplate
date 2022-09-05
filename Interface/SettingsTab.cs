using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal class SettingsTab : ConfigTab
    {
        public SettingsTab() : base("Settings") { }

        public override void TabContent()
        {
            CottonCollectorConfig config = CottonCollectorPlugin.DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            ImGui.Checkbox("Show Objects", ref config.showObjects);
            ImGui.Checkbox("Show Character Control", ref config.showCharacterControl);
        }

    }
}
