using System;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Objects;

using CottonCollector.Attributes;
using CottonCollector.Config;
using CottonCollector.Interface;

namespace CottonCollector
{
    internal class CottonCollectorPlugin : IDalamudPlugin
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        [PluginService]
        internal static DataManager DataManager { get; private set; }

        [PluginService]
        internal static ClientState ClientState { get; private set; }

        [PluginService]
        internal static CommandManager CommandManager { get; private set; }

        [PluginService]
        internal static ChatGui ChatGui { get; private set; }

        [PluginService]
        internal static Framework Framework { get; private set; }

        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; }

        [PluginService]
        internal static KeyState KeyState { get; private set; }

        internal static CottonCollectorConfig config { get; set; }
        private static ConfigWindow configWindow;
        private readonly PluginCommandManager<CottonCollectorPlugin> pluginCommandManager; 

        public string Name => "Cotton Collector";

        public CottonCollectorPlugin()
        {
            // Load config window
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            configWindow = new ConfigWindow();
            DalamudPluginInterface.UiBuilder.Draw += configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += configWindow.Open;
            Framework.Update += Update;

            // Load all commands
            pluginCommandManager = new PluginCommandManager<CottonCollectorPlugin>(this);
        }

        [Command("/cottoncollectormonitor")]
        [Aliases("/ccm")]
        public void OpenMonitorWindowCommand(string command, string args)
        {
            configWindow.Toggle();
        }

        private void Update(Framework framework)
        {
            var localPlayer = ClientState.LocalPlayer;

        }

        public void Dispose()
        {
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= Update;
            pluginCommandManager.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
