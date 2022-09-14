﻿using System;
using System.Diagnostics;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Plugin;
using Dalamud.Logging;

using CottonCollector.Attributes;
using CottonCollector.CameraManager;
using CottonCollector.Config;
using CottonCollector.Interface;

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
        internal static CommandManager CommandManager { get; private set; }

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

        internal static CharacterControl.CommandManager cmdManager = new CharacterControl.CommandManager();

        internal static CottonCollectorConfig config { get; set; }
        private readonly ConfigWindow configWindow;
        private readonly PluginCommandManager<CottonCollectorPlugin> pluginCommandManager;


        public string Name => "Cotton Collector";

        public CottonCollectorPlugin()
        {
            // temp
            // DalamudPluginInterface.ConfigFile.Delete();

            // Ini CameraHelpers
            CameraHelpers.Initialize();

            // Load config window
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            configWindow = new ConfigWindow(this);
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
            cmdManager.Update();
        }

        public void Dispose()
        {
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= Update;
            if (pluginCommandManager != null) pluginCommandManager.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
