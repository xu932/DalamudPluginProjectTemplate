using System.Numerics;

using ImGuiNET;

using CottonCollector.Config;
using Dalamud.Interface;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;
using System.Text.RegularExpressions;
using Dalamud.Utility;

namespace CottonCollector.Interface
{
    internal class PresetsTab : ConfigTab
    {
        internal static string filterString = "";
        internal static string newPresetName = "";
        // temp
        private string selectedPreset;

        public PresetsTab(ref CottonCollectorConfig config) : base("Presets", ref config)
        {
            selectedPreset = "";
        }

        public override void TabContent()
        {
            ImGui.BeginChild("TableWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (ImGui.BeginTable("LayoutsTable", 2 ,ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("Preset list", ImGuiTableColumnFlags.None, 200);
                ImGui.TableSetupColumn("TEMP PRESET NAME", ImGuiTableColumnFlags.None, 600);

                ImGui.TableHeadersRow();

                ImGui.TableNextColumn();
                ImGui.InputTextWithHint("##presetFilter", "search presets...", ref filterString, 100);
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddPreset"))
                {
                    ImGui.OpenPopup("Add preset");
                }
                ImGui.PopFont();

                if(ImGui.BeginPopup("Add preset"))
                {
                    ImGui.InputTextWithHint("##newPresetName", "Preset name", ref newPresetName, 100);
                    ImGui.SameLine();
                    if (ImGui.Button("Add")) {
                        if (newPresetName != "")
                        {
                            ImGui.CloseCurrentPopup();
                            config.presets.Add(newPresetName);
                        }
                    }
                    ImGui.EndPopup();
                }

                ImGui.BeginChild("PresetsTableSelector");
                foreach (var preset in config.presets)
                {
                    if (!filterString.IsNullOrEmpty() && !Regex.IsMatch(preset, filterString))
                    {
                        continue;
                    }
                    if (ImGui.Selectable($"{preset}", preset == selectedPreset))
                    {
                        selectedPreset = preset;
                    }
                }
                ImGui.EndChild();

                ImGui.TableNextColumn();
                ImGui.Text($"{selectedPreset}");

                ImGui.EndTable();
            }
            ImGui.EndChild();
        }

    }
}
