using ImGuiNET;

using Dalamud.Game.ClientState.Conditions;

using CottonCollector.Util;

namespace CottonCollector.Interface
{
    internal class SettingsTab : ConfigTab
    {
        public SettingsTab() : base("Settings") { }

        public override void TabContent()
        {
            ImGui.Checkbox(Ui.Uid("Show Objects"), ref config.showObjects);
            ImGui.Checkbox(Ui.Uid("Show Camera Info"), ref config.showCameraInfo);
            if (CottonCollectorPlugin.TargetManager.Target != null)
            {
                ImGui.Text("Curr Target: ");
                ImGui.Text($"Name: {CottonCollectorPlugin.TargetManager.Target.Name}");
                ImGui.Text($"Position: {CottonCollectorPlugin.TargetManager.Target.Position}");
                ImGui.Text($"Address: {CottonCollectorPlugin.TargetManager.Target.Address}");
            }

            ImGui.Text($"Is Diving? {CottonCollectorPlugin.GameCondition[ConditionFlag.Diving]}");
        }
    }
}
