using System;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Plugin;

using CottonCollector.Attributes;
using CottonCollector.CameraManager;
using CottonCollector.Config;
using CottonCollector.Interface;
using CottonCollector.Commands.Structures;

namespace CottonCollector
{
    internal unsafe class CottonCollectorPlugin : IDalamudPlugin
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        [PluginService]
        internal static DataManager DataManager { get; private set; }

        [PluginService]
        internal static ClientState ClientState { get; private set; }

        [PluginService]
        internal static Dalamud.Game.Command.CommandManager CommandManager { get; private set; }

        [PluginService]
        internal static ChatGui ChatGui { get; private set; }

        [PluginService]
        internal static Framework Framework { get; private set; }

        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; }

        [PluginService]
        internal static KeyState KeyState { get; private set; }

        [PluginService]
        internal static AetheryteList AetheryteList { get; private set; }

        internal static CommandManager rootCmdManager = new();

        internal static CottonCollectorConfig config { get; set; }
        private readonly ConfigWindow configWindow;
        private readonly PluginCommandManager<CottonCollectorPlugin> pluginCommandManager;


        public string Name => "Cotton Collector";

        public CottonCollectorPlugin()
        {
            // Ini CameraHelpers
            CameraHelpers.Initialize();

            // Load config window
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            configWindow = new ConfigWindow(this);
            DalamudPluginInterface.UiBuilder.Draw += configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += configWindow.Open;
            Framework.Update += rootCmdManager.Update;

            // Load all commands
            pluginCommandManager = new PluginCommandManager<CottonCollectorPlugin>(this);
        }

        [Command("/cottoncollectormonitor")]
        [Aliases("/ccm")]
        public void OpenMonitorWindowCommand(string command, string args)
        {
            configWindow.Toggle();
        }

        public void Dispose()
        {
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= rootCmdManager.Update;
            if (pluginCommandManager != null) pluginCommandManager.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
