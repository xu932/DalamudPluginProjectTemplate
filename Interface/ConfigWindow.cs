using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Config;
using System.Collections.Generic;
using System.Numerics;
using CottonCollector.Commands.Impls;
using CottonCollector.Commands.Structures;

namespace CottonCollector.Interface
{
    internal class ConfigWindow
    {
        private bool IsOpen = false;
        private CottonCollectorConfig config;

        private SettingsTab settingsTab;
        private ObjectTableTab objectTableTab;
        private CameraInfoTab cameraInfoTab;
        private CommandSetsTab presetsTab;
        private readonly CottonCollectorPlugin plugin;

        public List<Vector3> positions = new();

        public ConfigWindow(CottonCollectorPlugin plugin)
        {
            this.plugin = plugin;
            config = CottonCollectorPlugin.config;
            settingsTab = new SettingsTab(ref config);
            objectTableTab = new ObjectTableTab(ref config);
            cameraInfoTab = new CameraInfoTab(ref config);
            presetsTab = new CommandSetsTab(ref config);
        }

        public void Draw()
        {
            if (!IsOpen) return;

            var imGuiReady = ImGui.Begin("Cotton Collector Configuration", ref IsOpen, ImGuiWindowFlags.None);

            ImGui.BeginChild("RootWrapper", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            if (imGuiReady)
            {
                if (ImGui.BeginTabBar("", ImGuiTabBarFlags.None))
                {
                    settingsTab.Draw();
                    objectTableTab.Draw(config.showObjects);
                    cameraInfoTab.Draw(config.showCameraInfo);
                    presetsTab.Draw();

                    ImGui.EndTabBar();  
                }

            }

            if (ImGui.Button("Save"))
            {
                CottonCollectorPlugin.config = this.config;
                CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                PluginLog.Log("Cotton Collector config saved.");
            }

            ImGui.SameLine();
            if (ImGui.Button("Save and Close"))
            {
                CottonCollectorPlugin.config = this.config;
                CottonCollectorPlugin.DalamudPluginInterface.SavePluginConfig(config);
                PluginLog.Log("Cotton Collector config saved.");
                this.Close();
            }

            ImGui.EndChild();

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

        public void RunCommand(int i)
        {
            CommandSet cs = new("foo");
            FooCommand fcmd = new(positions[i]);
            cs.subCommands.AddLast(fcmd);
            CottonCollectorPlugin.rootCmdManager.Schedule(cs);
        }
    }
}
