using System;
using System.Numerics;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Logging;
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

        [Command("/recordpos")]
        [Aliases("/rp")]
        public void RecordPos(string command, string args)
        {
            /*
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            configWindow.positions.Add(player.Position);
            PluginLog.Log($"Recorded {player.Position.ToString()} at index {configWindow.positions.Count - 1}");
            */
        }

        [Command("/move")]
        public void MoveTo(string command, string args)
        {
            /*
            int idx;
            try
            {
                idx = Int32.Parse(args);
            }
            catch(FormatException)
            {
                PluginLog.Log($"Unable to parse args {args}");
                return;
            }
            configWindow.RunCommand(idx);
            */
        }

        [Command("/kill")]
        public void Kill(string command, string args)
        {
            CottonCollectorPlugin.rootCmdManager.KillSwitch();
        }

        [Command("/debug")]
        public void Debug(string command, string args)
        {
            /*
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, configWindow.positions[0]);
            var dist = MyMath.dist(player.Position, configWindow.positions[0]);
            PluginLog.Log($"Angle: {angle}, dist: {dist}");
            */
        }

        public void Dispose()
        {
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= rootCmdManager.Update;
            if (pluginCommandManager != null) pluginCommandManager.Dispose();
            CommandSet.CommandSetMap.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
